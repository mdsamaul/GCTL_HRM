using GCTL.Core.Data;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text.Json;

namespace GCTL.Service.DeleteHistories
{
    public class DependencyCheckResult
    {
        public bool CanDelete { get; set; }
        public List<string> DependentTables { get; set; }
        public string Message { get; set; }

        public DependencyCheckResult()
        {
            DependentTables = new List<string>();
        }
    }

    public class DeleteHistoryService : IDeleteHistoryService
    {
        private readonly IRepository<DeleteHistory> deleteHistoryRepository;
        private readonly GCTL_ERP_DB_DatapathContext context;

        private readonly IMemoryCache _cache;
        private readonly MemoryCacheEntryOptions _cacheOptions = new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) };
        private const string CacheKeyPrefix = "ReferentialIntegrityCandidateColumns_";


        public DeleteHistoryService(IRepository<DeleteHistory> deleteHistoryRepository, GCTL_ERP_DB_DatapathContext context, IMemoryCache memoryCache)
        {
            this.deleteHistoryRepository = deleteHistoryRepository;
            this.context = context;

            _cache = memoryCache;
        }

        public async Task<bool> LogDeletedRecordsAsync<T>(List<T> entities,  DeleteHistoryViewModel model) where T : class
        {
            if (entities == null || !entities.Any())
                return false;

            try
            {
                var deleteHistoryRecords = new List<DeleteHistory>();
                decimal currentDhid = await GenerateUniqueDHIDAsync();

                foreach (var entity in entities)
                {
                    var deleteHistory = new DeleteHistory
                    {
                        Dhid = currentDhid++,
                        TableName = model.tableName,
                        Lip = model.Lip,
                        Lmac = model.Lmac,
                        Ldate = model.Ldate,
                        Luser = model.Luser
                    };

                    var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                    int fieldIndex;
                    for (fieldIndex = 0; fieldIndex < Math.Min(properties.Length, 99); fieldIndex++)
                    {
                        var property = properties[fieldIndex];
                        var value = property.GetValue(entity);
                        var fieldName = $"Field{fieldIndex + 1}";
                        var fieldValue = value?.ToString();

                        var fieldProperty = typeof(DeleteHistory).GetProperty(fieldName);
                        fieldProperty?.SetValue(deleteHistory, fieldValue);
                    }

                    var jsonFieldName = $"Field{fieldIndex + 1}";
                    var jsonFieldProperty = typeof(DeleteHistory).GetProperty(jsonFieldName);
                    var jsonData = new Dictionary<string, object> { { model.tableName, entity } };
                    jsonFieldProperty?.SetValue(deleteHistory, JsonSerializer.Serialize(jsonData));

                    deleteHistoryRecords.Add(deleteHistory);
                }

                await deleteHistoryRepository.AddRangeAsync(deleteHistoryRecords);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<DeleteHistory>> GetDeletedRecordsByTableAsync(string tableName)
        {
            return await deleteHistoryRepository.All()
                .Where(dh => dh.TableName == tableName)
                .OrderByDescending(dh => dh.AutoId)
                .ToListAsync();
        }

        public async Task<DeleteHistory> GetDeletedRecordByDHIDAsync(decimal dhid)
        {
            return await deleteHistoryRepository.All()
                .FirstOrDefaultAsync(dh => dh.Dhid == dhid);
        }

        private async Task<decimal> GenerateUniqueDHIDAsync()
        {
            var maxDhid = await deleteHistoryRepository.All()
                .MaxAsync(dh => (decimal?)dh.Dhid);

            return (maxDhid ?? 0) + 1;
        }

        public void InvalidateCache()
        {
            _cache.Remove(CacheKeyPrefix + "Version");
        }

        private int GetCacheVersion()
        {
            if (!_cache.TryGetValue<int>(CacheKeyPrefix + "Version", out var v))
            {
                v = 1;
                _cache.Set(CacheKeyPrefix + "Version", v, _cacheOptions);
            }

            return v;
        }

        private string BuildCacheKey(IEnumerable<string> columnNames)
        {
            var version = GetCacheVersion();
            var joined = string.Join("__", columnNames.Select(s => s.ToLowerInvariant()));
            return CacheKeyPrefix + version + "__" + joined;
        }

        public async Task<DependencyCheckResult> CheckDependenciesAsync
            (
                string masterTableName, 
                string keyField, 
                List<string> keyValues, 
                List<string> alternateKeyColumns = null,
                List<string> ignoreTables = null
            )
        {
            if (string.IsNullOrWhiteSpace(masterTableName)) throw new ArgumentNullException(nameof(masterTableName));
            if (string.IsNullOrWhiteSpace(keyField)) throw new ArgumentNullException(nameof(keyField));
            if (keyValues == null || keyValues.Count == 0) throw new ArgumentNullException(nameof(keyValues));

            var ignoreSet = new HashSet<string>(
                ignoreTables?.Where(t => !string.IsNullOrWhiteSpace(t)) ?? Enumerable.Empty<string>(),
                StringComparer.OrdinalIgnoreCase
                );

            var columnNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { keyField };

            if (alternateKeyColumns != null)
                foreach (var a in alternateKeyColumns.Where(x => !string.IsNullOrWhiteSpace(x)))
                    columnNames.Add(a);
            
            var cacheKey = BuildCacheKey(columnNames);
            var candidates = await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.SetOptions(_cacheOptions);
                return await DiscoverCandidateColumnsAsync(columnNames, masterTableName);
            });

            var result = new DependencyCheckResult { CanDelete = true, Message = string.Empty };
            var dependentLines = new List<string>();

            if (candidates == null || candidates.Count == 0)
            {
                result.CanDelete = true;
                result.Message = "No candidate reference columns found.";
                return result;
            }

            var filteredCandidates = candidates
                .Where(c => !string.Equals(c.TableName, masterTableName, StringComparison.OrdinalIgnoreCase))
                .Where(c => !IsTableIgnored(c.TableSchema, c.TableName, ignoreSet))
                .ToList();

            if (filteredCandidates.Count == 0)
            {
                result.Message = "No references found. Safe to delete.";
                return result;
            }

            var stringValues = keyValues.Select(v => v?.ToString() ?? string.Empty).ToList();
            
            var conn = context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();

            var unionParts = new List<string>();
            var allParams = new Dictionary<string, object>();
            int totalParams = 0;
            const int SQL_SERVER_PARAM_LIMIT = 2000;

            for (int tableIndex =0; tableIndex<filteredCandidates.Count; tableIndex++)
            {
                var cand = filteredCandidates[tableIndex];
                var paramNames = new List<string>();

                for (int i = 0; i < stringValues.Count; i++)
                {
                    var pName = $"@t{tableIndex}_p{i}";
                    allParams[pName] = stringValues[i] ?? (object)DBNull.Value;
                    paramNames.Add(pName);
                    totalParams++;
                }

                if(totalParams > SQL_SERVER_PARAM_LIMIT)
                {
                    allParams.Remove(allParams.Keys.Last());
                    filteredCandidates = filteredCandidates.Take(tableIndex).ToList();
                    break;
                }

                string schemaQ = QuoteIdentifier(cand.TableSchema);
                string tableQ = QuoteIdentifier(cand.TableName);
                string columnQ = QuoteIdentifier(cand.ColumnName);
                string inClause = string.Join(", ", paramNames);

                unionParts.Add($@"SELECT {tableIndex} AS TableIdx, COUNT(1) AS Cnt
                                  FROM {schemaQ}.{tableQ}
                                  CROSS APPLY dbo.STRING_SPLIT(',', {columnQ}) AS split
                                  WHERE LTRIM(RTRIM(split.Value)) IN ({inClause})");
            }

            var countByIndex = new Dictionary<int, int>();

            if(unionParts.Count > 0)
            {
                var fullSql = string.Join("\nUNION ALL\n", unionParts);

                using var cmd = conn.CreateCommand();
                cmd.CommandText = fullSql;
                cmd.CommandType = System.Data.CommandType.Text;

                foreach (var kv in allParams)
                {
                    var p = cmd.CreateParameter();
                    p.ParameterName = kv.Key;
                    p.Value = kv.Value;
                    p.DbType = System.Data.DbType.String;
                    cmd.Parameters.Add(p);
                }

                try
                {
                    using var reader = await cmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        int idx = reader.GetInt32(0);
                        int cnt = reader.GetInt32(1);
                        countByIndex[idx] = cnt;
                    }
                }
                catch(Exception ex)
                {
                    return await FallbackParallelCheckAsync(filteredCandidates, stringValues, conn, result, ex.Message);
                }
            }

            for (int i = 0; i<filteredCandidates.Count; i++)
            {
                var cand = filteredCandidates[i];
                var count = countByIndex.GetValueOrDefault(i, 0);

                if(count > 0)
                {
                    result.CanDelete = false;
                    var friendlyName = await GetFriendlyNameForTableAsync(conn, cand.TableSchema, cand.TableName);
                    dependentLines.Add(friendlyName);
                }
            }

            result.DependentTables = dependentLines.Distinct().ToList();
            BuildResultMessage(result);

            return result;
        }

        private async Task<DependencyCheckResult> FallbackParallelCheckAsync(
        List<CandidateColumn> candidates,
        List<string> stringValues,
        DbConnection conn,
        DependencyCheckResult result,
        string unionError)
        {
            var dependentLines = new List<string>();
            var connectionString = context.Database.GetConnectionString();

            var tasks = candidates.Select(async cand =>
            {
                using var parallelConn = (DbConnection)Activator.CreateInstance(conn.GetType(), connectionString);
                await parallelConn.OpenAsync();

                using var cmd = parallelConn.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;

                var paramNames = new List<string>();
                for (int i = 0; i < stringValues.Count; i++)
                {
                    var p = cmd.CreateParameter();
                    p.ParameterName = $"@p{i}";
                    p.Value = stringValues[i] ?? (object)DBNull.Value;
                    p.DbType = System.Data.DbType.String;
                    cmd.Parameters.Add(p);
                    paramNames.Add(p.ParameterName);
                }

                string schemaQ = QuoteIdentifier(cand.TableSchema);
                string tableQ = QuoteIdentifier(cand.TableName);
                string columnQ = QuoteIdentifier(cand.ColumnName);
                string inClause = string.Join(", ", paramNames);

                cmd.CommandText = $@"
                SELECT COUNT(1)
                FROM {schemaQ}.{tableQ}
                CROSS APPLY dbo.STRING_SPLIT(',', {columnQ}) AS split
                WHERE LTRIM(RTRIM(split.Value)) IN ({inClause})
                ";

                try
                {
                    var scalar = await cmd.ExecuteScalarAsync();
                    int count = (scalar != null && scalar != DBNull.Value) ? Convert.ToInt32(scalar) : 0;
                    return (cand, count, error: (string)null);
                }
                catch (Exception ex)
                {
                    return (cand, count: 0, error: ex.Message);
                }
            });

            var allResults = await Task.WhenAll(tasks);

            foreach (var (cand, count, error) in allResults)
            {
                if (error != null)
                {
                    dependentLines.Add($"{cand.TableSchema}.{cand.TableName} => ERROR: {error}");
                    result.CanDelete = false;
                }
                else if (count > 0)
                {
                    result.CanDelete = false;
                    var friendlyName = await GetFriendlyNameForTableAsync(conn, cand.TableSchema, cand.TableName);
                    dependentLines.Add(friendlyName);
                }
            }

            result.DependentTables = dependentLines.Distinct().ToList();
            BuildResultMessage(result);
            return result;
        }

        private void BuildResultMessage(DependencyCheckResult result)
        {
            if (!result.CanDelete)
            {
                string joined = string.Join(", ", result.DependentTables);

                if (result.DependentTables.Count > 1)
                {
                    int lastComma = joined.LastIndexOf(", ");
                    if (lastComma >= 0)
                        joined = joined.Substring(0, lastComma) + " & " + joined.Substring(lastComma + 1);

                    result.Message = $"Can't be deleted. Please delete first from {joined} respectively.";
                }
                else
                {
                    result.Message = $"Can't be deleted. Please delete first from {joined}.";
                }
            }
            else
            {
                result.Message = "No references found. Safe to delete.";
            }
        }

        private static string QuoteIdentifier(string ident)
        {
            if (string.IsNullOrEmpty(ident)) return ident;
            return "[" + ident.Replace("]", "]]") + "]";
        }

        private async Task<List<CandidateColumn>> DiscoverCandidateColumnsAsync(HashSet<string> columnNames, string masterTableName)
        {
            var list = new List<CandidateColumn>();

            if (columnNames == null || columnNames.Count == 0) return list;

            // ✅ FIX: Don't use 'using' - let EF Core manage its connection
            var conn = context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandType = System.Data.CommandType.Text;

            var paramNames = new List<string>();
            int idx = 0;
            foreach (var c in columnNames)
            {
                var p = cmd.CreateParameter();
                p.ParameterName = $"@cn{idx}";
                p.Value = c;
                p.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(p);
                paramNames.Add(p.ParameterName);
                idx++;
            }

            cmd.CommandText = $@"
            SELECT TABLE_SCHEMA, TABLE_NAME, COLUMN_NAME
            FROM INFORMATION_SCHEMA.COLUMNS
            WHERE COLUMN_NAME IN ({string.Join(", ", paramNames)})
            AND (DATA_TYPE = 'nvarchar' OR DATA_TYPE = 'varchar' OR DATA_TYPE = 'char' OR DATA_TYPE = 'nchar')
            AND TABLE_NAME <> @masterTableName
            AND TABLE_SCHEMA NOT IN ('INFORMATION_SCHEMA', 'sys')
            ";

            var masterParam = cmd.CreateParameter();
            masterParam.ParameterName = "@masterTableName";
            masterParam.Value = masterTableName;
            masterParam.DbType = System.Data.DbType.String;
            cmd.Parameters.Add(masterParam);

            using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
            {
                var schema = rdr.GetString(0);
                var table = rdr.GetString(1);
                var col = rdr.GetString(2);
                list.Add(new CandidateColumn { TableSchema = schema, TableName = table, ColumnName = col });
            }

            return list;
        }

        private async Task<string> GetFriendlyNameForTableAsync(DbConnection conn, string schema, string tableName)
        {
            // Handle comma-separated table names
            var tableNames = tableName
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (tableNames.Length > 1)
            {
                var friendlyNames = new List<string>();
                foreach (var singleTable in tableNames)
                {
                    friendlyNames.Add(await GetFriendlyNameForSingleTableAsync(conn, schema, singleTable));
                }
                return string.Join(", ", friendlyNames);
            }

            return await GetFriendlyNameForSingleTableAsync(conn, schema, tableName.Trim());
        }

        private async Task<string> GetFriendlyNameForSingleTableAsync(DbConnection conn, string schema, string tableName)
        {
            try
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandType = System.Data.CommandType.Text;

                var p = cmd.CreateParameter();
                p.ParameterName = "@tbl";
                p.Value = tableName;
                p.DbType = System.Data.DbType.String;
                cmd.Parameters.Add(p);

                cmd.CommandText = @"
                SELECT TOP (1) COALESCE(NULLIF(ac.Title,''), NULLIF(ac.MenuText,''), NULLIF(ac.AccessCodeName,''), NULLIF(m.Title,'')) AS FriendlyName
                FROM dbo.Core_AccessCode ac
                LEFT JOIN dbo.Core_MenuTab2 m ON ac.MenuId = m.MenuId
                  WHERE m.TableName = @tbl
                ";

                var scalar = await cmd.ExecuteScalarAsync();
                if (scalar != null && scalar != DBNull.Value)
                {
                    var found = scalar.ToString();
                    if (!string.IsNullOrWhiteSpace(found))
                        return found;
                }
            }
            catch { /* swallow */ }

            try
            {
                using var cmd2 = conn.CreateCommand();
                cmd2.CommandType = System.Data.CommandType.Text;

                var pExact = cmd2.CreateParameter();
                pExact.ParameterName = "@tblExact";
                pExact.Value = tableName;
                pExact.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(pExact);

                var pLike = cmd2.CreateParameter();
                pLike.ParameterName = "@tblLike";
                pLike.Value = "%" + tableName + "%";
                pLike.DbType = System.Data.DbType.String;
                cmd2.Parameters.Add(pLike);

                cmd2.CommandText = @"
                SELECT TOP (1) COALESCE(NULLIF(ac.Title,''), NULLIF(ac.AccessCodeName,''), NULLIF(ac.MenuText,''), NULLIF(m.Title,'')) AS FriendlyName
                FROM dbo.Core_AccessCode ac
                LEFT JOIN dbo.Core_MenuTab2 m ON ac.MenuId = m.MenuId
                WHERE 
                    (LOWER(ac.MenuText) = LOWER(@tblExact) OR LOWER(ac.AccessCodeName) = LOWER(@tblExact) OR LOWER(ac.Title) = LOWER(@tblExact) OR  LOWER    (m.Title) = LOWER(@tblExact))
                UNION
                SELECT TOP (1) COALESCE(NULLIF(ac.Title,''), NULLIF(ac.MenuText,''), NULLIF(ac.AccessCodeName,''), NULLIF(m.Title,'')) AS FriendlyName
                FROM dbo.Core_AccessCode ac
                LEFT JOIN dbo.Core_MenuTab2 m ON ac.MenuId = m.MenuId
                WHERE
                    (ac.ControllerName IS NOT NULL AND LOWER(ac.ControllerName) LIKE LOWER(@tblLike))
                    OR (ac.ViewName IS NOT NULL AND LOWER(ac.ViewName) LIKE LOWER(@tblLike))
                    OR (ac.PageUrl IS NOT NULL AND LOWER(ac.PageUrl) LIKE LOWER(@tblLike))
                    OR (m.Title IS NOT NULL AND LOWER(m.Title) LIKE LOWER(@tblLike))
                ";

                var scalar2 = await cmd2.ExecuteScalarAsync();
                if (scalar2 != null && scalar2 != DBNull.Value)
                {
                    var found2 = scalar2.ToString();
                    if (!string.IsNullOrWhiteSpace(found2))
                        return found2;
                }
            }
            catch { /* swallow */ }

            var prefixes = new[]
            {
               "RMG_Prod_Def", "RMG_Prod_Temp", "RMG_Pro_BTB", "RMG_Prod", "RMG_Def", "RMG_Inv", "RMG",
               "HRM_Payroll", "HRM_ATD", "HRM_Att", "HRM_Def", "HRM_PAY", "HRM",
               "Def_Inv", "Inv_Def", "INV", "Prod_Def", "Sales_Def", "TB_Def", "CA_Def",
               "Core", "Acc", "POS", "TBM", "tbl", "dbo."
            };

            var pretty = tableName;
            foreach (var prefix in prefixes)
            {
                if (!string.IsNullOrEmpty(prefix) && pretty.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    pretty = pretty.Substring(prefix.Length);
                    break;
                }
            }

            pretty = pretty.Replace("_", " ").Trim();
            pretty = System.Text.RegularExpressions.Regex.Replace(pretty, @"(?<=[a-z])(?=[A-Z])|(?<=[A-Z])(?=[A-Z][a-z])", " ");

            if (pretty.EndsWith(" Entry", StringComparison.OrdinalIgnoreCase))
                pretty = pretty[..^6].TrimEnd();

            return string.IsNullOrWhiteSpace(pretty) ? tableName : pretty;
        }

        private bool IsTableIgnored(string tableSchema, string tableName, HashSet<string> ignoreSet)
        {
            string normalizedCandidate = tableName.ToNormalizedTableKey();

            foreach (var entry in ignoreSet)
            {
                if (string.Equals(entry, tableName, StringComparison.OrdinalIgnoreCase))
                    return true;

                if (string.Equals(entry, $"{tableSchema}.{tableName}", StringComparison.OrdinalIgnoreCase))
                    return true;

                if (string.Equals(entry.ToNormalizedTableKey(), normalizedCandidate, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        private class CandidateColumn
        {
            public string TableSchema { get; set; }
            public string TableName { get; set; }
            public string ColumnName { get; set; }
        }
    }
}
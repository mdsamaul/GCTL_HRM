using Dapper;
using GCTL.Core.Data;
using GCTL.Data.Extensions;
using GCTL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text.Json;

namespace GCTL.Service.Common
{
    public class CommonService : ICommonService
    {



        private readonly IConfiguration configuration;
        private readonly IRepository<DeleteHistory> dhRepo;
        private const string connectionStringName = "ApplicationDbConnection";
        public CommonService(IConfiguration configuration,
            IRepository<DeleteHistory> dhRepo
            )
        {
            this.configuration = configuration;
            this.dhRepo = dhRepo;

        }

        public void FindMaxNo(ref string strMaxNo, string strFldName, string strTableName, int intLenWithPadding)
        {
            string query = "Select isnull(MAX(convert(int," + strFldName + "))+1,0) as MaxNo from " + strTableName + "";
            int result = QueryExtensionsHelpers.QuerySingle<int>(configuration.GetConnectionString(connectionStringName), query);
            if (result != 0)
            {
                strMaxNo = result.ToString();
                strMaxNo = strMaxNo.PadLeft(intLenWithPadding, '0');
            }
            else
            {
                strMaxNo = "1";
                strMaxNo = strMaxNo.PadLeft(intLenWithPadding, '0');
            }
        }
        public void FindAccTwoDigit(ref string strMaxNo, string strFldName, string strTableName, int intLenWithPadding, string WhereColumn, string WhereValue)
        {
            try
            {
                string Query = "Select isnull(max(right(" + strFldName + ",2)),0)+1 as MaxNo from " + strTableName + " where " + WhereColumn + "='" + WhereValue + "'";
                int result = QueryExtensionsHelpers.QuerySingle<int>(configuration.GetConnectionString(connectionStringName), Query);
                if (result != 0)
                {
                    strMaxNo = result.ToString();
                    strMaxNo = strMaxNo.PadLeft(intLenWithPadding, '0');
                }
                else
                {
                    strMaxNo = "1";
                    strMaxNo = strMaxNo.PadLeft(intLenWithPadding, '0');
                }
            }
            catch (System.Exception ex)
            {
                string a = ex.Message.ToString();

            }
            finally
            {


            }
        }
        public string NextCode(string fieldName, string table, int length)
        {
            string nextCode = string.Empty;
            string query = "Select isnull(MAX(convert(int," + fieldName + "))+1,0) as MaxNo from " + table + "";
            int result = QueryExtensionsHelpers.QuerySingle<int>(configuration.GetConnectionString(connectionStringName), query);
            if (result != 0)
            {
                nextCode = result.ToString();
            }
            else
            {
                nextCode = "1";

            }

            return nextCode.PadLeft(length, '0'); ;
        }
        public void FindMaxGCTL(ref string strMaxNo, string strFldName, string strTableName, int intLenWithPadding, string WhereColumn, string WhereValue)
        {
            string query = "Select isnull(max(right(" + strFldName + ",6)),0)+1 as MaxNo from " + strTableName + " where left(right(" + WhereColumn + ",8),2)='" + WhereValue + "'";
            int result = QueryExtensionsHelpers.QuerySingle<int>(configuration.GetConnectionString(connectionStringName), query);
            if (result != 0)
            {
                strMaxNo = result.ToString();
                strMaxNo = strMaxNo.PadLeft(intLenWithPadding, '0');
            }
            else
            {
                strMaxNo = "1";
                strMaxNo = strMaxNo.PadLeft(intLenWithPadding, '0');
            }
        }

        public void FindMaxNoAuto(ref string strMaxNo, string strFldName, string strTableName)
        {
            string query = "Select isnull(MAX(convert(int," + strFldName + "))+1,0) as MaxNo from " + strTableName + "";
            int result = QueryExtensionsHelpers.QuerySingle<int>(configuration.GetConnectionString(connectionStringName), query);
            if (result != 0)
            {
                strMaxNo = result.ToString();
            }
            else
            {
                strMaxNo = "1";
            }
        }

        public void FindAccThreeDigit(ref string strMaxNo, string strFldName, string strTableName, int intLenWithPadding, string WhereColumn, string WhereValue)
        {
            string query = "Select isnull(max(right(" + strFldName + ",3)),0)+1 as MaxNo from " + strTableName + " where " + WhereColumn + "='" + WhereValue + "'";
            int result = QueryExtensionsHelpers.QuerySingle<int>(configuration.GetConnectionString(connectionStringName), query);
            if (result != 0)
            {
                strMaxNo = result.ToString();
                strMaxNo = strMaxNo.PadLeft(intLenWithPadding, '0');
            }
            else
            {
                strMaxNo = "1";
                strMaxNo = strMaxNo.PadLeft(intLenWithPadding, '0');
            }
        }

        public void FindAccFourDigit(ref string strMaxNo, string strFldName, string strTableName, int intLenWithPadding, string WhereColumn, string WhereValue)
        {
            string query = "Select isnull(max(right(" + strFldName + ",4)),0)+1 as MaxNo from " + strTableName + " where " + WhereColumn + "='" + WhereValue + "'";
            int result = QueryExtensionsHelpers.QuerySingle<int>(configuration.GetConnectionString(connectionStringName), query);
            if (result != 0)
            {
                strMaxNo = result.ToString();
                strMaxNo = strMaxNo.PadLeft(intLenWithPadding, '0');
            }
            else
            {
                strMaxNo = "1";
                strMaxNo = strMaxNo.PadLeft(intLenWithPadding, '0');
            }
        }
        public void FindAccFiveDigit(ref string strMaxNo, string strFldName, string strTableName, int intLenWithPadding, string WhereColumn, string WhereValue)
        {
            string query = "Select isnull(max(right(" + strFldName + ",5)),0)+1 as MaxNo from " + strTableName + " where " + WhereColumn + "='" + WhereValue + "'";
            int result = QueryExtensionsHelpers.QuerySingle<int>(configuration.GetConnectionString(connectionStringName), query);
            if (result != 0)
            {
                strMaxNo = result.ToString();
                strMaxNo = strMaxNo.PadLeft(intLenWithPadding, '0');
            }
            else
            {
                strMaxNo = "1";
                strMaxNo = strMaxNo.PadLeft(intLenWithPadding, '0');
            }
        }

        public string GenerateCode(string columnName, string tableName, string prefix = "", int length = 8)
        {
            string query = $"Select CONCAT('{prefix}',FORMAT(ISNULL(Max(CAST(RIGHT({columnName},{length}) as int)),0)+1,'00000000')) MaxCode from {tableName}";
            return QueryExtensionsHelpers.QuerySingle<string>(configuration.GetConnectionString(connectionStringName), query);
        }


        public string GenerateNextCode(string field, string table, int length = 3, string prefix = "")
        {
            string result = string.Empty;
            string query = $"Select isnull(max(right({field},{length})),0)+1 as MaxNo from " + table;
            int nextNumber = QueryExtensionsHelpers.QuerySingle<int>(configuration.GetConnectionString(connectionStringName), query);
            if (nextNumber != 0)
            {
                result = nextNumber.ToString();
                result = result.PadLeft(length, '0');
            }
            else
            {
                result = "1";
                result = result.PadLeft(length, '0');
            }

            if (!string.IsNullOrWhiteSpace(prefix))
                result = $"{prefix}{result}";

            return result;
        }


        public string GenerateNextNumber(string field, string table, int length = 2, string prefix = "")
        {
            string query = $"SELECT COALESCE(CAST(MAX(RIGHT(COALESCE({field}, ''), CHARINDEX('_', REVERSE(COALESCE({field}, '')) + '_') - 1)) AS INT), 0) From {table}";
            int number = QueryExtensionsHelpers.QuerySingle<int>(configuration.GetConnectionString(connectionStringName), query);
            string result;
            if (number > 0)
            {
                number++;
            }
            else
            {
                number = 1;
            }

            result = number.ToString().PadLeft(length, '0');

            if (!string.IsNullOrWhiteSpace(prefix))
                result = $"{prefix}{result}";

            return result;
        }
        public void FindVoucherNo(ref string strMaxNo, string VoucherType_Code)
        {
            string Query = "SELECT CONCAT((Select Voucher_TypeName from Acc_VoucherType"
           + " where VoucherType_Code = '" + VoucherType_Code + "'),'_',RIGHT(CONVERT(VARCHAR(8), GETDATE(), 1), 2),'_',"
           + " FORMAT(CONVERT(INT, ISNULL(MAX(CAST(RIGHT(VoucherNo, 6) as int)), 0) + 1), '000000')) MaxNo"
           + " From Acc_VoucherEntry Where LEFT(RIGHT(VoucherNo, 9), 2) = RIGHT(CONVERT(VARCHAR(8), GETDATE(), 1), 2) AND VoucherType_Code = '" + VoucherType_Code + "'";
            string result = QueryExtensionsHelpers.QuerySingle<string>(configuration.GetConnectionString(connectionStringName), Query);
            if (result != "")
            {
                strMaxNo = result.ToString();
            }

            else
            {
                strMaxNo = "";
            }

        }
        public void MaxNoWithYearAndTwoDight(ref string strMaxNo, string strFldName, string strTableName, int intLenWithPadding, string StartTwoDight)
        {
            string Query = "SELECT CONCAT('" + StartTwoDight + "','_',RIGHT(CONVERT(VARCHAR(8), GETDATE(), 1), 2),'_',"
           + " FORMAT(CONVERT(INT, ISNULL(MAX(CAST(RIGHT(" + strFldName + ", 6) as int)), 0) + 1), '000000')) MaxNo"
           + " From " + strTableName + " Where LEFT(RIGHT(" + strFldName + ", 9), 2) = RIGHT(CONVERT(VARCHAR(8), GETDATE(), 1), 2)";
            string result = QueryExtensionsHelpers.QuerySingle<string>(configuration.GetConnectionString(connectionStringName), Query);
            if (result != "")
            {
                strMaxNo = result.ToString();
            }

            else
            {
                strMaxNo = "";
            }

        }



        //    public List<string> CheckReferenceBeforeDelete(string tableName, string keyField, object keyValue)
        //    {
        //        var tablesReferencing = new List<string>();

        //        // Step 1: Get all tables in db having the keyField column except the current table
        //        string query = @"
        //    SELECT TABLE_NAME
        //    FROM INFORMATION_SCHEMA.COLUMNS
        //    WHERE COLUMN_NAME = @KeyField
        //    AND TABLE_NAME != @TableName
        //";

        //        using (var conn = new SqlConnection(configuration.GetConnectionString(connectionStringName)))
        //        {
        //            conn.Open();
        //            var tables = conn.Query<string>(query, new { KeyField = keyField, TableName = tableName }).ToList();

        //            foreach (var otherTable in tables)
        //            {
        //                // Step 2: check if key exists in that table
        //                string checkQuery = $"SELECT COUNT(1) FROM {otherTable} WHERE {keyField} = @KeyValue";
        //                int count = conn.ExecuteScalar<int>(checkQuery, new { KeyValue = keyValue });
        //                if (count > 0)
        //                    tablesReferencing.Add(otherTable);
        //            }
        //        }

        //        return tablesReferencing; // ✅ return now in all code paths
        //    }




        //public async Task SaveDeleteHistoryAsync(string tableName, object deletedRow)
        //{
        //    try
        //    {
        //        var dh = new DeleteHistory
        //        {
        //            TableName = tableName,
        //            Dhid= long.Parse(GenerateNextNumber("Dhid", "DeleteHistory",1))
        //        };

        //        // Reflection: Field1..Field100
        //        var props = deletedRow.GetType().GetProperties();
        //        for (int i = 0; i < props.Length && i < 100; i++)
        //        {
        //            var value = props[i].GetValue(deletedRow)?.ToString();

        //            // Correct reflection: use GetProperty with string name
        //            var prop = typeof(DeleteHistory).GetProperty($"Field{i + 1}");
        //            if (prop != null)
        //                prop.SetValue(dh, value);
        //        }

        //        // Dapper Insert
        //        string insertQuery = @"INSERT INTO DeleteHistory (TableName, Field1, Field2, Field3, Field4)
        //                   VALUES (@TableName, @Field1, @Field2, @Field3, @Field4, )";

        //        using (var conn = new SqlConnection(configuration.GetConnectionString(connectionStringName)))
        //        {
        //            await conn.ExecuteAsync(insertQuery, dh);
        //        }
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }

        //}



        // Check references and return table + controller + title
        //        public List<(string TableName, string ControllerName, string Title)> CheckReferenceBeforeDelete(string tableName, string keyField, object keyValue)
        //        {
        //            try
        //            {
        //                var result = new List<(string TableName, string ControllerName, string Title)>();

        //                string query = @"
        //SELECT TABLE_NAME
        //FROM INFORMATION_SCHEMA.COLUMNS
        //WHERE COLUMN_NAME = @KeyField
        //AND TABLE_NAME != @TableName
        //";

        //                using (var conn = new SqlConnection(configuration.GetConnectionString(connectionStringName)))
        //                {
        //                    conn.Open();
        //                    var tables = conn.Query<string>(query, new { KeyField = keyField, TableName = tableName }).ToList();

        //                    foreach (var otherTable in tables)
        //                    {
        //                        string checkQuery = $"SELECT COUNT(1) FROM {otherTable} WHERE {keyField} = @KeyValue";
        //                        int count = conn.ExecuteScalar<int>(checkQuery, new { KeyValue = keyValue });

        //                        if (count > 0)
        //                        {
        //                            tableAccessMap.TryGetValue(otherTable, out var map);
        //                            result.Add((otherTable, map.ControllerName, map.Title));
        //                        }
        //                    }
        //                }

        //                return result;
        //            }
        //            catch (Exception)
        //            {

        //                throw;
        //            }


        //        }



        //    public List<(string TableName, string ControllerName, string Title)> CheckReferenceBeforeDelete(string tableName, string keyField, object keyValue)
        //    {
        //        var result = new List<(string TableName, string ControllerName, string Title)>();

        //        string query = @"
        //    SELECT TABLE_NAME
        //    FROM INFORMATION_SCHEMA.COLUMNS
        //    WHERE COLUMN_NAME = @KeyField
        //    AND TABLE_NAME != @TableName
        //";

        //        using (var conn = new SqlConnection(configuration.GetConnectionString(connectionStringName)))
        //        {
        //            conn.Open();
        //            var tables = conn.Query<string>(query, new { KeyField = keyField, TableName = tableName }).ToList();

        //            foreach (var otherTable in tables)
        //            {
        //                string checkQuery = $"SELECT COUNT(1) FROM {otherTable} WHERE {keyField} = @KeyValue";
        //                int count = conn.ExecuteScalar<int>(checkQuery, new { KeyValue = keyValue });

        //                if (count > 0)
        //                {
        //                    // 🔹 Core_AccessCode থেকে ControllerName + Title match
        //                    var map = tableAccessMap
        //                        .FirstOrDefault(x => string.Equals(x.Key, otherTable, StringComparison.OrdinalIgnoreCase))
        //                        .Value;

        //                    // যদি mapping না থাকে, fallback null-safe
        //                    string controllerName = map.ControllerName ?? "UnknownController";
        //                    string title = map.Title ?? otherTable;

        //                    result.Add((otherTable, controllerName, title));
        //                }
        //            }
        //        }

        //        return result;
        //    }

        public List<(string TableName, string ControllerName, string Title)> CheckReferenceBeforeDelete(
    string tableName, string keyField, object keyValue)
        {
            var result = new List<(string TableName, string ControllerName, string Title)>();

            string query = @"
        SELECT TABLE_NAME
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE COLUMN_NAME = @KeyField
        AND TABLE_NAME != @TableName
    ";

            using (var conn = new SqlConnection(configuration.GetConnectionString(connectionStringName)))
            {
                conn.Open();
                var tables = conn.Query<string>(query, new { KeyField = keyField, TableName = tableName }).ToList();

                // Controllers + Titles load
                var controllers = LoadControllerAccessMap(); // Dictionary<string, string>

                foreach (var otherTable in tables)
                {
                    string checkQuery = $"SELECT COUNT(1) FROM {otherTable} WHERE {keyField} = @KeyValue";
                    int count = conn.ExecuteScalar<int>(checkQuery, new { KeyValue = keyValue });

                    if (count > 0)
                    {
                        var bestMatchController = GetBestMatchingController(otherTable, controllers.Keys.ToList());
                        string controllerName = bestMatchController ?? otherTable;
                        string title = controllerName != otherTable ? controllers[controllerName] : otherTable;

                        result.Add((otherTable, controllerName, title));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// TableName এর keywords এবং ControllerName এর keywords এর মধ্যে সবচেয়ে বেশি মিল আছে এমন controller select করে
        /// </summary>
        private string GetBestMatchingController(string tableName, List<string> controllerNames)
        {
            var tableKeywords = SplitKeywords(tableName);

            string bestController = null;
            int maxMatches = 0;

            foreach (var controller in controllerNames)
            {
                var controllerKeywords = SplitKeywords(controller);

                // matching keywords count
                int matches = tableKeywords.Count(tk => controllerKeywords.Any(ck => string.Equals(tk, ck, StringComparison.OrdinalIgnoreCase)));

                if (matches > maxMatches)
                {
                    maxMatches = matches;
                    bestController = controller;
                }
                // tie-breaker: choose longest match
                else if (matches == maxMatches && bestController != null)
                {
                    if (controller.Length > bestController.Length)
                        bestController = controller;
                }
            }

            return bestController;
        }

        /// <summary>
        /// underscore, dash, camelCase অনুযায়ী keywords split করে list return করে
        /// </summary>
        private List<string> SplitKeywords(string name)
        {
            if (string.IsNullOrEmpty(name)) return new List<string>();

            // split by _ or -
            var parts = name.Split(new[] { '_', '-' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            // camelCase split
            var finalParts = new List<string>();
            foreach (var part in parts)
            {
                var camelParts = System.Text.RegularExpressions.Regex
                    .Matches(part, @"([A-Z][a-z0-9]+|[a-z0-9]+)")
                    .Cast<System.Text.RegularExpressions.Match>()
                    .Select(m => m.Value)
                    .ToList();

                finalParts.AddRange(camelParts);
            }

            return finalParts;
        }

        /// <summary>
        /// ControllerName, Title map load
        /// </summary>
        public Dictionary<string, string> LoadControllerAccessMap()
        {
            string query = @"
        SELECT ControllerName, Title
        FROM Core_AccessCode
        WHERE IsActive = 1 AND ControllerName IS NOT NULL
    ";

            using (var conn = new SqlConnection(configuration.GetConnectionString(connectionStringName)))
            {
                conn.Open();

                var list = conn.Query<(string ControllerName, string Title)>(query).ToList();

                var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                foreach (var item in list)
                {
                    if (string.IsNullOrEmpty(item.ControllerName))
                        continue;

                    var title = string.IsNullOrEmpty(item.Title) ? item.ControllerName : item.Title;

                    if (!dict.ContainsKey(item.ControllerName))
                        dict.Add(item.ControllerName, title);
                }

                Console.WriteLine($"Controller count in map: {dict.Count}");
                return dict;
            }
        }




        // Log deleted records
        public async Task<bool> LogDeletedRecordsAsync<T>(List<T> entities, string tableName) where T : class
        {
            if (entities == null || !entities.Any()) return false;

            try
            {
                var deleteHistoryRecords = new List<DeleteHistory>();

                foreach (var entity in entities)
                {

                    string jsonData = JsonSerializer.Serialize(
                entity,
                new JsonSerializerOptions
                {
                    WriteIndented = false,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                    var deleteHistory = new DeleteHistory
                    {
                        Dhid = long.Parse(GenerateNextNumber("Dhid", "DeleteHistory", 1)),
                        TableName = tableName,

                    };

                    var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
                    for (int i = 0; i < Math.Min(properties.Length, 100); i++)
                    {
                        var property = properties[i];
                        var value = property.GetValue(entity);
                        var fieldProperty = typeof(DeleteHistory).GetProperty($"Field{i + 1}");
                        fieldProperty?.SetValue(deleteHistory, value?.ToString() ?? "");
                    }

                    deleteHistoryRecords.Add(deleteHistory);
                }

                await dhRepo.AddRangeAsync(deleteHistoryRecords);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //public async Task<bool> LogDeletedRecordsAsync<T>(List<T> entities, string tableName) where T : class
        //{
        //    if (entities == null || !entities.Any())
        //        return false;

        //    try
        //    {
        //        var deleteHistoryRecords = new List<DeleteHistory>();

        //        foreach (var entity in entities)
        //        {
        //            var deleteHistory = new DeleteHistory
        //            {
        //                Dhid = long.Parse(GenerateNextNumber("Dhid", "DeleteHistory", 1)),
        //                TableName = tableName
        //            };

        //            // Get all public instance properties of the entity
        //            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        //            // Map properties to Field1, Field2, ..., Field100
        //            for (int i = 0; i < Math.Min(properties.Length, 100); i++)
        //            {
        //                var property = properties[i];
        //                var value = property.GetValue(entity);
        //                var fieldName = $"Field{i + 1}";

        //                // Convert value to string
        //                var fieldValue = value?.ToString() ?? "";
        //                // Use reflection to set DeleteHistory.FieldX
        //                var fieldProperty = typeof(DeleteHistory).GetProperty(fieldName);
        //                fieldProperty?.SetValue(deleteHistory, fieldValue);
        //            }

        //            deleteHistoryRecords.Add(deleteHistory);

        //        }

        //        // Save all delete history records at once
        //        await dhRepo.AddRangeAsync(deleteHistoryRecords);
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
    }
}




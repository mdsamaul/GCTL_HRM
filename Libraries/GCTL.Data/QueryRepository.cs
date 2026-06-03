using GCTL.Data.Models;
using System.Data;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using GCTL.Core.Data;

namespace GCTL.Data
{
    public class QueryRepository<T> : IQueryRepository<T> where T : class
    {
        private readonly GCTL_ERP_DB_DatapathContext context;

        public QueryRepository(GCTL_ERP_DB_DatapathContext context)
        {
            this.context = context;
        }

        public IEnumerable<T> Query(string sql, object? param = null, CommandType commandType = CommandType.Text)
        {
            using (IDbConnection conn = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
            {
                return conn.Query<T>(sql, param, commandType: commandType);
            }
        }

        public T QuerySingle(string sql, object? param = null, CommandType commandType = CommandType.Text)
        {
            using (IDbConnection conn = new SqlConnection(context.Database.GetDbConnection().ConnectionString))
            {
                return conn.QueryFirstOrDefault<T>(sql, param, commandType: commandType);
            }
        }
    }
}

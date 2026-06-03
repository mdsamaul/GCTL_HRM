using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace GCTL.Data.Extensions
{
    public static class QueryExtensions<T>
    {
        public static IEnumerable<T> Query(string connectionString, string sql, object? param = null, CommandType commandType = CommandType.Text)
        {
            using (IDbConnection conn = new SqlConnection(connectionString))
            {
                return conn.Query<T>(sql, param, commandType: commandType);
            }
        }
    }
}

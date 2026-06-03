using System.Data;

namespace GCTL.Core.Data
{
    public interface IQueryRepository<T> where T : class
    {
        IEnumerable<T> Query(string sql, object param = null, CommandType commandType = CommandType.Text);
        T QuerySingle(string sql, object param = null, CommandType commandType = CommandType.Text);
    }
}
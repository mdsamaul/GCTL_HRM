using GCTL.Core.DataTables;
using GCTL.Core.ViewModels.Loggers;
using GCTL.Data.Models;

namespace GCTL.Service.LogsLoggers
{
    public interface ILogService
    {
        List<Logs> GetLogs();
        PagedList<Logs> GetPagedLogs(DataTablesOptions<LogFilterRequest> options);
    }
}
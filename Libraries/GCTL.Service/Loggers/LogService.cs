using Microsoft.Extensions.Options;
using GCTL.Core.Data;
using GCTL.Core.DataTables;
using GCTL.Core.ViewModels.Loggers;
using GCTL.Data.Models;
using GCTL.Service.LogsLoggers;

namespace GCTL.Service.Loggers
{
    public class LogService : AppService<Logs>, ILogService
    {
        private readonly IRepository<Logs> logRepository;

        public LogService(IRepository<Logs> logRepository)
            : base(logRepository)
        {
            this.logRepository = logRepository;
        }

        public List<Logs> GetLogs()
        {
            return logRepository.All().ToList();
        }

        public PagedList<Logs> GetPagedLogs(DataTablesOptions<LogFilterRequest> options)
        {
            return logRepository.All()
                .ApplyFilter(options)
                .ApplySearch(options)
                .ApplySort(options)
                .ToPagedList(options);
        }
    }
}

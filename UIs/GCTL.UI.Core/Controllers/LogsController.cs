using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using GCTL.Core.DataTables;
using GCTL.Core.ViewModels.Loggers;
using GCTL.Service.LogsLoggers;

namespace GCTL.UI.Core.Controllers
{
    public class LogsController : BaseController
    {
        private readonly ILogService logService;
        private readonly IMapper mapper;
        public LogsController(ILogService logService,
                              IMapper mapper)
        {
            this.logService = logService;
            this.mapper = mapper;
        }

        public IActionResult Index()
        {
            LogFilterRequest request = new LogFilterRequest();
            return View(request);
        }

        public ActionResult Grid()
        {
            var result = logService.GetLogs();
            return Json(new { data = result });
        }

        [HttpPost]
        public IActionResult GetLogs(DataTablesOptions<LogFilterRequest> options)
        {
            var logs = logService.GetPagedLogs(options);
            return DataTablesResult(logs);
        }
    }
}
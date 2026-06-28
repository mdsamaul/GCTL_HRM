using GCTL.Core.ViewModels.DailyAttendanceSummaryReport;
using GCTL.Service.DailyAttendanceSummaryReportService;
using GCTL.Service.RosterScheduleReport;
using GCTL.UI.Core.ViewModels.DailyAttendanceSummaryReport;
using GCTL.UI.Core.ViewModels.RosterScheduleEntry;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class DailyAttendanceSummaryReportController : BaseController
    {
        private readonly IDailyAttendanceSummaryReportService _dailyAttendanceSummaryReportService;

        public DailyAttendanceSummaryReportController(IDailyAttendanceSummaryReportService dailyAttendanceSummaryReportService)
        {
            _dailyAttendanceSummaryReportService = dailyAttendanceSummaryReportService;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var hasPermission = await _dailyAttendanceSummaryReportService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }
            DailyAttendanceSummaryViewModel model = new DailyAttendanceSummaryViewModel()
            {
                PageUrl = Url.Action(nameof(IndexAsync))
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetSummary([FromBody] DailyAttendanceSummaryFilterDto filter)
        {
            try
            {
                var data = await _dailyAttendanceSummaryReportService.GetSummaryAsync(filter);
                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DownloadExcel([FromBody] DailyAttendanceSummaryFilterDto filter)
        {
            try
            {
                var data = await _dailyAttendanceSummaryReportService.GetSummaryAsync(filter);
                var fileBytes = _dailyAttendanceSummaryReportService.GenerateExcel(data);

                return File(
                    fileBytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"DailyAttendanceSummaryReport_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                );
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}

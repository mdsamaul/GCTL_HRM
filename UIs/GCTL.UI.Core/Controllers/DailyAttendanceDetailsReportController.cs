using GCTL.Core.Data;
using GCTL.Core.ViewModels.DailyAttendanceDetailsReport;
using GCTL.Data.Models;
using GCTL.Service.DailyAttendanceDetailsReport;
using GCTL.UI.Core.ViewModels.DailyAttendanceDetailsReport;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GCTL.UI.Core.Controllers
{
    public class DailyAttendanceDetailsReportController : BaseController
    {
        private readonly IDailyAttendanceDetailsReportService _service;
        private readonly IRepository<HrmAttendanceType> attendanceTypeRepo;
        private readonly IWebHostEnvironment _env;

        public DailyAttendanceDetailsReportController(
            IDailyAttendanceDetailsReportService service,
            IRepository<HrmAttendanceType> attendanceTypeRepo,
            IWebHostEnvironment env)
        {
            _service = service;
            this.attendanceTypeRepo = attendanceTypeRepo;
            _env = env;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var hasPermission = await _service.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
                return RedirectToAction("Login", "Accounts");
            ViewBag.AttendanceTypeList = new SelectList(attendanceTypeRepo.All().Select(x => new { x.AttendanceTypeCode, x.AttendanceTypeName }), "AttendanceTypeCode", "AttendanceTypeName");
            ViewBag.AccessCode = LoginInfo.AccessCode;
            var model = new DailyAttendanceDetailsReportViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetSummary([FromBody] DailyAttendanceDetailsFilterDto filter)
        {
            try
            {
                filter.LoginEmployeeId = LoginInfo.EmployeeId;
                filter.AccessCodeId = LoginInfo.AccessCode;

                var data = await _service.GetReportDataAsync(filter);
                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DownloadExcel([FromBody] DailyAttendanceDetailsFilterDto filter)
        {
            try
            {
                filter.LoginEmployeeId = LoginInfo.EmployeeId;
                filter.AccessCodeId = LoginInfo.AccessCode;

                var logoPath = Path.Combine(_env.WebRootPath, "images", "DP_logo.png");
                var bytes = await _service.ExportExcelAsync(filter, logoPath);
                var fileName = $"DailyAttendance_{filter.ReportType}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                return File(bytes,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
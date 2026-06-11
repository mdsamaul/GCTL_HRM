using GCTL.Service.Common;
using GCTL.Service.DashboardAttendance;
using GCTL.UI.Core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GCTL.UI.Core.Controllers
{
    [Route("Dashboard")]
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICommonService _commonService;
        private readonly IDashboardAttendanceService _attendanceSvc;

        public HomeController(
            ILogger<HomeController> logger,
            ICommonService commonService,
            IDashboardAttendanceService attendanceSvc)
        {
            _logger = logger;
            _commonService = commonService;
            _attendanceSvc = attendanceSvc;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public IActionResult Index() => View();

        // ── Attendance DataTable ───────────────────────────────
        [HttpPost("attendance-datatable")]
        public async Task<IActionResult> AttendanceDataTable()
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                var length = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "10");
                var search = Request.Form["search[value]"].FirstOrDefault() ?? "";
                var companyCode = Request.Form["companyCode"].FirstOrDefault();
                var branchCode = Request.Form["branchCode"].FirstOrDefault();
                var departmentCode = Request.Form["departmentCode"].FirstOrDefault();

                if (length < 5) length = 5;

                int page = (start / length) + 1;
                int pageSize = length;

                var (summary, items, total) = await _attendanceSvc.GetAttendanceMovementAsync(
                    string.IsNullOrWhiteSpace(companyCode) ? null : companyCode.Trim(),
                    string.IsNullOrWhiteSpace(branchCode) ? null : branchCode.Trim(),
                    string.IsNullOrWhiteSpace(departmentCode) ? null : departmentCode.Trim(),
                    DateTime.Today,
                    page,
                    pageSize,
                    string.IsNullOrWhiteSpace(search) ? null : search.Trim());

                var rows = items.Select(x => new
                {
                    rowNum = x.RowNum,
                    employeeId = x.EmployeeId ?? "",
                    name = x.Name ?? "",
                    designation = x.Designation ?? "",
                    checkIn = x.CheckIn ?? "",
                    checkOut = x.CheckOut ?? "",
                    movement = x.Movement ?? "",
                    remarks = x.Remarks ?? "",    // ← নতুন
                    status = x.Status ?? "",
                    statusOrder = x.StatusOrder,
                    lateByMinutes = x.LateByMinutes,          // ← নতুন
                    dataDate = x.DataDate.ToString("dd MMM yyyy"),
                    photoSrc = x.Photo != null && x.Photo.Length > 0
                                        ? $"data:{x.ImgType};base64,{Convert.ToBase64String(x.Photo)}"
                                        : (string?)null
                }).ToList();

                var summaryOut = new
                {
                    totalEmployees = summary.TotalEmployees,
                    presentCount = summary.PresentCount,
                    absentCount = summary.AbsentCount,
                    lateCount = summary.LateCount,
                    onLeaveCount = summary.OnLeaveCount,
                    presentPct = summary.PresentPct,
                    absentPct = summary.AbsentPct,
                    latePct = summary.LatePct,
                    onLeavePct = summary.OnLeavePct,
                    dataDate = summary.DataDate.ToString("dd MMM yyyy"),
                    isToday = summary.DataDate.Date == DateTime.Today
                };

                return Json(new
                {
                    draw = Convert.ToInt32(draw),
                    recordsTotal = total,
                    recordsFiltered = total,
                    data = rows,
                    summary = summaryOut
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        // ── Leave Dashboard — Server-side pagination ───────────
        [HttpPost("leave-dashboard")]
        public async Task<IActionResult> LeaveDashboard()
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
                var length = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "10");
                var search = Request.Form["search[value]"].FirstOrDefault() ?? "";
                var companyCode = Request.Form["companyCode"].FirstOrDefault();
                var branchCode = Request.Form["branchCode"].FirstOrDefault();
                var departmentCode = Request.Form["departmentCode"].FirstOrDefault();
                var yearStr = Request.Form["year"].FirstOrDefault();
                var employeeId = Request.Form["employeeId"].FirstOrDefault();   // ← নতুন

                if (length < 5) length = 5;
                int page = (start / length) + 1;
                int pageSize = length;
                int year = int.TryParse(yearStr, out var y) ? y : DateTime.Now.Year;

                var result = await _attendanceSvc.GetLeaveDashboardAsync(
                    string.IsNullOrWhiteSpace(companyCode) ? null : companyCode.Trim(),
                    string.IsNullOrWhiteSpace(branchCode) ? null : branchCode.Trim(),
                    string.IsNullOrWhiteSpace(departmentCode) ? null : departmentCode.Trim(),
                    year,
                    page,
                    pageSize,
                    string.IsNullOrWhiteSpace(search) ? null : search.Trim(),
                    string.IsNullOrWhiteSpace(employeeId) ? null : employeeId.Trim()  // ← নতুন
                );

                return Json(new
                {
                    draw = Convert.ToInt32(draw),
                    recordsTotal = result.TotalCount,
                    recordsFiltered = result.TotalCount,
                    data = result.Employees,
                    summary = result.Summary,
                    leaveTypes = result.LeaveTypes
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
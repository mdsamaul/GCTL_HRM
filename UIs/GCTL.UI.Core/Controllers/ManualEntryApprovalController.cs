using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.AttendanceMovementRegisterReportDto;
using GCTL.Core.ViewModels.ManualEntryApproval;
using GCTL.Service.ManualEarnLeaveEntry;
using GCTL.Service.ManualEntryApprovalService;
using GCTL.UI.Core.ViewModels.ManualEntryApproval;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class ManualEntryApprovalController : BaseController
    {
        private readonly IManualEntryApprovalService manualEntryApprovalService;

        public ManualEntryApprovalController(IManualEntryApprovalService manualEntryApprovalService)
        {
            this.manualEntryApprovalService = manualEntryApprovalService;
        }
        private void InjectLoginInfo(ManualEntryApprovalFilterDto req)
        {
            req.AccessCode = LoginInfo.AccessCode;
            req.EmployeeId = LoginInfo.EmployeeId;
        }
        public async Task<IActionResult> Index()
        {
            var hasPermission = await manualEntryApprovalService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }
            ViewBag.AccessCode = LoginInfo.AccessCode;
            ManualEntryApprovalSetupViewModel model = new ManualEntryApprovalSetupViewModel()
            {
                PageUrl = Url.Action(nameof(Index)),
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetManualEntryFilter([FromBody] ManualEntryApprovalFilterDto filterDto)
        {
            InjectLoginInfo(filterDto);
            var result = await manualEntryApprovalService.GetManualEntryDataAsync(filterDto);
            if (result != null)
            {
                return Json(new { isSuccess = true, message = "Success data load", data = result });
            }
            return Json(new { isSuccess = false, message = "Data load failed" });
        }

        [HttpPost]
        public async Task<IActionResult> ApprovalSetUp([FromBody] ManualApprovalRequest modelData)
        {
            if (modelData == null)
            {
                return Json(new { isSuccess = false, message = "Invalid request" });
            }
            var hasPermission = await manualEntryApprovalService.SavePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
            }
            modelData.ToAudit(LoginInfo);
            var result = await manualEntryApprovalService.ApprovalManualEntries(modelData);
            return Json(new { isSuccess = result.isSuccess, message = result.isMessage });
        }

        [HttpPost]
        public async Task<IActionResult> GetManualEntryApproveGrid()
        {
            try
            {
                var draw = HttpContext.Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumn = Request.Form[$"columns[{sortColumnIndex}][data]"].FirstOrDefault();
                var sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;

                var data = await manualEntryApprovalService.GetManualEntryGridService();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    data = data.Where(d =>
                        (!string.IsNullOrEmpty(d.EmployeeId) && d.EmployeeId.Contains(searchValue, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(d.EmployeeName) && d.EmployeeName.Contains(searchValue, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(d.ManualCode) && d.ManualCode.Contains(searchValue, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(d.AttendanceTypeName) && d.AttendanceTypeName.Contains(searchValue, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(d.DesignationName) && d.DesignationName.Contains(searchValue, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(d.Remarks) && d.Remarks.Contains(searchValue, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortDirection))
                {
                    var propInfo = typeof(ManualEntryApprovalSetupViewModelDto).GetProperty(
                        sortColumn,
                        System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance
                    );

                    if (propInfo != null)
                    {
                        data = sortDirection == "asc"
                            ? data.OrderBy(x => propInfo.GetValue(x, null)).ToList()
                            : data.OrderByDescending(x => propInfo.GetValue(x, null)).ToList();
                    }
                }

                int recordsTotal = data.Count();
                List<ManualEntryApprovalSetupViewModelDto> dataPage;

                if (pageSize == -1)
                {
                    dataPage = data;
                }
                else
                {
                    dataPage = data.Skip(skip).Take(pageSize).ToList();
                }

                var modifiedData = dataPage.Select(item => new
                {
                    autoId = item.AutoId,
                    manualCode = item.ManualCode,
                    employeeId = item.EmployeeId,
                    employeeName = item.EmployeeName,
                    designationName = item.DesignationName,
                    attendanceTypeName = item.AttendanceTypeName,
                    date = item.Date,
                    time = item.Time,
                    remarks = item.Remarks,
                    approvalStatus = item.ApprovalStatus,
                    approvedBy = item.ApprovedBy,
                    approvalDatetime = item.ApprovalDatetime,
                    entryUser = item.EntryUser,
                    dayName = item.DayName,
                    showApprovalDatetime = item.ShowApprovalDatetime,
                    showDate = item.ShowDate,

                }).ToList();

                return Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = modifiedData
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = "An error occurred while loading data: " + ex.Message });
            }
        }
    }
}
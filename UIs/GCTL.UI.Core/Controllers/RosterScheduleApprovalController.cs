using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.AttendanceMovementRegisterReportDto;
using GCTL.Core.ViewModels.RosterScheduleApproval;
using GCTL.Core.ViewModels.RosterScheduleEntry;
using GCTL.Service.ManualEntryApprovalService;
using GCTL.Service.RosterScheduleApproval;
using GCTL.UI.Core.ViewModels.RosterScheduleApproval;
using GCTL.UI.Core.Views.RosterScheduleApproval;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class RosterScheduleApprovalController : BaseController
    {
        private readonly IRosterScheduleApprovalService rosterScheduleApprovalService;

        public RosterScheduleApprovalController(IRosterScheduleApprovalService rosterScheduleApprovalService)
        {
            this.rosterScheduleApprovalService = rosterScheduleApprovalService;
        }
        private void InjectLoginInfo(RosterFilterDto req)
        {
            req.AccessCode = LoginInfo.AccessCode;
            req.EmployeeId = LoginInfo.EmployeeId;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var hasPermission = await rosterScheduleApprovalService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }
            ViewBag.AccessCode = LoginInfo.AccessCode;
            RosterScheduleApprovalViewModel model = new RosterScheduleApprovalViewModel()
            {
                PageUrl = Url.Action(nameof(IndexAsync)),
            };
            return View(model);
        }

        // Controller for Cascading Dropdowns
        [HttpPost]
        public async Task<IActionResult> GetFilterDropdowns([FromBody] RosterFilterDto filterDto)
        {
            InjectLoginInfo(filterDto);
            var result = await rosterScheduleApprovalService.GetFilterDropdownsAsync(filterDto);
            if (result != null)
            {
                return Json(new { isSuccess = true, message = "Dropdowns loaded successfully", data = result });
            }
            return Json(new { isSuccess = false, message = "Failed to load dropdowns" });
        }

        // Controller for Grid Data with Server-Side Pagination
        [HttpPost]
        public async Task<IActionResult> GetRosterGridData([FromBody] RosterFilterDto filterDto)
        {
            var result = await rosterScheduleApprovalService.GetRosterGridDataAsync(filterDto);
            if (result != null)
            {
                return Json(new
                {
                    isSuccess = true,
                    message = "Data loaded successfully",
                    data = result.Employees,
                    recordsTotal = result.TotalRecords,
                    recordsFiltered = result.FilteredRecords
                });
            }
            return Json(new { isSuccess = false, message = "Failed to load data" });
        }

        [HttpPost]
        public async Task<IActionResult> ApprovalSetUp([FromBody] ApprovalRequest modelData)
        {
            try
            {
                if (modelData == null)
                {
                    return Json(new { isSuccess = false });
                }
                var hasPermission = await rosterScheduleApprovalService.SavePermissionAsync(LoginInfo.AccessCode);
                if (!hasPermission)
                {
                    return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
                }
                modelData.ToAudit(LoginInfo);
                var result = await rosterScheduleApprovalService.ApprovalRosterServices(modelData);
                return Json(result);
            }
            catch (Exception)
            {

                throw;
            }
           
        }

        [HttpPost]
        public async Task<IActionResult> GetRosterScheduleApproveGrid()
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

                var data = await rosterScheduleApprovalService.GetRosterScheduleGridService();

                if (!string.IsNullOrEmpty(searchValue))
                {
                    data = data.Where(d =>
                        (!string.IsNullOrEmpty(d.EmployeeID) && d.EmployeeID.Contains(searchValue, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(d.Name) && d.Name.Contains(searchValue, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(d.DesignationName) && d.DesignationName.Contains(searchValue, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(d.ShiftName) && d.ShiftName.Contains(searchValue, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }

                if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortDirection))
                {
                    switch (sortColumn.ToLower())
                    {
                        case "employeeid":
                            data = sortDirection == "asc" ? data.OrderBy(x => x.EmployeeID).ToList() : data.OrderByDescending(x => x.EmployeeID).ToList();
                            break;
                        case "name":
                            data = sortDirection == "asc" ? data.OrderBy(x => x.Name).ToList() : data.OrderByDescending(x => x.Name).ToList();
                            break;
                        case "designationname":
                            data = sortDirection == "asc" ? data.OrderBy(x => x.DesignationName).ToList() : data.OrderByDescending(x => x.DesignationName).ToList();
                            break;
                        case "shiftname":
                            data = sortDirection == "asc" ? data.OrderBy(x => x.ShiftName).ToList() : data.OrderByDescending(x => x.ShiftName).ToList();
                            break;
                        case "date":
                            data = sortDirection == "asc" ? data.OrderBy(x => x.Date).ToList() : data.OrderByDescending(x => x.Date).ToList();
                            break;
                        default:
                            var propInfo = typeof(RosterScheduleApprovalSetupViewModel).GetProperty(sortColumn, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                            if (propInfo != null)
                            {
                                data = sortDirection == "asc"
                                    ? data.OrderBy(x => propInfo.GetValue(x, null)).ToList()
                                    : data.OrderByDescending(x => propInfo.GetValue(x, null)).ToList();
                            }
                            break;
                    }
                }

                int recordsTotal = data.Count();
                List<RosterScheduleEntrySetupViewModel> dataPage;

                if (pageSize == -1)
                {
                    dataPage = data;
                }
                else
                {
                    dataPage = data.Skip(skip).Take(pageSize).ToList();
                }

                // Transform data to match the expected column structure
                var modifiedData = dataPage.Select(item => new
                {
                    // Removed the TC field that was used for checkbox
                    rosterScheduleId = item.RosterScheduleId,
                    employeeID = item.EmployeeID,
                    name = item.Name,
                    designationName = item.DesignationName,
                    date = item.Date,
                    shiftName = item.ShiftName,
                    remark = item.Remark,
                    approvalStatus = item.ApprovalStatus,
                    approvedBy = item.ApprovedBy,
                    approvalDatetime = item.ApprovalDatetime,
                    luser = item.Luser
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

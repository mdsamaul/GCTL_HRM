using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.ManualEarnLeaveEntry;
using GCTL.Core.ViewModels.PFAssignEntry;
using GCTL.Core.ViewModels.RosterScheduleEntry;
using GCTL.Service.ManualEarnLeaveEntry;
using GCTL.Service.ManualEntryApprovalService;
using GCTL.Service.PFAssignEntry;
using GCTL.Service.RosterScheduleEntry;
using GCTL.UI.Core.ViewModels.RosterScheduleEntry;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using OfficeOpenXml;

namespace GCTL.UI.Core.Controllers
{
    public class RosterScheduleEntryController : BaseController
    {
        private readonly IRosterScheduleEntryService rosterScheduleEntryService;

        public RosterScheduleEntryController(IRosterScheduleEntryService rosterScheduleEntryService)
        {
            this.rosterScheduleEntryService = rosterScheduleEntryService;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var hasPermission = await rosterScheduleEntryService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }
            RosterScheduleEntryViewModel model = new RosterScheduleEntryViewModel()
            {
                PageUrl = Url.Action(nameof(IndexAsync))
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetMasterBaseFilters([FromBody] MasterBaseFilterDto filterDto)
        {
            var result = await rosterScheduleEntryService.GetMasterBaseFiltersAsync(filterDto);

            if (result == null)
            {
                return Json(new
                {
                    isSuccess = false,
                    message = "Data load failed"
                });
            }

            return Json(new
            {
                isSuccess = true,
                message = "Data loaded successfully",
                data = result
            });
        }


        public async Task<IActionResult> getAllFilterEmp([FromBody] RosterScheduleEntryFilterDto filterDto)
        {
            //filterDto.EmployeeStatuses = new List<string> { "01" };
            var result = await rosterScheduleEntryService.GetFilterDataAsync(filterDto);
            if (result != null)
            {
                return Json(new { isSuccess = true, message = "successed data load", data = result });
            }
            return Json(new { isSuccess = false, message = "Data load Failed" });
        }

        //get pay manth
        [HttpGet]
        public async Task<IActionResult> GetPayMonth()
        {
            var Months = await rosterScheduleEntryService.getAllMonthService();
            return Json(new { isSuccess = true, data = Months });
        }

        //get all shift 

        [HttpGet]
        public async Task<IActionResult> getAlllShift()
        {
            var Shists = await rosterScheduleEntryService.getAlllShiftService();
            return Json(new { isSuccess = true, data = Shists });
        }

        //careate and update 
        [HttpPost]
        public async Task<IActionResult> CreateAndUpdateRosterSchedule([FromBody] RosterScheduleEntrySetupViewModel FromModel)
        {
            try
            {

                if (FromModel.isUpdate == false)
                {
                    var hasPermission = await rosterScheduleEntryService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (!hasPermission)
                    {
                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasPermission = await rosterScheduleEntryService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (!hasPermission)
                    {
                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
                    }
                }
                FromModel.ToAudit(LoginInfo, FromModel.isUpdate);
                var result = await rosterScheduleEntryService.CreateAndUpdateService(FromModel);

                
                return Json(new
                {
                    isSuccess = result.isSuccess,
                    isMessage = result.isMessage,
                    data = result.data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetRosterScheduleGrid()
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

                var data = await rosterScheduleEntryService.GetRosterScheduleGridService();

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
                    // Note: Changes in column indices due to removed checkbox column
                    switch (sortColumn.ToLower())
                    {
                        case "rosterscheduleid": // This is now the first column (index 0)
                            data = sortDirection == "asc" ? data.OrderBy(x => x.RosterScheduleId).ToList() : data.OrderByDescending(x => x.RosterScheduleId).ToList();
                            break;
                        case "employeeid":
                            data = sortDirection == "asc" ? data.OrderBy(x => x.EmployeeID).ToList() : data.OrderByDescending(x => x.EmployeeID).ToList();
                            break;
                        case "name":
                            data = sortDirection == "asc" ? data.OrderBy(x => x.Name).ToList() : data.OrderByDescending(x => x.Name).ToList();
                            break;
                        case "designationname":
                            data = sortDirection == "asc" ? data.OrderBy(x => x.DesignationName).ToList() : data.OrderByDescending(x => x.DesignationName).ToList();
                            break;
                        case "dateshow":
                            data = sortDirection == "asc" ? data.OrderBy(x => x.Date).ToList() : data.OrderByDescending(x => x.Date).ToList();
                            break;
                        case "dayname":
                            data = sortDirection == "asc" ? data.OrderBy(x => x.DayName).ToList() : data.OrderByDescending(x => x.DayName).ToList();
                            break;
                        case "shiftname":
                            data = sortDirection == "asc" ? data.OrderBy(x => x.ShiftName).ToList() : data.OrderByDescending(x => x.ShiftName).ToList();
                            break;
                        case "remark":
                            data = sortDirection == "asc" ? data.OrderBy(x => x.Remark).ToList() : data.OrderByDescending(x => x.Remark).ToList();
                            break;
                        case "luser":
                            data = sortDirection == "asc" ? data.OrderBy(x => x.Luser).ToList() : data.OrderByDescending(x => x.Luser).ToList();
                            break;
                        default:
                            var propInfo = typeof(RosterScheduleEntrySetupViewModel).GetProperty(sortColumn, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
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
                    // "All" selected, send all records
                    dataPage = data;
                }
                else
                {
                    dataPage = data.Skip(skip).Take(pageSize).ToList();
                }

                return Json(new
                {
                    draw = draw,
                    recordsFiltered = recordsTotal,
                    recordsTotal = recordsTotal,
                    data = dataPage
                });
            }
            catch (Exception ex)
            {
                return Json(new { error = "An error occurred while loading data." });
            }
        }
        [HttpPost]
        public async Task<IActionResult> EditGetRoster(string id)
        {
            var emp = await rosterScheduleEntryService.EditGetServices(id);
            if (emp == null)
            {
                return Json(new { isSuccess = false, message = "Employee Not Found" });
            }
            return Json(new { isSuccess = true, message = "Employee Found", data = emp });

        }


        public async Task<IActionResult> BulkDeleteRosterEmp([FromBody] RosterScheduleEntrySetupViewModel FromModel)
{
    try
    {
        // ✅ Permission check
        var hasPermission = await rosterScheduleEntryService.DeletePermissionAsync(LoginInfo.AccessCode);
        if (!hasPermission)
        {
            return Json(new { isSuccess = false, isMessage = "You have no access.", noSavePermission = true });
        }

        // ✅ Employee validation
        if (FromModel.EmployeeListID == null || !FromModel.EmployeeListID.Any())
        {
            return Json(new { isSuccess = false, isMessage = "Please select at least one employee." });
        }

        // ✅ Delete history model prepare
        DeleteHistoryViewModel deleteModel = new DeleteHistoryViewModel();
        deleteModel.ToAudit(LoginInfo);
        deleteModel.CompanyCode = LoginInfo.CompanyCode;

        // ✅ Service call
        var (isSuccess, hasDependency, message) = await rosterScheduleEntryService.BulkDeleteAsync(FromModel, deleteModel);

        if (hasDependency)
        {
            return Json(new { isSuccess = false, isDependency = true, isMessage = message });
        }

        if (!isSuccess)
        {
            return Json(new { isSuccess = false, isMessage = message });
        }

        return Json(new { isSuccess = true, isMessage = "Deleted successfully." });
    }
    catch (Exception ex)
    {
        return Json(new { isSuccess = false, isMessage = ex.Message });
    }
}

        //download excel

        public async Task<IActionResult> DownloadExcel()
        {
            try
            {
                var fileBytes = await rosterScheduleEntryService.GenerateEmpRosterExcelDownload();
                string excelName = $"RosterData.xlsx";
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
            }
            catch (Exception)
            {

                throw;
            }
        }
        //upload excel

        public async Task<IActionResult> UploadExcel(IFormFile excelFile, RosterScheduleEntrySetupViewModel modelVm)
        {
            var hasPermission = await rosterScheduleEntryService.SavePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
            }

            if (excelFile == null || excelFile.Length == 0)
            {
                return Json(new { isSuccess = false, message = "Please select a valid Excel file." });
            }

            var fileExtension = Path.GetExtension(excelFile.FileName)?.ToLower();
            if (fileExtension != ".xls" && fileExtension != ".xlsx")
            {
                return Json(new { isSuccess = false, message = "Only .xls and .xlsx files are supported." });
            }

            modelVm.ToAudit(LoginInfo);

            modelVm.EmployeeListID = new List<string>();
            modelVm.DateList = new List<string>();
            modelVm.ShiftCodeList = new List<string>();
            modelVm.RemarkList = new List<string>();

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                stream.Position = 0;

                // ── .xls → NPOI HSSFWorkbook | .xlsx → EPPlus ──────────────────
                if (fileExtension == ".xls")
                {
                    // NPOI handles legacy .xls format
                    IWorkbook workbook = new HSSFWorkbook(stream);
                    ISheet worksheet = workbook.GetSheetAt(0);

                    if (worksheet == null)
                        return Json(new { isSuccess = false, message = "The Excel file has no sheets." });

                    int rowCount = worksheet.LastRowNum;
                    if (rowCount < 1)
                        return Json(new { isSuccess = false, message = "The Excel file contains no data rows." });

                    // Helper: safely read any cell as trimmed string
                    string GetCell(IRow row, int col)
                    {
                        if (row == null) return string.Empty;
                        ICell cell = row.GetCell(col);
                        if (cell == null) return string.Empty;

                        return cell.CellType switch
                        {
                            CellType.String => cell.StringCellValue.Trim(),
                            CellType.Numeric => DateUtil.IsCellDateFormatted(cell)
                                                    ? cell.DateCellValue.Value.ToString("yyyy-MM-dd")
                                                    : cell.NumericCellValue.ToString(),
                            CellType.Boolean => cell.BooleanCellValue.ToString(),
                            CellType.Formula => cell.CachedFormulaResultType switch
                            {
                                CellType.String => cell.StringCellValue.Trim(),
                                CellType.Numeric => DateUtil.IsCellDateFormatted(cell)
                                                        ? cell.DateCellValue.Value.ToString("yyyy-MM-dd")
                                                        : cell.NumericCellValue.ToString(),
                                _ => cell.ToString().Trim()
                            },
                            CellType.Blank => string.Empty,
                            _ => cell.ToString().Trim()
                        };
                    }

                    for (int row = 1; row <= rowCount; row++) // row 0 = header, so start from 1
                    {
                        IRow currentRow = worksheet.GetRow(row);
                        if (currentRow == null) continue;

                        modelVm.EmployeeListID.Add(GetCell(currentRow, 0));
                        modelVm.DateList.Add(GetCell(currentRow, 1));
                        modelVm.ShiftCodeList.Add(GetCell(currentRow, 2));
                        modelVm.RemarkList.Add(GetCell(currentRow, 3));
                    }
                }
                else
                {
                    // EPPlus handles modern .xlsx format
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                    using var package = new ExcelPackage(stream);
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    if (worksheet?.Dimension == null)
                        return Json(new { isSuccess = false, message = "The Excel file has no sheets or data." });

                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++) // row 1 = header
                    {
                        modelVm.EmployeeListID.Add(worksheet.Cells[row, 1].Text.Trim());
                        modelVm.DateList.Add(worksheet.Cells[row, 2].Text.Trim());
                        modelVm.ShiftCodeList.Add(worksheet.Cells[row, 3].Text.Trim());
                        modelVm.RemarkList.Add(worksheet.Cells[row, 4].Text.Trim());
                    }
                }
            }

            var result = await rosterScheduleEntryService.SaveRosterExcel(modelVm);

            // ⚠️ Fixed typo: "isSuccss" → "isSuccess"
            return Json(new { isSuccss = result.isSuccess, message = result.message });
        }

        //public async Task<IActionResult> UploadExcel(IFormFile excelFile, RosterScheduleEntrySetupViewModel modelVm)
        //{
        //    var hasPermission = await rosterScheduleEntryService.SavePermissionAsync(LoginInfo.AccessCode);
        //    if (!hasPermission)
        //    {
        //        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
        //    }

        //    if (excelFile == null || excelFile.Length == 0)
        //    {
        //        return Json(new { isSuccess = false, message = "Please select a valid Excel file." });
        //    }
        //    modelVm.ToAudit(LoginInfo);
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        //    using (var stream = new MemoryStream())
        //    {
        //        await excelFile.CopyToAsync(stream);
        //        using (var package = new ExcelPackage(stream))
        //        {
        //            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
        //            int rowCount = worksheet.Dimension.Rows;

        //            modelVm.EmployeeListID = new List<string>();
        //            modelVm.DateList = new List<string>();
        //            modelVm.ShiftCodeList = new List<string>();
        //            modelVm.RemarkList = new List<string>();
        //            for (int row = 2; row <= rowCount; row++)
        //            {
        //                var empId = worksheet.Cells[row, 1].Text.Trim();
        //                var date = worksheet.Cells[row, 2].Text.Trim();
        //                var shiftCode = worksheet.Cells[row, 3].Text.Trim();
        //                var remark = worksheet.Cells[row, 4].Text.Trim();

        //                modelVm.EmployeeListID.Add(empId);
        //                modelVm.DateList.Add(date);
        //                modelVm.ShiftCodeList.Add(shiftCode);
        //                modelVm.RemarkList.Add(remark);
        //            }
        //        }
        //    }
        //    var result = await rosterScheduleEntryService.SaveRosterExcel(modelVm);
        //    return Json(new { isSuccss = result.isSuccess, message = result.message });
        //}

    }
}

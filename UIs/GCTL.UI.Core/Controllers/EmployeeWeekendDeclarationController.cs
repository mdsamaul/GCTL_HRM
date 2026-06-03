using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.Companies;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HRM_EmployeeWeekendDeclaration;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.Companies;
using GCTL.Service.EmployeeOfficialInfoReport;
using GCTL.Service.EmployeeWeekendDeclaration;
using GCTL.Service.HrmDefEmpTypes;
using GCTL.UI.Core.Controllers;
using GCTL.UI.Core.ViewModels.HRM_EmployeeWeekendDeclaration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using OfficeOpenXml;
using Org.BouncyCastle.Ocsp;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using CellType = NPOI.SS.UserModel.CellType;
namespace GCTL.UI.Core.Controllers
{
    public class EmployeeWeekendDeclarationController : BaseController
    {
        private readonly ICompanyService companyService;
        private readonly ICommonService commonService;
        private readonly IMapper mapper;
        private readonly IEmployeeWeekendDeclarationService employeeWeekendDeclarationService;
        private readonly IHrmDefEmpTypeService service;
        public EmployeeWeekendDeclarationController(ICompanyService companyService,
                                          ICommonService commonService,
                                          IMapper mapper,
                                           IEmployeeWeekendDeclarationService employeeWeekendDeclarationService,
                                             IHrmDefEmpTypeService service
                                          )
        {
            this.companyService = companyService;
            this.commonService = commonService;
            this.mapper = mapper;
            this.employeeWeekendDeclarationService = employeeWeekendDeclarationService;
            this.service = service;
        }
        public async Task<IActionResult> IndexAsync()
        {

            var hasPermission = await employeeWeekendDeclarationService.PagePermissionAsync(LoginInfo.AccessCode);

            if (!hasPermission)

            {

                return RedirectToAction("Login", "Accounts");

            }

            HRM_EmployeeWeekendDeclarationViewModel model = new HRM_EmployeeWeekendDeclarationViewModel()
            {
                PageUrl = Url.Action(nameof(IndexAsync)),
            };
            return View(model);
        }


        public IActionResult GetAllCompany()
        {
            var result = employeeWeekendDeclarationService.GetAllCompany();
            return Json(new { data = result });
        }

        [HttpPost]
        public async Task<IActionResult> getFilterEmp([FromBody] EmployeeFilterDto filter)
        {
            var data = await employeeWeekendDeclarationService.GetFilterDataAsync(filter);
            return Json(new { data = data });
        }



        [HttpPost]
        public async Task<IActionResult> SaveSelectedDatesAndEmployees([FromBody] HRM_EmployeeWeekendDeclarationDto modelVM)
        {
           
                var hasSavePermission = await employeeWeekendDeclarationService.SavePermissionAsync(LoginInfo.AccessCode);
                if (hasSavePermission)
                {

                    modelVM.ToAudit(LoginInfo);
                    var result = await employeeWeekendDeclarationService.SaveSelectedDatesAndEmployeesAsync(modelVM);
                    return Json(new
                    {
                        success = result.isSuccess,
                        message = result.message,
                        data = result.data
                    });
                }
                else
                {
                    return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
                }
            
           

        }
        public IActionResult GetWeekendEmployeeDeclaration()
        {
            var result = employeeWeekendDeclarationService.GetWeekendEmpDecService();
            return Json(new { data = result });
        }
        public async Task<ActionResult> BulkDeleteEmpWeelend(List<decimal> ids)
        {
            try
            {
                var hasPermission = await employeeWeekendDeclarationService.DeletePermissionAsync(LoginInfo.AccessCode);
                if (!hasPermission)
                {
                    return Json(new { success = false, message = "You have no access." });
                }


                if (ids == null || !ids.Any() || ids.Count == 0)
                {
                    return Json(new { isSuccess = false, message = "Employee not selected" });
                }
                DeleteHistoryViewModel Dmodel = new DeleteHistoryViewModel();
                Dmodel.ToAudit(LoginInfo);
                Dmodel.CompanyCode = LoginInfo.CompanyCode;
                var result = await employeeWeekendDeclarationService.BulkDeleteAsync(ids, Dmodel);
                if (!result)
                {
                    return Json(new { isSuccess = false, message = "Employee not found" });
                }
                return Json(new { isSuccess = true, message = $"Deleted Successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { isSuccess = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> editEmpWeekDec(string Id)
        {
            var emp = await employeeWeekendDeclarationService.GetEmployeeWeekendDeclarationByIdAsync(Id);

            if (emp == null)
                return NotFound();

            return Json(new
            {
                weekendDate = emp.WeekendDate,
                remarks = emp.Remarks,
                id = emp.ID,
                EmpId = emp.EmpID
            });
        }
        [HttpPost]
        public async Task<IActionResult> UpdateEmpWeekDec(string Id, String WeekendDate, string Remarks)
        {

            var hasUpdatePermission = await employeeWeekendDeclarationService.UpdatePermissionAsync(LoginInfo.AccessCode);
            if (hasUpdatePermission)
            {
                var (isSuccess, message) = await employeeWeekendDeclarationService
                                   .UpdateEmployeeWeekendDeclarationAsync(Id, WeekendDate, Remarks);
                return Json(new { success = isSuccess, message = message });
            }
            else
            {
                return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });
            }
           
        }


        //[HttpPost]
        //[Route("/EmployeeWeekendDeclaration/UploadExcelAsync")]
        //public async Task<IActionResult> UploadExcelAsync(IFormFile excelFile, HRM_EmployeeWeekendDeclarationDto modelVM)
        //{

        //    var hasSavePermission = await employeeWeekendDeclarationService.SavePermissionAsync(LoginInfo.AccessCode);
        //    if (hasSavePermission)
        //    {
        //        if (excelFile == null || excelFile.Length == 0)
        //        {
        //            return Json(new { isSuccess = false, message = "Please select a valid Excel file." });
        //        }

        //        modelVM.ToAudit(LoginInfo);

        //        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        //        using (var stream = new MemoryStream())
        //        {
        //            await excelFile.CopyToAsync(stream);
        //            using (var package = new ExcelPackage(stream))
        //            {
        //                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
        //                int rowCount = worksheet.Dimension.Rows;

        //                modelVM.WeekendEmployeeIds = new List<string>();
        //                modelVM.WeekendDates = new List<string>();
        //                modelVM.ExcelRemark = new List<string>();

        //                for (int row = 2; row <= rowCount; row++)
        //                {
        //                    var empId = worksheet.Cells[row, 1].Text.Trim();
        //                    var date = worksheet.Cells[row, 2].Text.Trim();
        //                    var remark = worksheet.Cells[row, 3].Text.Trim();

        //                    modelVM.WeekendEmployeeIds.Add(empId);
        //                    modelVM.WeekendDates.Add(date);
        //                    modelVM.ExcelRemark.Add(remark);
        //                }
        //            }
        //        }

        //        var result = await employeeWeekendDeclarationService.SaveSelectedDatesAndEmployeesFromExcelAsync(modelVM);

        //        return Json(new { isSuccess = result.isSuccess, message = result.message });
        //    }
        //    else
        //    {
        //        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
        //    }

        //}


        [HttpPost]
        [Route("/EmployeeWeekendDeclaration/UploadExcelAsync")]
        public async Task<IActionResult> UploadExcelAsync(IFormFile excelFile, HRM_EmployeeWeekendDeclarationDto modelVM)
        {
            var hasSavePermission = await employeeWeekendDeclarationService.SavePermissionAsync(LoginInfo.AccessCode);

            if (!hasSavePermission)
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

            modelVM.ToAudit(LoginInfo);

            modelVM.WeekendEmployeeIds = new List<string>();
            modelVM.WeekendDates = new List<string>();
            modelVM.ExcelRemark = new List<string>();

            using (var stream = new MemoryStream())
            {
                await excelFile.CopyToAsync(stream);
                stream.Position = 0;

                // ✅ ===== XLS (NPOI) =====
                if (fileExtension == ".xls")
                {
                    IWorkbook workbook = new HSSFWorkbook(stream);
                    ISheet worksheet = workbook.GetSheetAt(0);

                    if (worksheet == null)
                        return Json(new { isSuccess = false, message = "Excel sheet not found." });

                    int rowCount = worksheet.LastRowNum;

                    string GetCell(IRow row, int col)
                    {
                        if (row == null) return string.Empty;
                        var cell = row.GetCell(col);
                        if (cell == null) return string.Empty;

                        return cell.CellType switch
                        {
                            CellType.String => cell.StringCellValue.Trim(),
                            CellType.Numeric => DateUtil.IsCellDateFormatted(cell)
                                ? cell.DateCellValue?.ToString("yyyy-MM-dd")
                                : cell.NumericCellValue.ToString(),
                            CellType.Boolean => cell.BooleanCellValue.ToString(),
                            CellType.Formula => cell.ToString().Trim(),
                            _ => cell.ToString().Trim()
                        };
                    }

                    for (int row = 1; row <= rowCount; row++) // header skip
                    {
                        var currentRow = worksheet.GetRow(row);
                        if (currentRow == null) continue;

                        modelVM.WeekendEmployeeIds.Add(GetCell(currentRow, 0));
                        modelVM.WeekendDates.Add(GetCell(currentRow, 1));
                        modelVM.ExcelRemark.Add(GetCell(currentRow, 2));
                    }
                }
                // ✅ ===== XLSX (EPPlus) =====
                else
                {
                    ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                    using var package = new ExcelPackage(stream);
                    var worksheet = package.Workbook.Worksheets[0];

                    if (worksheet?.Dimension == null)
                    {
                        return Json(new { isSuccess = false, message = "Excel file has no data." });
                    }

                    int rowCount = worksheet.Dimension.Rows;

                    for (int row = 2; row <= rowCount; row++)
                    {
                        modelVM.WeekendEmployeeIds.Add(worksheet.Cells[row, 1].Text.Trim());
                        modelVM.WeekendDates.Add(worksheet.Cells[row, 2].Text.Trim());
                        modelVM.ExcelRemark.Add(worksheet.Cells[row, 3].Text.Trim());
                    }
                }
            }

            var result = await employeeWeekendDeclarationService.SaveSelectedDatesAndEmployeesFromExcelAsync(modelVM);

            return Json(new
            {
                isSuccess = result.isSuccess,
                message = result.message,
                data = result.data
            });
        }

        public async Task<IActionResult> DownloadExcel()
        {
            var fileBytes = await employeeWeekendDeclarationService.GenerateEmployeeWeekendDeclarationExcelAsync();
            string excelName = $"EmployeeData.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
    }
}

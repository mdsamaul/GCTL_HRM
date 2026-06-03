using AutoMapper;
using DocumentFormat.OpenXml.Office2016.Drawing.Command;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HRM_EmployeeWeekendDeclaration;
using GCTL.Core.ViewModels.ManualEarnLeaveEntry;
using GCTL.Service.Common;
using GCTL.Service.Companies;
using GCTL.Service.EmployeeOfficialInfoReport;
using GCTL.Service.EmployeeWeekendDeclaration;
using GCTL.Service.ManualEarnLeaveEntry;
using GCTL.UI.Core.ViewModels.ManualEarnLeaveEntry;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;
using OfficeOpenXml;
using System.Threading.Tasks;

namespace GCTL.UI.Core.Controllers
{
    public class ManualEarnLeaveEntryController : BaseController
    {
        private readonly IManualEarnLeaveEntryService manualEarnLeaveEntryService;

        public ManualEarnLeaveEntryController(
            IManualEarnLeaveEntryService manualEarnLeaveEntryService
            )
        {
            this.manualEarnLeaveEntryService = manualEarnLeaveEntryService;
        }
        public async Task<IActionResult> Index()
        {
            var hasPermission = await manualEarnLeaveEntryService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }
            ManualEarnLeaveEntryViewModel model = new ManualEarnLeaveEntryViewModel()
            {
                PageUrl = Url.Action(nameof(Index)),
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> getAllFilterEmp([FromBody] ManualEarnLeaveEntryEmployeeFilterDto filterDto)
        {
            //filterDto.EmployeeStatuses = new List<string> { "01" };
            var result = await manualEarnLeaveEntryService.GetFilterDataAsync(filterDto);
            if (result != null)
            {
                return Json(new { isSuccess = true, message = "successed data load", data = result });
            }
            return Json(new { isSuccess = false, message = "Data load Failed" });

        }
        [HttpPost]
        public async Task<IActionResult> CreateManualEarnLeave([FromBody] ManualEarnLeaveEntryEmployeeCreateDto FromData)
        {
            if(FromData.isUpdate == false)
            {
                var hasPermission = await manualEarnLeaveEntryService.SavePermissionAsync(LoginInfo.AccessCode);
                if (!hasPermission)
                {
                    return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
                }
            }
            else
            {

                var hasPermission = await manualEarnLeaveEntryService.UpdatePermissionAsync(LoginInfo.AccessCode);
                if (!hasPermission)
                {
                    return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
                }
            }

            FromData.ToAudit(LoginInfo, FromData.isUpdate);
            var result = await manualEarnLeaveEntryService.SaveUpdateEarnLeaveServices(FromData);
           
            return Json(new
            {
                success = result.isSuccess,
                message = result.message,
                data = result.data,
            });
        }

        public IActionResult GetEarmLeaveEmployee()
        {
            var result = manualEarnLeaveEntryService.GetEarnLeaveEmployeeService();
            return Json(new { data = result });
        }

        //download 
        public async Task<IActionResult> DownloadExcel()
        {
            var fileBytes = await manualEarnLeaveEntryService.GenerateEmpEarnLeaveExcelDownload();
            string excelName = $"EarnLeaveData.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

       

        public async Task<IActionResult> UploadExcel(IFormFile excelFile, ManualEarnLeaveEntryEmployeeCreateDto modelVm)
        {
            var hasPermission = await manualEarnLeaveEntryService.SavePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
            }

            if (excelFile == null || excelFile.Length == 0)
            {
                return Json(new { isSuccess = false, message = "Please select a valid Excel file." });
            }

            var extension = Path.GetExtension(excelFile.FileName).ToLower();
            if (extension != ".xlsx" && extension != ".xls")
            {
                return Json(new { isSuccess = false, message = "Only .xlsx and .xls files are supported." });
            }

            modelVm.ToAudit(LoginInfo);

            modelVm.EmployeeID = new List<string>();
            modelVm.YearList = new List<string>();
            modelVm.GrantedLeaveDaysList = new List<decimal>();
            modelVm.AvailedLeaveDaysList = new List<decimal>();
            modelVm.BalancedLeaveDaysList = new List<decimal>();
            modelVm.RemarksList = new List<string>();

            using var stream = new MemoryStream();
            await excelFile.CopyToAsync(stream);
            stream.Position = 0;

            if (extension == ".xlsx")
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    ReadExcelRow(
                        worksheet.Cells[row, 1].Text.Trim(),
                        worksheet.Cells[row, 2].Text.Trim(),
                        worksheet.Cells[row, 3].Text.Trim(),
                        worksheet.Cells[row, 4].Text.Trim(),
                        worksheet.Cells[row, 5].Text.Trim(),
                        worksheet.Cells[row, 6].Text.Trim(),
                        modelVm
                    );
                }
            }
            else
            {
                var workbook = new HSSFWorkbook(stream);
                var sheet = workbook.GetSheetAt(0);

                for (int row = 1; row <= sheet.LastRowNum; row++)
                {
                    var currentRow = sheet.GetRow(row);
                    if (currentRow == null) continue;

                    ReadExcelRow(
                        currentRow.GetCell(0)?.ToString()?.Trim() ?? "",
                        currentRow.GetCell(1)?.ToString()?.Trim() ?? "",
                        currentRow.GetCell(2)?.ToString()?.Trim() ?? "",
                        currentRow.GetCell(3)?.ToString()?.Trim() ?? "",
                        currentRow.GetCell(4)?.ToString()?.Trim() ?? "",
                        currentRow.GetCell(5)?.ToString()?.Trim() ?? "",
                        modelVm
                    );
                }
            }

            var result = await manualEarnLeaveEntryService.SaveEarnLeaveExcel(modelVm);
            return Json(new { isSuccess = result.isSuccess, message = result.message });
        }

        private void ReadExcelRow(string empId, string year, string granted, string availed, string balanced, string remark, ManualEarnLeaveEntryEmployeeCreateDto modelVm)
        {
            if (string.IsNullOrWhiteSpace(empId)) return;

            modelVm.EmployeeID.Add(empId);
            modelVm.YearList.Add(year);
            modelVm.GrantedLeaveDaysList.Add(decimal.TryParse(granted, out var g) ? g : 0);
            modelVm.AvailedLeaveDaysList.Add(decimal.TryParse(availed, out var a) ? a : 0);
            modelVm.BalancedLeaveDaysList.Add(decimal.TryParse(balanced, out var b) ? b : 0);
            modelVm.RemarksList.Add(remark ?? "");
        }


        public async Task<ActionResult> BulkDeleteEmpWeelend(List<decimal> ids)
        {
            try
            {

                var hasPermission = await manualEarnLeaveEntryService.DeletePermissionAsync(LoginInfo.AccessCode);
                if (!hasPermission)
                {
                    return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
                }

                if (ids == null || !ids.Any() || ids.Count == 0)
                {
                    return Json(new { isSuccess = false, message = "Employee not selected" });
                }
                DeleteHistoryViewModel dmodel = new DeleteHistoryViewModel();
                dmodel.ToAudit(LoginInfo);
                dmodel.CompanyCode = LoginInfo.CompanyCode;
                var result = await manualEarnLeaveEntryService.BulkDeleteAsync(ids, dmodel);
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
        public async Task<IActionResult> EditEarnLeaveEmployee(string id)
        {
            var emp = await manualEarnLeaveEntryService.getEarnLeaveEmployeeById(id);
            if (emp == null)
            {
                return Json(new { isSuccess = false, message = "Employee Not Found" });
            }
            return Json(new { isSuccess = true, message = "Employee Found", data = emp });
        }
        //[HttpPost]
        //public async Task<IActionResult> UpdateEarnLeaveEmployee([FromBody] ManualEarnLeaveEntryEmployeeCreateDto modelVM)
        //{
        //    var employee = await manualEarnLeaveEntryService.Update
        //    return Json(new {modelVM });
        //}

    }
}

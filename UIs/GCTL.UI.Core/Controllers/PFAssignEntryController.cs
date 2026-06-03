using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.ManualEarnLeaveEntry;
using GCTL.Core.ViewModels.PFAssignEntry;
using GCTL.Service.Common;
using GCTL.Service.EmployeeOfficialInfoReport;
using GCTL.Service.ManualEarnLeaveEntry;
using GCTL.Service.PFAssignEntry;
using GCTL.UI.Core.ViewModels.ManualEarnLeaveEntry;
using GCTL.UI.Core.ViewModels.PFAssignEntry;
using Microsoft.AspNetCore.Mvc;
using NPOI.HSSF.UserModel;
using OfficeOpenXml;

namespace GCTL.UI.Core.Controllers
{
    public class PFAssignEntryController : BaseController
    {
        private readonly IPFAssignEntryService pFAssignEntryService;

        public PFAssignEntryController(IPFAssignEntryService pFAssignEntryService)
        {
            this.pFAssignEntryService = pFAssignEntryService;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var hasPermission = await pFAssignEntryService.PagePermissionAsync(LoginInfo.AccessCode);

            if (!hasPermission)

            {

                return RedirectToAction("Login", "Accounts");

            }

            PFAssignEntryViewModel model = new PFAssignEntryViewModel()
            {
                PageUrl = Url.Action(nameof(IndexAsync)),
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> getAllFilterEmp([FromBody] PFAssignEntryFilterDto filterDto)
        {
            //filterDto.EmployeeStatuses = new List<string> { "01" };
            var result = await pFAssignEntryService.GetFilterDataAsync(filterDto);
            if (result != null)
            {
                return Json(new { isSuccess = true, message = "successed data load", data = result });
            }
            return Json(new { isSuccess = false, message = "Data load Failed" });

        }

        [HttpPost]
        public async Task<IActionResult> CreateEditPFAssignEntry([FromBody] PFAssignEntrySetupViewModel FromData)
        {
            if (FromData.PFAssignID == null)

            {

                var hasSavePermission = await pFAssignEntryService.SavePermissionAsync(LoginInfo.AccessCode);

                if (hasSavePermission)

                {

                    FromData.ToAudit(LoginInfo, FromData.isUpdate);
                    var result = await pFAssignEntryService.CreateUpdatePFAssignService(FromData);
                    return Json(new
                    {
                        isSuccess = result.isSuccess,
                        message = result.message,
                        data = FromData
                    });

                }

                else

                {

                    return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });

                }

            }

            else

            {

                var hasUpdatePermission = await pFAssignEntryService.UpdatePermissionAsync(LoginInfo.AccessCode);

                if (hasUpdatePermission)

                {

                    FromData.ToAudit(LoginInfo, FromData.isUpdate);
                    var result = await pFAssignEntryService.CreateUpdatePFAssignService(FromData);
                    return Json(new
                    {
                        isSuccess = result.isSuccess,
                        message = result.message,
                        data = FromData
                    });

                }

                else

                {

                    return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });

                }

            }

           
        }
        //download 
        public async Task<IActionResult> DownloadExcel()
        {
            var fileBytes = await pFAssignEntryService.GeneratePfAssignExcelDownload();
            string excelName = $"PFAssignData.xlsx";
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }

        //upload excelfile
        public async Task<IActionResult> UploadExcel(IFormFile excelFile, PFAssignEntrySetupViewModel fromData)
        {
            var hasSavePermission = await pFAssignEntryService.SavePermissionAsync(LoginInfo.AccessCode);
            if (!hasSavePermission)
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

            fromData.ToAudit(LoginInfo);

            fromData.EmployeeIds = new List<string>();
            fromData.EFDateList = new List<string>();
            fromData.ApprovalRemarkList = new List<string>();
            fromData.PFApprovedStatusList = new List<string>();

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
                        fromData
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
                        fromData
                    );
                }
            }

            var result = await pFAssignEntryService.SavePFAssignExcel(fromData);
            return Json(new { isSuccess = result.isSuccess, message = result.message });
        }

        // ✅ common helper method
        private void ReadExcelRow(string empId, string efDate, string pfApprovedStatus, string remarks, PFAssignEntrySetupViewModel fromData)
        {
            if (string.IsNullOrWhiteSpace(empId)) return;

            fromData.EmployeeIds.Add(empId);
            fromData.EFDateList.Add(efDate ?? "");
            fromData.PFApprovedStatusList.Add(pfApprovedStatus ?? "");
            fromData.ApprovalRemarkList.Add(remarks ?? "");
        }

        public IActionResult GetPfAssignData()
        {
            var result = pFAssignEntryService.GetPfAssignDataService();
            return Json(new { data = result });
        }


        public async Task<ActionResult> BulkDeleteEmpPFAssign(List<decimal> ids)
        {
            try
            {
                var hasPermission = await pFAssignEntryService.DeletePermissionAsync(LoginInfo.AccessCode);

                if (!hasPermission)

                {

                    return Json(new { success = false, message = "You have no access." });

                }


                if (ids == null || !ids.Any() || ids.Count == 0)
                {
                    return Json(new { isSuccess = false, message = "Employee not selected" });
                }
                DeleteHistoryViewModel dmodel = new DeleteHistoryViewModel();
                dmodel.ToAudit(LoginInfo);
                dmodel.CompanyCode = LoginInfo.CompanyCode;
                var result = await pFAssignEntryService.BulkDeleteAsync(ids, dmodel);
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
        public async Task<IActionResult> EditGetAssignValue(string id)
        {
            var emp = await pFAssignEntryService.getAssignValueById(id);
            if (emp == null)
            {
                return Json(new { isSuccess = false, message = "Employee Not Found" });
            }
            return Json(new { isSuccess = true, message = "Employee Found", data = emp });
        }
    }
}

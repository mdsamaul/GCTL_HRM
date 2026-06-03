using AutoMapper;
using ClosedXML.Excel;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.LeaveTypes;
using GCTL.Service.Common;
using GCTL.Service.LeaveTypes;
using GCTL.UI.Core.ViewModels.LeaveTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace GCTL.UI.Core.Controllers
{
    public class LeaveTypesController : BaseController
    {
        private readonly ILeaveTypeService leaveTypeService;
        private readonly ICommonService commonService;
        private readonly IMapper mapper;
        string strMaxNO = "";

        public LeaveTypesController(ILeaveTypeService leaveTypeService, ICommonService commonService, IMapper mapper)
        {
            this.leaveTypeService = leaveTypeService;
            this.commonService = commonService;
            this.mapper = mapper;
        }


        public async Task<IActionResult> Index(string? id)
        {
            var hasPermission = await leaveTypeService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            LeaveTypePageviewModel model = new LeaveTypePageviewModel();
            var leaveTypeList = await leaveTypeService.GetLeaveTypesAsync();
            model.LeaveTypeList = leaveTypeList ?? new List<LeaveTypeSetupViewModel>();


            if (!string.IsNullOrEmpty(id))
            {
                model.Setup = await leaveTypeService.GetLeaveTypeAsync(id);

            }
            var ymwdOptions = new List<SelectListItem>
             {
                 new SelectListItem { Value = "Year", Text = "Year" },
                 new SelectListItem { Value = "Month", Text = "Month" },
                 new SelectListItem { Value = "Week", Text = "Week" },
                 new SelectListItem { Value = "Day", Text = "Day" }
             };

            ViewBag.YmwdOptions = new SelectList(ymwdOptions, "Value", "Text");
            model.PageUrl = Url.Action(nameof(Index));
            return View(model);
        }


        #region Post
        [HttpPost]
        [ValidateAntiForgeryToken]


        public async Task<IActionResult> Setup(LeaveTypeSetupViewModel modelVM)
        {
            try
            {

                if (await leaveTypeService.IsLeaveTypeExistAsync(modelVM.Name, modelVM.LeaveTypeCode))
                {
                    return Json(new { isSuccess = false, message = $"Already <span style='color: blue;'>'{modelVM.Name}'</span> Exists", isDuplicate = true });
                }


                if (string.IsNullOrEmpty(modelVM.LeaveTypeCode))
                {
                    modelVM.LeaveTypeCode = await leaveTypeService.GenerateNextLeaveTypeCode();
                }


                modelVM.ToAudit(LoginInfo, modelVM.Id > 0);


                if (modelVM.Id == 0)
                {
                    var hasSavePermission = await leaveTypeService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await leaveTypeService.SaveLeaveTypeAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = modelVM.LeaveTypeCode });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await leaveTypeService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await leaveTypeService.UpdateLeaveTypeAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully", lastCode = modelVM.LeaveTypeCode });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to update", noUpdatePermission = true });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }




        #endregion


        #region CheckAvailability
        [HttpPost]
        public async Task<JsonResult> CheckAvailability(string name, string code)
        {
            if (await leaveTypeService.IsLeaveTypeExistAsync(name, code))
            {

                return Json(new { isSuccess = true, message = $"Already <span style='color: blue;'>'{name}'</span> Exists" });

            }

            return Json(new { isSuccess = false });
        }
        #endregion


        #region Delete
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] List<string> ids)
        {
            try
            {
                var hasPermission = await leaveTypeService.DeletePermissionAsync(LoginInfo.AccessCode);
                if (hasPermission)
                {
                    DeleteHistoryViewModel model = new DeleteHistoryViewModel();
                    model.ToAudit(LoginInfo);
                    model.CompanyCode = LoginInfo.CompanyCode;

                    var result = await leaveTypeService.DeleteLeaveType(ids, model);

                    return Json(new { isSuccess = result.success, message = result.message, refSuccess = result.refError });
                }
                else
                {
                    return Json(new { isSuccess = false, message = "You have no access" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting leave type: {ex.Message}");
                return StatusCode(500, new { isSuccess = false, message = ex.Message });
            }
        }


        #endregion


        #region NeaxtCode
        [HttpGet]
        public async Task<IActionResult> GenerateNextLeaveTypeCode()
        {
            var nextCode = await leaveTypeService.GenerateNextLeaveTypeCode();
            return Json(nextCode);
        }
        #endregion


        #region TabeleLodaing
        //For Update Tabele data after posting/Updating and Deleeting 
        [HttpGet]
        public async Task<IActionResult> GeLeaveTypesList()
        {
            try
            {
                var list = await leaveTypeService.GetLeaveTypesAsync();
                return PartialView("_Grid", list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        //
        #endregion


        public async Task<IActionResult> ExportToExcel()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("LeaveTypes");

                // Add title
                worksheet.Cell(1, 1).Value = "Leave Type Information";
                worksheet.Range(1, 1, 1, 8).Merge(); // Merge cells across the header columns
                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Row(1).Style.Font.FontSize = 14;
                worksheet.Row(1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Row(1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                worksheet.Range(2, 1, 2, 8).Merge();
                // Leave an empty row
                int dataStartRow = 3;

                // Add headers
                worksheet.Cell(dataStartRow, 1).Value = "Leave Type Id";
                worksheet.Cell(dataStartRow, 2).Value = "Leave Name";
                worksheet.Cell(dataStartRow, 3).Value = "Short Name";
                worksheet.Cell(dataStartRow, 4).Value = "Rull/Policy";
                worksheet.Cell(dataStartRow, 5).Value = "No.of Days";
                worksheet.Cell(dataStartRow, 6).Value = "For";
                worksheet.Cell(dataStartRow, 7).Value = "Year/Month";
                worksheet.Cell(dataStartRow, 8).Value = "Effective Date";
                worksheet.Row(dataStartRow).Style.Font.Bold = true;
                worksheet.Row(dataStartRow).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                worksheet.Row(dataStartRow).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                // Add data
                var designations = await leaveTypeService.GetLeaveTypesAsync();
                int row = dataStartRow + 1;
                foreach (var designation in designations)
                {

                    worksheet.Cell(row, 1).Value = "'" + designation.LeaveTypeCode.PadLeft(2, '0');
                    worksheet.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Cell(row, 2).Value = designation.Name;
                    worksheet.Cell(row, 2).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    worksheet.Cell(row, 3).Value = designation.ShortName;
                    worksheet.Cell(row, 3).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Cell(row, 4).Value = designation.RulePolicy;
                    worksheet.Cell(row, 4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    worksheet.Cell(row, 5).Value = designation.NoOfDay;
                    worksheet.Cell(row, 5).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Cell(row, 6).Value = designation.For;
                    worksheet.Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    worksheet.Cell(row, 7).Value = designation.Ymwd;
                    worksheet.Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;

                    worksheet.Cell(row, 8).Value = designation.Wef.ToString("dd/MM/yyyy");
                    worksheet.Cell(row, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

                    row++;
                }

                worksheet.Columns().AdjustToContents();

                // Save to a stream
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(),
                                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                "LeaveTypes.xlsx");
                }
            }
        }
    }
}
using GCTL.Core.ViewModels.HolidayTypes;
using GCTL.Core.ViewModels.HrmAtdShifts;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.HRMCompanyWeekEnds;
using GCTL.Service.Common;
using GCTL.Service.HolidayTypes;
using GCTL.Service.HrmAtdShifts;
using GCTL.Service.HRMCompanyWeekEnds;
using GCTL.Service.LeaveTypes;
using GCTL.UI.Core.ViewModels.HolidayTypes;
using GCTL.UI.Core.ViewModels.HRMCompanyWeekEnds;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using GCTL.Core.ViewModels.Departments;
using ClosedXML.Excel;

namespace GCTL.UI.Core.Controllers
{
    public class HolidayTypesController : BaseController
    {

        private readonly IHrmDefHolidayTypeService service;
        private readonly ICommonService commonService;
        private readonly ICompositeViewEngine viewEngine;
        string strMaxNO = "";
        public HolidayTypesController(IHrmDefHolidayTypeService service, ICommonService commonService, ICompositeViewEngine viewEngine)
        {
            this.service = service;
            this.commonService = commonService;
            this.viewEngine = viewEngine;

        }

        #region GettALLById
        public async Task<IActionResult> Index(string? id)
        {
            var hasPermission = await service.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }
            //Get all
            HRMDefHolidayTypePageViewModel model = new HRMDefHolidayTypePageViewModel();
            var list = await service.GetAllAsync();
            model.HRMHolidayTypesList = list ?? new List<HRMDefHolidayTypeSetupViewModel>();
            
            //Get By Id
            if (!string.IsNullOrEmpty(id))
            {

                model.Setup = await service.GetByIdAsync(id);
               
            }
            //
            if (model.Setup == null)
            {
                commonService.FindMaxNo(ref strMaxNO, "HolidayTypeCode", "HRM_Def_HolidayType", 2);
                model.Setup = new HRMDefHolidayTypeSetupViewModel
                {
                    HolidayTypeCode = strMaxNO
                };
                
            }
            //
            model.PageUrl = Url.Action(nameof(Index));
            return View(model);



        }
        #endregion


        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]


        public async Task<IActionResult> Setup(HRMDefHolidayTypeSetupViewModel modelVM)
        {
            try
            {

                if (await service.IsExistAsync(modelVM.HolidayType, modelVM.HolidayTypeCode))
                {
                    return Json(new { isSuccess = false, message = $"Already Exists!", isDuplicate = true });
                }


                if (string.IsNullOrEmpty(modelVM.HolidayTypeCode))
                {
                    modelVM.HolidayTypeCode = await service.GenerateNextCode();
                }


                modelVM.ToAudit(LoginInfo, modelVM.AutoId > 0);
                if (modelVM.AutoId == 0)
                {
                    var hasSavePermission = await service.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await service.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.HolidayTypeCode });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await service.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await service.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.HolidayTypeCode });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to update.", noUpdatePermission = true });
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
            if (await service.IsExistAsync(name, code))
            {

                return Json(new { isSuccess = true, message = $"Already Exists!" });

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

                var hasPermission = await service.DeletePermissionAsync(LoginInfo.AccessCode);
                if (hasPermission)
                {

                    foreach (var id in ids)
                    {
                        var result = service.DeleteLeaveType(id);

                    }

                    return Json(new { isSuccess = true, message = "Data Deleted Successfully" });
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
        public async Task<IActionResult> GenerateNextCodeHolidayTypes()
        {
            var nextCode = await service.GenerateNextCode();
            return Json(nextCode);
        }
        #endregion

        #region TabeleLodaing

        [HttpGet]
        public async Task<IActionResult> GetTableData()
        {
            try
            {
                var list = await service.GetAllAsync();
                return PartialView("_Grid", list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        #endregion


        public async Task<IActionResult> ExportToExcel()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("HolidayTypes");

                // Add headers
                worksheet.Cell(1, 1).Value = "HolidayType Code";
                worksheet.Cell(1, 2).Value = "Holiday Type";
                worksheet.Cell(1, 3).Value = "Short Name";


                // Add data
                var designations = await service.GetAllAsync();
                int row = 2;
                foreach (var designation in designations)
                {
                    worksheet.Cell(row, 1).Value = designation.HolidayTypeCode;



                     worksheet.Cell(row, 2).Value = designation.HolidayType;
                    worksheet.Cell(row, 3).Value = designation.ShortName;

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
                                "HolidayTypes.xlsx");
                }
            }
        }

    }
}


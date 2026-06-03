using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.SeparationTypes;
using GCTL.Service.Common;
using GCTL.Service.SeparationTypes;
using GCTL.UI.Core.ViewModels.SeparationTypes;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class SeparationTypesController : BaseController
    {
        private readonly ISeparationTypesService separationTypesService;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;

        public SeparationTypesController(
            ISeparationTypesService separationTypesService, ICommonService commonService
            )
        {
            this.separationTypesService = separationTypesService;
            this.commonService = commonService;
        }

        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await separationTypesService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new SeparationTypesPageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {

                var list = await separationTypesService.GetAllAsync();
                model.SeparationList = list ?? new List<SeparationTypesSetupViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "SeparationTypeId", "HRM_Def_SeparationType", 2);
                model.Setup = new SeparationTypesSetupViewModel
                {
                    SeparationTypeId = strMaxNO
                };

            }
            catch (Exception ex)
            {

                model.SeparationList = new List<SeparationTypesSetupViewModel>();
                model.Setup = new SeparationTypesSetupViewModel();
                Console.WriteLine("Error" + ex.Message);
            }


            if (child)
                return PartialView(model);

            return View(model);
        }

        public async Task<IActionResult> Setup(string id)
        {
            SeparationTypesSetupViewModel model = new SeparationTypesSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "SeparationTypeId", "HRM_Def_SeparationType", 2);

            if (!string.IsNullOrEmpty(id))
            {

                model = await separationTypesService.GetByIdAsync(id);
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.SeparationTypeId = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        //
        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(SeparationTypesSetupViewModel modelVM)
        {
            try
            {

                if (await separationTypesService.IsExistAsync(modelVM.SeparationType, modelVM.SeparationTypeId))
                {
                    return Json(new { isSuccess = false, message = $"Already Exists!", isDuplicate = true });
                }


                if (!ModelState.IsValid)
                {

                    var errorMessage = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                    return Json(new { isSuccess = false, message = errorMessage });
                }

                modelVM.ToAudit(LoginInfo, modelVM.SeparationTypeCode > 0);
                if (modelVM.SeparationTypeCode == 0)
                {
                    var hasSavePermission = await separationTypesService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await separationTypesService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Data Saved Successfully.", lastCode = modelVM.SeparationTypeId });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await separationTypesService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await separationTypesService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Data Updated Successfully.", lastCode = modelVM.SeparationTypeId });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to update.", noUpdatePermission = true });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:{ex.Message}");
                return RedirectToAction("Login", "Accounts");

            }
        }

        #endregion

        //
        #region CheckAvailability
        [HttpPost]
        public async Task<JsonResult> CheckAvailability(string name, string code)
        {
            if (await separationTypesService.IsExistAsync(name, code))
            {

                return Json(new { isSuccess = true, message = $"Already Exists!" });

            }

            return Json(new { isSuccess = false });
        }
        #endregion


        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] List<string> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return BadRequest(new { success = false, message = "No IDs provided for delete." });
                }

                var hasPermission = await separationTypesService.DeletePermissionAsync(LoginInfo.AccessCode);
                if (!hasPermission)
                {
                    return Json(new { success = false, message = "You have no access." });
                }


                DeleteHistoryViewModel model = new DeleteHistoryViewModel();
                model.ToAudit(LoginInfo);
                model.CompanyCode = LoginInfo.CompanyCode;


                var success = await separationTypesService.DeleteTab(ids, model);
                return Json(new { success = success.succses, message = success.messege, refSuccess = success.refSuccess });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        #region TabeleLodaing

        [HttpGet]
        public async Task<IActionResult> GetTableData()
        {
            try
            {
                var list = await separationTypesService.GetAllAsync();
                return PartialView("_Grid", list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        #endregion
    }
}

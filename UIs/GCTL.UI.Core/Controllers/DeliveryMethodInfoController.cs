using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.DeliveryMethodInfo;
using GCTL.Service.Common;
using GCTL.Service.DeliveryMethodInfo;
using GCTL.UI.Core.ViewModels.DeliveryMethodInfo;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class DeliveryMethodInfoController : BaseController
    {
        #region Service & Repository
        public readonly IDeliveryMethodInfoService deliveryMethodInfoService;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;

        public DeliveryMethodInfoController(
            IDeliveryMethodInfoService deliveryMethodInfoService,
            ICommonService commonService

            )
        {
            this.deliveryMethodInfoService = deliveryMethodInfoService;
            this.commonService = commonService;
        }

        #endregion

        #region Index
        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await deliveryMethodInfoService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new DeliveryMethodInfoPageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {

                var list = await deliveryMethodInfoService.GetAllAsync();
                model.DeliveryMethodList = list ?? new List<DeliveryMethodInfoSetupViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "DeliveryMethodId", "RMG_Prod_Def_DeliveryMethod", 3);

                model.Setup = new DeliveryMethodInfoSetupViewModel
                {
                    DeliveryMethodId = strMaxNO
                };

            }
            catch (Exception ex)
            {

                model.DeliveryMethodList = new List<DeliveryMethodInfoSetupViewModel>();
                model.Setup = new DeliveryMethodInfoSetupViewModel();
                Console.WriteLine("Error" + ex.Message);
            }

            if (child)
                return PartialView(model);

            return View(model);
        }
        #endregion

        #region Setup

        public async Task<IActionResult> Setup(string id)
        {
            DeliveryMethodInfoSetupViewModel model = new DeliveryMethodInfoSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "DeliveryMethodId", "RMG_Prod_Def_DeliveryMethod", 3);

            if (!string.IsNullOrEmpty(id))
            {

                model = await deliveryMethodInfoService.GetByIdAsync(id);
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.DeliveryMethodId = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        #endregion

        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(DeliveryMethodInfoSetupViewModel modelVM)
        {
            try
            {

                if (await deliveryMethodInfoService.IsExistAsync(modelVM.DeliveryMethod, modelVM.DeliveryMethodId))
                {
                    return Json(new { isSuccess = false, message = $"Already Exists!", isDuplicate = true });
                }


                if (!ModelState.IsValid)
                {

                    var errorMessage = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                    return Json(new { isSuccess = false, message = errorMessage });
                }

                modelVM.ToAudit(LoginInfo, modelVM.Tc > 0);
                modelVM.CompanyId = LoginInfo.CompanyCode;
                if (modelVM.Tc == 0)
                {
                    var hasSavePermission = await deliveryMethodInfoService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await deliveryMethodInfoService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.DeliveryMethodId });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await deliveryMethodInfoService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await deliveryMethodInfoService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.DeliveryMethodId });
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

        #region Delete

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] List<string> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest(new { success = false, message = "No IDs provided for delete." });
            }

            var hasPermission = await deliveryMethodInfoService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            DeleteHistoryViewModel model = new DeleteHistoryViewModel();
            model.ToAudit(LoginInfo, false);
            model.CompanyCode = LoginInfo.CompanyCode;

            var success = await deliveryMethodInfoService.DeleteTab(ids, model);
            if (success.succses)
            {
                return Json(new { success = true, message = "Deleted Successfully." });
            }
            else
            {
                return Json(new { success = false, message = success.messege });
            }
        }

        #endregion

        #region TabeleLodaing

        [HttpGet]
        public async Task<IActionResult> GetTableData()
        {
            try
            {
                var list = await deliveryMethodInfoService.GetAllAsync();
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

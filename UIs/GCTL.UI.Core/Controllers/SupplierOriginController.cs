using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.SupplierOrigin;
using GCTL.Service.ColorInformation;
using GCTL.Service.Common;
using GCTL.Service.SupplierOrigin;
using GCTL.UI.Core.ViewModels.SupplierOrigin;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class SupplierOriginController : BaseController
    {
        #region Service & Repository
        public readonly ISupplierOriginService supplierOriginService;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;

        public SupplierOriginController(
            ISupplierOriginService supplierOriginService,
            ICommonService commonService
            
            )
        {
            this.supplierOriginService = supplierOriginService;
            this.commonService = commonService;
        }

        #endregion

        #region Index
        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await supplierOriginService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new SupplierOriginPageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {

                var list = await supplierOriginService.GetAllAsync();
                model.SupplierOriginList = list ?? new List<SupplierOriginSetupViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "SupplierOriginID", "Inv_Def_SupplierOrigin", 3);

                model.Setup = new SupplierOriginSetupViewModel
                {
                    SupplierOriginId = strMaxNO
                };

            }
            catch (Exception ex)
            {

                model.SupplierOriginList = new List<SupplierOriginSetupViewModel>();
                model.Setup = new SupplierOriginSetupViewModel();
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
            SupplierOriginSetupViewModel model = new SupplierOriginSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "SupplierOriginID", "Inv_Def_SupplierOrigin", 3);

            if (!string.IsNullOrEmpty(id))
            {

                model = await supplierOriginService.GetByIdAsync(id);
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.SupplierOriginId = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        #endregion

        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(SupplierOriginSetupViewModel modelVM)
        {
            try
            {

                if (await supplierOriginService.IsExistAsync(modelVM.SupplierOrigin, modelVM.SupplierOriginId))
                {
                    return Json(new { isSuccess = false, message = $"Already Exists!", isDuplicate = true });
                }


                if (!ModelState.IsValid)
                {

                    var errorMessage = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                    return Json(new { isSuccess = false, message = errorMessage });
                }

                modelVM.ToAudit(LoginInfo, modelVM.Tc > 0);
                if (modelVM.Tc == 0)
                {
                    var hasSavePermission = await supplierOriginService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await supplierOriginService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.SupplierOriginId });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await supplierOriginService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await supplierOriginService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.SupplierOriginId });
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

            var hasPermission = await supplierOriginService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await supplierOriginService.DeleteTab(ids);
            if (success)
            {
                return Json(new { success = true, message = "Deleted Successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Deletion failed." });
            }
        }

        #endregion

        #region TabeleLodaing

        [HttpGet]
        public async Task<IActionResult> GetTableData()
        {
            try
            {
                var list = await supplierOriginService.GetAllAsync();
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

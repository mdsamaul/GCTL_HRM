using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.ColorInformation;
using GCTL.Core.ViewModels.SupplierType;
using GCTL.Service.ColorInformation;
using GCTL.Service.Common;
using GCTL.Service.SupplierType;
using GCTL.UI.Core.ViewModels.SupplierType;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class SupplierTypeController : BaseController
    {
        #region Service & Repository
        public readonly ISupplierTypeService supplierTypeService;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;

        public SupplierTypeController(
            ISupplierTypeService supplierTypeService,
            ICommonService commonService
            
            )
        {
            this.supplierTypeService = supplierTypeService;
            this.commonService = commonService;
        }

        #endregion

        #region Index
        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await supplierTypeService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new SupplierTypePageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {

                var list = await supplierTypeService.GetAllAsync();
                model.SupplierTypeList = list ?? new List<SupplierTypeSetupViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "SupplierTypeID", "Inv_Def_SupplierType", 3);

                model.Setup = new SupplierTypeSetupViewModel
                {
                    SupplierTypeId = strMaxNO
                };

            }
            catch (Exception ex)
            {

                model.SupplierTypeList = new List<SupplierTypeSetupViewModel>();
                model.Setup = new SupplierTypeSetupViewModel();
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
            SupplierTypeSetupViewModel model = new SupplierTypeSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "SupplierTypeID", "Inv_Def_SupplierType", 3);

            if (!string.IsNullOrEmpty(id))
            {

                model = await supplierTypeService.GetByIdAsync(id);
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.SupplierTypeId = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        #endregion

        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(SupplierTypeSetupViewModel modelVM)
        {
            try
            {

                if (await supplierTypeService.IsExistAsync(modelVM.SupplierTypeName, modelVM.SupplierTypeId))
                {
                    return Json(new { isSuccess = false, message = $"Already Exists!", isDuplicate = true });
                }


                if (!ModelState.IsValid)
                {

                    var errorMessage = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                    return Json(new { isSuccess = false, message = errorMessage });
                }

                modelVM.ToAudit(LoginInfo, modelVM.SupplierTypeCode > 0);
                if (modelVM.SupplierTypeCode == 0)
                {
                    var hasSavePermission = await supplierTypeService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await supplierTypeService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.SupplierTypeId });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await supplierTypeService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await supplierTypeService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.SupplierTypeId });
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

            var hasPermission = await supplierTypeService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await supplierTypeService.DeleteTab(ids);
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
                var list = await supplierTypeService.GetAllAsync();
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

using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.SupplierCategory;
using GCTL.Service.Common;
using GCTL.Service.SupplierCategory;
using GCTL.UI.Core.ViewModels.SupplierCategory;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class SupplierCategoryController : BaseController
    {
        #region Service & Repository
        public readonly ISupplierCategoryService supplierCategoryService;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;

        public SupplierCategoryController(
            ISupplierCategoryService supplierCategoryService,
            ICommonService commonService
            
            )
        {
            this.supplierCategoryService = supplierCategoryService;
            this.commonService = commonService;
        }

        #endregion

        #region Index
        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await supplierCategoryService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new SupplierCategoryPageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {

                var list = await supplierCategoryService.GetAllAsync();
                model.SupplierCategoryList = list ?? new List<SupplierCategorySetupViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "SupplierCategoryID", "Inv_Def_SupplierCategory", 3);

                model.Setup = new SupplierCategorySetupViewModel
                {
                    SupplierCategoryId = strMaxNO
                };

            }
            catch (Exception ex)
            {

                model.SupplierCategoryList = new List<SupplierCategorySetupViewModel>();
                model.Setup = new SupplierCategorySetupViewModel();
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
            SupplierCategorySetupViewModel model = new SupplierCategorySetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "SupplierCategoryID", "Inv_Def_SupplierCategory", 3);

            if (!string.IsNullOrEmpty(id))
            {

                model = await supplierCategoryService.GetByIdAsync(id);
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.SupplierCategoryId = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        #endregion

        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(SupplierCategorySetupViewModel modelVM)
        {
            try
            {

                if (await supplierCategoryService.IsExistAsync(modelVM.SupplierCategory, modelVM.SupplierCategoryId))
                {
                    return Json(new { isSuccess = false, message = $"Already Exists!", isDuplicate = true });
                }


                if (!ModelState.IsValid)
                {

                    var errorMessage = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                    return Json(new { isSuccess = false, message = errorMessage });
                }

                modelVM.ToAudit(LoginInfo, modelVM.SupplierCategoryCode > 0);
                if (modelVM.SupplierCategoryCode == 0)
                {
                    var hasSavePermission = await supplierCategoryService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await supplierCategoryService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.SupplierCategoryId });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await supplierCategoryService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await supplierCategoryService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.SupplierCategoryId });
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

            var hasPermission = await supplierCategoryService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await supplierCategoryService.DeleteTab(ids);
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
                var list = await supplierCategoryService.GetAllAsync();
                return PartialView("_Grid", list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        #endregion

        #region Chake Degian

        //public IActionResult Index()
        //{
        //    SupplierCategoryPageViewModel model = new SupplierCategoryPageViewModel
        //    {
        //        Setup = new SupplierCategorySetupViewModel()
        //    };
        //    return View(model);
        //}

        #endregion
    }
}

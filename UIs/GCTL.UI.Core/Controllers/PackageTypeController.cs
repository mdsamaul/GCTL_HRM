using GCTL.Core.ViewModels.ColorInformation;
using GCTL.Core.ViewModels.PackageType;
using GCTL.Service.ColorInformation;
using GCTL.Service.Common;
using GCTL.Core.Helpers;
using GCTL.Service.PackageType;
using GCTL.UI.Core.ViewModels.ColorInformation;
using GCTL.UI.Core.ViewModels.PackageType;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class PackageTypeController : BaseController
    {
        #region Service & Repository
        public readonly IPackageTypeService packageTypeService;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;

        public PackageTypeController(
            IPackageTypeService packageTypeService, 
            ICommonService commonService
            
            )
        {
            this.packageTypeService = packageTypeService;
            this.commonService = commonService;
        }

        #endregion

        #region Index
        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await packageTypeService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new PackageTypePageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {

                var list = await packageTypeService.GetAllAsync();
                model.PackageList = list ?? new List<PackageTypeSetupViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "PackageTypeID", "Inv_Def_PackageType", 3);

                model.Setup = new PackageTypeSetupViewModel
                {
                    PackageTypeId = strMaxNO
                };

            }
            catch (Exception ex)
            {

                model.PackageList = new List<PackageTypeSetupViewModel>();
                model.Setup = new PackageTypeSetupViewModel();
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
            PackageTypeSetupViewModel model = new PackageTypeSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "PackageTypeID", "Inv_Def_PackageType", 3);

            if (!string.IsNullOrEmpty(id))
            {

                model = await packageTypeService.GetByIdAsync(id);
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.PackageTypeId = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        #endregion

        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(PackageTypeSetupViewModel modelVM)
        {
            try
            {

                if (await packageTypeService.IsExistAsync(modelVM.PackageType, modelVM.PackageTypeId))
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
                    var hasSavePermission = await packageTypeService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await packageTypeService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.PackageTypeId });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await packageTypeService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await packageTypeService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.PackageTypeId });
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

            var hasPermission = await packageTypeService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await packageTypeService.DeleteTab(ids);
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
                var list = await packageTypeService.GetAllAsync();
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
        //    PackageTypePageViewModel model = new PackageTypePageViewModel
        //    {
        //        Setup = new PackageTypeSetupViewModel()
        //    };
        //    return View(model);
        //}

        #endregion
    }
}

using GCTL.Core.Data;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.PackagingInformation;
using GCTL.Core.ViewModels.SupplierCategory;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.PackagingInformation;
using GCTL.Service.SupplierCategory;
using GCTL.UI.Core.ViewModels.PackagingInformation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GCTL.UI.Core.Controllers
{
    public class PackagingInformationController : BaseController
    {
        #region Service & Repository
        public readonly IPackagingInformationService packagingInformationService;
        private readonly IRepository<RmgProdDefUnitType> unitTyperepository;
        private readonly IRepository<InvDefPackageType> invTyperepository;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;

        public PackagingInformationController(
            IPackagingInformationService packagingInformationService, 
            IRepository<RmgProdDefUnitType> unitTyperepository, 
            IRepository<InvDefPackageType> invTyperepository,
            ICommonService commonService
            
            )
        {
            this.packagingInformationService = packagingInformationService;
            this.unitTyperepository = unitTyperepository;
            this.invTyperepository = invTyperepository;
            this.commonService = commonService;
        }

        #endregion

        #region Index
        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await packagingInformationService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new PackagingInformationPageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {

                var list = await packagingInformationService.GetAllAsync();
                model.PackagingList = list ?? new List<PackagingInformationSetupViewModel>();

                ViewBag.TypeDD = new SelectList(invTyperepository.All(), "PackageTypeId", "PackageType");
                ViewBag.UnitTypDD = new SelectList(unitTyperepository.All(), "UnitTypId", "UnitTypeName");

                commonService.FindMaxNo(ref strMaxNO, "PackageID", "RMG_Prod_Def_Package", 3);

                model.Setup = new PackagingInformationSetupViewModel
                {
                    PackageId = strMaxNO
                };

            }
            catch (Exception ex)
            {

                model.PackagingList = new List<PackagingInformationSetupViewModel>();
                model.Setup = new PackagingInformationSetupViewModel();
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
            PackagingInformationSetupViewModel model = new PackagingInformationSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "PackageID", "RMG_Prod_Def_Package", 3);

            if (!string.IsNullOrEmpty(id))
            {

                model = await packagingInformationService.GetByIdAsync(id);
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.PackageId = strMaxNO;
            }

            ViewBag.TypeDD = new SelectList(invTyperepository.All(), "PackageTypeId", "PackageType");
            ViewBag.UnitTypDD = new SelectList(unitTyperepository.All(), "UnitTypId", "UnitTypeName");

            return PartialView($"_{nameof(Setup)}", model);
        }

        #endregion

        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(PackagingInformationSetupViewModel modelVM)
        {
            try
            {

                if (await packagingInformationService.IsExistAsync(modelVM.PackageName, modelVM.PackageId, modelVM.Type))
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
                    var hasSavePermission = await packagingInformationService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await packagingInformationService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.PackageId });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await packagingInformationService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await packagingInformationService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.PackageId });
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

            var hasPermission = await packagingInformationService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await packagingInformationService.DeleteTab(ids);
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
                var list = await packagingInformationService.GetAllAsync();
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
        //    PackagingInformationPageViewModel model = new PackagingInformationPageViewModel
        //    {
        //        Setup = new PackagingInformationSetupViewModel()
        //    };
        //    return View(model);
        //}

        #endregion
    }
}

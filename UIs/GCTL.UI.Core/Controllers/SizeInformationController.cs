using GCTL.Core.ViewModels.SizeInformation;
using GCTL.Service.Common;
using GCTL.Core.Helpers;
using GCTL.Service.SizeInformation;
using GCTL.UI.Core.ViewModels.SizeInformation;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class SizeInformationController : BaseController
    {
        #region Service & Repository
        public readonly ISizeInformationService sizeInformationService;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;

        public SizeInformationController(
            ISizeInformationService sizeInformationService,
            ICommonService commonService
            
            )
        {
            this.sizeInformationService = sizeInformationService;
            this.commonService = commonService;
        }

        #endregion

        #region Index
        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await sizeInformationService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new SizeInformationPageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {

                var list = await sizeInformationService.GetAllAsync();
                model.SizeList = list ?? new List<SizeInformationSetupViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "SizeId", "RMG_Prod_Def_Size", 3);

                model.Setup = new SizeInformationSetupViewModel
                {
                    SizeId = strMaxNO
                };

            }
            catch (Exception ex)
            {

                model.SizeList = new List<SizeInformationSetupViewModel>();
                model.Setup = new SizeInformationSetupViewModel();
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
            SizeInformationSetupViewModel model = new SizeInformationSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "SizeId", "RMG_Prod_Def_Size", 3);

            if (!string.IsNullOrEmpty(id))
            {

                model = await sizeInformationService.GetByIdAsync(id);
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.SizeId = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        #endregion

        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(SizeInformationSetupViewModel modelVM)
        {
            try
            {

                if (await sizeInformationService.IsExistAsync(modelVM.Size, modelVM.SizeId))
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
                    var hasSavePermission = await sizeInformationService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await sizeInformationService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.SizeId });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await sizeInformationService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await sizeInformationService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.SizeId });
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

            var hasPermission = await sizeInformationService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await sizeInformationService.DeleteTab(ids);
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
                var list = await sizeInformationService.GetAllAsync();
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
        //    SizeInformationPageViewModel model = new SizeInformationPageViewModel
        //    {
        //        Setup = new SizeInformationSetupViewModel()
        //    };
        //    return View(model);
        //}

        #endregion
    }
}

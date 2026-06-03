using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.ColorInformation;
using GCTL.Service.ColorInformation;
using GCTL.Service.Common;
using GCTL.UI.Core.ViewModels.ColorInformation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GCTL.UI.Core.Controllers
{
    public class ColorInformationController : BaseController
    {
        #region Service & Repository
        public readonly IColorInformationService colorInformationService;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;

        public ColorInformationController(
            IColorInformationService colorInformationService, 
            ICommonService commonService
            
            )
        {
            this.colorInformationService = colorInformationService;
            this.commonService = commonService;
        }

        #endregion

        #region Index
        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await colorInformationService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new ColorInformationPageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {

                var list = await colorInformationService.GetAllAsync();
                model.ColorList = list ?? new List<ColorInformationSetupViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "ColorId", "RMG_Prod_Def_Color", 3);

                model.Setup = new ColorInformationSetupViewModel
                {
                    ColorId = strMaxNO
                };

            }
            catch (Exception ex)
            {

                model.ColorList = new List<ColorInformationSetupViewModel>();
                model.Setup = new ColorInformationSetupViewModel();
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
            ColorInformationSetupViewModel model = new ColorInformationSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "ColorId", "RMG_Prod_Def_Color", 3);

            if (!string.IsNullOrEmpty(id))
            {

                model = await colorInformationService.GetByIdAsync(id);
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.ColorId = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        #endregion

        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(ColorInformationSetupViewModel modelVM)
        {
            try
            {

                if (await colorInformationService.IsExistAsync(modelVM.Color, modelVM.ColorId))
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
                    var hasSavePermission = await colorInformationService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await colorInformationService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.ColorId });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await colorInformationService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await colorInformationService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.ColorId });
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

            var hasPermission = await colorInformationService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await colorInformationService.DeleteTab(ids);
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
                var list = await colorInformationService.GetAllAsync();
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
        //    ColorInformationPageViewModel model = new ColorInformationPageViewModel
        //    {
        //        Setup = new ColorInformationSetupViewModel()
        //    };
        //    return View(model);
        //}

        #endregion
    }
}

using GCTL.Core.ViewModels.ColorInformation;
using GCTL.Core.ViewModels.SeasonInformation;
using GCTL.Service.ColorInformation;
using GCTL.Service.Common;
using GCTL.Core.Helpers;
using GCTL.Service.SeasonInformation;
using GCTL.UI.Core.ViewModels.ColorInformation;
using GCTL.UI.Core.ViewModels.SeasonInformation;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class SeasonInformationController : BaseController
    {
        #region Service & Repository
        public readonly ISeasonInformationService seasonInformationService;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;

        public SeasonInformationController(
            ISeasonInformationService seasonInformationService,
            ICommonService commonService
            
            )
        {
            this.seasonInformationService = seasonInformationService;
            this.commonService = commonService;
        }

        #endregion

        #region Index
        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await seasonInformationService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new SeasonInformationPageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {

                var list = await seasonInformationService.GetAllAsync();
                model.SeasonList = list ?? new List<SeasonInformationSetupViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "SeasonId", "RMG_Prod_Def_Season", 3);

                model.Setup = new SeasonInformationSetupViewModel
                {
                    SeasonId = strMaxNO
                };

            }
            catch (Exception ex)
            {

                model.SeasonList = new List<SeasonInformationSetupViewModel>();
                model.Setup = new SeasonInformationSetupViewModel();
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
            SeasonInformationSetupViewModel model = new SeasonInformationSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "SeasonId", "RMG_Prod_Def_Season", 3);

            if (!string.IsNullOrEmpty(id))
            {

                model = await seasonInformationService.GetByIdAsync(id);
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.SeasonId = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        #endregion

        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(SeasonInformationSetupViewModel modelVM)
        {
            try
            {

                if (await seasonInformationService.IsExistAsync(modelVM.Season, modelVM.SeasonId))
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
                    var hasSavePermission = await seasonInformationService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await seasonInformationService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.SeasonId });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await seasonInformationService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await seasonInformationService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.SeasonId });
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

            var hasPermission = await seasonInformationService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await seasonInformationService.DeleteTab(ids);
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
                var list = await seasonInformationService.GetAllAsync();
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
        //    SeasonInformationPageViewModel model = new SeasonInformationPageViewModel
        //    {
        //        Setup = new SeasonInformationSetupViewModel()
        //    };
        //    return View(model);
        //}

        #endregion
    }
}

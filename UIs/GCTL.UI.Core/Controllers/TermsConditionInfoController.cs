using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.TermsConditionInfo;
using GCTL.Service.Common;
using GCTL.Service.TermsConditionInfo;
using GCTL.UI.Core.ViewModels.TermsConditionInfo;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class TermsConditionInfoController : BaseController
    {
        #region Service & Repository
        public readonly ITermsConditionInfoService termsConditionInfoService;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;

        public TermsConditionInfoController(
            ITermsConditionInfoService termsConditionInfoService,
            ICommonService commonService

            )
        {
            this.termsConditionInfoService = termsConditionInfoService;
            this.commonService = commonService;
        }

        #endregion

        #region Index
        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await termsConditionInfoService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new TermsConditionInfoPageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {

                var list = await termsConditionInfoService.GetAllAsync();
                model.TermsConditionList = list ?? new List<TermsConditionInfoSetupViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "TermsConditionId", "RMG_TermsCondition", 4);

                model.Setup = new TermsConditionInfoSetupViewModel
                {
                    TermsConditionId = strMaxNO
                };

            }
            catch (Exception ex)
            {

                model.TermsConditionList = new List<TermsConditionInfoSetupViewModel>();
                model.Setup = new TermsConditionInfoSetupViewModel();
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
            TermsConditionInfoSetupViewModel model = new TermsConditionInfoSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "TermsConditionId", "RMG_TermsCondition", 4);

            if (!string.IsNullOrEmpty(id))
            {

                model = await termsConditionInfoService.GetByIdAsync(id);
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.TermsConditionId = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        #endregion

        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(TermsConditionInfoSetupViewModel modelVM)
        {
            try
            {

                if (await termsConditionInfoService.IsExistAsync(modelVM.TermsConditionName, modelVM.TermsConditionId))
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
                    var hasSavePermission = await termsConditionInfoService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await termsConditionInfoService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.TermsConditionId });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await termsConditionInfoService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await termsConditionInfoService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.TermsConditionId });
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

            var hasPermission = await termsConditionInfoService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            DeleteHistoryViewModel model = new DeleteHistoryViewModel();
            model.ToAudit(LoginInfo);
            model.CompanyCode = LoginInfo.CompanyCode;

            var success = await termsConditionInfoService.DeleteTab(ids, model);
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
                var list = await termsConditionInfoService.GetAllAsync();
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
        //    TermsConditionInfoPageViewModel model = new TermsConditionInfoPageViewModel
        //    {
        //        Setup = new TermsConditionInfoSetupViewModel()
        //    };
        //    return View(model);
        //}

        #endregion
    }
}

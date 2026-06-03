using GCTL.Core.ViewModels.FebricTest;
using GCTL.Core.ViewModels.SizeInformation;
using GCTL.Service.Common;
using GCTL.Core.Helpers;
using GCTL.Service.FebricTest;
using GCTL.Service.SizeInformation;
using GCTL.UI.Core.ViewModels.FebricTest;
using GCTL.UI.Core.ViewModels.SizeInformation;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class FebricTestController : BaseController
    {
        #region Service & Repository
        public readonly IFebricTestService febricTestService;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;

        public FebricTestController(
            IFebricTestService febricTestService,
            ICommonService commonService
            
            )
        {
            this.febricTestService = febricTestService;
            this.commonService = commonService;
        }

        #endregion

        #region Index
        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await febricTestService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new FebricTestPageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {

                var list = await febricTestService.GetAllAsync();
                model.FebricTestList = list ?? new List<FebricTestSetupViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "FebricTestD", "Inv_Def_FebricTesting", 3);

                model.Setup = new FebricTestSetupViewModel
                {
                    FebricTestD = strMaxNO
                };

            }
            catch (Exception ex)
            {

                model.FebricTestList = new List<FebricTestSetupViewModel>();
                model.Setup = new FebricTestSetupViewModel();
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
            FebricTestSetupViewModel model = new FebricTestSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "FebricTestD", "Inv_Def_FebricTesting", 3);

            if (!string.IsNullOrEmpty(id))
            {

                model = await febricTestService.GetByIdAsync(id);
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.FebricTestD = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        #endregion

        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(FebricTestSetupViewModel modelVM)
        {
            try
            {

                if (await febricTestService.IsExistAsync(modelVM.FebricTestName, modelVM.FebricTestD))
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
                    var hasSavePermission = await febricTestService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await febricTestService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.FebricTestD });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await febricTestService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await febricTestService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.FebricTestD });
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

            var hasPermission = await febricTestService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await febricTestService.DeleteTab(ids);
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
                var list = await febricTestService.GetAllAsync();
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

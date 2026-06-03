using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.GarmentsTest;
using GCTL.Core.ViewModels.SizeInformation;
using GCTL.Service.Common;
using GCTL.Service.GarmentsTest;
using GCTL.Service.SizeInformation;
using GCTL.UI.Core.ViewModels.GarmentsTest;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class GarmentsTestController : BaseController
    {
        #region Service & Repository
        private readonly IGarmentsTestService garmentsTestService;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;

        public GarmentsTestController(
            IGarmentsTestService garmentsTestService,        
            ICommonService commonService
            
            )
        {
            this.garmentsTestService = garmentsTestService;
            this.commonService = commonService;
        }

        #endregion

        #region Index
        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await garmentsTestService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new GarmentsTestPageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {

                var list = await garmentsTestService.GetAllAsync();
                model.GarmentsTestList = list ?? new List<GarmentsTestSetupViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "GarmentsTestD", "Inv_Def_GarmentsTesing", 3);

                model.Setup = new GarmentsTestSetupViewModel
                {
                    GarmentsTestD = strMaxNO
                };

            }
            catch (Exception ex)
            {

                model.GarmentsTestList = new List<GarmentsTestSetupViewModel>();
                model.Setup = new GarmentsTestSetupViewModel();
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
            GarmentsTestSetupViewModel model = new GarmentsTestSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "GarmentsTestD", "Inv_Def_GarmentsTesing", 3);

            if (!string.IsNullOrEmpty(id))
            {

                model = await garmentsTestService.GetByIdAsync(id);
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.GarmentsTestD = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        #endregion

        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(GarmentsTestSetupViewModel modelVM)
        {
            try
            {

                if (await garmentsTestService.IsExistAsync(modelVM.GarmentsTestName, modelVM.GarmentsTestD))
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
                    var hasSavePermission = await garmentsTestService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await garmentsTestService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.GarmentsTestD });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await garmentsTestService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await garmentsTestService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.GarmentsTestD });
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

            var hasPermission = await garmentsTestService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await garmentsTestService.DeleteTab(ids);
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
                var list = await garmentsTestService.GetAllAsync();
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

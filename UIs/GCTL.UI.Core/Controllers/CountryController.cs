using GCTL.Core.ViewModels.Country;
using GCTL.Core.ViewModels.SupplierCategory;
using GCTL.Service.Common;
using GCTL.Service.Country;
using GCTL.Core.Helpers;
using GCTL.Service.SupplierCategory;
using GCTL.UI.Core.ViewModels.Country;
using GCTL.UI.Core.ViewModels.SupplierCategory;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class CountryController : BaseController
    {
        #region Service & Repository
        public readonly ICountryService countryService;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;

        public CountryController(
            ICountryService countryService,
            ICommonService commonService
            
            )
        {
            this.countryService = countryService;
            this.commonService = commonService;
        }

        #endregion

        #region Index
        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await countryService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new CountryPageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {

                var list = await countryService.GetAllAsync();
                model.CountryList = list ?? new List<CountrySetuoViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "CountryId", "CA_Def_Country", 3);

                model.Setup = new CountrySetuoViewModel
                {
                    CountryId = strMaxNO
                };

            }
            catch (Exception ex)
            {

                model.CountryList = new List<CountrySetuoViewModel>();
                model.Setup = new CountrySetuoViewModel();
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
            CountrySetuoViewModel model = new CountrySetuoViewModel();
            commonService.FindMaxNo(ref strMaxNO, "CountryId", "CA_Def_Country", 3);

            if (!string.IsNullOrEmpty(id))
            {

                model = await countryService.GetByIdAsync(id);
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.CountryId = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        #endregion

        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(CountrySetuoViewModel modelVM)
        {
            try
            {

                if (await countryService.IsExistAsync(modelVM.CountryName, modelVM.CountryId))
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
                    var hasSavePermission = await countryService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await countryService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.CountryId });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await countryService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await countryService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.CountryId });
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

            var hasPermission = await countryService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await countryService.DeleteTab(ids);
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
                var list = await countryService.GetAllAsync();
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

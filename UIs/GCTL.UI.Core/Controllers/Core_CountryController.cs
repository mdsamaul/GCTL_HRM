using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.Core_Country;
using GCTL.Service.Core_Countrys;
using GCTL.UI.Core.ViewModels.Core_Country;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class Core_CountryController : BaseController
    {
        private readonly ICore_CountryService core_CountryService;

        public Core_CountryController(
             ICore_CountryService core_CountryService
            )
        {
            this.core_CountryService = core_CountryService;
        }
        public IActionResult Index(bool isPartial)
        {
            Core_CountryViewModel model = new Core_CountryViewModel() { 
            PageUrl=Url.Action(nameof(Index))
            };
            if (isPartial) { 
            return PartialView(model);
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> LoadData()
        {
            try
            {
                var data = await core_CountryService.GetAllAsync();
                return Json(new { data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> AutoCountryId()
        {
            try
            {
                var newCategoryId = await core_CountryService.AutoCountryIdAsync();
                return Json(new { data = newCategoryId });
            }
            catch (Exception)
            {
                throw;
            }

        }


        [HttpPost]
        public async Task<IActionResult> CreateUpdate([FromBody] Core_CountrySetupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { isSuccess = false });
            }
            try
            {
                model.ToAudit(LoginInfo);
                if (model.CountryCode == 0)
                {
                    bool hasParmision = await core_CountryService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasParmision)
                    {
                        var result = await core_CountryService.CreateUpdateAsync(model);
                        return Json(new { isSuccess = result.isSuccess, message = result.message, data = result.data });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
                    }
                }
                else
                {
                    var hasUpdatePermission = await core_CountryService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        var result = await core_CountryService.CreateUpdateAsync(model);
                        return Json(new { isSuccess = result.isSuccess, message = result.message, data = result.data });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });
                    }
                }
            }
            catch (Exception)
            {

                return Json(new { isSuccess = false, message = "Faild" });
            }


        }

        [HttpGet]
        public async Task<IActionResult> PopulatedDataForUpdate(string id)
        {
            var result = await core_CountryService.GetByIdAsync(id);
            return Json(new { result });
        }

        [HttpPost]
        public async Task<IActionResult> deleteCountry([FromBody] List<int> selectedIds)
        {
            var hasUpdatePermission = await core_CountryService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (hasUpdatePermission)
            {
                var result = await core_CountryService.DeleteAsync(selectedIds);
                return Json(new { isSuccess = result.isSuccess, message = result.message, data = result });
            }
            else
            {
                return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });
            }

        }
        [HttpPost]
        public async Task<IActionResult> alreadyExist([FromBody] string CountryValue)
        {
            var result = await core_CountryService.AlreadyExistAsync(CountryValue);
            return Json(new { isSuccess = result.isSuccess, message = result.message, data = result });
        }
    }
}

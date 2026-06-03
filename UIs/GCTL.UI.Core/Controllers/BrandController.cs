using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.Brand;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Service.Departments;
using GCTL.Service.HRM_Brand;
using GCTL.UI.Core.ViewModels.Brand;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class BrandController : BaseController
    {
        private readonly IHRM_BrandService hRM_BrandService;

        public BrandController(
            IHRM_BrandService hRM_BrandService
            )
        {
            this.hRM_BrandService = hRM_BrandService;
        }
        public IActionResult Index(bool isPartial)
        {
            BrandViewModel model = new BrandViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };
            if (isPartial)
            {
                return PartialView(model);
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> LoadData()
        {
            try
            {
                var data = await hRM_BrandService.GetAllAsync();
                return Json(new { data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> AutoBrandId()
        {
            try
            {
                var newBrandId = await hRM_BrandService.AutoBrandIdAsync();
                return Json(new { data = newBrandId });
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateUpdate([FromBody] BrandSetupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { isSuccess = false });
            }
            try
            {
                model.ToAudit(LoginInfo);
                if (model.AutoId == 0)
                {
                    bool hasParmision = await hRM_BrandService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasParmision)
                    {
                        var result = await hRM_BrandService.CreateUpdateAsync(model);
                        return Json(new { isSuccess = result.isSuccess, message = result.message, data = result.data });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
                    }
                }
                else
                {
                    var hasUpdatePermission = await hRM_BrandService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        var result = await hRM_BrandService.CreateUpdateAsync(model);
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
            var result = await hRM_BrandService.GetByIdAsync(id);
            return Json(new { result });
        }

        [HttpPost]
        public async Task<IActionResult> deleteBrand([FromBody] List<string> selectedIds)
        {
            var hasUpdatePermission = await hRM_BrandService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (hasUpdatePermission)
            {
                DeleteHistoryViewModel model = new DeleteHistoryViewModel();
                model.ToAudit(LoginInfo);
                model.CompanyCode = LoginInfo.CompanyCode;
                var result = await hRM_BrandService.DeleteAsync(selectedIds, model);
                return Json(new { isSuccess = result.success, message = result.message, data = result });
            }
            else
            {
                return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });
            }

        }
        [HttpPost]
        public async Task<IActionResult> alreadyExist([FromBody] string BrandValue)
        {
            var result = await hRM_BrandService.AlreadyExistAsync(BrandValue);
            return Json(new { isSuccess = result.isSuccess, message = result.message, data = result });
        }
    }
}

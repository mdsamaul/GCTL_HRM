using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HRM_Size;
using GCTL.Service.HRM_Size;
using GCTL.UI.Core.ViewModels.HRM_Size;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class HRM_SizeController : BaseController
    {
        private readonly IHRM_SizeService sizeService;

        public HRM_SizeController(
            IHRM_SizeService sizeService
            )
        {
            this.sizeService = sizeService;
        }
        public IActionResult Index(bool isPartial)
        {
            HRM_SizeViewModel model = new HRM_SizeViewModel() {
             PageUrl= Url.Action(nameof(Index)),
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
                var data = await sizeService.GetAllAsync();
                return Json(new { data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> AutoSizeID()
        {
            try
            {
                var newSizeId = await sizeService.AutoSizeyIdAsync();
                return Json(new { data = newSizeId });
            }
            catch (Exception)
            {
                throw;
            }

        }


        [HttpPost]
        public async Task<IActionResult> CreateUpdate([FromBody] HRM_SizeSetupViewModel model)
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
                    bool hasParmision = await sizeService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasParmision)
                    {
                        var result = await sizeService.CreateUpdateAsync(model);
                        return Json(new { isSuccess = result.isSuccess, message = result.message, data = result.data });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
                    }
                }
                else
                {
                    var hasUpdatePermission = await sizeService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        var result = await sizeService.CreateUpdateAsync(model);
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
            var result = await sizeService.GetByIdAsync(id);
            return Json(new { result });
        }

        [HttpPost]
        public async Task<IActionResult> deleteSize([FromBody] List<string> selectedIds)
        {
            var hasUpdatePermission = await sizeService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (hasUpdatePermission)
            {
                DeleteHistoryViewModel model = new DeleteHistoryViewModel();
                model.ToAudit(LoginInfo);
                model.CompanyCode = LoginInfo.CompanyCode;
                var result = await sizeService.DeleteAsync(selectedIds, model);
                return Json(new { isSuccess = result.success, message = result.message, data = result });
            }
            else
            {
                return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });
            }

        }
        [HttpPost]
        public async Task<IActionResult> alreadyExist([FromBody] string SizeyValue)
        {
            var result = await sizeService.AlreadyExistAsync(SizeyValue);
            return Json(new { isSuccess = result.isSuccess, message = result.message, data = result });
        }
    }
}

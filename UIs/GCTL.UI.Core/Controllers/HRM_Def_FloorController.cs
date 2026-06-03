using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.HRM_Def_Floor;
using GCTL.Service.HRM_Def_Floors;
using GCTL.UI.Core.ViewModels.HRM_Def_Floor;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class HRM_Def_FloorController : BaseController
    {
        private readonly IHRM_Def_FloorService hRM_Def_FloorService;

        public HRM_Def_FloorController(
            IHRM_Def_FloorService hRM_Def_FloorService
            )
        {
            this.hRM_Def_FloorService = hRM_Def_FloorService;
        }
        public IActionResult Index(bool isPartial)
        {
            HRM_Def_FloorViewModel model = new HRM_Def_FloorViewModel()
            {
                PageUrl= Url.Action(nameof(Index))
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
                var data = await hRM_Def_FloorService.GetAllAsync();
                return Json(new { data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> AutoFloorId()
        {
            try
            {
                var newCategoryId = await hRM_Def_FloorService.AutoFloorIdAsync();
                return Json(new { data = newCategoryId });
            }
            catch (Exception)
            {
                throw;
            }

        }


        [HttpPost]
        public async Task<IActionResult> CreateUpdate([FromBody] HRM_Def_FloorSetupViewModel model)
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
                    bool hasParmision = await hRM_Def_FloorService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasParmision)
                    {
                        var result = await hRM_Def_FloorService.CreateUpdateAsync(model, LoginInfo.CompanyCode, LoginInfo.EmployeeId);
                        return Json(new { isSuccess = result.isSuccess, message = result.message, data = result.data });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
                    }
                }
                else
                {
                    var hasUpdatePermission = await hRM_Def_FloorService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        var result = await hRM_Def_FloorService.CreateUpdateAsync(model, LoginInfo.CompanyCode, LoginInfo.EmployeeId);
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
            var result = await hRM_Def_FloorService.GetByIdAsync(id);
            return Json(new { result });
        }

        [HttpPost]
        public async Task<IActionResult> deleteFloor([FromBody] List<string> selectedIds)
        {
            var hasUpdatePermission = await hRM_Def_FloorService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (hasUpdatePermission)
            {
                var result = await hRM_Def_FloorService.DeleteAsync(selectedIds);
                return Json(new { isSuccess = result.isSuccess, message = result.message, data = result });
            }
            else
            {
                return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });
            }

        }
        [HttpPost]
        public async Task<IActionResult> alreadyExist([FromBody] string FloorValue)
        {
            var result = await hRM_Def_FloorService.AlreadyExistAsync(FloorValue);
            return Json(new { isSuccess = result.isSuccess, message = result.message, data = result });
        }
    }
}

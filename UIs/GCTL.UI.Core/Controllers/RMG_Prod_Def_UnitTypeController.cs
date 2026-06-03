using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.RMG_Prod_Def_UnitType;
using GCTL.Service.RMG_Prod_Def_UnitType;
using GCTL.UI.Core.ViewModels.RMG_Prod_Def_UnitType;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class RMG_Prod_Def_UnitTypeController : BaseController
    {
        private readonly IRMG_Prod_Def_UnitTypeService unitTypeService;

        public RMG_Prod_Def_UnitTypeController(
            IRMG_Prod_Def_UnitTypeService unitTypeService
            )
        {
            this.unitTypeService = unitTypeService;
        }
        public IActionResult Index(bool isPartial)
        {
            RMG_Prod_Def_UnitTypeViewModel model = new RMG_Prod_Def_UnitTypeViewModel()
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
                var data = await unitTypeService.GetAllAsync();
                return Json(new { data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> AutoUnitTypeId()
        {
            try
            {
                var newUnitTypeId = await unitTypeService.AutoUnitTypeIdAsync();
                return Json(new { data = newUnitTypeId });
            }
            catch (Exception)
            {
                throw;
            }

        }


        [HttpPost]
        public async Task<IActionResult> CreateUpdate([FromBody] RMG_Prod_Def_UnitTypeSetupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { isSuccess = false });
            }
            try
            {
                model.ToAudit(LoginInfo);
                if (model.TC == 0)
                {
                    bool hasParmision = await unitTypeService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasParmision)
                    {
                        var result = await unitTypeService.CreateUpdateAsync(model);
                        return Json(new { isSuccess = result.isSuccess, message = result.message, data = result.data });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
                    }
                }
                else
                {
                    var hasUpdatePermission = await unitTypeService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        var result = await unitTypeService.CreateUpdateAsync(model);
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
            var result = await unitTypeService.GetByIdAsync(id);
            return Json(new { result });
        }

        [HttpPost]
        public async Task<IActionResult> deleteUnitType([FromBody] List<string> selectedIds)
        {
            var hasUpdatePermission = await unitTypeService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (hasUpdatePermission)
            {
                DeleteHistoryViewModel model = new DeleteHistoryViewModel();
                model.ToAudit(LoginInfo);
                model.CompanyCode = LoginInfo.CompanyCode;
                var result = await unitTypeService.DeleteAsync(selectedIds, model);
                return Json(new { isSuccess = result.success, message = result.message, data = result });
            }
            else
            {
                return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });
            }

        }
        [HttpPost]
        public async Task<IActionResult> alreadyExist([FromBody] string UnitTypeValue)
        {
            var result = await unitTypeService.AlreadyExistAsync(UnitTypeValue);
            return Json(new { isSuccess = result.isSuccess, message = result.message, data = result });
        }
    }
}

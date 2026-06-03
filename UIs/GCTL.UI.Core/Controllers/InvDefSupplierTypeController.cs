using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.INV_Catagory;
using GCTL.Core.ViewModels.InvDefSupplierType;
using GCTL.Service.InvDefSupplierTypes;
using GCTL.UI.Core.ViewModels.InvDefSupplierType;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class InvDefSupplierTypeController : BaseController
    {
        private readonly IInvDefSupplierTypeService suplierService;

        public InvDefSupplierTypeController(
            IInvDefSupplierTypeService suplierService
            )
        {
            this.suplierService = suplierService;
        }
        public IActionResult Index(bool isPartial)
        {
            InvDefSupplierTypeViewModel model = new InvDefSupplierTypeViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };
            if(isPartial) return PartialView(model);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> LoadData()
        {
            try
            {
                var data = await suplierService.GetAllAsync();
                return Json(new { data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> AutoSuplierTypeId()
        {
            try
            {
                var newCategoryId = await suplierService.AutoSuplierTypeIdAsync();
                return Json(new { data = newCategoryId });
            }
            catch (Exception)
            {
                throw;
            }

        }


        [HttpPost]
        public async Task<IActionResult> CreateUpdate([FromBody] InvDefSupplierTypeSetupViewModel model)
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
                    bool hasParmision = await suplierService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasParmision)
                    {
                        var result = await suplierService.CreateUpdateAsync(model);
                        return Json(new { isSuccess = result.isSuccess, message = result.message, data = result.data });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
                    }
                }
                else
                {
                    var hasUpdatePermission = await suplierService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        var result = await suplierService.CreateUpdateAsync(model);
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
            var result = await suplierService.GetByIdAsync(id);
            return Json(new { result });
        }

        [HttpPost]
        public async Task<IActionResult> deleteSupplierType([FromBody] List<string> selectedIds)
        {
            var hasUpdatePermission = await suplierService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (hasUpdatePermission)
            {
                var result = await suplierService.DeleteAsync(selectedIds);
                return Json(new { isSuccess = result.isSuccess, message = result.message, data = result });
            }
            else
            {
                return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });
            }

        }
        [HttpPost]
        public async Task<IActionResult> alreadyExist([FromBody] string CatagoryValue)
        {
            var result = await suplierService.AlreadyExistAsync(CatagoryValue);
            return Json(new { isSuccess = result.isSuccess, message = result.message, data = result });
        }
    }
}

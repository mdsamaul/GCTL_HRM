using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.SalesDefVehicleType;
using GCTL.Service.SalesDefVehicleTypeService;
using GCTL.UI.Core.ViewModels.SalesDefVehicleType;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class SalesDefVehicleTypeController : BaseController
    {
        private readonly ISalesDefVehicleTypeService VehicleTypeService;

        public SalesDefVehicleTypeController(
            ISalesDefVehicleTypeService VehicleTypeService
            )
        {
            this.VehicleTypeService = VehicleTypeService;
        }
        public IActionResult Index(bool isPartial)
        {
            SalesDefVehicleTypeViewModel model = new SalesDefVehicleTypeViewModel()
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
                var data = await VehicleTypeService.GetAllAsync();
                return Json(new { data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> AutoVehicleTypeId()
        {
            try
            {
                var newCategoryId = await VehicleTypeService.AutoVehicleTypeIdAsync();
                return Json(new { data = newCategoryId });
            }
            catch (Exception)
            {
                throw;
            }

        }


        [HttpPost]
        public async Task<IActionResult> CreateUpdate([FromBody] SalesDefVehicleTypeSetupViewModel model)
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
                    bool hasParmision = await VehicleTypeService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasParmision)
                    {
                        var result = await VehicleTypeService.CreateUpdateAsync(model, LoginInfo.CompanyCode, LoginInfo.EmployeeId);
                        return Json(new { isSuccess = result.isSuccess, message = result.message, data = result.data });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
                    }
                }
                else
                {
                    var hasUpdatePermission = await VehicleTypeService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        var result = await VehicleTypeService.CreateUpdateAsync(model, LoginInfo.CompanyCode, LoginInfo.EmployeeId);
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
            var result = await VehicleTypeService.GetByIdAsync(id);
            return Json(new { result });
        }

        [HttpPost]
        public async Task<IActionResult> deleteTransport([FromBody] List<int> selectedIds)
        {
            var hasUpdatePermission = await VehicleTypeService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (hasUpdatePermission)
            {
                var result = await VehicleTypeService.DeleteAsync(selectedIds);
                return Json(new { isSuccess = result.isSuccess, message = result.message, data = result });
            }
            else
            {
                return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });
            }

        }
        [HttpPost]
        public async Task<IActionResult> alreadyExist([FromBody] string TransportValue)
        {
            var result = await VehicleTypeService.AlreadyExistAsync(TransportValue);
            return Json(new { isSuccess = result.isSuccess, message = result.message, data = result });
        }
    }
}

using GCTL.Core.Data;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.SalesDefVehicle;
using GCTL.Core.ViewModels.SalesDefVehicleType;
using GCTL.Data.Models;
using GCTL.Service.SalesDefVehicleService;
using GCTL.UI.Core.ViewModels.SalesDefVehicle;
using GCTL.UI.Core.ViewModels.SalesDefVehicleType;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GCTL.UI.Core.Controllers
{
    public class SalesDefVehicleController : BaseController
    {
        private readonly ISalesDefVehicleService TransportInfoService;
        private readonly IRepository<CoreCompany> comRepo;
        private readonly IRepository<SalesDefVehicleType> transportTypeRepo;

        public SalesDefVehicleController(
            ISalesDefVehicleService TransportInfoService,
            IRepository<CoreCompany> comRepo,
            IRepository<SalesDefVehicleType> transportTypeRepo
            )
        {
            this.TransportInfoService = TransportInfoService;
            this.comRepo = comRepo;
            this.transportTypeRepo = transportTypeRepo;
        }
        public IActionResult Index()
        {
            ViewBag.CompnayList = new SelectList(comRepo.All().Select(x => new { x.CompanyCode, x.CompanyName }), "CompanyCode", "CompanyName");
            ViewBag.TransportTypeList = new SelectList(transportTypeRepo.All().Select(x => new { x.VehicleTypeId, x.VehicleType }), "VehicleTypeId", "VehicleType");
            SalesDefVehicleViewModel model = new SalesDefVehicleViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> LoadData()
        {
            try
            {
                var data = await TransportInfoService.GetAllAsync();
                return Json(new { data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> CloaseLoadedTransportTypelist()
        {
            try
            {
                var transportTypeList = await transportTypeRepo.All().OrderByDescending(x=> x.Tc).ToListAsync();
                return Json(new { transportTypeList });

            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        public async Task<IActionResult> AutoTransportInfoId()
        {
            try
            {
                var newCategoryId = await TransportInfoService.AutoTransportInfoIdAsync();
                return Json(new { data = newCategoryId });
            }
            catch (Exception)
            {
                throw;
            }

        }


        [HttpPost]
        public async Task<IActionResult> CreateUpdate([FromBody] SalesDefVehicleSetupViewModel model)
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
                    bool hasParmision = await TransportInfoService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasParmision)
                    {
                        var result = await TransportInfoService.CreateUpdateAsync(model, LoginInfo.CompanyCode, LoginInfo.EmployeeId);
                        return Json(new { isSuccess = result.isSuccess, message = result.message, data = result.data });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
                    }
                }
                else
                {
                    var hasUpdatePermission = await TransportInfoService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        var result = await TransportInfoService.CreateUpdateAsync(model, LoginInfo.CompanyCode, LoginInfo.EmployeeId);
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
            var result = await TransportInfoService.GetByIdAsync(id);
            return Json(new { result });
        }

        [HttpPost]
        public async Task<IActionResult> deleteTransport([FromBody] List<int> selectedIds)
        {
            var hasUpdatePermission = await TransportInfoService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (hasUpdatePermission)
            {
                var result = await TransportInfoService.DeleteAsync(selectedIds);
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
            var result = await TransportInfoService.AlreadyExistAsync(TransportValue);
            return Json(new { isSuccess = result.isSuccess, message = result.message, data = result });
        }
    }
}

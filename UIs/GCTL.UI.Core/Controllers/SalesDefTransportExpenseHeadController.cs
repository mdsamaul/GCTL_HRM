using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.SalesDefTransportExpenseHead;
using GCTL.Core.ViewModels.SalesDefVehicleType;
using GCTL.Service.SalesDefTransportExpenseHeadService;
using GCTL.UI.Core.ViewModels.SalesDefTransportExpenseHead;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class SalesDefTransportExpenseHeadController : BaseController
    {
        public ISalesDefTransportExpenseHead TransportExpenseService { get; }

        public SalesDefTransportExpenseHeadController(
            ISalesDefTransportExpenseHead TransportExpenseService
            )
        {
            this.TransportExpenseService = TransportExpenseService;
        }
        public IActionResult Index()
        {
            SalesDefTransportExpenseHeadViewModel model = new SalesDefTransportExpenseHeadViewModel()
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
                var data = await TransportExpenseService.GetAllAsync();
                return Json(new { data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> AutoId()
        {
            try
            {
                var newCategoryId = await TransportExpenseService.AutoIdAsync();
                return Json(new { data = newCategoryId });
            }
            catch (Exception)
            {
                throw;
            }

        }


        [HttpPost]
        public async Task<IActionResult> CreateUpdate([FromBody] SalesDefTransportExpenseHeadSetupViewModel model)
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
                    bool hasParmision = await TransportExpenseService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasParmision)
                    {
                        var result = await TransportExpenseService.CreateUpdateAsync(model, LoginInfo.CompanyCode, LoginInfo.EmployeeId);
                        return Json(new { isSuccess = result.isSuccess, message = result.message, data = result.data });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
                    }
                }
                else
                {
                    var hasUpdatePermission = await TransportExpenseService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        var result = await TransportExpenseService.CreateUpdateAsync(model, LoginInfo.CompanyCode, LoginInfo.EmployeeId);
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
            var result = await TransportExpenseService.GetByIdAsync(id);
            return Json(new { result });
        }

        [HttpPost]
        public async Task<IActionResult> deleteTransport([FromBody] List<int> selectedIds)
        {
            var hasUpdatePermission = await TransportExpenseService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (hasUpdatePermission)
            {
                var result = await TransportExpenseService.DeleteAsync(selectedIds);
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
            var result = await TransportExpenseService.AlreadyExistAsync(TransportValue);
            return Json(new { isSuccess = result.isSuccess, message = result.message, data = result });
        }
    }
}

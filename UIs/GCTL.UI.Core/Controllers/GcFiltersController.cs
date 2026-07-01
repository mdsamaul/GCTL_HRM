using GCTL.Core.ViewModels.EachGcFilterRequest;
using GCTL.Service.EachGcFilterRequestService;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    [Route("GcFilters")]
    public class GcFiltersController : BaseController
    {
        private readonly IGcFilterService _svc;

        public GcFiltersController(IGcFilterService svc) => _svc = svc;

        public IActionResult Index() => View();

        [HttpPost("company")]
        public async Task<IActionResult> Company([FromBody] GcFilterRequestDto req)
            => Json(new { isSuccess = true, data = await _svc.GetCompaniesAsync(req) });

        [HttpPost("branch")]
        public async Task<IActionResult> Branch([FromBody] GcFilterRequestDto req)
            => Json(new { isSuccess = true, data = await _svc.GetBranchesAsync(req) });

        [HttpPost("division")]
        public async Task<IActionResult> Division([FromBody] GcFilterRequestDto req)
            => Json(new { isSuccess = true, data = await _svc.GetDivisionsAsync(req) });

        [HttpPost("department")]
        public async Task<IActionResult> Department([FromBody] GcFilterRequestDto req)
            => Json(new { isSuccess = true, data = await _svc.GetDepartmentsAsync(req) });

        [HttpPost("designation")]
        public async Task<IActionResult> Designation([FromBody] GcFilterRequestDto req)
            => Json(new { isSuccess = true, data = await _svc.GetDesignationsAsync(req) });

        [HttpPost("employee")]
        public async Task<IActionResult> Employee([FromBody] GcFilterRequestDto req)
        {
            req.EmployeeStatuses = req.EmployeeStatuses?.Count > 0 ? req.EmployeeStatuses : new List<string> { "01" };
            var data = await _svc.GetEmployeesAsync(req);
            return Json(new { isSuccess = true, data = await _svc.GetEmployeesAsync(req) });
        }
           
    }
}
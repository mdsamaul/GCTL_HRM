using GCTL.Core.ViewModels.GcAccessFilterRequest;
using GCTL.Service.GcAccessFilterService;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    [Route("HRM/GcAccessFilter")]
    public class GcAccessFilterController : BaseController
    {
        private readonly IGcAccessFilterService _service;

        public GcAccessFilterController(IGcAccessFilterService service)
        {
            _service = service;
        }

        public IActionResult Index()
        {
            return View();
        }

        // ─── Auth inject helper ───────────────────────────────────
        private void InjectLoginInfo(GcAccessFilterRequestDto req)
        {
            req.AccessCode = LoginInfo.AccessCode;
            req.EmployeeId = LoginInfo.EmployeeId;
        }

        // POST /GcAccessFilter/companies
        [HttpPost("companies")]
        public async Task<IActionResult> GetCompanies([FromBody] GcAccessFilterRequestDto req)
        {
            InjectLoginInfo(req);
            var data = await _service.GetCompanyListByAccessAsync(req);
            return Json(new { isSuccess = true, data });
        }

        // POST /GcAccessFilter/branches
        [HttpPost("branches")]
        public async Task<IActionResult> GetBranches([FromBody] GcAccessFilterRequestDto req)
        {
            InjectLoginInfo(req);
            var data = await _service.GetBranchListByAccessAsync(req);
            return Json(new { isSuccess = true, data });
        }

        // POST /GcAccessFilter/divisions
        [HttpPost("divisions")]
        public async Task<IActionResult> GetDivisions([FromBody] GcAccessFilterRequestDto req)
        {
            InjectLoginInfo(req);
            var data = await _service.GetDivisionListByAccessAsync(req);
            return Json(new { isSuccess = true, data });
        }

        // POST /GcAccessFilter/departments
        [HttpPost("departments")]
        public async Task<IActionResult> GetDepartments([FromBody] GcAccessFilterRequestDto req)
        {
            InjectLoginInfo(req);
            var data = await _service.GetDepartmentListByAccessAsync(req);
            return Json(new { isSuccess = true, data });
        }

        // POST /GcAccessFilter/designations
        [HttpPost("designations")]
        public async Task<IActionResult> GetDesignations([FromBody] GcAccessFilterRequestDto req)
        {
            InjectLoginInfo(req);
            var data = await _service.GetDesignationListByAccessAsync(req);
            return Json(new { isSuccess = true, data });
        }

        // POST /GcAccessFilter/employees
        [HttpPost("employees")]
        public async Task<IActionResult> GetEmployees([FromBody] GcAccessFilterRequestDto req)
        {
            InjectLoginInfo(req);
            var data = await _service.GetEmployeeListByAccessAsync(req);
            return Json(new { isSuccess = true, data });
        }
    }
}
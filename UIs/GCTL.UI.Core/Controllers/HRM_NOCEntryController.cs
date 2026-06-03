using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HRM_NOCEntry;
using GCTL.Service.HRLettersReport;
using GCTL.Service.HRM_NOCEntry;
using GCTL.Service.HRMPayrollLoan;
using GCTL.UI.Core.ViewModels.HRLettersReportSetupViewModel;
using GCTL.UI.Core.ViewModels.HRM_NOCEntryViewModel;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class HRM_NOCEntryController : BaseController
    {
        private readonly IHRM_NOCEntryService _service;

        public HRM_NOCEntryController(IHRM_NOCEntryService service)
        {
            _service = service;
        }

        // GET: /HRM_NOCEntry/Index
        public async Task<IActionResult> Index()
        {
            var hasPermission = await _service.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }
            var model = new HRM_NOCEntryViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };
            return View(model);
        }

        // POST: /HRM_NOCEntry/EmpDetails
        [HttpPost]
        public async Task<IActionResult> EmpDetailsAsync([FromBody] string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new { message = "Employee code is required." });

            var profile = await _service.GetByEmployeeCodeAsync(id);
            if (profile == null)
                return NotFound(new { message = "Employee not found." });

            return Ok(profile);
        }

        // GET: /HRM_NOCEntry/GetNewNocId
        [HttpGet]
        public async Task<IActionResult> GetNewNocId()
        {
            var nocId = await _service.GenerateNewNocIdAsync();
            return Ok(new { nocId });
        }

        // GET: /HRM_NOCEntry/GetById?autoId=5
        [HttpGet]
        public async Task<IActionResult> GetById([FromQuery] long autoId)
        {
            if (autoId <= 0)
                return BadRequest(new { message = "Invalid AutoId." });

            var record = await _service.GetNocByAutoIdAsync(autoId);
            if (record == null)
                return NotFound(new { message = "NOC record not found." });

            return Ok(record);
        }

        // POST: /HRM_NOCEntry/Save
        [HttpPost]
        public async Task<IActionResult> Save([FromBody] HRM_NOCEntrySetupViewModel model)
        {
            if (model == null)
                return BadRequest(new { message = "Invalid data." });

            // Set audit fields
            model.ToAudit(LoginInfo);

            var hasSavePermission = await _service.SavePermissionAsync(LoginInfo.AccessCode);
            if (hasSavePermission)
            {

                var result = await _service.SaveNocAsync(model, LoginInfo.CompanyCode);
                if (!result.Success)
                    return BadRequest(new { message = result.Message });

                return Ok(new { autoId = result.AutoId, nocId = result.NocId, lDate = result.LDate });
            }
            else
            {
                return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
            }

           
        }

        // POST: /HRM_NOCEntry/Update
        [HttpPost]
        public async Task<IActionResult> Update([FromBody] HRM_NOCEntrySetupViewModel model)
        {
            if (model == null || model.AutoId <= 0)
                return BadRequest(new { message = "Invalid data." });

            var hasUpdatePermission = await _service.UpdatePermissionAsync(LoginInfo.AccessCode);
            if (hasUpdatePermission)
            {
                model.ToAudit(LoginInfo);

                var result = await _service.UpdateNocAsync(model);
                if (!result.Success)
                    return BadRequest(new { message = result.Message });

                return Ok(new { modifyDate = result.ModifyDate });
            }
            else
            {
                return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });
            }
           
        }

        // POST: /HRM_NOCEntry/Delete
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] List<decimal> autoIds)
        {
            if (autoIds == null || !autoIds.Any())
                return BadRequest(new { message = "No records selected." });

           


            var hasPermission = await _service.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            DeleteHistoryViewModel dModel = new DeleteHistoryViewModel();
            dModel.ToAudit(LoginInfo);
            dModel.CompanyCode = LoginInfo.CompanyCode;
            var result = await _service.DeleteNocAsync(autoIds, dModel);
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            var msg = autoIds.Count == 1
                ? "Deleted successfully."
                : $"{autoIds.Count} record(s) deleted successfully.";

            return Ok(new { message = msg });

        }
        [HttpGet]
        public async Task<IActionResult> GetList(string nocType)
        {
            if (string.IsNullOrWhiteSpace(nocType))
                return BadRequest("nocType is required.");

            var data = await _service.GetListAsync(nocType);
            return Ok(data);
        }
    }
}

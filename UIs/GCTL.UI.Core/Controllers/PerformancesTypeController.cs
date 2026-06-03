using GCTL.Core.ViewModels.HrmDefPerformance2;
using GCTL.Service.Common;
using GCTL.Core.Helpers;
using GCTL.Service.PerformancesType;
using GCTL.UI.Core.ViewModels.HrmDefPerformance2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GCTL.Service.JobTitles;

namespace GCTL.UI.Core.Controllers
{
    public class PerformancesTypeController : BaseController
    {
        private readonly IHrmPerformancesTypeService service;
        private readonly ICommonService commonService;
        private readonly IJobTitleService jobTitleService;

        string strMaxNO = "";
        public PerformancesTypeController(IHrmPerformancesTypeService service, ICommonService commonService, IJobTitleService jobTitleService)
        {
            this.service = service;
            this.commonService = commonService;
            this.jobTitleService = jobTitleService;
        }

        #region GettALLById

        public async Task<IActionResult> Index(string? id)
        {
            var hasPermission = await service.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }
            //Get all
            HrmDefPerformance2PageViewModel model = new HrmDefPerformance2PageViewModel();
            var list = await service.GetAllAsync();
            model.HrmDefPerformanceList2 = list ?? new List<HrmDefPerformance2SetupViewModel>();

            //Get By Id
            if (!string.IsNullOrEmpty(id))
            {

                model.Setup = await service.GetByIdAsync(id)
;

            }
            //
            ViewBag.JobTitleIdDD = new SelectList(await jobTitleService.SelectionHrmDefJobTitleAsync(), "Code", "Name");
            //
            model.PageUrl = Url.Action(nameof(Index));
            return View(model);
        }
        #endregion

        #region Post Update 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Setup(HrmDefPerformance2SetupViewModel modelVM)
        {
            try
            {

                if (await service.IsExistAsync(modelVM.Performance, modelVM.PerformanceCode))
                {
                    return Json(new { isSuccess = false, message = $"Already <span style='color: blue;'>'{modelVM.Performance}'</span> Exists!", isDuplicate = true });
                }


                if (string.IsNullOrEmpty(modelVM.PerformanceCode))
                {
                    modelVM.PerformanceCode = await service.GenerateNextCode();
                }


                modelVM.ToAudit(LoginInfo, modelVM.AutoId > 0);
                if (modelVM.AutoId == 0)
                {
                    var hasSavePermission = await service.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await service.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = modelVM.PerformanceCode });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await service.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await service.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully", lastCode = modelVM.PerformanceCode });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to update", noUpdatePermission = true });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion

        #region CheckAvailability
        [HttpPost]
        public async Task<JsonResult> CheckAvailability(string name, string code)
        {
            if (await service.IsExistAsync(name, code))
            {

                return Json(new { isSuccess = true, message = $"Already <span style='color: blue;'>'{name}'</span> Exists" });

            }

            return Json(new { isSuccess = false });
        }
        #endregion

        #region Delete
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] List<string> ids)
        {
            try
            {

                var hasPermission = await service.DeletePermissionAsync(LoginInfo.AccessCode);
                if (hasPermission)
                {

                    foreach (var id in ids)
                    {
                        var result = service.DeleteLeaveType(id)
;

                    }

                    return Json(new { isSuccess = true, message = "Deleted Successfully" });
                }
                else
                {

                    return Json(new { isSuccess = false, message = "You have no access" });
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error deleting leave type: {ex.Message}");

                return StatusCode(500, new { isSuccess = false, message = ex.Message });
            }
        }
        #endregion

        #region NeaxtCode
        [HttpGet]
        public async Task<IActionResult> GenerateNextCode()
        {
            var nextCode = await service.GenerateNextCode();
            return Json(nextCode);
        }
        #endregion

        #region TabeleLodaing

        public ActionResult Grid(string jobTitleCode)
        {

            var resutl = service.GetByIdJobTitleAsync(jobTitleCode);
            return Json(new { data = resutl });

        }

        //

        [HttpGet]
        public async Task<IActionResult> GetTableData(string jobTitleCode)
        {
            try
            {
                var list = await service.GetByIdJobTitleAsync(jobTitleCode);
                return PartialView("_Grid", list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion
    }
}
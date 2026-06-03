
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.JobTitles;
using GCTL.Service.Common;
using GCTL.Service.JobTitles;
using GCTL.UI.Core.ViewModels.JobTitles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace GCTL.UI.Core.Controllers
{
    public class JobTitlesController : BaseController
    {
        private readonly IJobTitleService service;
        private readonly ICommonService commonService;
        private readonly ICompositeViewEngine viewEngine;
        string strMaxNO = "";
        public JobTitlesController(IJobTitleService service, ICommonService commonService, ICompositeViewEngine viewEngine)
        {
            this.service = service;
            this.commonService = commonService;
            this.viewEngine = viewEngine;
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
            JobTitlePageViewModel model = new JobTitlePageViewModel();
            var list = await service.GetAllAsync();
            model.JobTitleList = list ?? new List<JobTitleSetupViewModel>();

            //Get By Id
            if (!string.IsNullOrEmpty(id))
            {

                model.Setup = await service.GetByIdAsync(id);

            }
            //

            //
            model.PageUrl = Url.Action(nameof(Index));
            return View(model);
        }
        #endregion

        #region Post Update 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Setup(JobTitleSetupViewModel modelVM)
        {
            try
            {

                if (await service.IsExistAsync(modelVM.JobTitle, modelVM.JobTitleId))
                {
                    return Json(new { isSuccess = false, message = $"Already Exists!", isDuplicate = true });
                }


                if (string.IsNullOrEmpty(modelVM.JobTitleId))
                {
                    modelVM.JobTitleId = await service.GenerateNextCode();
                }


                modelVM.ToAudit(LoginInfo, modelVM.AutoId > 0);
                if (modelVM.AutoId == 0)
                {
                    var hasSavePermission = await service.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await service.SaveAsync(modelVM, LoginInfo.CompanyCode);
                        return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = modelVM.JobTitleId });
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
                        return Json(new { isSuccess = true, message = "Updated Successfully", lastCode = modelVM.JobTitleId });
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

                return Json(new { isSuccess = true, message = $"Already Exists" });

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
                if (ids == null || ids.Count == 0)
                {
                    return BadRequest(new { success = false, message = "No IDs provided for delete." });
                }

                var hasPermission = await service.DeletePermissionAsync(LoginInfo.AccessCode);
                if (!hasPermission)
                {
                    return Json(new { success = false, message = "You have no access." });
                }

                DeleteHistoryViewModel model = new DeleteHistoryViewModel();
                model.ToAudit(LoginInfo);
                model.CompanyCode = LoginInfo.CompanyCode;

                var success = await service.DeleteTab(ids, model);
                if (success.succses)
                {
                    return Json(new { success = true, message = "Deleted Successfully." });
                }
                else
                {
                    return Json(new { success = false, message = success.messege });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }

        //[HttpPost]
        //public async Task<IActionResult> Delete([FromBody] List<string> ids)
        //{
        //    try
        //    {

        //        var hasPermission = await service.DeletePermissionAsync(LoginInfo.AccessCode);
        //        if (hasPermission)
        //        {

        //            foreach (var id in ids)
        //            {
        //                var result = service.DeleteLeaveType(id);

        //            }

        //            return Json(new { isSuccess = true, message = "Deleted Successfully" });
        //        }
        //        else
        //        {

        //            return Json(new { isSuccess = false, message = "You have no access" });
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        Console.WriteLine($"Error deleting leave type: {ex.Message}");

        //        return StatusCode(500, new { isSuccess = false, message = ex.Message });
        //    }
        //}
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

        [HttpGet]
        public async Task<IActionResult> GetTableData()
        {
            try
            {
                var list = await service.GetAllAsync();
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

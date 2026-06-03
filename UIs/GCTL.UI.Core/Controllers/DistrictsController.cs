
using GCTL.Service.Common;
using GCTL.Service.Districts;
using GCTL.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using GCTL.Core.ViewModels.Districts;
using GCTL.UI.Core.ViewModels.Districts;

namespace GCTL.UI.Core.Controllers
{
    public class DistrictsController : BaseController
    {
        private readonly IDistrictsService service;
        private readonly ICommonService commonService;
        string strMaxNO = "";
        public DistrictsController(IDistrictsService service, ICommonService commonService)
        {
            this.service = service;
            this.commonService = commonService;        
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
            DistrictsPageViewModel model = new DistrictsPageViewModel();
            var list = await service.GetAllAsync();
            model.DistrictList = list ?? new List<DistrictsSetupViewModel>();

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
        public async Task<IActionResult> Setup(DistrictsSetupViewModel modelVM)
        {
            try
            {

                if (await service.IsExistAsync(modelVM.DistrictId, modelVM.DistrictId))
                {
                    return Json(new { isSuccess = false, message = $"Already <span style='color: blue;'>'{modelVM.DistrictId}'</span> Exists!", isDuplicate = true });
                }


                if (string.IsNullOrEmpty(modelVM.DistrictId))
                {
                    modelVM.DistrictId = await service.GenerateNextCode();
                }


                modelVM.ToAudit(LoginInfo, modelVM.AutoId > 0);
                if (modelVM.AutoId == 0)
                {
                    var hasSavePermission = await service.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await service.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = modelVM.DistrictId });
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
                        return Json(new { isSuccess = true, message = "Updated Successfully", lastCode = modelVM.DistrictId });
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
        public async Task<IActionResult> Delete(List<string> ids)
        {
            try
            {

                var hasPermission = await service.DeletePermissionAsync(LoginInfo.AccessCode);
                if (hasPermission)
                {

                    foreach (var id in ids)
                    {
                        var result = service.DeleteLeaveType(id);

                    }

                    return Json(new { isSuccess = true, message = "Data Deleted Successfully" });
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

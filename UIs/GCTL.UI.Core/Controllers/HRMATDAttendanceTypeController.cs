using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HRMATDAttendanceTypes;
using GCTL.Service.Common;
using GCTL.Service.HRMATDAttendanceTypes;
using GCTL.Service.LeaveTypes;
using GCTL.UI.Core.ViewModels.HRMATDAttendanceTypes;
using Microsoft.AspNetCore.Mvc;
namespace GCTL.UI.Core.Controllers
{
    public class HRMATDAttendanceTypeController : BaseController
    {
        private readonly IHRMATDAttendanceTypeService service;
        private readonly ICommonService commonService;

        public HRMATDAttendanceTypeController(IHRMATDAttendanceTypeService service, ICommonService commonService)
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
            HRMATDAttendanceTypPageViewModel model = new HRMATDAttendanceTypPageViewModel();
            var list = await service.GetAllAsync();
            model.TableDataList = list ?? new List<HRMATDAttendanceTypeSetupViewModel>();
            //Get By Id
            if (!string.IsNullOrEmpty(id))
            {

                model.Setup = await service.GetByIdAsync(id);
            }

            model.PageUrl = Url.Action(nameof(Index));

            return View(model);



        }
        #endregion

        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Setup(HRMATDAttendanceTypeSetupViewModel modelVM)
        {
            try
            {


                if (await service.IsExistAsync(modelVM.AttendanceTypeName, modelVM.AttendanceTypeCode))
                {
                    return Json(new { isSuccess = false, message = $"Already  Exists!", isDuplicate = true });
                }


                if (string.IsNullOrEmpty(modelVM.AttendanceTypeCode))
                {
                    modelVM.AttendanceTypeCode = await service.GenerateNextCode();
                }


                modelVM.ToAudit(LoginInfo, modelVM.AutoId > 0);
                if (modelVM.AutoId == 0)
                {
                    var hasSavePermission = await service.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await service.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = modelVM.AttendanceTypeCode });
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
                        return Json(new { isSuccess = true, message = "Updated Successfully", lastCode = modelVM.AttendanceTypeCode });
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


        #region Delete

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] List<string> ids)
        {
            try
            {

                var hasPermission = await service.DeletePermissionAsync(LoginInfo.AccessCode);
                if (hasPermission)
                {
                    DeleteHistoryViewModel model = new DeleteHistoryViewModel();
                    model.ToAudit(LoginInfo);
                    model.CompanyCode = LoginInfo.CompanyCode;

                    var result = await service.DeleteLeaveType(ids, model);

                    return Json(new { isSuccess = result.success, message = result.message, refError = result.refError });
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

        #region CheckAvailability

        [HttpPost]
        public async Task<JsonResult> CheckAvailability(string name, string typeCode)
        {

            if (await service.IsExistAsync(name, typeCode))
            {
                return Json(new { isSuccess = true, message = $"Already exists!." });
            }

            return Json(new { isSuccess = false });
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

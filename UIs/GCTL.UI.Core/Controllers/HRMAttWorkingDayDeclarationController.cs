using GCTL.Core.ViewModels.HrmAttWorkingDayDeclarations;
using GCTL.Core.ViewModels.HRMCompanyWeekEnds;
using GCTL.Core.Helpers;
using GCTL.Service.Common;
using GCTL.Service.HRMAttWorkingDayDeclarations;
using GCTL.Service.HRMCompanyWeekEnds;
using GCTL.UI.Core.ViewModels.HrmAttWorkingDayDeclarations;
using GCTL.UI.Core.ViewModels.HRMCompanyWeekEnds;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace GCTL.UI.Core.Controllers
{
    public class HRMAttWorkingDayDeclarationController : BaseController
    {
        private readonly IHRMAttWorkingDayDeclarationService service;
        private readonly ICommonService commonService;

        public HRMAttWorkingDayDeclarationController(IHRMAttWorkingDayDeclarationService service, ICommonService commonService)
        {
            this.service = service;
            this.commonService = commonService;
        }

        #region GetByallId
        public async Task<IActionResult> Index(string? id)
        {
            var hasPermission = await service.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }
            //Get all
            HrmAttWorkingDayDeclarationPageViewModel model = new HrmAttWorkingDayDeclarationPageViewModel();
            var list = await service.GetAllAsync();
            model.TableListdata = list ?? new List<HrmAttWorkingDayDeclarationSetupViewModel>();
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


        public async Task<IActionResult> Setup(HrmAttWorkingDayDeclarationSetupViewModel modelVM)
        {
            try
            {

                if (await service.IsExistAsync( modelVM.WorkingDayCode, modelVM.WorkingDayDate.ToString("MM/dd/yyyy")))
                {
                    return Json(new { isSuccess = false, message = $"Already  Exists!", isDuplicate = true });
                }


                if (string.IsNullOrEmpty(modelVM.WorkingDayCode))
                {
                    modelVM.WorkingDayCode = await service.GenerateNextCode();
                }

                modelVM.ToAudit(LoginInfo, modelVM.AutoId > 0);
                if (modelVM.AutoId == 0)
                {
                    var hasSavePermission = await service.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await service.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = modelVM.WorkingDayCode });
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
                        return Json(new { isSuccess = true, message = "Updated Successfully", lastCode = modelVM.WorkingDayCode });
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

                Console.WriteLine($"Error deleting  type: {ex.Message}");

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

        #region CheckAvailability

        [HttpPost]


   
        public async Task<JsonResult> CheckAvailability( string typecode,  string workingDayDate)
        {

            if (await service.IsExistAsync( typecode, workingDayDate))
            {
                return Json(new { isSuccess = true, message = $"Already exists!." });
            }

            return Json(new { isSuccess = false });
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

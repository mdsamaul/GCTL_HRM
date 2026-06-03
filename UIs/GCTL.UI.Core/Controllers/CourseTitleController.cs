using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.CourseTitle;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Service.Common;
using GCTL.Service.CourseTitle;
using GCTL.UI.Core.ViewModels.CourseTitle;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class CourseTitleController : BaseController
    {
        private readonly ICourseTitleService courseTitleService;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;

        public CourseTitleController(
            ICourseTitleService courseTitleService, ICommonService commonService
            )
        {
            this.courseTitleService = courseTitleService;
            this.commonService = commonService;
        }

        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await courseTitleService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new CourseTitlePageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {

                var list = await courseTitleService.GetAllAsync();
                model.CourseTitleList = list ?? new List<CourseTitleSetupViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "CourseCode", "HRM_Def_CourseTitle", 3);
                model.Setup = new CourseTitleSetupViewModel
                {
                    CourseCode = strMaxNO
                };

            }
            catch (Exception ex)
            {

                model.CourseTitleList = new List<CourseTitleSetupViewModel>();
                model.Setup = new CourseTitleSetupViewModel();
                Console.WriteLine("Error" + ex.Message);
            }


            if (child)
                return PartialView(model);

            return View(model);
        }

        public async Task<IActionResult> Setup(string id)
        {
            CourseTitleSetupViewModel model = new CourseTitleSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "CourseCode", "HRM_Def_CourseTitle", 3);

            if (!string.IsNullOrEmpty(id))
            {

                model = await courseTitleService.GetByIdAsync(id);
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.CourseCode = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }


        //
        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(CourseTitleSetupViewModel modelVM)
        {
            try
            {

                if (await courseTitleService.IsExistAsync(modelVM.CourseName, modelVM.CourseCode))
                {
                    return Json(new { isSuccess = false, message = $"Already Exists!", isDuplicate = true });
                }


                if (!ModelState.IsValid)
                {

                    var errorMessage = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                    return Json(new { isSuccess = false, message = errorMessage });
                }

                modelVM.ToAudit(LoginInfo, modelVM.AutoId > 0);
                if (modelVM.AutoId == 0)
                {
                    var hasSavePermission = await courseTitleService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await courseTitleService.SaveAsync(modelVM, LoginInfo.CompanyCode);
                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.CourseCode });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await courseTitleService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await courseTitleService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.CourseCode });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to update.", noUpdatePermission = true });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:{ex.Message}");
                return RedirectToAction("Login", "Accounts");

            }
        }

        #endregion

        //
        #region CheckAvailability
        [HttpPost]
        public async Task<JsonResult> CheckAvailability(string name, string code)
        {
            if (await courseTitleService.IsExistAsync(name, code))
            {

                return Json(new { isSuccess = true, message = $"Already Exists!" });

            }

            return Json(new { isSuccess = false });
        }
        #endregion


        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] List<string> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest(new { success = false, message = "No IDs provided for delete." });
            }

            var hasPermission = await courseTitleService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            DeleteHistoryViewModel model = new DeleteHistoryViewModel();
            model.ToAudit(LoginInfo);
            model.CompanyCode = LoginInfo.CompanyCode;
            var success = await courseTitleService.DeleteTab(ids, model);
            if (success.succses)
            {
                return Json(new { success = true, message = "Deleted Successfully." });
            }
            else
            {
                return Json(new { success = false, message = success.messege });
            }
        }

        #region TabeleLodaing

        [HttpGet]
        public async Task<IActionResult> GetTableData()
        {
            try
            {
                var list = await courseTitleService.GetAllAsync();
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

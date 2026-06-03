using GCTL.Core.ViewModels.HRMDefBoardCountryNames;
using GCTL.Service.Common;
using GCTL.Service.HRMDefBoardCountryNames;
using GCTL.Service.HRMDefExamTitles;
using GCTL.UI.Core.ViewModels.HRMDefBoardCountryNames;
using Microsoft.AspNetCore.Mvc;
using GCTL.Core.Helpers;
using GCTL.UI.Core.ViewModels.HRMDefExamTitles;
using GCTL.Core.ViewModels.HRMDefExamTitles;
namespace GCTL.UI.Core.Controllers
{
    public class HRMDefExamTitlesController : BaseController
    {
        private readonly IHRMDefExamTitlesService hrmDefExamtitleService;
        private readonly ICommonService commonService;

        string strMaxNO = string.Empty;
        public HRMDefExamTitlesController(
                                     ICommonService commonService,
                                     IHRMDefExamTitlesService hrmDefExamtitleService
                                   )
        {
            this.hrmDefExamtitleService = hrmDefExamtitleService;
            this.commonService = commonService;

        }
       

        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await hrmDefExamtitleService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new HRMDefExamTitlesPageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {

                var list = await hrmDefExamtitleService.GetAllAsync();
                model.TableListData = list ?? new List<HRMDefExamTitlesSetupViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "ExamTitleCode", "HRM_Def_ExamTitle", 2);
                model.Setup = new HRMDefExamTitlesSetupViewModel
                {
                    ExamTitleCode = strMaxNO
                };
               
            }
            catch (Exception ex)
            {

                model.TableListData = new List<HRMDefExamTitlesSetupViewModel>();
                model.Setup = new HRMDefExamTitlesSetupViewModel();
                Console.WriteLine("Error" + ex.Message);
            }


            if (child)
                return PartialView(model);

            return View(model);
        }




        public async Task<IActionResult> Setup(string id)
        {
            HRMDefExamTitlesSetupViewModel model = new HRMDefExamTitlesSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "ExamTitleCode", "HRM_Def_ExamTitle", 2);

            if (!string.IsNullOrEmpty(id))
            {

                model = await hrmDefExamtitleService.GetByIdAsync(id);
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.ExamTitleCode = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }


        //
        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(HRMDefExamTitlesSetupViewModel modelVM)
        {
            try
            {

                if (await hrmDefExamtitleService.IsExistAsync(modelVM.ExamTitleName, modelVM.ExamTitleCode))
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
                    var hasSavePermission = await hrmDefExamtitleService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await hrmDefExamtitleService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Data Saved Successfully.", lastCode = modelVM.ExamTitleCode });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await hrmDefExamtitleService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await hrmDefExamtitleService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Data Updated Successfully.", lastCode = modelVM.ExamTitleCode });
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
            if (await hrmDefExamtitleService.IsExistAsync(name, code))
            {

                return Json(new { isSuccess = true, message = $"Already Exists!" });

            }

            return Json(new { isSuccess = false });
        }
        #endregion


        [HttpPost]
        public async Task<IActionResult> Delete(List<string> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest(new { success = false, message = "No IDs provided for delete." });
            }

            var hasPermission = await hrmDefExamtitleService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await hrmDefExamtitleService.DeleteTab(ids);
            if (success)
            {
                return Json(new { success = true, message = "Data Deleted Successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Deletion failed. Some entities may still exists." });
            }
        }

        #region TabeleLodaing

        [HttpGet]
        public async Task<IActionResult> GetTableData()
        {
            try
            {
                var list = await hrmDefExamtitleService.GetAllAsync();
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

using GCTL.Core.ViewModels.HrmDefExamGroupInfos;
using GCTL.Core.ViewModels.HRMDefExamTitles;
using GCTL.Service.Common;
using GCTL.Service.HrmDefExamGroupInfos;
using GCTL.Service.HRMDefExamTitles;
using GCTL.UI.Core.ViewModels.HrmDefExamGroupInfos;
using GCTL.UI.Core.ViewModels.HRMDefExamTitles;
using Microsoft.AspNetCore.Mvc;
using GCTL.Core.Helpers;

namespace GCTL.UI.Core.Controllers
{
    public class HrmDefExamGroupInfosController : BaseController
    {



        private readonly IHrmDefExamGroupInfosService hrmDefExamGroupService;
        private readonly ICommonService commonService;

        string strMaxNO = string.Empty;
        public HrmDefExamGroupInfosController(
                                     ICommonService commonService,
                                     IHrmDefExamGroupInfosService hrmDefExamGroupService
                                   )
        {
            this.hrmDefExamGroupService = hrmDefExamGroupService;
            this.commonService = commonService;

        }


        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await hrmDefExamGroupService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new HrmDefExamGroupInfosPageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {

                var list = await hrmDefExamGroupService.GetAllAsync();
                model.TableList = list ?? new List<HrmDefExamGroupInfosSetupViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "GroupCode", "HRM_Def_ExamGroupInfo", 2);
                model.Setup = new HrmDefExamGroupInfosSetupViewModel
                {
                    GroupCode = strMaxNO
                };

            }
            catch (Exception ex)
            {

                model.TableList = new List<HrmDefExamGroupInfosSetupViewModel>();
                model.Setup = new HrmDefExamGroupInfosSetupViewModel();
                Console.WriteLine("Error" + ex.Message);
            }


            if (child)
                return PartialView(model);

            return View(model);
        }




        public async Task<IActionResult> Setup(string id)
        {
            HrmDefExamGroupInfosSetupViewModel model = new HrmDefExamGroupInfosSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "GroupCode", "HRM_Def_ExamGroupInfo", 2);

            if (!string.IsNullOrEmpty(id))
            {

                model = await hrmDefExamGroupService.GetByIdAsync(id);
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.GroupCode = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }


        
        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(HrmDefExamGroupInfosSetupViewModel modelVM)
        {
            try
            {

                if (await hrmDefExamGroupService.IsExistAsync(modelVM.GroupName, modelVM.GroupCode))
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
                    var hasSavePermission = await hrmDefExamGroupService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await hrmDefExamGroupService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Data Saved Successfully.", lastCode = modelVM.GroupCode });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await hrmDefExamGroupService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await hrmDefExamGroupService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Data Updated Successfully.", lastCode = modelVM.GroupCode });
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

        
        #region CheckAvailability
        [HttpPost]
        public async Task<JsonResult> CheckAvailability(string name, string code)
        {
            if (await hrmDefExamGroupService.IsExistAsync(name, code))
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

            var hasPermission = await hrmDefExamGroupService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await hrmDefExamGroupService.DeleteTab(ids);
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
                var list = await hrmDefExamGroupService.GetAllAsync();
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


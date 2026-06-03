using GCTL.Core.ViewModels.HrmDefExamGroupInfos;
using GCTL.Core.ViewModels.HrmDefGradeTypes;
using GCTL.Service.Common;
using GCTL.Service.HrmDefExamGroupInfos;
using GCTL.Service.HrmDefGradeTypes;
using GCTL.UI.Core.ViewModels.HrmDefExamGroupInfos;
using GCTL.UI.Core.ViewModels.HrmDefGradeTypes;
using Microsoft.AspNetCore.Mvc;
using GCTL.Core.Helpers;

namespace GCTL.UI.Core.Controllers
{
    public class HrmDefGradeTypesController : BaseController
    {
       

        private readonly IHrmDefGradeTypesService hrmDefGradeService;
        private readonly ICommonService commonService;

        string strMaxNO = string.Empty;
        public HrmDefGradeTypesController(
                                     ICommonService commonService,
                                     IHrmDefGradeTypesService hrmDefGradeService
                                   )
        {
            this.hrmDefGradeService = hrmDefGradeService;
            this.commonService = commonService;

        }


        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await hrmDefGradeService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new HrmDefGradeTypesPageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {

                var list = await hrmDefGradeService.GetAllAsync();
                model.TableListData = list ?? new List<HrmDefGradeTypesSetupViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "GradeTypeID", "HRM_Def_GradeType", 2);
                model.Setup = new HrmDefGradeTypesSetupViewModel
                {
                    GradeTypeId = strMaxNO
                };

            }
            catch (Exception ex)
            {

                model.TableListData = new List<HrmDefGradeTypesSetupViewModel>();
                model.Setup = new HrmDefGradeTypesSetupViewModel();
                Console.WriteLine("Error" + ex.Message);
            }


            if (child)
                return PartialView(model);

            return View(model);
        }




        public async Task<IActionResult> Setup(string id)
        {
            HrmDefGradeTypesSetupViewModel model = new HrmDefGradeTypesSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "GradeTypeID", "HRM_Def_GradeType", 2);

            if (!string.IsNullOrEmpty(id))
            {

                model = await hrmDefGradeService.GetByIdAsync(id);
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.GradeTypeId = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }


        //
        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(HrmDefGradeTypesSetupViewModel modelVM)
        {
            try
            {

                if (await hrmDefGradeService.IsExistAsync(modelVM.GradeType, modelVM.GradeTypeId))
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
                    var hasSavePermission = await hrmDefGradeService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await hrmDefGradeService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Data Saved Successfully.", lastCode = modelVM.GradeTypeId });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await hrmDefGradeService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await hrmDefGradeService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Data Updated Successfully.", lastCode = modelVM.GradeTypeId });
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
            if (await hrmDefGradeService.IsExistAsync(name, code))
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

            var hasPermission = await hrmDefGradeService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await hrmDefGradeService.DeleteTab(ids);
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
                var list = await hrmDefGradeService.GetAllAsync();
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

using GCTL.Core.ViewModels.Grades;
using GCTL.Service.Common;
using GCTL.Service.Grades;
using GCTL.UI.Core.ViewModels.Grades;
using Microsoft.AspNetCore.Mvc;
using GCTL.Core.Helpers;
using GCTL.Service.HrmDefGradeTypes;
using GCTL.Service.BranchesTypeInfo;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GCTL.UI.Core.Controllers
{
    public class GradesController : BaseController
    {
        private readonly IGradesService gradesService;
        private readonly ICommonService commonService;
        private readonly IHrmDefGradeTypesService hrmDefGradeTypesService;

        string strMaxNO = string.Empty;
        public GradesController(IGradesService gradesService, IHrmDefGradeTypesService hrmDefGradeTypesService, ICommonService commonService)
        {
            this.gradesService = gradesService;
            this.commonService = commonService;
            this.hrmDefGradeTypesService = hrmDefGradeTypesService;
        }

        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await gradesService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new GradesPageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {

                var list = await gradesService.GetAllAsync();
                model.TableList = list ?? new List<GradesSetupViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "GradeCode", "HRM_Def_Grade", 2);
                model.Setup = new GradesSetupViewModel
                {
                    GradeCode = strMaxNO
                };

            }
            catch (Exception ex)
            {

                model.TableList = new List<GradesSetupViewModel>();
                model.Setup = new GradesSetupViewModel();
                Console.WriteLine("Error" + ex.Message);
            }


            if (child)
            return PartialView(model);
            ViewBag.GradeTypeDD = new SelectList(await hrmDefGradeTypesService.SelectionHrmDefGradeTypeAsync(), "Code", "Name");

            return View(model);
        }




        public async Task<IActionResult> Setup(string id)
        {
            GradesSetupViewModel model = new GradesSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "GradeCode", "HRM_Def_Grade", 2);

            if (!string.IsNullOrEmpty(id))
            {

                model = await gradesService.GetByIdAsync(id);
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.GradeCode = strMaxNO;
            }
            ViewBag.GradeTypeDD = new SelectList(await hrmDefGradeTypesService.SelectionHrmDefGradeTypeAsync(), "Code", "Name");
            return PartialView($"_{nameof(Setup)}", model);
        }


    
        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(GradesSetupViewModel modelVM)
        {
            try
            {

                if (await gradesService.IsExistAsync(modelVM.GradeName, modelVM.GradeCode))
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
                    var hasSavePermission = await gradesService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await gradesService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Data Saved Successfully.", lastCode = modelVM.GradeCode });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await gradesService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await gradesService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Data Updated Successfully.", lastCode = modelVM.GradeCode });
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
            if (await gradesService.IsExistAsync(name, code))
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

            var hasPermission = await gradesService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await gradesService.DeleteTab(ids);
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
                var list = await gradesService.GetAllAsync();
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

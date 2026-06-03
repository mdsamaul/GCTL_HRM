using GCTL.Core.ViewModels.HrmDefDegrees;
using GCTL.Core.ViewModels.HrmDefInstitutes;
using GCTL.Service.Common;
using GCTL.Service.HrmDefDegrees;
using GCTL.Service.HrmDefInstitutes;
using GCTL.UI.Core.ViewModels.HrmDefDegrees;
using GCTL.UI.Core.ViewModels.HrmDefInstitutes;
using Microsoft.AspNetCore.Mvc;
using GCTL.Core.Helpers;

namespace GCTL.UI.Core.Controllers
{
    public class HrmDefInstitutesController : BaseController
    {
      

        private readonly IHrmDefInstitutesService hrmDefInstituteService;
        private readonly ICommonService commonService;

        string strMaxNO =string.Empty;
        public HrmDefInstitutesController(
                                     ICommonService commonService,
                                     IHrmDefInstitutesService hrmDefInstituteService
                                   )
        {
            this.hrmDefInstituteService = hrmDefInstituteService;
            this.commonService = commonService;

        }





        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await hrmDefInstituteService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new HrmDefInstitutesPageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {

                var list = await hrmDefInstituteService.GetAllAsync();
                model.TableListData = list ?? new List<HrmDefInstitutesSetupViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "InstituteCode", "HRM_Def_Institute", 3);
                model.Setup = new HrmDefInstitutesSetupViewModel
                {
                    InstituteCode = strMaxNO
                };
                
            }
            catch (Exception ex)
            {

                model.TableListData = new List<HrmDefInstitutesSetupViewModel>();
                model.Setup = new HrmDefInstitutesSetupViewModel();
                Console.WriteLine("Error" + ex.Message);
            }


            if (child)
                return PartialView(model);

            return View(model);
        }




        public async Task<IActionResult> Setup(string id)
        {
            HrmDefInstitutesSetupViewModel model = new HrmDefInstitutesSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "InstituteCode", "HRM_Def_Institute", 3);

            if (!string.IsNullOrEmpty(id))
            {

                model = await hrmDefInstituteService.GetByIdAsync(id);
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.InstituteCode = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }


        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(HrmDefInstitutesSetupViewModel modelVM)
        {
            try
            {

                if (await hrmDefInstituteService.IsExistAsync(modelVM.InstituteName, modelVM.InstituteCode))
                {
                    return Json(new { isSuccess = false, message = $"Already Exists!", isDuplicate = true });
                }


                if (!ModelState.IsValid)
                {
                    // Return validation errors in the response if the model state is invalid
                    var errorMessage = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                    return Json(new { isSuccess = false, message = errorMessage });
                }

                modelVM.ToAudit(LoginInfo, modelVM.AutoId > 0);
                if (modelVM.AutoId == 0)
                {
                    var hasSavePermission = await hrmDefInstituteService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await hrmDefInstituteService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Data Saved Successfully.", lastCode = modelVM.InstituteCode });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await hrmDefInstituteService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await hrmDefInstituteService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Data Updated Successfully.", lastCode = modelVM.InstituteCode });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to update.", noUpdatePermission = true });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error{ex.Message}");
              //  return StatusCode(500, ex.Message);
                return RedirectToAction("Login", "Accounts");
            }
        }

        #endregion

        
        #region CheckAvailability
        [HttpPost]
        public async Task<JsonResult> CheckAvailability(string name, string code)
        {
            if (await hrmDefInstituteService.IsExistAsync(name, code))
            {

                return Json(new { isSuccess = true, message = $"Already Exists!" });

            }

            return Json(new { isSuccess = false });
        }
        #endregion


        #region Delete
        [HttpPost]
        public async Task<IActionResult> Delete(List<string> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest(new { success = false, message = "No IDs provided for delete." });
            }

            try
            {
                // Check delete permission
                var hasPermission = await hrmDefInstituteService.DeletePermissionAsync(LoginInfo.AccessCode);
                if (!hasPermission)
                {
                    return Json(new { success = false, message = "You have no access." });
                }

                // Perform the delete operation
                bool success = await hrmDefInstituteService.DeleteTab(ids);
                if (success)
                {
                    return Json(new { success = true, message = "Data Deleted Successfully." });
                }
                else
                {
                    return Json(new { success = false, message = "Deletion failed. Some entities may still exist." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during delete: {ex.Message}");
                return RedirectToAction("Login", "Accounts");
            }
        }
        #endregion


        #region TabeleLodaing

        [HttpGet]
        public async Task<IActionResult> GetTableData()
        {
            try
            {
                var list = await hrmDefInstituteService.GetAllAsync();
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

using GCTL.Core.ViewModels.HrmDefInstitutes;
using GCTL.Core.ViewModels.HrmDefOccupations;
using GCTL.Service.Common;
using GCTL.Service.HrmDefInstitutes;
using GCTL.Service.HrmDefOccupations;
using GCTL.UI.Core.ViewModels.HrmDefInstitutes;
using GCTL.UI.Core.ViewModels.HrmDefOccupations;
using Microsoft.AspNetCore.Mvc;
using GCTL.Core.Helpers;

namespace GCTL.UI.Core.Controllers
{
    public class HrmDefOccupationsController : BaseController
    {

        private readonly IHrmDefOccupationsService  hrmDefOccupationsService;
        private readonly ICommonService commonService;

        string strMaxNO = string.Empty;
        public HrmDefOccupationsController(
                                     ICommonService commonService,
                                     IHrmDefOccupationsService hrmDefOccupationsService
                                   )
        {
            this.hrmDefOccupationsService = hrmDefOccupationsService;
            this.commonService = commonService;

        }


        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await hrmDefOccupationsService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new HrmDefOccupationsPageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {
                var list = await hrmDefOccupationsService.GetAllAsync();
                model.TableListData = list ?? new List<HrmDefOccupationsSetupViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "OccupationCode", "HRM_Def_Occupation", 2);
                model.Setup = new HrmDefOccupationsSetupViewModel
                {
                    OccupationCode = strMaxNO
                };

            }
            catch (Exception ex)
            {
                model.TableListData = new List<HrmDefOccupationsSetupViewModel>();
                model.Setup = new HrmDefOccupationsSetupViewModel();
                Console.WriteLine("Error" + ex.Message);
            }


            if (child)
                return PartialView(model);

            return View(model);
        }



        public async Task<IActionResult> Setup(string id)
        {
            HrmDefOccupationsSetupViewModel model = new HrmDefOccupationsSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "OccupationCode", "HRM_Def_Occupation", 2);

            if (!string.IsNullOrEmpty(id))
            {
                model = await hrmDefOccupationsService.GetByIdAsync(id);
                if (model == null)
                {
                    return NotFound();
                }
            }
            else
            {
                model.OccupationCode = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        
        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(HrmDefOccupationsSetupViewModel modelVM)
        {
            try
            {

                if (await hrmDefOccupationsService.IsExistAsync(modelVM.Occupation, modelVM.OccupationCode))
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
                    var hasSavePermission = await hrmDefOccupationsService.SavePaermissonAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await hrmDefOccupationsService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Data Saved Successfully.", lastCode = modelVM.OccupationCode });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await hrmDefOccupationsService.UpdatePermisson(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await hrmDefOccupationsService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Data Updated Successfully.", lastCode = modelVM.OccupationCode });
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
                 return RedirectToAction("Login", "Accounts");
            }
        }

        #endregion

        
        #region CheckAvailability
        [HttpPost]
        public async Task<JsonResult> CheckAvailability(string name, string code)
        {
            if (await hrmDefOccupationsService.IsExistAsync(name, code))
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
                var hasPermission = await hrmDefOccupationsService.DeletePermissonAsync(LoginInfo.AccessCode);
                if (!hasPermission)
                {
                    return Json(new { success = false, message = "You have no access." });
                }

                // Perform the delete operation
                bool success = await hrmDefOccupationsService.DeleteAsyncTab(ids);
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
                var list = await hrmDefOccupationsService.GetAllAsync();
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

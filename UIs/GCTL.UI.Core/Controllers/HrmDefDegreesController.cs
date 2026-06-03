using AutoMapper;
//using GCTL.Core.ViewModels.CoreBankAccountInformations;
using GCTL.Core.ViewModels.Departments;
using GCTL.Core.ViewModels.HolidayTypes;
using GCTL.Core.ViewModels.HrmDefDegrees;
using GCTL.Service.Common;
using GCTL.Service.Departments;
using GCTL.Service.HrmDefDegrees;
using GCTL.UI.Core.ViewModels.Departments;
using GCTL.UI.Core.ViewModels.HrmDefDegrees;
using Microsoft.AspNetCore.Mvc;
using GCTL.Core.Helpers;
namespace GCTL.UI.Core.Controllers
{
    public class HrmDefDegreesController : BaseController
    {
        private readonly IHrmDefDegreesService hrmDefDegreesService;
        private readonly ICommonService commonService;
      
        string strMaxNO = "";
        public HrmDefDegreesController(
                                     ICommonService commonService,
                                     IHrmDefDegreesService hrmDefDegreesService
                                   )
        {
            this.hrmDefDegreesService = hrmDefDegreesService;
            this.commonService = commonService;
           
        }
       




        public async Task<IActionResult> Index( bool child = false)
        {
            var hasPermission = await hrmDefDegreesService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new HrmDefDegreesPageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {

                var list = await hrmDefDegreesService.GetAllAsync();
                model.TableList = list ?? new List<HrmDefDegreesSetupViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "DegreeCode", "HRM_Def_Degree", 3);
                model.Setup = new HrmDefDegreesSetupViewModel
                {
                    DegreeCode = strMaxNO
                };
                //if (!string.IsNullOrEmpty(id))
                //{

                //    model.Setup = await hrmDefDegreesService.GetByIdAsync(id);
                //}
            }
            catch (Exception ex)
            {

                model.TableList = new List<HrmDefDegreesSetupViewModel>();
                model.Setup = new HrmDefDegreesSetupViewModel();
                Console.WriteLine("Error" + ex.Message);
            }


            if (child)
                return PartialView(model);

            return View(model);
        }




        public async Task<IActionResult> Setup(string id)
        {
            HrmDefDegreesSetupViewModel model =new HrmDefDegreesSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "DegreeCode", "HRM_Def_Degree", 3);

            if (!string.IsNullOrEmpty(id))
            {
               
                model = await hrmDefDegreesService.GetByIdAsync(id);
                if (model == null)
                {
                  
                    return NotFound();
                }
            }
            else
            {
               
                model.DegreeCode = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }


        //
        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(HrmDefDegreesSetupViewModel modelVM)
        {
            try
            {

                if (await hrmDefDegreesService.IsExistAsync(modelVM.DegreeName, modelVM.DegreeCode))
                {
                    return Json(new { isSuccess = false, message = $"Already Exists!", isDuplicate = true });
                }


                if (!ModelState.IsValid)
                {
                    // Return validation errors in the response if the model state is invalid
                    var errorMessage = ModelState.Values .SelectMany(v => v.Errors) .FirstOrDefault()?.ErrorMessage;
                    return Json(new { isSuccess = false, message = errorMessage });
                }
               
                modelVM.ToAudit(LoginInfo, modelVM.AutoId > 0);
                if (modelVM.AutoId == 0)
                {
                    var hasSavePermission = await hrmDefDegreesService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await hrmDefDegreesService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Data Saved Successfully.", lastCode = modelVM.DegreeCode });
                   
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await hrmDefDegreesService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await hrmDefDegreesService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Data Updated Successfully.", lastCode = modelVM.DegreeCode });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to update.", noUpdatePermission = true });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        #endregion

        //
        #region CheckAvailability
        [HttpPost]
        public async Task<JsonResult> CheckAvailability(string name, string code)
        {
            if (await hrmDefDegreesService.IsExistAsync(name, code))
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

            var hasPermission = await hrmDefDegreesService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success =await hrmDefDegreesService.DeleteTab(ids);
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
                var list = await hrmDefDegreesService.GetAllAsync();
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

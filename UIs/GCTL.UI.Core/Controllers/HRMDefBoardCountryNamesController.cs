using GCTL.Core.ViewModels.HRMDefBoardCountryNames;
using GCTL.Core.ViewModels.HrmDefDegrees;
using GCTL.Service.Common;
using GCTL.Service.HRMDefBoardCountryNames;
using GCTL.Service.HrmDefDegrees;
using GCTL.UI.Core.ViewModels.HRMDefBoardCountryNames;
using GCTL.UI.Core.ViewModels.HrmDefDegrees;
using Microsoft.AspNetCore.Mvc;
using GCTL.Core.Helpers;
namespace GCTL.UI.Core.Controllers
{
    public class HRMDefBoardCountryNamesController : BaseController
    {
        private readonly IHRMDefBoardCountryNamesService hrmDefBoardCountryNameService;
        private readonly ICommonService commonService;

        string strMaxNO = "";
        public HRMDefBoardCountryNamesController(
                                     ICommonService commonService,
                                     IHRMDefBoardCountryNamesService hrmDefBoardCountryNameService
                                   )
        {
            this.hrmDefBoardCountryNameService = hrmDefBoardCountryNameService;
            this.commonService = commonService;

        }
        //public IActionResult Index(bool child)
        //{

        //    HrmDefDegreesPageViewModel model = new HrmDefDegreesPageViewModel
        //    {
        //        Setup = new HrmDefDegreesSetupViewModel(),
        //        TableList = new List<HrmDefDegreesSetupViewModel>()
        //    };


        //    return View(model);
        //}




        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await hrmDefBoardCountryNameService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new HRMDefBoardCountryNamesPageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {

                var list = await hrmDefBoardCountryNameService.GetAllAsync();
                model.TableListData = list ?? new List<HRMDefBoardCountryNamesSetupViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "BoardCode", "HRM_Def_BoardCountryName", 3);
                model.Setup = new HRMDefBoardCountryNamesSetupViewModel
                {
                    BoardCode = strMaxNO
                };
                //if (!string.IsNullOrEmpty(id))
                //{

                //    model.Setup = await hrmDefBoardCountryNameService.GetByIdAsync(id);
                //}
            }
            catch (Exception ex)
            {

                model.TableListData = new List<HRMDefBoardCountryNamesSetupViewModel>();
                model.Setup = new HRMDefBoardCountryNamesSetupViewModel();
                Console.WriteLine("Error" + ex.Message);
            }


            if (child)
                return PartialView(model);

            return View(model);
        }




        public async Task<IActionResult> Setup(string id)
        {
            HRMDefBoardCountryNamesSetupViewModel model = new HRMDefBoardCountryNamesSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "BoardCode", "HRM_Def_BoardCountryName", 3);

            if (!string.IsNullOrEmpty(id))
            {

                model = await hrmDefBoardCountryNameService.GetByIdAsync(id);
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.BoardCode = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }


        //
        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(HRMDefBoardCountryNamesSetupViewModel modelVM)
        {
            try
            {

                if (await hrmDefBoardCountryNameService.IsExistAsync(modelVM.BoardName, modelVM.BoardCode))
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
                    var hasSavePermission = await hrmDefBoardCountryNameService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await hrmDefBoardCountryNameService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Data Saved Successfully.", lastCode = modelVM.BoardCode });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await hrmDefBoardCountryNameService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await hrmDefBoardCountryNameService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Data Updated Successfully.", lastCode = modelVM.BoardCode });
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
            if (await hrmDefBoardCountryNameService.IsExistAsync(name, code))
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

            var hasPermission = await hrmDefBoardCountryNameService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await hrmDefBoardCountryNameService.DeleteTab(ids);
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
                var list = await hrmDefBoardCountryNameService.GetAllAsync();
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

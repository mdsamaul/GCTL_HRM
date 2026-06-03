using GCTL.Core.ViewModels.CompanyInfos;
using GCTL.Service.Common;
using GCTL.Service.CompanyInfos;
using GCTL.Core.Helpers;
using GCTL.UI.Core.ViewModels.CompanyInfos;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class CompanyInfosController : BaseController
    {
        private readonly ICompanyInfosService companyInfosService;
        private readonly ICommonService commonService;

        string strMaxNO = string.Empty;
        public CompanyInfosController(ICompanyInfosService companyInfosService, ICommonService commonService)
        {
            this.companyInfosService = companyInfosService;
            this.commonService = commonService;
        }

        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await companyInfosService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new CompanyInfosPageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {
                var list = await companyInfosService.GetAllAsync();
                model.TableListData = list ?? new List<CompanyInfosSetupViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "CompanyNameID", "HRM_Def_CompanyInfo", 2);
                model.Setup = new CompanyInfosSetupViewModel
                {
                    CompanyNameId = strMaxNO
                };

            }
            catch (Exception ex)
            {
                model.TableListData = new List<CompanyInfosSetupViewModel>();
                model.Setup = new CompanyInfosSetupViewModel();
                Console.WriteLine("Error" + ex.Message);
            }

            if (child)
                return PartialView(model);

            return View(model);
        }




        public async Task<IActionResult> Setup(string id)
        {
            CompanyInfosSetupViewModel model = new CompanyInfosSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "CompanyNameID", "HRM_Def_CompanyInfo", 2);

            if (!string.IsNullOrEmpty(id))
            {
                model = await companyInfosService.GetByIdAsync(id);
                if (model == null)
                {
                    return NotFound();
                }
            }
            else
            {
                model.CompanyNameId = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }


        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(CompanyInfosSetupViewModel modelVM)
        {
            try
            {

                if (await companyInfosService.IsExistAsync(modelVM.CompanyName, modelVM.CompanyNameId))
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
                    var hasSavePermission = await companyInfosService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await companyInfosService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Data Saved Successfully.", lastCode = modelVM.CompanyNameId});

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await companyInfosService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await companyInfosService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Data Updated Successfully.", lastCode = modelVM.CompanyNameId });
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
            if (await companyInfosService.IsExistAsync(name, code))
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

            var hasPermission = await companyInfosService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await companyInfosService.DeleteTab(ids);
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
                var list = await companyInfosService.GetAllAsync();
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

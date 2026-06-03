using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.HrmAtdShifts;
using GCTL.Core.ViewModels.HRMCompanyWeekEnds;
using GCTL.Service.Common;
using GCTL.Service.HrmAtdShifts;
using GCTL.Service.HRMCompanyWeekEnds;
using GCTL.UI.Core.ViewModels.HrmAtdShifts;
using GCTL.UI.Core.ViewModels.HRMCompanyWeekEnds;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace GCTL.UI.Core.Controllers
{
    public class HRMCompanyWeekEndsController : BaseController
    {

        private readonly IHRMCompanyWeekEndService hrmAtdComapanyWeekendService;
        private readonly ICommonService commonService;
        private readonly ICompositeViewEngine viewEngine;
        public HRMCompanyWeekEndsController(IHRMCompanyWeekEndService hrmAtdComapanyWeekendService, ICommonService commonService, ICompositeViewEngine viewEngine)
        {
            this.hrmAtdComapanyWeekendService = hrmAtdComapanyWeekendService;
            this.commonService = commonService;
            this.viewEngine = viewEngine;

        }
        #region GetByallId
        public async Task<IActionResult> Index(string? id)
        {
            var hasPermission = await hrmAtdComapanyWeekendService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }
            //Get all
            HRMCompanyWeekEndPageViewModel model = new HRMCompanyWeekEndPageViewModel();
            var list = await hrmAtdComapanyWeekendService.GetAllAsync();
            model.HRMCompanyWeekEndList = list ?? new List<HRMCompanyWeekEndSetupViewModel>();
            //Get By Id
            if (!string.IsNullOrEmpty(id))
            {

                model.Setup = await hrmAtdComapanyWeekendService.GetByIdAsync(id);

                //Get all but Ldate and ModifyDate


            }
            
            //

            //

            model.PageUrl = Url.Action(nameof(Index));
            return View(model);
        }
        #endregion


        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]


        public async Task<IActionResult> Setup(HRMCompanyWeekEndSetupViewModel modelVM)
        {
            try
            {

                //var sortedWeekends = string.Join(",", modelVM.Weekend.OrderBy(w => w));


                //if (await hrmAtdComapanyWeekendService.IsExistAsync(sortedWeekends, modelVM.CompanyWeekEndCode))
                //{
                //    return Json(new { isSuccess = false, message = $"The combination <span style='color: blue;'>'{sortedWeekends}'</span> already exists!", isDuplicate = true });
                //}


                var sortedWeekends = string.Join(",", modelVM.Weekend.OrderBy(w => w));
                var weekendCount = modelVM.Weekend.Count;

                if (weekendCount == 1)
                {
                    
                    var singleWeekend = modelVM.Weekend.First();
                    if (await hrmAtdComapanyWeekendService.IsExistAsync(singleWeekend, modelVM.CompanyWeekEndCode))
                    {
                      
                        return Json(new { isSuccess = false, message = $" Already exists!", isDuplicate = true });
                    }
                }
                else if (weekendCount > 1)
                {
                   
                    if (await hrmAtdComapanyWeekendService.IsExistAsync(sortedWeekends, modelVM.CompanyWeekEndCode))
                    {
                       
                        return Json(new { isSuccess = false, message = $"The combination of <span style='color: blue;'>'{sortedWeekends}'</span> already exists!", isDuplicate = true });
                    }
                }

             

                if (string.IsNullOrEmpty(modelVM.CompanyWeekEndCode))
                {
                    modelVM.CompanyWeekEndCode = await hrmAtdComapanyWeekendService.GenerateNextCode();
                }

                modelVM.ToAudit(LoginInfo, modelVM.AutoId > 0);
                if (modelVM.AutoId == 0)
                {
                    var hasSavePermission = await hrmAtdComapanyWeekendService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await hrmAtdComapanyWeekendService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = modelVM.CompanyWeekEndCode });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await hrmAtdComapanyWeekendService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await hrmAtdComapanyWeekendService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully", lastCode = modelVM.CompanyWeekEndCode });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to update", noUpdatePermission = true });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }




        #endregion



        #region Delete

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] List<string> ids)
        {
            try
            {

                var hasPermission = await hrmAtdComapanyWeekendService.DeletePermissionAsync(LoginInfo.AccessCode);
                if (hasPermission)
                {

                    foreach (var id in ids)
                    {
                        var result = hrmAtdComapanyWeekendService.DeleteLeaveType(id);

                    }

                    return Json(new { isSuccess = true, message = "Data Deleted Successfully" });
                }
                else
                {

                    return Json(new { isSuccess = false, message = "You have no access" });
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error deleting  type: {ex.Message}");

                return StatusCode(500, new { isSuccess = false, message = ex.Message });
            }
        }


        #endregion

        #region NeaxtCode
        [HttpGet]
        public async Task<IActionResult> GenerateNextCode()
        {
            var nextCode = await hrmAtdComapanyWeekendService.GenerateNextCode();
            return Json(nextCode);
        }
        #endregion

        #region CheckAvailability

        [HttpPost]


        public async Task<JsonResult> CheckAvailability(List<string> weekends, string code)
        {
          
            var sortedWeekends = string.Join(",", weekends.OrderBy(w => w));

           
            bool exists = await hrmAtdComapanyWeekendService.IsExistAsync(sortedWeekends, code);

            if (exists)
            {
                return Json(new { isSuccess = true, message = $"Already exists!" });
                // return Json(new { isSuccess = true, message = $"This data <span style='color: blue;'>'{sortedWeekends}'</span> already exists!" });
            }

            return Json(new { isSuccess = false });
        }







        #endregion

        #region TabeleLodaing

        [HttpGet]
        public async Task<IActionResult> GetTableData()
        {
            try
            {
                var list = await hrmAtdComapanyWeekendService.GetAllAsync();
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





using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.AccFinancialYears;
using GCTL.Service.AccFinancialYears;
using GCTL.Service.Common;
using GCTL.UI.Core.ViewModels.AccFinancialYears;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class AccFinancialYearController : BaseController
    {
        private readonly IAccFinancialYearService yearService;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;

        public AccFinancialYearController
            (
            IAccFinancialYearService yearService,
            ICommonService commonService
            )
        {
            this.yearService = yearService;
            this.commonService = commonService;
        }

        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await yearService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
                return RedirectToAction("Login", "Account");

            var model = new AccFinancialYearPageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {
                var list = await yearService.GetAllAsync();
                model.FinancialYearList = list ?? new List<AccFinancialYearSetupViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "FinancialCodeNo", "Acc_FinancialYear", 3);
                model.Setup = new AccFinancialYearSetupViewModel
                {
                    FinancialCodeNo = strMaxNO
                };
            }
            catch (Exception ex)
            {
                model.FinancialYearList = new List<AccFinancialYearSetupViewModel>();
                model.Setup = new AccFinancialYearSetupViewModel();
                Console.WriteLine("Error" + ex.Message);
            }

            if (child)
                return PartialView(model);

            return View(model);
        }

        public async Task<IActionResult> Setup(string id)
        {
            AccFinancialYearSetupViewModel model = new AccFinancialYearSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "FinancialCodeNo", "Acc_FinancialYear", 3);

            if (id != null)
            {
                model = await yearService.GetByIdAsync(id);
                if (model == null)
                    return NotFound();
            }
            else
                model.FinancialCodeNo = strMaxNO;

            return PartialView($"_{nameof(Setup)}", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Setup(AccFinancialYearSetupViewModel modelVM)
        {
            try
            {
                if (await yearService.IsExistAcync(modelVM.StartDate, modelVM.EndDate, modelVM.FinancialCodeNo))
                {
                    return Json(new { isSuccess = false, message = $"Financial Year already exists!", isDuplicate = true });
                }

                if (!ModelState.IsValid)
                {
                    var errorMessage = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                    return Json(new { isSuccess = false, message = errorMessage });
                }

                modelVM.ToAudit(LoginInfo, modelVM.Tc > 0);
                if (modelVM.Tc == 0)
                {
                    var hasSavePermission = await yearService.SavePermissionAsync(LoginInfo.AccessCode);

                    if (hasSavePermission)
                    {
                        await yearService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Data Saved Successfully.", lastCode = modelVM.FinancialCodeNo });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {
                    var hasUpdatePermission = await yearService.UpdatePermissionAsync(LoginInfo.AccessCode);

                    if (hasUpdatePermission)
                    {
                        await yearService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Data Saved Successfully.", lastCode = modelVM.FinancialCodeNo });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:{ex.Message}");
                return RedirectToAction("Login", "Accounts");
            }
        }

        [HttpPost]
        public async Task<JsonResult> CheckAvailability(DateTime? start, DateTime? end, string code)
        {
            if (await yearService.IsExistAcync(start, end, code))
            {

                return Json(new { isSuccess = true, message = $"Already Exists!" });
            }

            return Json(new { isSuccess = false });
        }

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] List<string> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest(new { success = false, message = "No IDs provided for delete." });
            }

            var hasPermission = await yearService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await yearService.DeleteTab(ids);
            if (success)
            {
                return Json(new { success = true, message = "Data Deleted Successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Deletion failed. Some entities may still exists." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTableData()
        {
            try
            {
                var list = await yearService.GetAllAsync();
                return PartialView("_Grid", list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}

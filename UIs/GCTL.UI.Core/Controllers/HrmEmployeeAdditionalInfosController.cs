using GCTL.Core.Data;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HrmEmployeeAdditionalInfos;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.HrmEmployeeAdditionalInfos;
using GCTL.Service.HrmEmployees2;
using GCTL.UI.Core.ViewModels.HrmEmployeeAdditionalInfos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GCTL.UI.Core.Controllers
{
    public class HrmEmployeeAdditionalInfosController : BaseController
    {
        #region Declaration
        private readonly IHrmEmployeeAdditionalInfosService hrmEmployeeAdditionalInfosService;
        private readonly ICommonService commonService;
        private readonly IRepository<CoreBranch> coreBranchRepository;
        private readonly IRepository<CoreCompany> coreCompanyRepository;
        private readonly IHrmEmployee2Service hrmEmployee;
        private readonly IRepository<SalesDefBankInfo> salesBankRepository;
        private readonly IRepository<SalesDefBankBranchInfo> salesBankBranchRepository;

        public HrmEmployeeAdditionalInfosController(
            IHrmEmployeeAdditionalInfosService hrmEmployeeAdditionalInfosService,
            IRepository<SalesDefBankBranchInfo> salesBankBranchRepository,
            IRepository<SalesDefBankInfo> salesBankRepository, 
            IHrmEmployee2Service hrmEmployee,
            ICommonService commonService, 
            IRepository<CoreBranch> coreBranchRepository,
            IRepository<CoreCompany> coreCompanyRepository
            
            )
        {
            this.hrmEmployeeAdditionalInfosService = hrmEmployeeAdditionalInfosService;
            this.commonService = commonService;
            this.coreBranchRepository = coreBranchRepository;
            this.coreCompanyRepository = coreCompanyRepository;
            this.hrmEmployee = hrmEmployee;
            this.salesBankRepository = salesBankRepository;
            this.salesBankBranchRepository = salesBankBranchRepository;
        }

        #endregion

        #region Index
        public async Task<IActionResult> Index()
        {

            var hasPermission = await hrmEmployeeAdditionalInfosService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }
            HrmEmployeeAdditionalInfoPageViewModel model = new HrmEmployeeAdditionalInfoPageViewModel();

            model.AddUrl = Url.Action(nameof(Setup));
            model.PageUrl = Url.Action(nameof(Index));
            return View(model);
        }

        #endregion

        #region Get Employee Details By Code

        [HttpGet]
        public async Task<IActionResult> GetEmployeeDetailsByCode(string code)
        {
            var result = await hrmEmployeeAdditionalInfosService.GetEmployeeByCode(code);
            if (result == null)
            {
                return Json(new { error = "No data found" });
            }
            return Json(result);
        }

        #endregion

        #region Get Company and Branch Details By Code

        [HttpGet]
        public async Task<IActionResult> GetCompanyByCode(string ComapnyCode)
        {
            var result = await hrmEmployeeAdditionalInfosService.GetComapnyByCode(ComapnyCode);
            if (result == null)
            {
                return Json(new { error = "No data found" });
            }
            return Json(result);
        }


        [HttpGet]
        public async Task<IActionResult> GetBranchByCode(string ComapnyCode)
        {
            var result = await hrmEmployeeAdditionalInfosService.GetComapnyByBranchCode(ComapnyCode);
            if (result == null)
            {
                return Json(new { error = "No data found" });
            }
            return Json(result);
        }

        #endregion

        #region Get Setup

        public async Task<IActionResult> Setup(string id)
        {
            var model = await hrmEmployeeAdditionalInfosService.GetByIdAsync(id)
                ?? new HrmEmployeeAdditionalInfoSetupViewModel
                {
                    SalaryBankId = "08",
                    BankIdsibl = "15",
                    BankIducbl = "14",
                };
            var companies = coreCompanyRepository.All();

            if (companies.Count() == 1)
            {
                model.CompanyCode = companies.First().CompanyCode; // Set the selected value in the model
            }
            else if (id != null)
            {
                model.CompanyCode = (await hrmEmployeeAdditionalInfosService.GetByIdAsync(id))?.CompanyCode;
            }
            else
            {
                model.CompanyCode = null;
            }


            ViewBag.CoreCompanyDD = new SelectList(companies, "CompanyCode", "CompanyName");
            ViewBag.CoreBranchDD = new SelectList(coreBranchRepository.All(), "BranchCode", "BranchName");
            // ViewBag.CoreCompanyDD = new SelectList(coreCompanyRepository.All(), "CompanyCode", "CompanyName");
            ViewBag.EmployeeDD = new SelectList(await hrmEmployee.GetEmployeeDropSelections(), "Code", "Name");
            ViewBag.BankDD = new SelectList(salesBankRepository.All(), "BankId", "BankName");
            ViewBag.BakBranchDD = new SelectList(salesBankBranchRepository.All(), "BankBranchId", "BankBranchName");
            // model.AddUrl = Url.Action(nameof(Setup));
            if (model.EmployeAddInfoId == null)
            {
                model.EmployeAddInfoId = commonService.NextCode("EmployeAddInfoID", "HRM_EmployeeAdditionalInfo", 4);
                ViewBag.IsEditMode = false;
            }
            else
            {
                ViewBag.IsEditMode = true;
            }
            return View(model);
        }

        #endregion

        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]


        public async Task<IActionResult> Setup(HrmEmployeeAdditionalInfoSetupViewModel modelVM)
        {
            try
            {

                //if (!ModelState.IsValid)
                //{
                //    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                //    return Json(new { isSuccess = false, message = "Validation failed", errors });
                //}


                if (await hrmEmployeeAdditionalInfosService.IsExistByCodeAsync(modelVM.EmployeeId, modelVM.EmployeAddInfoId))
                {
                    return Json(new { isSuccess = false, message = $"Already  Exists!", isDuplicate = true });
                }


                if (string.IsNullOrEmpty(modelVM.EmployeAddInfoId))
                {
                    modelVM.EmployeAddInfoId = await hrmEmployeeAdditionalInfosService.GenerateNextCode();
                }


                modelVM.ToAudit(LoginInfo, modelVM.AutoId > 0);
                if (modelVM.AutoId == 0)
                {

                    var hasSavePermission = await hrmEmployeeAdditionalInfosService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await hrmEmployeeAdditionalInfosService.SaveAsync(modelVM, LoginInfo.CompanyCode);
                        //TempData["Data"] = "This is the data I want to share.";
                        //return RedirectToAction("Index", "HrmEmployee2");

                        return Json(new { isSuccess = true, message = "Saved Successfully.", redirectUrl = Url.Action("Index", "HrmEmployeeAdditionalInfos"), lastCode = modelVM.EmployeAddInfoId });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await hrmEmployeeAdditionalInfosService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await hrmEmployeeAdditionalInfosService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully.", redirectUrl = Url.Action("Index", "HrmEmployeeAdditionalInfos"), lastCode = modelVM.EmployeAddInfoId });
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

        #region TabeleLodaing

        [HttpGet]

        public async Task<IActionResult> GetTableData()
        {
            try
            {
                var list = await hrmEmployeeAdditionalInfosService.GetAllAsync();
                return PartialView("_Grid", list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }


        #endregion

        #region NeaxtCode

        [HttpGet]
        public async Task<IActionResult> GenerateNextCode()
        {
            var nexCode = await hrmEmployeeAdditionalInfosService.GenerateNextCode();
            return Json(nexCode);
        }
        #endregion

        #region Delete

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] List<string> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest(new { success = false, message = "No IDs provided for delete." });
            }

            var hasPermission = await hrmEmployeeAdditionalInfosService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            DeleteHistoryViewModel model = new DeleteHistoryViewModel();
            model.ToAudit(LoginInfo);
            model.CompanyCode = LoginInfo.CompanyCode;

            bool success = await hrmEmployeeAdditionalInfosService.DeleteTab(ids, model);
            if (success)
            {
                return Json(new { success = true, message = "Deleted Successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Deletion failed. Some entities may still exists." });
            }
        }


        //        [HttpPost]
        //        public async Task<IActionResult> Delete([FromBody] List<string> ids)
        //        {
        //            try
        //            {

        //                var hasPermission = await hrmEmployeeAdditionalInfosService.DeletePermissionAsync(LoginInfo.AccessCode);
        //                if (hasPermission)
        //                {

        //                    foreach (var id in ids)
        //                    {
        //                        var result = hrmEmployeeAdditionalInfosService.DeleteLeaveType(id)
        //;

        //                    }

        //                    return Json(new { isSuccess = true, message = "Deleted Successfully." });
        //                }
        //                else
        //                {

        //                    return Json(new { isSuccess = false, message = "You have no access." });
        //                }
        //            }
        //            catch (Exception ex)
        //            {

        //                Console.WriteLine($"Error deleting: {ex.Message}");

        //                return StatusCode(500, new { isSuccess = false, message = ex.Message });
        //            }
        //        }


        #endregion

        #region CheckAvailability
        [HttpPost]
        public async Task<JsonResult> CheckAvailability(string code, string employeeCode)
        {
            if (await hrmEmployeeAdditionalInfosService.IsExistByCodeAsync(code, employeeCode))
            {

                return Json(new { isSuccess = true, message = $"Already Exists!" });

            }

            return Json(new { isSuccess = false });
        }
        #endregion

    }
}

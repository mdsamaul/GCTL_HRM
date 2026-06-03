using GCTL.Core.Data;
using GCTL.Core.ViewModels.HrmDefBankAndNomineeInfos;
using GCTL.Core.ViewModels.HrmEmployeeDocumentInfos;
using GCTL.Data.Models;
using GCTL.Service.BranchesTypeInfo;
using GCTL.Service.Common;
using GCTL.Service.Companies;
using GCTL.Service.HrmDefBankAndNomineeInfos;
using GCTL.Service.HrmEmployees2;
using GCTL.UI.Core.ViewModels.HrmDefBankAndNomineeInfos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GCTL.Core.Helpers;
using GCTL.Service.BankInformations;
using GCTL.Service.BankBranchInformations;
using GCTL.Service.Relationships;
using GCTL.Service.HolidayTypes;
using Microsoft.EntityFrameworkCore;

namespace GCTL.UI.Core.Controllers
{
    public class HrmDefBankAndNomineeInfosController : BaseController
    {
        #region Dependency 
        private readonly IHrmDefBankAndNomineeInfosService hrmEmployeNominee;
        private readonly ICompanyService companyService;
        private readonly IBranchTypeInfoService branchTypeInfoService;
        private readonly IHrmEmployee2Service hrmEmployee2Service;
        private readonly ICommonService commonService;
        private readonly ISalesDefBankBranchInfosService salesDefBankBranchInfosService;
        private readonly IRepository<HrmEmployee> em2;
        private readonly IBankInformationsService bankInformationsService;
        private readonly IRelationshipsService hrmDefRelationship;
        private readonly IRepository<SalesDefBankBranchInfo> branch;
        string strMaxNO = string.Empty;
        private const string TableName = "HRM_Def_BankAndNomineeInfo";
        private const string ColumnName = "BankAndNomineeId";

        public HrmDefBankAndNomineeInfosController(ICompanyService companyService,
            ICommonService commonService,
            IBranchTypeInfoService branchTypeInfoService,
            IHrmDefBankAndNomineeInfosService hrmEmployeNominee,
            IHrmEmployee2Service hrmEmployee2Service,
            ISalesDefBankBranchInfosService salesDefBankBranchInfosService,
            IRepository<HrmEmployee> em2,
            IBankInformationsService bankInformationsService
,
            IRelationshipsService hrmDefRelationship
,
            IRepository<SalesDefBankBranchInfo> branch = null)
        {
            this.companyService = companyService;
            this.branchTypeInfoService = branchTypeInfoService;
            this.hrmEmployeNominee = hrmEmployeNominee;
            this.hrmEmployee2Service = hrmEmployee2Service;
            this.commonService = commonService;
            this.em2 = em2;
            this.salesDefBankBranchInfosService = salesDefBankBranchInfosService;
            this.bankInformationsService = bankInformationsService;
            this.hrmDefRelationship = hrmDefRelationship;
            this.branch = branch;
        }


        #endregion


        #region Getall and GetById
        public async Task<IActionResult> Index(bool child = false)
        {
            HrmDefBankAndNomineeInfosPageViewModel model = new HrmDefBankAndNomineeInfosPageViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };

            commonService.FindMaxNo(ref strMaxNO, "BankAndNomineeId", "HRM_Def_BankAndNomineeInfo", 2);

            model.Setup = new HrmDefBankAndNomineeInfosSetupViewModel
            {
                BankAndNomineeId = strMaxNO,
            };

            if (child)
                return PartialView(model);

            var comapanies = await companyService.GetCompanyDropDown();
            string selectedCompanyCode = comapanies.Count() == 1 ? comapanies.First().Code : null;
            ViewBag.CompanyDD = new SelectList(await companyService.GetCompanyDropDown(), "Code", "Name", selectedCompanyCode);
            ViewBag.BranchDD = new SelectList(await branchTypeInfoService.GetCompanieBranchSelections(), "Code", "Name");
            ViewBag.EmpDD = new SelectList(await hrmEmployee2Service.GetEmployeeDropSelections(), "Code", "Name");
            ViewBag.BankDD = new SelectList(bankInformationsService.BankDropSelectionAsync(), "Code", "Name");
            ViewBag.BankBranchDD = new SelectList(salesDefBankBranchInfosService.BankBranchDropSelectionAsync(), "Code", "Name");
            ViewBag.RelationDD = new SelectList(await hrmDefRelationship.RelationshipsSelectionAsync(), "Code", "Name");

            return View(model);
        }

        public async Task<IActionResult> Setup(string id)
        {
            HrmDefBankAndNomineeInfosSetupViewModel model = new HrmDefBankAndNomineeInfosSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "BankAndNomineeId", "HRM_Def_BankAndNomineeInfo", 2);

            if (!string.IsNullOrEmpty(id))
            {

                model = await hrmEmployeNominee.GetByIdAsync(id)

;
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.BankAndNomineeId = strMaxNO;
            }

            var comapanies = await companyService.GetCompanyDropDown();
            string selectedCompanyCode = comapanies.Count() == 1 ? comapanies.First().Code : null;
            ViewBag.CompanyDD = new SelectList(await companyService.GetCompanyDropDown(), "Code", "Name", selectedCompanyCode);
            ViewBag.BranchDD = new SelectList(await branchTypeInfoService.GetCompanieBranchSelections(), "Code", "Name");
            ViewBag.EmpDD = new SelectList(await hrmEmployee2Service.GetEmployeeDropSelections(), "Code", "Name");
            ViewBag.BankDD = new SelectList(bankInformationsService.BankDropSelectionAsync(), "Code", "Name");
            ViewBag.BankBranchDD = new SelectList(salesDefBankBranchInfosService.BankBranchDropSelectionAsync(), "Code", "Name");
            ViewBag.RelationDD = new SelectList(await hrmDefRelationship.RelationshipsSelectionAsync(), "Code", "Name");
            return PartialView($"_{nameof(Setup)}", model);
        }
        #endregion
        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(HrmDefBankAndNomineeInfosSetupViewModel modelVM)
        {
            try
            {

                if (await hrmEmployeNominee.IsExistAsync(modelVM.BankId, modelVM.BankBranchId, modelVM.BankAccountName, modelVM.BankAccountNo, modelVM.NomineeName, modelVM.BankAndNomineeId))
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
                    var hasSavePermission = await hrmEmployeNominee.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await hrmEmployeNominee.SaveAsync(modelVM, LoginInfo.CompanyCode);
                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.BankAndNomineeId });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await hrmEmployeNominee.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await hrmEmployeNominee.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.BankAndNomineeId });
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

        #region delete 
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] List<string> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest(new { success = false, message = "No IDs provided for delete." });
            }

            var hasPermission = await hrmEmployeNominee.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await hrmEmployeNominee.DeleteTab(ids);
            if (success)
            {
                return Json(new { success = true, message = "Deleted Successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Deletion failed. Some entities may still exists." });
            }
        }

        #endregion
        #region Duplicate Check
        [HttpPost]
        public async Task<JsonResult> CheckAvailability(string bankCode, string branchBankCode, string acName, string acNO, string nomineeName, string code)
        {

            if (await hrmEmployeNominee.IsExistAsync(bankCode, branchBankCode, acName, acNO, nomineeName, code))
            {
                return Json(new { isSuccess = true, message = "Already Exists!" });
            }

            return Json(new { isSuccess = false });
        }
        #endregion

        #region TabeleLodaing

        [HttpGet]
        public async Task<IActionResult> GetTableData(string employeeId)
        {
            try
            {
                if (employeeId == null)
                {

                    return PartialView("_Grid", new List<HrmDefBankAndNomineeInfosSetupViewModel>());
                }
                else
                {
                    var list = await hrmEmployeNominee.GetAllAsync(employeeId);
                    return PartialView("_Grid", list);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBranchBank(string bankId)
        {
            try
            {
                if (string.IsNullOrEmpty(bankId))
                    return BadRequest("Bank ID is required.");

                var branchList = await branch.All().Where(x => x.BankId == bankId)
                    .Select(x => new
                    {
                        x.BankBranchId,
                        x.BankBranchName
                    }).ToListAsync();

                return Ok(branchList);
            }
            catch (Exception ex)
            {
                // Optional: log the exception
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




        #endregion

        #region  Get Company Branch Employee Details 
        [HttpGet]
        public async Task<IActionResult> GetBranchByCode(string ComapnyCode)
        {
            var result = await hrmEmployeNominee.GetComapnyByBranchCode(ComapnyCode);
            if (result == null)
            {
                return Json(new { error = "No data found" });
            }
            return Json(result);
        }



        [HttpGet]
        public async Task<IActionResult> GetEmployeeDetailsByComapnyCode(string companyCode)
        {
            var result = await hrmEmployeNominee.GetEmployeeByCompanyCode(companyCode);
            if (result == null)
            {
                return Json(new { error = "No data found" });
            }
            return Json(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetEmployeeNameDesDeptByCode(string employeeId)
        {
            var result = await hrmEmployeNominee.GetEmployeeNameDesDeptByCode(employeeId);
            if (result == null)
            {
                return Json(new { error = "No data found" });
            }
            return Json(result);
        }
        #endregion

    }
}
using GCTL.Core.ViewModels.HrmEmployeeDocumentInfos;
using GCTL.Core.ViewModels.HrmEmployeeEducations;
using GCTL.Core.ViewModels.HrmEmployeeFamilys;
using GCTL.Data.Models;
using GCTL.Service.BranchesTypeInfo;
using GCTL.Service.Common;
using GCTL.Service.Companies;
using GCTL.Service.HrmEmployeeDocumentInfos;
using GCTL.Service.HrmEmployeeFamilys;
using GCTL.Service.HrmEmployees2;
using GCTL.UI.Core.ViewModels.HrmEmployeeDocumentInfos;
using GCTL.UI.Core.ViewModels.HrmEmployeeFamilys;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GCTL.Core.Helpers;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.HrmEmployeeQualifications;
using GCTL.Service.HrmEmployeeQualifications;
using GCTL.Service.HrmEmployeeAdditionalInfos;

namespace GCTL.UI.Core.Controllers
{
    public class HrmEmployeeDocumentInfosController : BaseController
    {
        private readonly IHrmEmployeeDocumentInfosService hrmEmployeeDocumentInfosService;
        private readonly ICompanyService companyService;
        private readonly IBranchTypeInfoService branchTypeInfoService;
        private readonly IHrmEmployee2Service hrmEmployee2Service;
        private readonly ICommonService commonService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IRepository<HrmEmployee> em2;
        string strMaxNO = string.Empty;
        private const string TableName = "HRM_EmployeeDocumentInfo";
        private const string ColumnName = "EmpDocID";

        public HrmEmployeeDocumentInfosController(ICompanyService companyService,
            ICommonService commonService,
            IBranchTypeInfoService branchTypeInfoService,
            IHrmEmployeeDocumentInfosService hrmEmployeeDocumentInfosService,
            IHrmEmployee2Service hrmEmployee2Service,
            IWebHostEnvironment webHostEnvironment = null,
            IRepository<HrmEmployee> em2 = null

            )
        {
            this.companyService = companyService;
            this.branchTypeInfoService = branchTypeInfoService;
            this.hrmEmployeeDocumentInfosService = hrmEmployeeDocumentInfosService;
            this.hrmEmployee2Service = hrmEmployee2Service;
            this.commonService = commonService;
            this.webHostEnvironment = webHostEnvironment;
            this.em2 = em2;
        }

        public async Task<IActionResult> Index(bool child = false)
        {
            HrmEmployeeDocumentInfosPageViewModel model = new HrmEmployeeDocumentInfosPageViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };



            model.Setup = new HrmEmployeeDocumentInfosSetup
            {

                EmpDocId = strMaxNO,

            };

            if (child)
                return PartialView(model);

            var comapanies = await companyService.GetCompanyDropDown();
            string selectedCompanyCode = comapanies.Count() == 1 ? comapanies.First().Code : null;
            ViewBag.CompanyDD = new SelectList(await companyService.GetCompanyDropDown(), "Code", "Name", selectedCompanyCode);
            ViewBag.BranchDD = new SelectList(await branchTypeInfoService.GetCompanieBranchSelections(), "Code", "Name");
            ViewBag.EmpDD = new SelectList(await hrmEmployee2Service.GetEmployeeDropSelections(), "Code", "Name");

            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> GetBranchByCode(string ComapnyCode)
        {
            var result = await hrmEmployeeDocumentInfosService.GetComapnyByBranchCode(ComapnyCode);
            return Json(result);
        }


        public async Task<IActionResult> Setup(string id)
        {
            HrmEmployeeDocumentInfosSetup model = new HrmEmployeeDocumentInfosSetup();
            commonService.FindMaxNo(ref strMaxNO, "EmpDocID", "HRM_EmployeeDocumentInfo", 2);

            if (!string.IsNullOrEmpty(id))
            {

                model = await hrmEmployeeDocumentInfosService.GetByIdAsync(id)
;
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.EmpDocId = strMaxNO;
            }

            var comapanies = await companyService.GetCompanyDropDown();
            string selectedCompanyCode = comapanies.Count() == 1 ? comapanies.First().Code : null;
            ViewBag.CompanyDD = new SelectList(await companyService.GetCompanyDropDown(), "Code", "Name", selectedCompanyCode);
            ViewBag.BranchDD = new SelectList(await branchTypeInfoService.GetCompanieBranchSelections(), "Code", "Name");
            ViewBag.EmpDD = new SelectList(await hrmEmployee2Service.GetEmployeeDropSelections(), "Code", "Name");

            return PartialView($"_{nameof(Setup)}", model);
        }



        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(HrmEmployeeDocumentInfosSetup modelVM)
        {
            try
            {

                if (await hrmEmployeeDocumentInfosService.IsExistAsync(modelVM.EmployeeId, modelVM.EmpDocId, modelVM.DocumentName))
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
                    var hasSavePermission = await hrmEmployeeDocumentInfosService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await hrmEmployeeDocumentInfosService.SaveAsync(modelVM, LoginInfo.CompanyCode);
                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.EmpDocId });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await hrmEmployeeDocumentInfosService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await hrmEmployeeDocumentInfosService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.EmpDocId });
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
        public async Task<JsonResult> CheckAvailability(string degreeCode, string employeeCode, string code)
        {

            if (await hrmEmployeeDocumentInfosService.IsExistAsync(employeeCode, code, degreeCode))
            {
                return Json(new { isSuccess = true, message = "Already Exists!" });
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

            var hasPermission = await hrmEmployeeDocumentInfosService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await hrmEmployeeDocumentInfosService.DeleteTab(ids);
            if (success)
            {
                return Json(new { success = true, message = "Deleted Successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Deletion failed. Some entities may still exists." });
            }
        }

        #region TabeleLodaing

        [HttpGet]
        public async Task<IActionResult> GetTableData(string employeeId)
        {
            try
            {
                if (employeeId == null)
                {

                    return PartialView("_Grid", new List<HrmEmployeeDocumentInfosSetup>());
                }
                else
                {
                    var list = await hrmEmployeeDocumentInfosService.GetAllAsync(employeeId);
                    return PartialView("_Grid", list);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeDetailsByComapnyCode(string companyCode)
        {
            var result = await hrmEmployeeDocumentInfosService.GetEmployeeByCompanyCode(companyCode);
            if (result == null)
            {
                return Json(new { error = "No data found" });
            }
            return Json(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetEmployeeNameDesDeptByCode(string employeeId)
        {
            var result = await hrmEmployeeDocumentInfosService.GetEmployeeNameDesDeptByCode(employeeId);
            if (result == null)
            {
                return Json(new { error = "No data found" });
            }
            return Json(result);
        }

        #endregion

    }
}
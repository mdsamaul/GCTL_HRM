using GCTL.Core.Data;
using GCTL.Core.ViewModels.EmployeeReferenceInfos;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.EmployeeReferenceInfos;
using GCTL.UI.Core.ViewModels.EmployeeReferenceInfos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GCTL.Core.Helpers;
using GCTL.Service.HrmEmployees2;

namespace GCTL.UI.Core.Controllers
{
    public class EmployeeReferenceInfosController : BaseController
    {
        #region Service & Rep
        private readonly IEmployeeReferenceInfosService employeeReferenceInfosService;
        private readonly ICommonService commonService;
        private readonly IRepository<CoreCompany> coreCompanyRepository;
        private readonly IRepository<CoreBranch> coreBranchRepository;
        private readonly IHrmEmployee2Service hrmEmployee;
        private readonly IRepository<HrmDefDepartment> departmentRepository;
        private readonly IRepository<HrmDefDesignation> designationRepository;
        private readonly IRepository<HrmDefRelationship> relationshipRepository;
        private readonly IRepository<HrmDefNationality> nationalRepository;
        private readonly IRepository<HrmEmployeeOfficialInfo> hrmEmpOffialInfo;

        string strMaxNO = string.Empty;

        public EmployeeReferenceInfosController(
            IEmployeeReferenceInfosService employeeReferenceInfosService,
            ICommonService commonService,
            IRepository<CoreCompany> coreCompanyRepository,
            IRepository<CoreBranch> coreBranchRepository,
           IHrmEmployee2Service hrmEmployee,
           IRepository<HrmDefDepartment> departmentRepository,
           IRepository<HrmDefDesignation> designationRepository,
           IRepository<HrmDefRelationship> relationshipRepository,
           IRepository<HrmDefNationality> nationalRepository,
           IRepository<HrmEmployeeOfficialInfo> hrmEmpOffialInfo
            )
        {
            this.employeeReferenceInfosService = employeeReferenceInfosService;
            this.commonService = commonService;
            this.coreCompanyRepository = coreCompanyRepository;
            this.coreBranchRepository = coreBranchRepository;
            this.departmentRepository = departmentRepository;
            this.designationRepository = designationRepository;
            this.relationshipRepository = relationshipRepository;
            this.nationalRepository = nationalRepository;
            this.hrmEmployee = hrmEmployee;
            this.hrmEmpOffialInfo = hrmEmpOffialInfo;
        }

        #endregion

        #region Index

        //public async Task<ActionResult> Grid()
        //{
        //    var result = await employeeReferenceInfosService.GetAllAsync();
        //    return Json(new { data = result });
        //}

        public async Task<IActionResult> Index(bool child = false)
        {
            EmployeeReferenceInfosPageViewModel model = new EmployeeReferenceInfosPageViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };


            var companies = coreCompanyRepository.All().ToList();
            string selectedCompanyCode = companies.Count == 1 ? companies.First().CompanyCode : null;

            ViewBag.CoreCompanyDD = new SelectList(companies, "CompanyCode", "CompanyName", selectedCompanyCode);
            commonService.FindMaxNo(ref strMaxNO, "EmpReferenceId", "HRM_EmployeeReferenceInfo", 4);
            ViewBag.CoreBranchDD = new SelectList(coreBranchRepository.All(), "BranchCode", "BranchName");
            ViewBag.DeptDD = new SelectList(departmentRepository.All(), "DepartmentCode", "DepartmentName");
            ViewBag.DesigDD = new SelectList(designationRepository.All(), "DesignationCode", "DesignationName");
            ViewBag.EmployeeDD = new SelectList(await hrmEmployee.GetEmployeeDropSelections(), "Code", "Name");
            //  ViewBag.EmployeeDD = new SelectList(employeeRepository.GetEmployeeDropSelections(), "Code", "Name");
            ViewBag.RelationshipDD = new SelectList(relationshipRepository.All(), "RelationshipCode", "Relationship");
            ViewBag.NationalityDD = new SelectList(nationalRepository.All(), "NationalityCode", "Nationality");

            //var List = await employeeReferenceInfosService.GetAllAsync();
            //if (List == null || !List.Any())
            //{
            //    ViewData["List"] = new List<EmployeeReferenceInfosSetupViewModel>();
            //    ViewBag.Message = "No Data Available";
            //}
            //else
            //{
            //    ViewData["List"] = List;
            //}
            ////
            //var list = await employeeReferenceInfosService.GetAllAsync();

            model.Setup = new EmployeeReferenceInfosSetupViewModel
            {
                EmpReferenceId = strMaxNO,

            };

            if (child)
                return PartialView(model);

            return View(model);
        }

        #endregion

        #region Setup

        public async Task<IActionResult> Setup(string id)
        {
            EmployeeReferenceInfosSetupViewModel model = new EmployeeReferenceInfosSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "EmpReferenceId", "HRM_EmployeeReferenceInfo", 4);

            if (!string.IsNullOrEmpty(id))
            {

                model = await employeeReferenceInfosService.GetByIdAsync(id);
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.EmpReferenceId = strMaxNO;
            }

            var companies = coreCompanyRepository.All().ToList();
            string selectedCompanyCode = companies.Count == 1 ? companies.First().CompanyCode : null;

            ViewBag.CoreCompanyDD = new SelectList(companies, "CompanyCode", "CompanyName", selectedCompanyCode);

            // ViewBag.CoreCompanyDD = new SelectList(coreCompanyRepository.All(), "CompanyCode", "CompanyName",model.CompanyCode);
            ViewBag.CoreBranchDD = new SelectList(coreBranchRepository.All(), "BranchCode", "BranchName", model.BranchCode);
            ViewBag.DeptDD = new SelectList(departmentRepository.All(), "DepartmentCode", "DepartmentName", model.DepartmentName);
            ViewBag.DesigDD = new SelectList(designationRepository.All(), "DesignationCode", "DesignationName");
            ViewBag.EmployeeDD = new SelectList(await hrmEmployee.GetEmployeeDropSelections(), "Code", "Name");
            //   ViewBag.EmployeeDD = new SelectList(employeeRepository.GetEmployeeDropSelections(), "Code", "Name");
            ViewBag.RelationshipDD = new SelectList(relationshipRepository.All(), "RelationshipCode", "Relationship");
            ViewBag.NationalityDD = new SelectList(nationalRepository.All(), "NationalityCode", "Nationality");
            return PartialView($"_{nameof(Setup)}", model);
        }

        #endregion

        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(EmployeeReferenceInfosSetupViewModel modelVM)
        {
            try
            {
                if (await employeeReferenceInfosService.IsExistAsync(modelVM.EmployeeId, modelVM.CompanyCode, modelVM.ReferenceName, modelVM.EmpReferenceId))
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
                    var hasSavePermission = await employeeReferenceInfosService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await employeeReferenceInfosService.SaveAsync(modelVM, LoginInfo.CompanyCode);
                        return Json(new { isSuccess = true, message = "Data Saved Successfully.", lastCode = modelVM.EmpReferenceId });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {
                    var hasUpdatePermission = await employeeReferenceInfosService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await employeeReferenceInfosService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Data Updated Successfully.", lastCode = modelVM.EmpReferenceId });
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
        public async Task<JsonResult> CheckAvailability(string ReferenceName, string employeeCode, string code, string empReferenceId)
        {

            if (await employeeReferenceInfosService.IsExistAsync(employeeCode, code, ReferenceName, empReferenceId))
            {
                return Json(new { isSuccess = true, message = "Already Exists!" });
            }

            return Json(new { isSuccess = false });
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

            var hasPermission = await employeeReferenceInfosService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await employeeReferenceInfosService.DeleteTab(ids);
            if (success)
            {
                return Json(new { success = true, message = "Data Deleted Successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Deletion failed. Some entities may still exists." });
            }
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

                    return PartialView("_Grid", new List<EmployeeReferenceInfosSetupViewModel>());
                }
                else
                {
                    var list = await employeeReferenceInfosService.GetAllAsync(employeeId);
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
            var result = await employeeReferenceInfosService.GetEmployeeByCompanyCode(companyCode);
            if (result == null)
            {
                return Json(new { error = "No data found" });
            }
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetBranchByCode(string ComapnyCode)
        {
            var result = await employeeReferenceInfosService.GetComapnyByBranchCode(ComapnyCode);
            if (result == null)
            {
                return Json(new { error = "No data found" });
            }
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeNameDesDeptByCode(string employeeId)
        {
            var result = await employeeReferenceInfosService.GetEmployeeNameDesDeptByCode(employeeId);

            if (result == null)
            {
                return Json(new { error = "No data found" });
            }

            return Json(result);
        }

        #endregion

    }
}

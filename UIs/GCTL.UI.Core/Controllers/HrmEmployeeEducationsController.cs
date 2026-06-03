using GCTL.Core.Data;
using GCTL.Core.ViewModels.HrmEmployeeEducations;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.HrmEmployeeEducations;
using GCTL.UI.Core.ViewModels.HrmEmployeeEducations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GCTL.Core.Helpers;
using GCTL.Service.HrmEmployees2;

namespace GCTL.UI.Core.Controllers
{
    public class HrmEmployeeEducationsController : BaseController
    {
        #region Service Injection
        private readonly IHrmEmployeeEducationsService hrmDefEmpEduService;
        private readonly ICommonService commonService;
        private readonly IRepository<HrmDefDegree> degreeRepository;
        private readonly IRepository<HrmDefBoardCountryName> hrmBoardCountryName;
        private readonly IRepository<HrmDefExamTitle> hrmDefexamTitle;
        private readonly IRepository<HrmDefInstitute> hrmDefInstitute;
        private readonly IRepository<HrmDefExamGroupInfo> hrmExamGroupInfo;
        private readonly IRepository<CoreCompany> coreCompanyRepository;
        private readonly IRepository<CoreBranch> coreBranchRepository;
        private readonly IHrmEmployee2Service hrmEmployee;
        private readonly IRepository<HrmDefDepartment> departmentRepository;
        private readonly IRepository<HrmDefDesignation> designationRepository;

        string strMaxNO = string.Empty;

        public HrmEmployeeEducationsController(
            IHrmEmployeeEducationsService hrmDefEmpEduService, 
            ICommonService commonService, 
            IRepository<HrmDefDegree> degreeRepository, 
            IRepository<HrmDefBoardCountryName> hrmBoardCountryName,
            IRepository<HrmDefExamTitle> hrmDefexamTitle,
            IRepository<HrmDefInstitute> hrmDefInstitute, 
            IRepository<HrmDefExamGroupInfo> hrmExamGroupInfo,
            IRepository<CoreCompany> coreCompanyRepository,
            IRepository<CoreBranch> coreBranchRepository,
            IHrmEmployee2Service hrmEmployee,
            IRepository<HrmDefDepartment> departmentRepository,
            IRepository<HrmDefDesignation> designationRepository
            )
        {
            this.hrmDefEmpEduService = hrmDefEmpEduService;
            this.commonService = commonService;
            this.degreeRepository = degreeRepository;
            this.hrmBoardCountryName = hrmBoardCountryName;
            this.hrmDefexamTitle = hrmDefexamTitle;
            this.hrmDefInstitute = hrmDefInstitute;
            this.hrmExamGroupInfo = hrmExamGroupInfo;
            this.coreCompanyRepository = coreCompanyRepository;
            this.coreBranchRepository = coreBranchRepository;
            this.hrmEmployee = hrmEmployee;
            this.departmentRepository = departmentRepository;
            this.designationRepository = designationRepository;
        }

        #endregion

        #region Index

        public async Task<IActionResult> Index(bool child = false)
        {
            HrmEmployeeEducationsPageViewModel model = new HrmEmployeeEducationsPageViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };


            var companies = coreCompanyRepository.All().ToList();
            string selectedCompanyCode = companies.Count == 1 ? companies.First().CompanyCode : null;

            ViewBag.CoreCompanyDD = new SelectList(companies, "CompanyCode", "CompanyName", selectedCompanyCode);
            commonService.FindMaxNo(ref strMaxNO, "EmpEduCode", "HRM_EmployeeEducation", 3);
            ViewBag.DegreeDD = new SelectList(degreeRepository.All(), "DegreeCode", "DegreeName");
            ViewBag.BoardDD = new SelectList(hrmBoardCountryName.All(), "BoardCode", "BoardName");
            ViewBag.ExamTitleDD = new SelectList(hrmDefexamTitle.All(), "ExamTitleCode", "ExamTitleName");
            ViewBag.InstituteDD = new SelectList(hrmDefInstitute.All(), "InstituteCode", "InstituteName");
            ViewBag.ExamGroupDD = new SelectList(hrmExamGroupInfo.All(), "GroupCode", "GroupName");
            ViewBag.CoreBranchDD = new SelectList(coreBranchRepository.All(), "BranchCode", "BranchName");
            ViewBag.DeptDD = new SelectList(departmentRepository.All(), "DepartmentCode", "DepartmentName");
            ViewBag.DesigDD = new SelectList(designationRepository.All(), "DesignationCode", "DesignationName");
            ViewBag.EmployeeDD = new SelectList(await hrmEmployee.GetEmployeeDropSelections(), "Code", "Name");



            model.Setup = new HrmEmployeeEducationsSetupViewModel
            {
                EmpEduCode = strMaxNO,

            };

            if (child)
                return PartialView(model);

            return View(model);
        }

        #endregion

        #region Setup
        public async Task<IActionResult> Setup(string id)
        {
            HrmEmployeeEducationsSetupViewModel model = new HrmEmployeeEducationsSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "EmpEduCode", "HRM_EmployeeEducation", 3);

            if (!string.IsNullOrEmpty(id))
            {

                model = await hrmDefEmpEduService.GetByIdAsync(id)
;
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.EmpEduCode = strMaxNO;
            }

            var companies = coreCompanyRepository.All().ToList();
            string selectedCompanyCode = companies.Count == 1 ? companies.First().CompanyCode : null;

            ViewBag.CoreCompanyDD = new SelectList(companies, "CompanyCode", "CompanyName", selectedCompanyCode);

            ViewBag.DegreeDD = new SelectList(degreeRepository.All(), "DegreeCode", "DegreeName", model.DegreeCode);
            ViewBag.BoardDD = new SelectList(hrmBoardCountryName.All(), "BoardCode", "BoardName", model.BoardCode);
            ViewBag.ExamTitleDD = new SelectList(hrmDefexamTitle.All(), "ExamTitleCode", "ExamTitleName", model.ExamTitleCode);
            ViewBag.InstituteDD = new SelectList(hrmDefInstitute.All(), "InstituteCode", "InstituteName", model.InstitueCode);
            ViewBag.ExamGroupDD = new SelectList(hrmExamGroupInfo.All(), "GroupCode", "GroupName", model.GroupCode);
            ViewBag.CoreBranchDD = new SelectList(coreBranchRepository.All(), "BranchCode", "BranchName", model.BranchCode);
            ViewBag.DeptDD = new SelectList(departmentRepository.All(), "DepartmentCode", "DepartmentName", model.DegreeCode);
            ViewBag.DesigDD = new SelectList(designationRepository.All(), "DesignationCode", "DesignationName");
            ViewBag.EmployeeDD = new SelectList(await hrmEmployee.GetEmployeeDropSelections(), "Code", "Name");
            return PartialView($"_{nameof(Setup)}", model);
        }

        #endregion

        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(HrmEmployeeEducationsSetupViewModel modelVM)
        {
            try
            {

                if (await hrmDefEmpEduService.IsExistAsync(modelVM.EmployeeId, modelVM.ExamTitleCode, modelVM.DegreeCode,modelVM.EmpEduCode))
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
                    var hasSavePermission = await hrmDefEmpEduService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await hrmDefEmpEduService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Data Saved Successfully.", lastCode = modelVM.EmpEduCode });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await hrmDefEmpEduService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await hrmDefEmpEduService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Data Updated Successfully.", lastCode = modelVM.EmpEduCode });
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
        public async Task<JsonResult> CheckAvailability(string degreeCode, string employeeCode, string code, string eduCode)
        {

            if (await hrmDefEmpEduService.IsExistAsync(employeeCode, code, degreeCode,eduCode))
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

            var hasPermission = await hrmDefEmpEduService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await hrmDefEmpEduService.DeleteTab(ids);
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
                    return PartialView("_Grid", new List<HrmEmployeeEducationsSetupViewModel>());

                }
                else
                {
                    var list = await hrmDefEmpEduService.GetAllAsync(employeeId);
                    return PartialView("_Grid", list);
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetEmployeeNameDesDeptByCode(string code)
        {
            var result = await hrmDefEmpEduService.GetEmployeeNameDesDeptByCode(code);
            if (result == null)
            {
                return Json(new { error = "No data found" });
            }
            return Json(result);
        }


        [HttpGet]
        public async Task<IActionResult> GetEmployeeDetailsByComapnyCode(string companyCode)
        {
            var result = await hrmDefEmpEduService.GetEmployeeByCompanyCode(companyCode);
            if (result == null)
            {
                return Json(new { error = "No data found" });
            }
            return Json(result);
        }
        #endregion
    }
}
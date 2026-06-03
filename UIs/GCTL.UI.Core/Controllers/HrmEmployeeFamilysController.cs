using GCTL.Core.Data;
using GCTL.Core.ViewModels.HrmEmployeeEducations;
using GCTL.Core.ViewModels.HrmEmployeeFamilys;
using GCTL.Data.Models;
using GCTL.Service.Common;

using GCTL.Service.HrmEmployeeFamilys;
using GCTL.UI.Core.ViewModels.HrmEmployeeEducations;
using GCTL.UI.Core.ViewModels.HrmEmployeeFamilys;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GCTL.Core.Helpers;
using System.Globalization;
using GCTL.Service.HrmEmployees2;

namespace GCTL.UI.Core.Controllers
{
    public class HrmEmployeeFamilysController : BaseController
    {
        private IHrmEmployeeFamilysService hrmEmployeeFamilysService;

        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;
        private readonly IRepository<CoreCompany> coreCompanyRepository;
        private readonly IRepository<CoreBranch> coreBranchRepository;
        private readonly IHrmEmployee2Service hrmEmployee;
        private readonly IRepository<HrmDefDepartment> departmentRepository;
        private readonly IRepository<HrmDefDesignation> designationRepository;
        private readonly IRepository<HrmDefRelationship> hrmRelationShip;
        private readonly IRepository<HrmDefBloodGroup> bloodGroupRepository;
        private readonly IRepository<HrmDefOccupation> occupationRepository;

        string strMaxNO = string.Empty;
        private const string TableName = "HRM_EmployeeFamily";
        private const string ColumnName = "EmpFamilyID";

        public HrmEmployeeFamilysController(IHrmEmployeeFamilysService hrmEmployeeFamilysService, IRepository<CoreAccessCode> accessCodeRepository, ICommonService commonService, IRepository<CoreCompany> coreCompanyRepository, IRepository<CoreBranch> coreBranchRepository, IHrmEmployee2Service hrmEmployee, IRepository<HrmDefDepartment> departmentRepository, IRepository<HrmDefDesignation> designationRepository, IRepository<HrmDefRelationship> hrmRelationShip, IRepository<HrmDefBloodGroup> bloodGroupRepository, IRepository<HrmDefOccupation> occupationRepository)
        {
            this.hrmEmployeeFamilysService = hrmEmployeeFamilysService;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
            this.coreCompanyRepository = coreCompanyRepository;
            this.coreBranchRepository = coreBranchRepository;
            this.hrmEmployee = hrmEmployee;
            this.departmentRepository = departmentRepository;
            this.designationRepository = designationRepository;
            this.hrmRelationShip = hrmRelationShip;
            this.bloodGroupRepository = bloodGroupRepository;
            this.occupationRepository = occupationRepository;
        }

        public async Task<IActionResult> Index(bool child = false)
        {
            HrmEmployeeFamilysPageViewModel model = new HrmEmployeeFamilysPageViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };

            var companies = coreCompanyRepository.All().ToList();
            string selectedCompanyCode = companies.Count == 1 ? companies.First().CompanyCode : null;

            ViewBag.CoreCompanyDD = new SelectList(companies, "CompanyCode", "CompanyName", selectedCompanyCode);
            commonService.FindMaxNo(ref strMaxNO, "EmpFamilyID", "HRM_EmployeeFamily", 3);
            ViewBag.CoreBranchDD = new SelectList(coreBranchRepository.All(), "BranchCode", "BranchName");
            ViewBag.DeptDD = new SelectList(departmentRepository.All(), "DepartmentCode", "DepartmentName");
            ViewBag.DesigDD = new SelectList(designationRepository.All(), "DesignationCode", "DesignationName");
            ViewBag.EmployeeDD = new SelectList(await hrmEmployee.GetEmployeeDropSelections(), "Code", "Name");
            ViewBag.BloodGroupDD = new SelectList(bloodGroupRepository.All(), "BloodGroupCode", "BloodGroup");
            ViewBag.OccupationDD = new SelectList(occupationRepository.All().Select(x => new { x.OccupationCode, x.Occupation }), "OccupationCode", "Occupation");
            ViewBag.ShipDD = new SelectList(hrmRelationShip.All(), "RelationshipCode", "Relationship");


            model.Setup = new HrmEmployeeFamilysSetViewModel
            {
                EmpFamilyId = strMaxNO,

            };

            if (child)
                return PartialView(model);

            return View(model);
        }

        #region For Employee get Data OnChange

        [HttpGet]
        public async Task<IActionResult> GetEmployeeDetailsByComapnyCode(string companyCode)
        {
            var result = await hrmEmployeeFamilysService.GetEmployeeByCompanyCode(companyCode);
            if (result == null)
            {
                return Json(new { error = "No data found" });
            }
            return Json(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetEmployeeNameDesDeptByCode(string employeeId)
        {
            var result = await hrmEmployeeFamilysService.GetEmployeeNameDesDeptByCode(employeeId);
            if (result == null)
            {
                return Json(new { error = "No data found" });
            }
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetTableData(string employeeId)
        {
            try
            {
                if (employeeId == null)
                {
                    return PartialView("_Grid", new List<HrmEmployeeFamilysSetViewModel>());
                }
                else
                {
                    var list = await hrmEmployeeFamilysService.GetAllAsync(employeeId);
                    return PartialView("_Grid", list);
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        #endregion


        public async Task<IActionResult> Setup(string id)
        {
            HrmEmployeeFamilysSetViewModel model = new HrmEmployeeFamilysSetViewModel();
            commonService.FindMaxNo(ref strMaxNO, "EmpFamilyID", "HRM_EmployeeFamily", 3);

            if (!string.IsNullOrEmpty(id))
            {

                model = await hrmEmployeeFamilysService.GetByIdAsync(id)
;
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.EmpFamilyId = strMaxNO;
            }

            var companies = coreCompanyRepository.All().ToList();
            string selectedCompanyCode = companies.Count == 1 ? companies.First().CompanyCode : null;

            ViewBag.CoreCompanyDD = new SelectList(companies, "CompanyCode", "CompanyName", selectedCompanyCode);
            commonService.FindMaxNo(ref strMaxNO, "EmpFamilyID", "HRM_EmployeeFamily", 3);
            ViewBag.CoreBranchDD = new SelectList(coreBranchRepository.All(), "BranchCode", "BranchName");
            ViewBag.DeptDD = new SelectList(departmentRepository.All(), "DepartmentCode", "DepartmentName");
            ViewBag.DesigDD = new SelectList(designationRepository.All(), "DesignationCode", "DesignationName");
            ViewBag.EmployeeDD = new SelectList(await hrmEmployee.GetEmployeeDropSelections(), "Code", "Name");
            ViewBag.BloodGroupDD = new SelectList(bloodGroupRepository.All(), "BloodGroupCode", "BloodGroup");
            ViewBag.OccupationDD = new SelectList(occupationRepository.All().Select(x => new { x.OccupationCode, x.Occupation }), "OccupationCode", "Occupation");
            ViewBag.ShipDD = new SelectList(hrmRelationShip.All(), "RelationshipCode", "Relationship");

            return PartialView($"_{nameof(Setup)}", model);
        }



        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(HrmEmployeeFamilysSetViewModel modelVM)
        {
            try
            {

                if (await hrmEmployeeFamilysService.IsExistAsync(modelVM.EmployeeId, modelVM.EmpFamilyId, modelVM.Name))
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
                    var hasSavePermission = await hrmEmployeeFamilysService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await hrmEmployeeFamilysService.SaveAsync(modelVM, LoginInfo.CompanyCode);
                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.EmpFamilyId });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await hrmEmployeeFamilysService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await hrmEmployeeFamilysService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.EmpFamilyId });
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
        public async Task<JsonResult> CheckAvailability(string name, string employeeCode, string code)
        {

            if (await hrmEmployeeFamilysService.IsExistAsync(employeeCode, code, name))
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

            var hasPermission = await hrmEmployeeFamilysService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await hrmEmployeeFamilysService.DeleteTab(ids);
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




        #endregion

    }
}
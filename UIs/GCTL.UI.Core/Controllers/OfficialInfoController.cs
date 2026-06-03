using GCTL.Core.Data;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HrmEmployeeOfficialInfo;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.EmployeeOfficialInfo;
using GCTL.UI.Core.ViewModels.HrmEmployeeOfficialInfo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GCTL.UI.Core.Controllers
{
    public class OfficialInfoController : BaseController
    {
        #region Services & Repositories
        private readonly IEmployeeOfficialInfoService _employeeOfficialInfoService;
        private readonly ICommonService _commonService;
        private readonly IRepository<HrmEmployee> _emplooyeeRepository;
        private readonly IRepository<CoreCompany> _companyRepository;
        private readonly IRepository<HrmDefDepartment> _departmentRepository;
        private readonly IRepository<HrmDefEmpType> _defEmpTypeRepository;
        private readonly IRepository<HrmAtdShift> _shiftRepository;
        private readonly IRepository<CoreBranch> _branchRepository;
        private readonly IRepository<HrmDefDivision> _divisionRepository;
        private readonly IRepository<HrmDefDesignation> _designationRepository;
        private readonly IRepository<HrmDefEmployeeStatus> _employeeStatusRepository;
        private readonly IRepository<HrmDefGrade> _gradeRepository;
        private readonly IRepository<HrmEmployeeOfficialInfo> employeeOfficialInfoRepository;
        private readonly IRepository<HrmEisDefEmploymentNature> _employmentNature;
        private readonly IRepository<CorePeriodInfo> corePeriodInforepository;

        string strMaxNo = "";


        public OfficialInfoController(
            IEmployeeOfficialInfoService employeeOfficialInfoService,
            ICommonService commonService,
            IRepository<HrmEmployee> emplooyeeRepository,
            IRepository<CoreCompany> companyRepository,
            IRepository<HrmDefDepartment> departmentRepository,
            IRepository<HrmAtdShift> shiftRepository,
            IRepository<CoreBranch> branchRepository,
            IRepository<HrmDefEmpType> defEmpTypeRepository,
            IRepository<HrmDefDesignation> designationRepository,
            IRepository<HrmDefDivision> defDivisionRepository,
            IRepository<HrmDefEmployeeStatus> employeeStatusRepository,
            IRepository<HrmDefGrade> gradeRepository,
            IRepository<HrmEmployeeOfficialInfo> employeeOfficialInfoRepository,
             IRepository<CorePeriodInfo> corePeriodInforepository,
            IRepository<HrmEisDefEmploymentNature> employmentNature)
        {
            _employeeOfficialInfoService = employeeOfficialInfoService;
            _commonService = commonService;
            _emplooyeeRepository = emplooyeeRepository;
            _companyRepository = companyRepository;
            _departmentRepository = departmentRepository;
            _shiftRepository = shiftRepository;
            _branchRepository = branchRepository;
            _defEmpTypeRepository = defEmpTypeRepository;
            _designationRepository = designationRepository;
            _divisionRepository = defDivisionRepository;
            _employeeStatusRepository = employeeStatusRepository;
            _gradeRepository = gradeRepository;
            this.employeeOfficialInfoRepository = employeeOfficialInfoRepository;
            _employmentNature = employmentNature;
            this.corePeriodInforepository = corePeriodInforepository;
        }
        #endregion

        #region Index / GetById / GetAll
        public async Task<IActionResult> Index(string code)
        {
            var hasPermission = await _employeeOfficialInfoService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }
            
            HrmEmployeeOfficialInfoPageViewModel model = new HrmEmployeeOfficialInfoPageViewModel();
            model.AddUrl = Url.Action(nameof(Setup));
            model.PageUrl = Url.Action(nameof(Index));

            return View(model);
        }
        #endregion

        #region Setup
        public async Task<ActionResult> Setup(string id)
        {
            var model = await _employeeOfficialInfoService.GetByIdAsync(id) ?? new HrmEmployeeOfficialInfoSetupViewModel();

            var companies = await _companyRepository.AllAsync();
            if(companies.Count() == 1)
            {
                model.OfficialInfoCompanyCode = companies.FirstOrDefault().CompanyCode;
            }

            ViewBag.EmployeeDD = new SelectList(_employeeOfficialInfoService.EmployeeSelection(), "Code", "Name");
            ViewBag.CompanyDD = new SelectList(_companyRepository.All(), "CompanyCode", "CompanyName");
            ViewBag.DepartmentDD = new SelectList(_departmentRepository.All(), "DepartmentCode", "DepartmentName");
            ViewBag.EmployeeNatureDD = new SelectList(_employmentNature.All(), "EmploymentNatureId", "EmploymentNature");
            ViewBag.ShiftDD = new SelectList(_shiftRepository.All(), "ShiftCode", "ShiftName");
            ViewBag.EmployeeStatusDD = new SelectList(_employeeStatusRepository.All(), "EmployeeStatusId", "EmployeeStatus", "01");
            ViewBag.BranchDD = new SelectList(_branchRepository.All(), "BranchCode", "BranchName");
            ViewBag.DesignationDD = new SelectList(_designationRepository.All(), "DesignationCode", "DesignationName");
            ViewBag.GradeDD = new SelectList(_gradeRepository.All(), "GradeCode", "GradeName");
            ViewBag.SupervisorDD = new SelectList(_employeeOfficialInfoService.EmployeeSelection(), "Code", "Name");
            ViewBag.DivisionDD = new SelectList(_divisionRepository.All(), "DivisionCode", "DivisionName");
            ViewBag.EmployeeTypeDD = new SelectList(_defEmpTypeRepository.All(), "EmpTypeCode", "EmpTypeName");
            ViewBag.HeadOfDepartmentDD = new SelectList(_employeeOfficialInfoService.EmployeeSelection(), "Code", "Name");
            ViewBag.ProbationTimeDD = new SelectList(corePeriodInforepository.All(), "PeriodInfoId", "PeriodName", "04");

            ViewBag.LunchBillEligibleDD = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Yes", Value = "Yes"},
                new SelectListItem { Text = "No", Value = "No"}
            }, "Value", "Text");

            ViewBag.GovtHolidayEligibleDD = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Yes", Value = "Yes"},
                new SelectListItem { Text = "No", Value = "No"}
            }, "Value", "Text");

            ViewBag.OverTimeEligibleDD = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "50%", Value = "50%"},
                new SelectListItem { Text = "60%", Value = "60%"}
            }, "Value", "Text");

            ViewBag.AttendanceBonusEligibleDD = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "Yes", Value = "Yes"},
                new SelectListItem { Text = "No", Value = "No"}
            }, "Value", "Text");

            ViewBag.ExtraDutyEligibleDD = new SelectList(new List<SelectListItem>
            {
                new SelectListItem { Text = "40%", Value = "40%"},
                new SelectListItem { Text = "50%", Value = "50%"},
                new SelectListItem { Text = "60%", Value = "60%"}
            }, "Value", "Text");
            
            model.AddUrl = Url.Action(nameof(Setup));

            if(model.EmployeeId == null)
            {
                ViewBag.IsEditMode = false;
            }
            else
            {
                ViewBag.IsEditMode = true;
            }

            return View(model);

        }

        #endregion

        #region GetEmployeeDetailsByCode for EmployeeId Dropdown

        [HttpGet]
        public async Task<IActionResult> GetEmployeeDetailsByCode(string code)
        {
            // var employee = await _employeeOfficialInfoService.GetEmployeeDetailsByCode(code);

            var employee = await AllInfoEmployeeById(code);

            var autoId = employeeOfficialInfoRepository.All().Where(x => x.EmployeeId == code).Select(x => x.AutoId).FirstOrDefault();


            if (employee != null)
            {
                var isExist = await _employeeOfficialInfoService.IsExistsByCode(code);
                if (isExist)
                {
                    return Json(new {
                        data = employee,
                        isSuccess = true, 
                        autoId = autoId,    
                        message = "Already Exists!" 
                    });
                }

                return Json(new
                {
                    data = employee,
                    autoId = autoId,
                });
            }
            else
            {
                return Json(null);
            }
        }

        #endregion

        #region GetByIdAsync
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            var model = await _employeeOfficialInfoService.GetByIdAsync(id) ?? new HrmEmployeeOfficialInfoSetupViewModel();


         

            var companies = _companyRepository.All();

            if (companies.Count() == 1)
            {
                model.OfficialInfoCompanyCode = companies.First().CompanyCode;
            }
            else
            {
                model.OfficialInfoCompanyCode = null;
            }
            
            model.AddUrl = Url.Action(nameof(Setup));
            if (model.EmployeeId == null)
            {
                ViewBag.IsEditMode = false;
            }
            else
            {
                ViewBag.IsEditMode = true;
            }

            return View(model);
        }

        #endregion

        #region Create
        [HttpPost]
        public async Task<IActionResult> Setup(HrmEmployeeOfficialInfoSetupViewModel model)
        {
            try
            {
                model.ToAudit(LoginInfo, model.AutoId > 0);
                model.CompanyCode = LoginInfo.CompanyCode;
                if (model.AutoId == 0)
                {
                    var isExists = await _employeeOfficialInfoService.IsExistsByCode(model.EmployeeId);
                    if (isExists)
                    {
                        return Json(new { isSuccess = true, message = "Already Exists!", isDuplicate = true });
                    }

                    var hasSavePermission = await _employeeOfficialInfoService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await _employeeOfficialInfoService.SaveAsync(model);
                        return Json(new { isSuccess = true, message = "Saved Successfully.", redirectUrl = Url.Action("Index", "OfficialInfo"), lastCode = model.EmployeeId });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {
                    var hasUpdatePermission = await _employeeOfficialInfoService.UpdatePermissionAsync(LoginInfo.AccessCode);  
                    if (hasUpdatePermission)
                    {
                        await _employeeOfficialInfoService.UpdateAsync(model);
                        return Json(new { isSuccess = true, message = "Updated Successfully.", redirectUrl = Url.Action("Index", "OfficialInfo"), lastCode = model.EmployeeId });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to update.", redirectUrl = Url.Action("Index", "OfficialInfo"), noUpdatePermission = true });
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        #endregion

        #region GetAll
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var listData = await _employeeOfficialInfoService.GetAllAsync();
                return PartialView("_Grid", listData);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
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

            var hasPermission = await _employeeOfficialInfoService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { isSuccess = false, message = "You have no access." });
            }

            DeleteHistoryViewModel model = new DeleteHistoryViewModel();
            model.ToAudit(LoginInfo);
            model.CompanyCode = LoginInfo.CompanyCode;
            

            bool success = await _employeeOfficialInfoService.DeleteTab(ids, model);
            if (success)
            {
                return Json(new { isSuccess = true, message = "Successfully Deleted" });
            }
            else
            {
                return Json(new { isSuccess = false, message = "Deletion failed. Some entities may still exists." });
            }
        }
        #endregion

        #region IsExistsByCode / Duplicate check by Code
        public async Task<IActionResult> IsExistsByCode(string code)
        {
            var isExist = await _employeeOfficialInfoService.IsExistsByCode(code);

            if (isExist)
            {
                return Json(new { isSuccess = true, message = "Already Exists!" });
            }
            return Json(new { isSuccess = false });
        }
        #endregion

        #region GetCalendarData
        public async Task<IActionResult> GetCalendarData(int year)
        {
            var data = await _employeeOfficialInfoService.GetHolidayAndWeekendAsync(year);
            return Json(data);
        }

        #endregion

    }
}
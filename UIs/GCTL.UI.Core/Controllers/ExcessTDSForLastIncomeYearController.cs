using GCTL.Core.Data;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.ExcessTDSForLastIncomeYear;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.ExcessTDSForLastIncomeYear;
using GCTL.Service.HrmEmployees2;
using GCTL.UI.Core.ViewModels.ExcessTDSForLastIncomeYear;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GCTL.UI.Core.Controllers
{
    public class ExcessTDSForLastIncomeYearController : BaseController
    {
        #region Service & Repository
        public readonly IExcessTDSForLastIncomeYearService excessTDSForLastIncomeYearService;
        private readonly ICommonService commonService;
        private readonly IRepository<CoreCompany> coreCompanyRepository;
        private readonly IRepository<CoreBranch> coreBranchRepository;
        private readonly IRepository<HrmEmployee> hrmEmployee;
        private readonly IHrmEmployee2Service hrmEmployee2;
        private readonly IRepository<HrmDefDepartment> departmentRepository;
        private readonly IRepository<HrmDefDesignation> designationRepository;
        private readonly IRepository<HrmEmployeeOfficialInfo> employeeOfficialInfoRepository;
        private readonly IRepository<HrmDefEmpType> employeeEmpTypeRepository;
        private readonly IRepository<HrmDefEmployeeStatus> employeeEmpStatusRepository;
        private readonly IRepository<HrmEisDefEmploymentNature> employmentNatureRepository;
        private readonly IRepository<AccFinancialYear> accFinancialYearRepository;

        string strMaxNO = string.Empty;

        public ExcessTDSForLastIncomeYearController(
            IExcessTDSForLastIncomeYearService excessTDSForLastIncomeYearService,
            ICommonService commonService, 
            IRepository<CoreCompany> coreCompanyRepository, 
            IRepository<CoreBranch> coreBranchRepository, 
            IRepository<HrmEmployee> hrmEmployee, 
            IHrmEmployee2Service hrmEmployee2, 
            IRepository<HrmDefDepartment> departmentRepository,
            IRepository<HrmDefDesignation> designationRepository,
            IRepository<HrmEmployeeOfficialInfo> employeeOfficialInfoRepository,
            IRepository<HrmDefEmpType> employeeEmpTypeRepository, 
            IRepository<HrmDefEmployeeStatus> employeeEmpStatusRepository, 
            IRepository<HrmEisDefEmploymentNature> employmentNatureRepository,
            IRepository<AccFinancialYear> accFinancialYearRepository

            )
        {
            this.excessTDSForLastIncomeYearService = excessTDSForLastIncomeYearService;
            this.commonService = commonService;
            this.coreCompanyRepository = coreCompanyRepository;
            this.coreBranchRepository = coreBranchRepository;
            this.hrmEmployee = hrmEmployee;
            this.hrmEmployee2 = hrmEmployee2;
            this.departmentRepository = departmentRepository;
            this.designationRepository = designationRepository;
            this.employeeOfficialInfoRepository = employeeOfficialInfoRepository;
            this.employeeEmpTypeRepository = employeeEmpTypeRepository;
            this.employeeEmpStatusRepository = employeeEmpStatusRepository;
            this.employmentNatureRepository = employmentNatureRepository;
            this.accFinancialYearRepository = accFinancialYearRepository;
        }

        #endregion

        #region Index

        public async Task<IActionResult> Index(bool child = false)
        {
            ExcessTDSForLastIncomeYearPageViewModel model = new ExcessTDSForLastIncomeYearPageViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };

            var list = await excessTDSForLastIncomeYearService.GetAllAsync();
            model.ExcessTDSForLastIncomeYearList2 = list ?? new List<ExcessTDSForLastIncomeYearSetupViewModel>();

            var companies = coreCompanyRepository.All().ToList();
            string selectedCompanyCode = companies.Count == 1 ? companies.First().CompanyCode : null;

            commonService.FindMaxNo(ref strMaxNO, "ETDSLIYID", "HRM_PAY_ExcessTDSForLastIncomeYearEntry", 8);

            ViewBag.FinancialDD = new SelectList(accFinancialYearRepository.All(), "FinancialCodeNo", "Name");

            DateTime today = DateTime.Now;

            var currentYear = accFinancialYearRepository.All().FirstOrDefault(x => x.StartDate <= today && x.EndDate >= today);

            // Fallback: if no match is found, select the most recent financial year
            if (currentYear == null)
            {
                currentYear = accFinancialYearRepository.All().OrderByDescending(x => x.EndDate).FirstOrDefault(); // Assumes the most recent ends latest
            }

            // Now build the SelectList
            ViewBag.FinancialDD = new SelectList(
                accFinancialYearRepository.All().Select(x => new { x.FinancialCodeNo, x.Name }),
                "FinancialCodeNo",
                "Name",
                currentYear?.FinancialCodeNo // selected value (can still be null)
            );

            model.Setup = new ExcessTDSForLastIncomeYearSetupViewModel
            {
                Etdsliyid = strMaxNO,

            };

            if (child)
                return PartialView(model);

            return View(model);
        }

        #endregion

        #region Setup

        [HttpGet]
        public async Task<IActionResult> GetById(string code)
        {
            var result = await excessTDSForLastIncomeYearService.GetByIdAsync(code);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeById(string employeeId)
        {
            var res = await excessTDSForLastIncomeYearService.GetPopulateEmployee(employeeId);
            return Json(new { data = res });
        }

        public async Task<IActionResult> Setup(string id)
        {
            ExcessTDSForLastIncomeYearSetupViewModel model = new ExcessTDSForLastIncomeYearSetupViewModel();

            var companies = coreCompanyRepository.All().ToList();
            string selectedCompanyCode = companies.Count == 1 ? companies.First().CompanyCode : null;
            commonService.FindMaxNo(ref strMaxNO, "ETDSLIYID", "HRM_PAY_ExcessTDSForLastIncomeYearEntry", 8);

            //ViewBag.FinancialDD = new SelectList(accFinancialYearRepository.All(), "FinancialCodeNo", "Name");

            DateTime today = DateTime.Now;

            var currentYear = accFinancialYearRepository.All()
                .FirstOrDefault(x => x.StartDate <= today && x.EndDate >= today);

            // Fallback: if no match is found, select the most recent financial year
            if (currentYear == null)
            {
                currentYear = accFinancialYearRepository.All()
                    .OrderByDescending(x => x.EndDate) // Assumes the most recent ends latest
                    .FirstOrDefault();
            }

            // Now build the SelectList
            ViewBag.FinancialDD = new SelectList(
                accFinancialYearRepository.All().Select(x => new { x.FinancialCodeNo, x.Name }),
                "FinancialCodeNo",
                "Name",
                currentYear?.FinancialCodeNo // selected value (can still be null)
            );


            if (!string.IsNullOrEmpty(id))
            {
                model = await excessTDSForLastIncomeYearService.GetByIdAsync(id);
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {
                model.Etdsliyid = strMaxNO;
            }

            commonService.FindMaxNo(ref strMaxNO, "ETDSLIYID", "HRM_PAY_ExcessTDSForLastIncomeYearEntry", 8);

            return PartialView($"_{nameof(Setup)}", model);
        }

        #endregion

        #region Post Update    

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Setup(ExcessTDSForLastIncomeYearSetupViewModel modelVM)
        {
            try
            {
                if (modelVM?.Tdsamount == null) modelVM.Tdsamount = 0;

                if (!ModelState.IsValid)
                {
                    var errorMessage = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                    return Json(new { isSuccess = false, message = errorMessage });
                }

                modelVM.ToAudit(LoginInfo, modelVM.AutoId > 0);
                if (modelVM.AutoId == 0)
                {
                    var hasSavePermission = await excessTDSForLastIncomeYearService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        if (await excessTDSForLastIncomeYearService.IsExistAsync(modelVM.EmployeeId, modelVM.FinancialCodeNo, (DateTime)modelVM.EffectiveDate, modelVM.Tdsamount))
                        {
                            return Json(new { isSuccess = false, message = $"Already Exists!", isDuplicate = true });
                        }

                        await excessTDSForLastIncomeYearService.SaveAsync(modelVM, LoginInfo.CompanyCode);
                        commonService.FindMaxNo(ref strMaxNO, "ETDSLIYID", "HRM_PAY_ExcessTDSForLastIncomeYearEntry", 8);
                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = strMaxNO });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
                    }
                }
                else
                {
                    var hasUpdatePermission = await excessTDSForLastIncomeYearService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        var result = await excessTDSForLastIncomeYearService.UpdateAsync(modelVM);
                        commonService.FindMaxNo(ref strMaxNO, "ETDSLIYID", "HRM_PAY_ExcessTDSForLastIncomeYearEntry", 8);
                        return Json(new { isSuccess = result, message = "Updated Successfully.", lastCode = strMaxNO });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });
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

        #region Delete

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] List<string> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest(new { success = false, message = "No IDs provided for delete." });
            }

            var hasPermission = await excessTDSForLastIncomeYearService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await excessTDSForLastIncomeYearService.DeleteTab(ids);
            if (success)
            {
                commonService.FindMaxNo(ref strMaxNO, "ETDSLIYID", "HRM_PAY_ExcessTDSForLastIncomeYearEntry", 8);
                return Json(new { success = true, message = "Deleted Successfully.", lastCode = strMaxNO });
            }
            else
            {
                return Json(new { success = false, message = "Deletion failed. Some entities may still exists." });
            }
        }

        #endregion

        #region TabeleLodaing

        [HttpGet]
        public async Task<IActionResult> GetTableDataSalary()
        {
            try
            {
                var list = await excessTDSForLastIncomeYearService.GetAllAsync();
                return PartialView("_Grid", list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> getResult([FromBody] ExcessTDSForLastIncomeYearFilterDto filter)
        {

            var result = await excessTDSForLastIncomeYearService.GetFilterDataAsync(filter);
            return Json(new { data = result });
        }
        #endregion

        #region GenerateNewId

        public async Task<IActionResult> GenerateNewId()
        {
            await Task.Delay(100);
            commonService.FindMaxNo(ref strMaxNO, "ETDSLIYID", "HRM_PAY_ExcessTDSForLastIncomeYearEntry", 8);
            var newId = strMaxNO.ToString();
            return Json(newId);
        }

        #endregion

        #region Chake Degian

        //public IActionResult Index()
        //{
        //    ExcessTDSForLastIncomeYearPageViewModel model = new ExcessTDSForLastIncomeYearPageViewModel
        //    {
        //        Setup = new ExcessTDSForLastIncomeYearSetupViewModel()
        //    };
        //    return View(model);
        //}

        #endregion

    }
}

using GCTL.Core.Data;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.ExperianceInfos;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.ExperianceInfos;
using GCTL.Service.HrmEmployees2;
using GCTL.UI.Core.ViewModels.ExperianceInfos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GCTL.UI.Core.Controllers
{
    public class ExperianceInfosController: BaseController
    {
        #region Service & Rep
        private readonly IExperianceInfosService experianceInfosService;
        private readonly ICommonService commonService;
        private readonly IRepository<HrmDefExamGroupInfo> hrmExamGroupInfo;
        private readonly IRepository<CoreCompany> coreCompanyRepository;
        private readonly IHrmEmployee2Service hrmEmployee;
        private readonly IRepository<HrmDefDepartment> departmentRepository;
        private readonly IRepository<HrmDefDesignation> designationRepository;
        private readonly IRepository<HrmDefCompanyInfo> companyRepository;
        private readonly IRepository<HrmDefEmpType> repository;


        string strMaxNO = string.Empty;

        public ExperianceInfosController(IExperianceInfosService experianceInfosService, 
            ICommonService commonService, 
            IRepository<HrmDefExamGroupInfo> hrmExamGroupInfo,
            IRepository<CoreCompany> coreCompanyRepository,
            IHrmEmployee2Service hrmEmployee,
            IRepository<HrmDefDepartment> departmentRepository, 
            IRepository<HrmDefDesignation> designationRepository, 
            IRepository<HrmDefCompanyInfo> companyRepository,
            IRepository<HrmDefEmpType> repository

            )
        {
            this.experianceInfosService = experianceInfosService;
            this.commonService = commonService;
            this.hrmExamGroupInfo = hrmExamGroupInfo;
            this.coreCompanyRepository = coreCompanyRepository;
            this.hrmEmployee = hrmEmployee;
            this.departmentRepository = departmentRepository;
            this.designationRepository = designationRepository;
            this.companyRepository = companyRepository;
            this.repository = repository;
           
        }

        #endregion

        #region Index

        //public async Task<ActionResult> Grid()
        //{
        //    var result = await experianceInfosService.GetAllAsync();
        //    return Json(new { data = result });
        //}


        public async Task<IActionResult> Index(bool child = false)
        {
            ExperianceInfosPageViewModel model = new ExperianceInfosPageViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };


            var companies = coreCompanyRepository.All().ToList();
            string selectedCompanyCode = companies.Count == 1 ? companies.First().CompanyCode : null;

            ViewBag.CoreCompanyDD = new SelectList(companies, "CompanyCode", "CompanyName", selectedCompanyCode);
            commonService.FindMaxNo(ref strMaxNO, "EmpExpID", "HRM_EmployeeExp", 4);
            ViewBag.DeptDD = new SelectList(departmentRepository.All(), "DepartmentCode", "DepartmentName");
            ViewBag.DesigDD = new SelectList(designationRepository.All(), "DesignationCode", "DesignationName");
            ViewBag.EmployeeDD = new SelectList(await hrmEmployee.GetEmployeeDropSelections(), "Code", "Name");
            ViewBag.CompanyDD = new SelectList(companyRepository.All(), "CompanyNameId", "CompanyName");
            ViewBag.EmpDD = new SelectList(repository.All(), "EmpTypeCode", "EmpTypeName");

            //var List = await experianceInfosService.GetAllAsync();
            //if (List == null || !List.Any())
            //{
            //    ViewData["List"] = new List<ExperianceInfosSetupViewModel>();
            //    ViewBag.Message = "No Data Available";
            //}
            //else
            //{
            //    ViewData["List"] = List;
            //}
            
            //var list = await experianceInfosService.GetAllAsync();

            model.Setup = new ExperianceInfosSetupViewModel
            {
                EmpExpId = strMaxNO,

            };

            if (child)
                return PartialView(model);

            return View(model);
        }

        #endregion

        #region Setup

        public async Task<IActionResult> Setup(string id)
        {
            ExperianceInfosSetupViewModel model = new ExperianceInfosSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "EmpExpID", "HRM_EmployeeExp", 4);

            if (!string.IsNullOrEmpty(id))
            {

                model = await experianceInfosService.GetByIdAsync(id);
                if (model == null)
                {
                    return NotFound();
                }
            }
            else
            {

                model.EmpExpId = strMaxNO;
            }

            var companies = coreCompanyRepository.All().ToList();
            string selectedCompanyCode = companies.Count == 1 ? companies.First().CompanyCode : null;

            ViewBag.CoreCompanyDD = new SelectList(companies, "CompanyCode", "CompanyName", selectedCompanyCode);

            ViewBag.DeptDD = new SelectList(departmentRepository.All(), "DepartmentCode", "DepartmentName");
            ViewBag.DesigDD = new SelectList(designationRepository.All(), "DesignationCode", "DesignationName");
            ViewBag.EmployeeDD = new SelectList(await hrmEmployee.GetEmployeeDropSelections(), "Code", "Name");
            ViewBag.CompanyDD = new SelectList(companyRepository.All(), "CompanyNameId", "CompanyName");
            ViewBag.EmpDD = new SelectList(repository.All(), "EmpTypeCode", "EmpTypeName");
            return PartialView($"_{nameof(Setup)}", model);
        }

        #endregion

        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(ExperianceInfosSetupViewModel modelVM)
        {
            try
            {
                if (await experianceInfosService.IsExistAsync(modelVM.EmployeeId, modelVM.EmpExpId, modelVM.CompanyNameId))
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
                    var hasSavePermission = await experianceInfosService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await experianceInfosService.SaveAsync(modelVM, LoginInfo.CompanyCode);
                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.EmpExpId });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {
                    var hasUpdatePermission = await experianceInfosService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await experianceInfosService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.EmpExpId });
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
        public async Task<JsonResult> CheckAvailability(string CompanyNameId, string employeeCode, string code)
        {
            if (await experianceInfosService.IsExistAsync(employeeCode, code, CompanyNameId))
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

            var hasPermission = await experianceInfosService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await experianceInfosService.DeleteTab(ids);
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

        #region TabeleLodaing

        [HttpGet]
        public async Task<IActionResult> GetTableData(string employeeId)
        {
            try
            {
                var list = await experianceInfosService.GetAllAsync(employeeId);
                return PartialView("_Grid", list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        #endregion

        #region Company & Employee

        [HttpGet]
        public async Task<IActionResult> GetEmployeeDetailsByComapnyCode(string companyCode)
        {
            var result = await experianceInfosService.GetEmployeeByCompanyCode(companyCode);
            if (result == null)
            {
                return Json(new { error = "No data found" });
            }
            return Json(result);
        }


        [HttpGet]
        public async Task<IActionResult> GetEmployeeNameDesDeptByCode(string employeeId)
        {
            var result = await experianceInfosService.GetEmployeeNameDesDeptByCode(employeeId);
            if (result == null)
            {
                return Json(new { error = "No data found" });
            }
            return Json(result);
        }

        #endregion

        #region GetCalendarData

        public async Task<IActionResult> GetCalendarData(int year)
        {
            var data = await experianceInfosService.GetHolidayAndWeekendAsync(year);
            return Json(data);
        }

        #endregion

    }
}
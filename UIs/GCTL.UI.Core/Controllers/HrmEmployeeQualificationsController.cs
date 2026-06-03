using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.HrmEmployeeQualifications;
using GCTL.Service.Common;
using GCTL.Service.Companies;
using GCTL.Service.CourseTitle;
using GCTL.Service.HrmDefDegrees;
using GCTL.Service.HrmDefInstitutes;
using GCTL.Service.HrmEmployeeQualifications;
using GCTL.Service.HrmEmployees2;
using GCTL.UI.Core.ViewModels.HrmEmployeeQualifications;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GCTL.UI.Core.Controllers
{
    public class HrmEmployeeQualificationsController : BaseController
    {
        private readonly IHrmEmployeeQualificationsService hrmEmployeeQualificationsService;
        private readonly ICompanyService companyService;
        private readonly IHrmEmployee2Service employeeService;
        private readonly IHrmDefInstitutesService hrmDefInstitutesService;
        private readonly IHrmDefDegreesService hrmDefDegreesService;
        private readonly ICourseTitleService courseTitleService;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;
        private const string TableName = "HRM_EmployeeQualification";
        private const string ColumnName = "EmpQualificationID";
        public HrmEmployeeQualificationsController(
            IHrmEmployeeQualificationsService hrmEmployeeQualificationsService,
            ICommonService commonService,
            ICompanyService companyService,
            IHrmEmployee2Service employeeService, 
            IHrmDefInstitutesService hrmDefInstitutesService, 
            ICourseTitleService courseTitleService,
            IHrmDefDegreesService hrmDefDegreesService
            )
        {
            this.hrmEmployeeQualificationsService = hrmEmployeeQualificationsService;
            this.companyService = companyService;
            this.employeeService = employeeService;
            this.hrmDefInstitutesService = hrmDefInstitutesService;
            this.hrmDefDegreesService = hrmDefDegreesService;
            this.courseTitleService = courseTitleService;
            this.commonService = commonService;
        }

        public async Task<IActionResult> Index(bool child = false)
        {
            HrmEmployeeQualificationsPageViewModel model = new HrmEmployeeQualificationsPageViewModel()
            {
                PageUrl = Url.Action(nameof(Index)),
            };
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 6);
            var companies = await companyService.GetCompanyDropDown();
            var selectedCompanyCode = companies.Count() == 1 ? companies.First().Code : null;
            ViewBag.CoreCompanyDD = new SelectList(companies, "Code", "Name", selectedCompanyCode);
            ViewBag.InstituteDD = new SelectList(await hrmDefInstitutesService.SelectionHrmDefInstituteTypeAsync(), "Code", "Name");
            ViewBag.empDD = new SelectList(await employeeService.GetEmployeeDropSelections(), "Code", "Name");
            ViewBag.DegreeDD = new SelectList(await courseTitleService.SelectionCourseTitleAsync(), "Code", "Name");

            model.Setup = new HrmEmployeeQualificationsSetupViewModel
            {
                EmpQualificationId = strMaxNO,

            };
            if (child) return PartialView(model);
            return View(model);
        }

        public async Task<IActionResult> Setup(string id)
        {
            HrmEmployeeQualificationsSetupViewModel model = new HrmEmployeeQualificationsSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 6);

            if (!string.IsNullOrEmpty(id))
            {

                model = await hrmEmployeeQualificationsService.GetByIdAsync(id)
;
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.EmpQualificationId = strMaxNO;
            }

            var companies = await companyService.GetCompanyDropDown();
            var selectedCompanyCode = companies.Count() == 1 ? companies.First().Code : null;
            ViewBag.CoreCompanyDD = new SelectList(companies, "Code", "Name", selectedCompanyCode);
            ViewBag.InstituteDD = new SelectList(await hrmDefInstitutesService.SelectionHrmDefInstituteTypeAsync(), "Code", "Name");
            ViewBag.empDD = new SelectList(await employeeService.GetEmployeeDropSelections(), "Code", "Name");
            ViewBag.DegreeDD = new SelectList(await courseTitleService.SelectionCourseTitleAsync(), "Code", "Name");

            return PartialView($"_{nameof(Setup)}", model);
        }

        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(HrmEmployeeQualificationsSetupViewModel modelVM)
        {
            try
            {

                if (await hrmEmployeeQualificationsService.IsExistAsync(modelVM.EmpQualificationId, modelVM.EmployeeId, modelVM.CourseCode, modelVM.CourseTitleCode))
                {
                    return Json(new { isSuccess = false, message = "Already Exists!" });
                }

                if (!ModelState.IsValid)
                {

                    var errorMessage = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                    return Json(new { isSuccess = false, message = errorMessage });
                }


                modelVM.ToAudit(LoginInfo, modelVM.AutoId > 0);
                if (modelVM.AutoId == 0)
                {
                    var hasSavePermission = await hrmEmployeeQualificationsService.SavePermissonAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await hrmEmployeeQualificationsService.SaveAsync(modelVM, LoginInfo.CompanyCode);
                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.EmpQualificationId });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await hrmEmployeeQualificationsService.UpdateParmissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await hrmEmployeeQualificationsService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.EmpQualificationId });
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
        public async Task<JsonResult> CheckAvailability(string code, string employeeCode, string courseTypeId, string couresetitleID)
        {

            if (await hrmEmployeeQualificationsService.IsExistAsync(code, employeeCode, courseTypeId, couresetitleID))
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

            var hasPermission = await hrmEmployeeQualificationsService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await hrmEmployeeQualificationsService.DeleteTab(ids);
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

                    return PartialView("_Grid", new List<HrmEmployeeQualificationsSetupViewModel>());
                }
                else
                {
                    var list = await hrmEmployeeQualificationsService.GetAllAsync(employeeId);
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
            var result = await hrmEmployeeQualificationsService.GetEmployeeByCompanyCode(companyCode);
            if (result == null)
            {
                return Json(new { error = "No data found" });
            }
            return Json(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetEmployeeNameDesDeptByCode(string employeeId)
        {
            var result = await hrmEmployeeQualificationsService.GetEmployeeNameDesDeptByCode(employeeId);
            if (result == null)
            {
                return Json(new { error = "No data found" });
            }
            return Json(result);
        }

        #endregion
    }
}

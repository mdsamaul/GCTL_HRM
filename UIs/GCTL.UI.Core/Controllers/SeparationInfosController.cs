using GCTL.Core.Data;
using GCTL.Core.ViewModels.SeparationInfos;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.SeparationInfos;
using GCTL.UI.Core.ViewModels.SeparationInfos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GCTL.Core.Helpers;
using GCTL.Service.HrmEmployees2;

namespace GCTL.UI.Core.Controllers
{
    public class SeparationInfosController : BaseController
    {
        private readonly ISeparationInfosService SeparationInfosService;
        private readonly ICommonService commonService;
        private readonly IRepository<CoreCompany> coreCompanyRepository;
        private readonly IHrmEmployee2Service hrmEmployee;
        private readonly IRepository<HrmDefDepartment> departmentRepository;
        private readonly IRepository<HrmDefDesignation> designationRepository;
        private readonly IRepository<HrmDefSeparationType> separationTypeRepository;
        private readonly IRepository<HrmEmployeeOfficialInfo> hrmEmpOffialInfo;

        string strMaxNO = string.Empty;

        public SeparationInfosController(
          ISeparationInfosService SeparationInfosService,
          ICommonService commonService,
          IRepository<CoreCompany> coreCompanyRepository,
          IHrmEmployee2Service hrmEmployee,
          IRepository<HrmDefDepartment> departmentRepository,
          IRepository<HrmDefDesignation> designationRepository,
          IRepository<HrmDefSeparationType> separationTypeRepository,
          IRepository<HrmEmployeeOfficialInfo> hrmEmpOffialInfo

          )
        {
            this.SeparationInfosService = SeparationInfosService;
            this.commonService = commonService;
            this.coreCompanyRepository = coreCompanyRepository;
            this.hrmEmployee = hrmEmployee;
            this.departmentRepository = departmentRepository;
            this.designationRepository = designationRepository;
            this.separationTypeRepository = separationTypeRepository;
            this.hrmEmpOffialInfo = hrmEmpOffialInfo;
        }

        public async Task<ActionResult> Grid()
        {
            var result = await SeparationInfosService.GetAllAsync();
            return Json(new { data = result });
        }

        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await SeparationInfosService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new SeparationInfosPageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {
                var companies = coreCompanyRepository.All().ToList();
                string selectedCompanyCode = companies.Count == 1 ? companies.First().CompanyCode : null;

                ViewBag.CoreCompanyDD = new SelectList(companies, "CompanyCode", "CompanyName", selectedCompanyCode);
                commonService.FindMaxNo(ref strMaxNO, "SeparationId", "HRM_Separation", 4);
                ViewBag.DeptDD = new SelectList(departmentRepository.All(), "DepartmentCode", "DepartmentName");
                ViewBag.DesigDD = new SelectList(designationRepository.All(), "DesignationCode", "DesignationName");
                ViewBag.EmployeeDD = new SelectList(await hrmEmployee.GetEmployeeDropSelections(), "Code", "Name");
                ViewBag.SepratDD = new SelectList(separationTypeRepository.All(), "SeparationTypeId", "SeparationType");
                var list = await SeparationInfosService.GetAllAsync();
                model.TableListData = list ?? new List<SeparationInfosSetupViewModel>();

                model.Setup = new SeparationInfosSetupViewModel
                {
                    SeparationId = strMaxNO
                };

            }
            catch (Exception ex)
            {

                model.TableListData = new List<SeparationInfosSetupViewModel>();
                model.Setup = new SeparationInfosSetupViewModel();
                Console.WriteLine("Error" + ex.Message);
            }


            if (child)
                return PartialView(model);

            return View(model);
        }

        public async Task<IActionResult> Setup(string id)
        {
            SeparationInfosSetupViewModel model = new SeparationInfosSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "SeparationId", "HRM_Separation", 4);

            if (!string.IsNullOrEmpty(id))
            {

                model = await SeparationInfosService.GetByIdAsync(id);
                if (model == null)
                {
                    return NotFound();
                }
            }
            else
            {

                model.SeparationId = strMaxNO;
            }
           
            var companies = coreCompanyRepository.All().ToList();
            string selectedCompanyCode = companies.Count == 1 ? companies.First().CompanyCode : null;

            ViewBag.CoreCompanyDD = new SelectList(companies, "CompanyCode", "CompanyName", selectedCompanyCode);
            ViewBag.DeptDD = new SelectList(departmentRepository.All(), "DepartmentCode", "DepartmentName");
            ViewBag.DesigDD = new SelectList(designationRepository.All(), "DesignationCode", "DesignationName");
            ViewBag.EmployeeDD = new SelectList(await hrmEmployee.GetEmployeeDropSelections(), "Code", "Name");
            ViewBag.SepratDD = new SelectList(separationTypeRepository.All(), "SeparationTypeId", "SeparationType");
            return PartialView($"_{nameof(Setup)}", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeNameDesDeptByCode(string employeeId)
        {
            if (string.IsNullOrEmpty(employeeId))
            {
                return Json(new { success = false, message = "Invalid employee ID" });
            }

            var result = await SeparationInfosService.GetEmployeeByCode(employeeId);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeDetailsByComapnyCode(string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
            {
                return Json(new { success = false, message = "Invalid company code" });
            }

            var result = await SeparationInfosService.GetComapnyByCode(companyCode);
            return Json(result);
        }


        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(SeparationInfosSetupViewModel modelVM)
        {
            try
            {

                if (await SeparationInfosService.IsExistAsync(modelVM.EmployeeId, modelVM.SeparationId, modelVM.CompanyCode))
                {
                    return Json(new { isSuccess = false, message = $"Already Exists!", isDuplicate = true });
                }



                if (!ModelState.IsValid)
                {

                    var errorMessage = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                    return Json(new { isSuccess = false, message = errorMessage });
                }

                modelVM.ToAudit(LoginInfo, modelVM.SeparationCode > 0);
                if (modelVM.SeparationCode == 0)
                {
                    var hasSavePermission = await SeparationInfosService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await SeparationInfosService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Data Saved Successfully.", lastCode = modelVM.SeparationId });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await SeparationInfosService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await SeparationInfosService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Data Updated Successfully.", lastCode = modelVM.SeparationId });
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
        public async Task<JsonResult> CheckAvailability(string CompanyCode, string employeeCode, string code)
        {

            if (await SeparationInfosService.IsExistAsync(employeeCode, code, CompanyCode))
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

            var hasPermission = await SeparationInfosService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await SeparationInfosService.DeleteTab(ids);
            if (success)
            {
                return Json(new { success = true, message = "Data Deleted Successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Deletion failed. Some entities may still exists." });
            }
        }

        #region TabeleLodaing

        [HttpGet]
        public async Task<IActionResult> GetTableData()
        {
            try
            {
                var list = await SeparationInfosService.GetAllAsync();
                return PartialView("_Grid", list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        #endregion

    }
}

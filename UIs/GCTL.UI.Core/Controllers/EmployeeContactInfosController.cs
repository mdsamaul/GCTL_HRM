using GCTL.Core.Data;
using GCTL.Core.ViewModels.EmployeeContactInfos;
using GCTL.Data.Models;
using GCTL.Core.Helpers;
using GCTL.Service.Common;
using GCTL.Service.EmployeeContactInfos;
using GCTL.UI.Core.ViewModels.EmployeeContactInfos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GCTL.Service.HrmEmployees2;



namespace GCTL.UI.Core.Controllers
{
    public class EmployeeContactInfosController : BaseController
    {
        #region Service & Rep
        private readonly IEmployeeContactInfoService employeeContactInfoService;
        private readonly ICommonService commonService;
        private readonly IRepository<CoreBranch> coreBranchRepository;
        private readonly IRepository<CoreCompany> coreCompanyRepository;
        private readonly IRepository<HrmDefRelationship> relationshipRepository;
        private readonly IRepository<HrmDefDistrict> districtRepository;
        private readonly IHrmEmployee2Service hrmEmployee;

        string strMaxNO = "";

        public EmployeeContactInfosController( 
            ICommonService commonService,
            IEmployeeContactInfoService employeeContactInfoService,

            IRepository<CoreBranch> coreBranchRepository,
            IRepository<CoreCompany> coreCompanyRepository,

            IHrmEmployee2Service hrmEmployee,
            IRepository<HrmDefRelationship> relationshipRepository,
            IRepository<HrmDefDistrict> districtRepository

            )
        {
            this.commonService = commonService;
            this.employeeContactInfoService = employeeContactInfoService;
            this.coreBranchRepository = coreBranchRepository;
            this.coreCompanyRepository = coreCompanyRepository;
            this.hrmEmployee = hrmEmployee;
            this.relationshipRepository = relationshipRepository;
            this.districtRepository = districtRepository;
        }

        #endregion

        #region Index

        public async Task<IActionResult> Index(string id)
        {
            var hasPermission = await employeeContactInfoService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }
            EmployeeContactInfosPageViewModel model = new EmployeeContactInfosPageViewModel();

            model.AddUrl = Url.Action(nameof(Setup));
            model.PageUrl = Url.Action(nameof(Index));
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeNameDesDeptByCode(string employeeId)
        {
            if (string.IsNullOrEmpty(employeeId))
            {
                return Json(new { success = false, message = "Invalid employee ID" });
            }

            var result = await employeeContactInfoService.GetEmployeeByCode(employeeId);
           
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployeeDetailsByComapnyCode(string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
            {
                return Json(new { success = false, message = "Invalid company code" });
            }

            var result = await employeeContactInfoService.GetComapnyByCode(companyCode);
            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetBranchByCode(string companyCode)
        {
            if (string.IsNullOrEmpty(companyCode))
            {
                return Json(new { success = false, message = "Invalid company code" });
            }

            var result = await employeeContactInfoService.GetComapnyByBranchCode(companyCode);
            return Json(result);
        }

        #endregion

        #region Setup

        public async Task<ActionResult> Setup(string id)
        {
            var model = await employeeContactInfoService.GetByIdAsync(id) ?? new EmployeeContactInfosSetupViewModel();

            var companies = coreCompanyRepository.All();

            if (companies.Count() == 1)
            {
                model.CompanyCode = companies.First().CompanyCode; // Set the selected value in the model
            }
            else if (id != null) 
            {
                model.CompanyCode = (await employeeContactInfoService.GetByIdAsync(id))?.CompanyCode;
            }
            else
            {
                model.CompanyCode = null;
            }

            ViewBag.RelationshipDD = new SelectList(relationshipRepository.All(), "RelationshipCode", "Relationship");
            ViewBag.CoreBranchDD = new SelectList(coreBranchRepository.All(), "BranchCode", "BranchName", model.BranchCode);
            ViewBag.CoreCompanyDD = new SelectList(coreCompanyRepository.All(), "CompanyCode", "CompanyName", model.CompanyCode);
            ViewBag.EmployeeDD = new SelectList(await hrmEmployee.GetEmployeeDropSelections(), "Code", "Name");
            ViewBag.DistrictDD = new SelectList(districtRepository.All(), "DistrictId", "District");

            model.AddUrl = Url.Action(nameof(Setup));
            if (model.EmpContactId == null)
            {
                model.EmployeeId = commonService.NextCode("EmpContactID", "HRM_EmployeeContactInfo", 4);
                ViewBag.IsEditMode = false;
            }
            else
            {
                ViewBag.IsEditMode = true;
            }
            return View(model);
        }

        #endregion

        #region Post Update 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Setup(EmployeeContactInfosSetupViewModel modelVM)
        {
            try
            {
                //if (!ModelState.IsValid)
                //{
                //    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                //    return Json(new { isSuccess = false, message = "Validation failed", errors });
                //}

                if (await employeeContactInfoService.IsExistByAsync(modelVM.EmployeeId, modelVM.EmpContactId))
                {
                    return Json(new { isSuccess = false, message = $"Already  Exists!", isDuplicate = true });
                }


                if (string.IsNullOrEmpty(modelVM.EmpContactId))
                {
                    modelVM.EmpContactId = commonService.NextCode("EmpContactID", "HRM_EmployeeContactInfo", 4);
                    //await employeeContactInfoService.GenerateNextCode();
                }


                modelVM.ToAudit(LoginInfo, modelVM.AutoId > 0);
                if (modelVM.AutoId == 0)
                {

                    var hasSavePermission = await employeeContactInfoService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await employeeContactInfoService.SaveAsync(modelVM, LoginInfo.CompanyCode);
                        //TempData["Data"] = "This is the data I want to share.";
                        //return RedirectToAction("Index", "HRM_EmployeeContactInfo");

                        return Json(new { isSuccess = true, message = "Saved Successfully", redirectUrl = Url.Action("Index", "EmployeeContactInfos"), lastCode = modelVM.EmpContactId });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save", noSavePermission = true });
                    }
                }
                else 
                {

                    var hasUpdatePermission = await employeeContactInfoService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await employeeContactInfoService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully", redirectUrl = Url.Action("Index", "EmployeeContactInfos"), lastCode = modelVM.EmpContactId });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to update", noUpdatePermission = true });
                    }
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        #endregion

        #region Delete

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] List<string> ids)
        {
            try
            {

                var hasPermission = await employeeContactInfoService.DeletePermissionAsync(LoginInfo.AccessCode);
                if (hasPermission)
                {

                    foreach (var id in ids)
                    {
                        var result = employeeContactInfoService.DeleteLeaveType(id);

                    }

                    return Json(new { isSuccess = true, message = "Deleted Successfully" });
                }
                else
                {

                    return Json(new { isSuccess = false, message = "You have no access" });
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error deleting leave type: {ex.Message}");

                return StatusCode(500, new { isSuccess = false, message = ex.Message });
            }
        }

        #endregion

        #region NeaxtCode
        [HttpGet]
        public async Task<IActionResult> GenerateNextCode()
        {
            var nextCode = await employeeContactInfoService.GenerateNextCode();
            return Json(nextCode);

        }
        #endregion

        #region TabeleLodaing

        [HttpGet]
        public async Task<IActionResult> GetTableData()
        {
            try
            {
                var list = await employeeContactInfoService.GetAllAsync();
                return PartialView("_Grid", list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        #endregion

        #region CheckAvailability
        [HttpPost] 
        public async Task<JsonResult> CheckAvailability(string code, string EmployeeId)
        {

            if (await employeeContactInfoService.IsExistByAsync(code,  EmployeeId))
            {
                return Json(new { isSuccess = true, message = $"Already exists!." });
            }

            return Json(new { isSuccess = false });
        }
        #endregion
    }
}

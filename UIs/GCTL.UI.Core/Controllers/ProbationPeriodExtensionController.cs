using GCTL.Core.Data;
using GCTL.Core.ViewModels.ProbationPeriodExtension;
using GCTL.Service.Common;
using GCTL.Core.Helpers;
using GCTL.Service.HrmEmployees2;
using GCTL.Service.ProbationPeriodExtension;
using GCTL.UI.Core.ViewModels.ProbationPeriodExtension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GCTL.Data.Models;

namespace GCTL.UI.Core.Controllers
{
    public class ProbationPeriodExtensionController : BaseController
    {
        #region Service & Repository
        private readonly IProbationPeriodExtensionService probationPeriodExtensionService;
        private readonly ICommonService commonService;
        private readonly IRepository<HrmDefExamGroupInfo> hrmExamGroupInfo;
        private readonly IRepository<CoreCompany> coreCompanyRepository;
        private readonly IHrmEmployee2Service hrmEmployee;
        private readonly IRepository<HrmDefDepartment> departmentRepository;
        private readonly IRepository<HrmDefDesignation> designationRepository;
        private readonly IRepository<HrmDefCompanyInfo> companyRepository;
        private readonly IRepository<CorePeriodInfo> corePeriodInforepository;

        string strMaxNO = string.Empty;

        public ProbationPeriodExtensionController(
            IProbationPeriodExtensionService probationPeriodExtensionService,
            ICommonService commonService,
            IRepository<HrmDefExamGroupInfo> hrmExamGroupInfo,
            IRepository<CoreCompany> coreCompanyRepository,
            IHrmEmployee2Service hrmEmployee,
            IRepository<HrmDefDepartment> departmentRepository,
            IRepository<HrmDefDesignation> designationRepository,
            IRepository<HrmDefCompanyInfo> companyRepository,
            IRepository<CorePeriodInfo> corePeriodInforepository

            )
        {
            this.probationPeriodExtensionService = probationPeriodExtensionService;
            this.commonService = commonService;
            this.hrmExamGroupInfo = hrmExamGroupInfo;
            this.coreCompanyRepository = coreCompanyRepository;
            this.hrmEmployee = hrmEmployee;
            this.departmentRepository = departmentRepository;
            this.designationRepository = designationRepository;
            this.companyRepository = companyRepository;
            this.corePeriodInforepository = corePeriodInforepository;
        }

        #endregion

        #region Index

        public async Task<IActionResult> Index(bool child = false)
        {
           // await Task.Delay(100);
            ProbationPeriodExtensionPageViewModel model = new ProbationPeriodExtensionPageViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };

            var list = await probationPeriodExtensionService.GetAllAsync();
            model.ProbationPeriodExtensionList2 = list ?? new List<ProbationPeriodExtensionGetAll>();

            commonService.FindMaxNo(ref strMaxNO, "PPEID", "HRM_Def_ProbationPeriodExtension", 8);

            ViewBag.ExtendedDD = new SelectList(corePeriodInforepository.All(), "PeriodInfoId", "PeriodName");

            model.Setup = new ProbationPeriodExtensionSetupViewModel
            {
                Ppeid = strMaxNO,
            };

            if (child)
                return PartialView(model);

            return View(model);
        }

        #endregion

        #region GetById

        [HttpGet]
        public async Task<IActionResult> GetById(string code)
        {
            var result = await probationPeriodExtensionService.GetByIdAsync(code);
            return Json(result);
        }

        #endregion

        #region Setup

        public async Task<IActionResult> Setup(string id)
        {
            ProbationPeriodExtensionSetupViewModel model = new ProbationPeriodExtensionSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "PPEID", "HRM_Def_ProbationPeriodExtension", 8);

            if (!string.IsNullOrEmpty(id))
            {
                model = await probationPeriodExtensionService.GetByIdAsync(id);
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {
                model.Ppeid = strMaxNO;
            }

            commonService.FindMaxNo(ref strMaxNO, "PPEID", "HRM_Def_ProbationPeriodExtension", 8);
            ViewBag.ExtendedDD = new SelectList(corePeriodInforepository.All(), "PeriodInfoId", "PeriodName");

            return PartialView($"_{nameof(Setup)}", model);
        }

        #endregion

        #region Post Update   

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Setup(ProbationPeriodExtensionSetupViewModel modelVM)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errorMessage = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                    return Json(new { isSuccess = false, message = errorMessage });
                }

                modelVM.ToAudit(LoginInfo, modelVM.AutoId > 0);
                if (modelVM.AutoId == 0)
                {
                    var hasSavePermission = await probationPeriodExtensionService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        if (await probationPeriodExtensionService.IsExistAsync(modelVM.EmployeeId, modelVM.ProbationPeriod))
                        {
                            return Json(new { isSuccess = false, message = $"Already Exists!", isDuplicate = true });
                        }

                        await probationPeriodExtensionService.SaveAsync(modelVM);
                        commonService.FindMaxNo(ref strMaxNO, "PPEID", "HRM_Def_ProbationPeriodExtension", 8);
                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = strMaxNO /*, employeeId = modelVM.EmployeeId, compCode = modelVM.CompanyCode*/});
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
                    }
                }
                else
                {
                    var hasUpdatePermission = await probationPeriodExtensionService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        var result = await probationPeriodExtensionService.UpdateAsync(modelVM);
                        commonService.FindMaxNo(ref strMaxNO, "PPEID", "HRM_Def_ProbationPeriodExtension", 8);
                        return Json(new { isSuccess = result, message = "Updated Successfully.", lastCode = strMaxNO /*, employeeId = modelVM.EmployeeId, compCode = modelVM.CompanyCode*/ });
                    }
                    else
                    {
                        //commonService.FindMaxNo(ref strMaxNO, "SalaryOnHoldID", "HRM_PAY_SalaryOnHold", 8);
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

            var hasPermission = await probationPeriodExtensionService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await probationPeriodExtensionService.DeleteTab(ids);
            if (success)
            {
                commonService.FindMaxNo(ref strMaxNO, "PPEID", "HRM_Def_ProbationPeriodExtension", 8);
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
        public async Task<IActionResult> GetTableData()
        {
            try
            {
                var list = await probationPeriodExtensionService.GetAllAsync();
                return PartialView("_Grid", list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        #endregion

        #region GetProbationData

        [HttpPost]
        public async Task<IActionResult> GetProbationData([FromBody] ProbationExtensionViewModel request)
        {
            var result = await probationPeriodExtensionService.GetProbationExtensionDataAsync(request.EmployeeID, request.CompanyCode);
            return Json(result);
        }

        #endregion

        #region GenerateNewId
        public async Task<IActionResult> GenerateNewId()
        {
            await Task.Delay(100);
            commonService.FindMaxNo(ref strMaxNO, "PPEID", "HRM_Def_ProbationPeriodExtension", 8);
            var newId = strMaxNO.ToString();
            return Json(newId);
        }

        #endregion

        #region comend Index

        //public IActionResult Index()
        //{
        //    ProbationPeriodExtensionPageViewModel model = new ProbationPeriodExtensionPageViewModel
        //    {
        //        Setup = new ProbationPeriodExtensionSetupViewModel()
        //    };

        //    return View(model);
        //}
        #endregion
    }
}

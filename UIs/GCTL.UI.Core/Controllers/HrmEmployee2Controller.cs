using GCTL.Core.Data;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HrmEmployees2;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.HrmEmployees2;
using GCTL.UI.Core.ViewModels.HrmEmployees2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GCTL.UI.Core.Controllers
{
    public class HrmEmployee2Controller : BaseController
    {
        #region Service & Repository
        private readonly IHrmEmployee2Service empService;
        private readonly ICommonService commonService;
        private readonly IRepository<HrmDefBloodGroup> bloodRepository;
        private readonly IRepository<HrmDefSex> sexRepository;
        private readonly IRepository<HrmDefReligion> religionRepository;
        private readonly IRepository<HrmDefMaritalStatus> maritalRepository;
        private readonly IRepository<CoreBranch> coreBranchRepository;
        private readonly IRepository<CoreCompany> coreCompanyRepository;
        private readonly IRepository<HrmDefNationality> nationalityRepositopry;

        string strMaxNO = string.Empty;

        public HrmEmployee2Controller(IHrmEmployee2Service empService,
            ICommonService commonService,
            IRepository<HrmDefBloodGroup> bloodRepository,
            IRepository<HrmDefSex> sexRepository,
            IRepository<HrmDefReligion> religionRepository,
            IRepository<HrmDefMaritalStatus> maritalRepository,
            IRepository<CoreBranch> coreBranchRepository,
            IRepository<CoreCompany> coreCompanyRepository,
           IRepository<HrmDefNationality> nationalityRepositopry

            )
        {
            this.empService = empService;
            this.commonService = commonService;
            this.bloodRepository = bloodRepository;
            this.sexRepository = sexRepository;
            this.religionRepository = religionRepository;
            this.maritalRepository = maritalRepository;
            this.coreBranchRepository = coreBranchRepository;
            this.coreCompanyRepository = coreCompanyRepository;
            this.nationalityRepositopry = nationalityRepositopry;
        }

        #endregion

        #region Index

        public async Task<IActionResult> Index(string id)
        {
            var hasPermission = await empService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }
            HrmEmployee2PageViewModel model = new HrmEmployee2PageViewModel();
            //var list = await empService.GetAllAsync();
            //model.ListTableData = list ?? new List<HrmEmployee2SetUpViewModel>();
            if (!string.IsNullOrEmpty(id))
            {
                model.Setup = await empService.GetByIdAsync(id);
            }

            ViewBag.SexesDD = new SelectList(sexRepository.All(), "SexCode", "Sex");
            ViewBag.BloodGroupDD = new SelectList(bloodRepository.All(), "BloodGroupCode", "BloodGroup");
            ViewBag.ReligionDD = new SelectList(religionRepository.All(), "ReligionCode", "Religion");
            ViewBag.NationalityDD = new SelectList(nationalityRepositopry.All(), "NationalityCode", "Nationality");
            ViewBag.CoreBranchDD = new SelectList(coreBranchRepository.All(), "BranchCode", "BranchName");
            ViewBag.CoreCompanyDD = new SelectList(coreCompanyRepository.All(), "CompanyCode", "CompanyName");
            ViewBag.MaritalyDD = new SelectList(maritalRepository.All(), "MaritalStatusCode", "MaritalStatus");
            model.AddUrl = Url.Action(nameof(Setup));
            model.PageUrl = Url.Action(nameof(Index));
            return View(model);

        }

        #endregion

        #region GetBranchesByCompanyCode


        [HttpGet]
        public async Task<JsonResult> GetBranchesByCompanyCode(string companyCode)
        {

            var branches = await (from c in coreCompanyRepository.All()
                                  join br in coreBranchRepository.All() on c.CompanyCode equals br.CompanyCode into cBraJoin
                                  from br in cBraJoin.DefaultIfEmpty()
                                  where c.CompanyCode == companyCode
                                  select new
                                  {
                                      value = br.BranchCode,
                                      text = br.BranchName ?? "No Branch Available"
                                  }).ToListAsync();



            return Json(branches);



        }

        #endregion

        #region Setup

        public async Task<ActionResult> Setup(string id)
        {

            var model = await empService.GetByIdAsync(id)
 ?? new HrmEmployee2SetUpViewModel();


            var companies = coreCompanyRepository.All();

            if (companies.Count() == 1)
            {
                model.CompanyCode = companies.First().CompanyCode;
            }
            else if (id != null)
            {
                model.CompanyCode = (await empService.GetByIdAsync(id))?.CompanyCode;
            }
            else
            {
                model.CompanyCode = null;
            }


            ViewBag.CoreCompanyDD = new SelectList(companies, "CompanyCode", "CompanyName");
            ViewBag.SexesDD = new SelectList(sexRepository.All(), "SexCode", "Sex", model.SexCode);
            ViewBag.BloodGroupDD = new SelectList(bloodRepository.All(), "BloodGroupCode", "BloodGroup", model.BloodGroupCode);
            ViewBag.ReligionDD = new SelectList(religionRepository.All(), "ReligionCode", "Religion", model.ReligionCode);
            ViewBag.NationalityDD = new SelectList(nationalityRepositopry.All(), "NationalityCode", "Nationality", model.NationalityCode);
            ViewBag.CoreBranchDD = new SelectList(coreBranchRepository.All(), "BranchCode", "BranchName", model.BranchCode);
            ViewBag.MaritalyDD = new SelectList(maritalRepository.All(), "MaritalStatusCode", "MaritalStatus", model.MaritalStatusCode);
            model.AddUrl = Url.Action(nameof(Setup));

            //if (string.IsNullOrEmpty(model.EmployeeId))
            //{
            //    model.EmployeeId = await empService.GenerateNextCode();
            //   // commonService.FindMaxNo(ref strMaxNO, "EmployeeID", "HRM_Employee", 12);
            //}


            return View(model);
        }

        #endregion

        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]


        public async Task<IActionResult> Setup(HrmEmployee2SetUpViewModel modelVM)
        {
            try
            {

                //if (!ModelState.IsValid)
                //{
                //    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                //    return Json(new { isSuccess = false, message = "Validation failed", errors });
                //}


                if (await empService.IsExistByAsync(modelVM.FirstName, modelVM.FatherName, modelVM.DateOfBirthOrginal?.ToString("MM/dd/yyyy"), modelVM.EmployeeId))
                {
                    return Json(new { isSuccess = false, message = $"Already  Exists!", isDuplicate = true });
                }


                //if (string.IsNullOrEmpty(modelVM.EmployeeId))
                //{
                //    modelVM.EmployeeId = await empService.GenerateNextCode();
                //    //commonService.FindMaxNo(ref strMaxNO, "EmployeeID", "HRM_Employee", 12);
                //}


                modelVM.ToAudit(LoginInfo, modelVM.AutoId > 0);
                if (modelVM.AutoId == 0)
                {

                    var hasSavePermission = await empService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await empService.SaveAsync(modelVM, LoginInfo.CompanyCode);
                        //TempData["Data"] = "This is the data I want to share.";
                        //return RedirectToAction("Index", "HrmEmployee2");

                        return Json(new { isSuccess = true, message = "Saved Successfully", redirectUrl = Url.Action("Index", "HrmEmployee2"), lastCode = modelVM.EmployeeId });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await empService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await empService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully", redirectUrl = Url.Action("Index", "HrmEmployee2"), lastCode = modelVM.EmployeeId });
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
                var hasPermission = await empService.DeletePermissionAsync(LoginInfo.AccessCode);
                if (hasPermission)
                {
                    DeleteHistoryViewModel model = new DeleteHistoryViewModel();
                    model.tableName = "HRM_Employee";
                    model.ToAudit(LoginInfo);

                    var result = await empService.DeleteLeaveType(ids, model);
                    return Json(new { isSuccess = result.isSuccess, message = result.message, refSuccess = result.refError });
                }
                else
                {
                    return Json(new { isSuccess = false, message = "You have no access" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting: {ex.Message}");
                return StatusCode(500, new { isSuccess = false, message = ex.Message });
            }
        }

        #endregion

        #region NeaxtCode

        [HttpGet]

        //public async Task<IActionResult> GenerateNextCode()
        //{
        //   // var nextCode = await empService.GenerateNextCode();
        //    return Json(nextCode);

        //}
        #endregion

        #region TabeleLodaing

        [HttpGet]

        public async Task<IActionResult> GetTableData()
        {
            try
            {
                var list = await empService.GetAllAsync();
                return Json(list);
                // return PartialView("_Grid", list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }

        }

        private void Delete_Photo(string id)
        {
            string sql = "Delete from HRM_EmployeePhoto where EmployeeID='" + id + "'";
        }

        #endregion

        #region CheckAvailability

        [HttpPost] //
        public async Task<JsonResult> CheckAvailability(string code, string firstName, string dateOfBirthOriginal, string fathersName)
        {

            if (await empService.IsExistByAsync(code, firstName, dateOfBirthOriginal, fathersName))
            {
                return Json(new { isSuccess = true, message = $"Already exists!." });
            }

            return Json(new { isSuccess = false });
        }


        #endregion

    }
}

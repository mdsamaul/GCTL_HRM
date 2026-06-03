//using GCTL.Core.Data;
//using GCTL.Core.ViewModels.CompanyInfo;
//using GCTL.Core.ViewModels.PaymentTerms;
//using GCTL.Data.Models;
//using GCTL.Service.Common;
//using GCTL.Core.Helpers;
//using GCTL.Service.CompanyInfo;
//using GCTL.Service.PaymentTerms;
//using GCTL.UI.Core.ViewModels.CompanyInfo;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;

//namespace GCTL.UI.Core.Controllers
//{
//    public class CompanyInfoController : BaseController
//    {
//        #region Service & Repository
//        public readonly ICompanyInfoSupService companyInfoService;
//        private readonly IRepository<InvDefCompanyFor> companyforrep;
//        private readonly IRepository<CaDefCountry> cadefcountry;
//        private readonly ICommonService commonService;
//        string strMaxNO = string.Empty;

//        public CompanyInfoController(
//            ICompanyInfoSupService companyInfoService,
//            IRepository<InvDefCompanyFor> companyforrep,
//            IRepository<CaDefCountry> cadefcountry, 
//            ICommonService commonService

//            )
//        {
//            this.companyInfoService = companyInfoService;
//            this.companyforrep = companyforrep;
//            this.cadefcountry = cadefcountry;
//            this.commonService = commonService;
//        }

//        #endregion

//        #region Index
//        public async Task<IActionResult> Index(bool child = false)
//        {
//            var hasPermission = await companyInfoService.PagePermissionAsync(LoginInfo.AccessCode);
//            if (!hasPermission)
//            {
//                return RedirectToAction("Login", "Accounts");
//            }

//            var model = new CompanyInfoPageViewModel
//            {
//                PageUrl = Url.Action(nameof(Index))
//            };

//            try
//            {

//                var list = await companyInfoService.GetAllAsync();
//                model.CompanyInfoList = list ?? new List<CompanyInfoSetupViewModel>();

//                commonService.FindMaxNo(ref strMaxNO, "CompanyID", "Inv_Def_CompanyInfo", 3);

//                ViewBag.CompanyForDD = new SelectList(companyforrep.All(), "CompanyForId", "CompanyForName");
//                ViewBag.CountryDD = new SelectList(cadefcountry.All(), "CountryId", "CountryName");


//                model.Setup = new CompanyInfoSetupViewModel
//                {
//                    CompanyId = strMaxNO
//                };

//            }
//            catch (Exception ex)
//            {

//                model.CompanyInfoList = new List<CompanyInfoSetupViewModel>();
//                model.Setup = new CompanyInfoSetupViewModel();
//                Console.WriteLine("Error" + ex.Message);
//            }


//            if (child)
//                return PartialView(model);

//            return View(model);
//        }

//        #endregion

//        #region Setup

//        public async Task<IActionResult> Setup(string id)
//        {
//            CompanyInfoSetupViewModel model = new CompanyInfoSetupViewModel();
//            commonService.FindMaxNo(ref strMaxNO, "CompanyID", "Inv_Def_CompanyInfo", 3);

//            ViewBag.CompanyForDD = new SelectList(companyforrep.All(), "CompanyForId", "CompanyForName");
//            ViewBag.CountryDD = new SelectList(cadefcountry.All(), "CountryId", "CountryName");

//            if (!string.IsNullOrEmpty(id))
//            {

//                model = await companyInfoService.GetByIdAsync(id);
//                if (model == null)
//                {

//                    return NotFound();
//                }
//            }
//            else
//            {

//                model.CompanyId = strMaxNO;
//            }

//            return PartialView($"_{nameof(Setup)}", model);
//        }

//        #endregion

//        #region Post Update 

//        [HttpPost]
//        [ValidateAntiForgeryToken]

//        public async Task<IActionResult> Setup(CompanyInfoSetupViewModel modelVM)
//        {
//            try
//            {

//                if (await companyInfoService.IsExistAsync(modelVM.CompanyName, modelVM.CompanyForId))
//                {
//                    return Json(new { isSuccess = false, message = $"Already Exists!", isDuplicate = true });
//                }


//                if (!ModelState.IsValid)
//                {

//                    var errorMessage = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
//                    return Json(new { isSuccess = false, message = errorMessage });
//                }

//                modelVM.ToAudit(LoginInfo, modelVM.Tc > 0);
//                if (modelVM.Tc == 0)
//                {
//                    var hasSavePermission = await companyInfoService.SavePermissionAsync(LoginInfo.AccessCode);
//                    if (hasSavePermission)
//                    {
//                        await companyInfoService.SaveAsync(modelVM);
//                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.CompanyId });

//                    }
//                    else
//                    {
//                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
//                    }
//                }
//                else
//                {

//                    var hasUpdatePermission = await companyInfoService.UpdatePermissionAsync(LoginInfo.AccessCode);
//                    if (hasUpdatePermission)
//                    {
//                        await companyInfoService.UpdateAsync(modelVM);
//                        return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.CompanyId });
//                    }
//                    else
//                    {
//                        return Json(new { isSuccess = false, message = "You have no access to update.", noUpdatePermission = true });
//                    }
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Error:{ex.Message}");
//                return RedirectToAction("Login", "Accounts");

//            }
//        }

//        #endregion

//        #region Delete

//        [HttpPost]
//        public async Task<IActionResult> Delete([FromBody] List<string> ids)
//        {
//            if (ids == null || ids.Count == 0)
//            {
//                return BadRequest(new { success = false, message = "No IDs provided for delete." });
//            }

//            var hasPermission = await companyInfoService.DeletePermissionAsync(LoginInfo.AccessCode);
//            if (!hasPermission)
//            {
//                return Json(new { success = false, message = "You have no access." });
//            }

//            bool success = await companyInfoService.DeleteTab(ids);
//            if (success)
//            {
//                return Json(new { success = true, message = "Deleted Successfully." });
//            }
//            else
//            {
//                return Json(new { success = false, message = "Deletion failed." });
//            }
//        }

//        #endregion

//        #region TabeleLodaing

//        [HttpGet]
//        public async Task<IActionResult> GetTableData()
//        {
//            try
//            {
//                var list = await companyInfoService.GetAllAsync();
//                return PartialView("_Grid", list);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }

//        #endregion

//        #region Chake Degian

//        //public IActionResult Index()
//        //{
//        //    CompanyInfoPageViewModel model = new CompanyInfoPageViewModel
//        //    {
//        //        Setup = new CompanyInfoSetupViewModel()
//        //    };
//        //    return View(model);
//        //}

//        #endregion
//    }
//}

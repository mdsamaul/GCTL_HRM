//using GCTL.Core.Data;
//using GCTL.Core.Helpers;
//using GCTL.Core.ViewModels.StyleInformation;
//using GCTL.Data.Models;
//using GCTL.Service.Common;
//using GCTL.Service.PaymentTerms;
//using GCTL.Service.StyleInformation;
//using GCTL.UI.Core.ViewModels.StyleInformation;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;

//namespace GCTL.UI.Core.Controllers
//{
//    public class StyleInformationController : BaseController
//    {
//        #region Service & Repository
//        public readonly IStyleInformationService styleInformationService;
//        private readonly IRepository<ProdDefBuyer> buyerRepository;
//        private readonly ICommonService commonService;
//        string strMaxNO = string.Empty;

//        public StyleInformationController(
//            IStyleInformationService styleInformationService,
//            IRepository<ProdDefBuyer> buyerRepository,
//            ICommonService commonService
//            )
//        {
//            this.styleInformationService = styleInformationService;
//            this.buyerRepository = buyerRepository;
//            this.commonService = commonService;
//        }

//        #endregion

//        #region Index
//        public async Task<IActionResult> Index(bool child = false)
//        {
//            var hasPermission = await styleInformationService.PagePermissionAsync(LoginInfo.AccessCode);
//            if (!hasPermission)
//            {
//                return RedirectToAction("Login", "Accounts");
//            }

//            var model = new StyleInformationPageViewModel
//            {
//                PageUrl = Url.Action(nameof(Index))
//            };

//            try
//            {

//                var list = await styleInformationService.GetAllAsync();

//                model.StyleList = list ?? new List<StyleInformationSetupViewModel>();

//                commonService.FindMaxNo(ref strMaxNO, "StyleId", "Prod_Def_Style", 3);

//                ViewBag.BuyerIdDD = new SelectList(buyerRepository.All(), "BuyerId", "Name");


//                model.Setup = new StyleInformationSetupViewModel
//                {
//                    StyleId = strMaxNO
//                };

//            }
//            catch (Exception ex)
//            {

//                model.StyleList = new List<StyleInformationSetupViewModel>();
//                model.Setup = new StyleInformationSetupViewModel();
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
//            StyleInformationSetupViewModel model = new StyleInformationSetupViewModel();
//            commonService.FindMaxNo(ref strMaxNO, "StyleId", "Prod_Def_Style", 3);

//            ViewBag.BuyerIdDD = new SelectList(buyerRepository.All(), "BuyerId", "Name");

//            if (!string.IsNullOrEmpty(id))
//            {

//                model = await styleInformationService.GetByIdAsync(id);
//                if (model == null)
//                {

//                    return NotFound();
//                }
//            }
//            else
//            {

//                model.StyleId = strMaxNO;
//            }

//            return PartialView($"_{nameof(Setup)}", model);
//        }

//        #endregion

//        #region Post Update 

//        [HttpPost]
//        [ValidateAntiForgeryToken]

//        public async Task<IActionResult> Setup(StyleInformationSetupViewModel modelVM)
//        {
//            try
//            {

//                if (await styleInformationService.IsExistAsync(modelVM.Style, modelVM.StyleId))
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
//                    var hasSavePermission = await styleInformationService.SavePermissionAsync(LoginInfo.AccessCode);
//                    if (hasSavePermission)
//                    {
//                        await styleInformationService.SaveAsync(modelVM);
//                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.StyleId });

//                    }
//                    else
//                    {
//                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
//                    }
//                }
//                else
//                {

//                    var hasUpdatePermission = await styleInformationService.UpdatePermissionAsync(LoginInfo.AccessCode);
//                    if (hasUpdatePermission)
//                    {
//                        await styleInformationService.UpdateAsync(modelVM);
//                        return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.StyleId });
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

//            var hasPermission = await styleInformationService.DeletePermissionAsync(LoginInfo.AccessCode);
//            if (!hasPermission)
//            {
//                return Json(new { success = false, message = "You have no access." });
//            }

//            bool success = await styleInformationService.DeleteTab(ids);
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
//        public async Task<IActionResult> GetTableData(string id)
//        {
//            try
//            {
//                var list = await styleInformationService.GetAllAsync(id);
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
//        //    StyleInformationPageViewModel model = new StyleInformationPageViewModel
//        //    {
//        //        Setup = new StyleInformationSetupViewModel()
//        //    };
//        //    return View(model);
//        //}

//        #endregion
//    }
//}

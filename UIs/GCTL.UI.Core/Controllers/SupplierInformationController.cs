//using GCTL.Core.Data;
//using GCTL.Core.Helpers;
//using GCTL.Core.ViewModels.SupplierInformation;
//using GCTL.Data.Models;
//using GCTL.Service.Common;
//using GCTL.Service.SupplierInformation;
//using GCTL.UI.Core.ViewModels.SupplierInformation;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Org.BouncyCastle.Asn1.X509;
//using System.Threading.Tasks;

//namespace GCTL.UI.Core.Controllers
//{
//    public class SupplierInformationController : BaseController
//    {
//        #region Service & Repository
//        private readonly ISupplierInformationService supplierInformationService;
//        private readonly IRepository<CoreAccessCode> accessCodeRepository;
//        private readonly ICommonService commonService;
//        private readonly IRepository<InvDefSupplierOrigin> supplierInformationOriginRepository;
//        private readonly IRepository<InvDefSupplierType> supplierInformationTypeRepository;
//        private readonly IRepository<InvDefSupplierCategory> supplierInformationCategoryRepository;
//        private readonly IRepository<InvDefCompanyInfo> supplierInformationCoreCompanyRepository;
//        private readonly IRepository<CaDefCountry> caDefCountryRepository;
//        private readonly IRepository<SalesContactPerson> salesContactPersonRepository;
//        private readonly IRepository<SalesDefBankInfo> salesDefBankInfoRepository;
//        private readonly IRepository<SalesDefBankBranchInfo> salesDefBankBranchInfoRepository;
//        private readonly IRepository<CoreBankAccountInformation> salesDefBankAccountInformationRepository;
//        private readonly IRepository<SalesSupplierBankAccountTemp> salesSupplierBankAccountTempRepository;
//        private readonly IRepository<RmgDefSupplier> supplierInformationRepository;
//        private readonly IRepository<InvDefSalesPerson> invDefSalesPersonRepository;
//        private readonly IRepository<HrmDefDesignation> hrmDesignationRepository;

//        string strMaxNO = string.Empty;

//        public SupplierInformationController(
//            ISupplierInformationService supplierInformationService,
//            IRepository<CoreAccessCode> accessCodeRepository,
//            ICommonService commonService,
//            IRepository<InvDefSupplierOrigin> supplierInformationOriginRepository,
//            IRepository<InvDefSupplierType> supplierInformationTypeRepository,
//            IRepository<InvDefSupplierCategory> supplierInformationCategoryRepository,
//            IRepository<InvDefCompanyInfo> supplierInformationCoreCompanyRepository,
//            IRepository<CaDefCountry> caDefCountryRepository,
//            IRepository<SalesContactPerson> salesContactPersonRepository,
//            IRepository<SalesDefBankInfo> salesDefBankInfoRepository,
//            IRepository<SalesDefBankBranchInfo> salesDefBankBranchInfoRepository,
//            IRepository<CoreBankAccountInformation> salesDefBankAccountInformationRepository,
//            IRepository<SalesSupplierBankAccountTemp> salesSupplierBankAccountTempRepository,
//            IRepository<RmgDefSupplier> supplierInformationRepository,
//            IRepository<InvDefSalesPerson> invDefSalesPersonRepository,
//            IRepository<HrmDefDesignation> hrmDesignationRepository

//            )
//        {
//            this.supplierInformationService = supplierInformationService;
//            this.accessCodeRepository = accessCodeRepository;
//            this.commonService = commonService;
//            this.supplierInformationOriginRepository = supplierInformationOriginRepository;
//            this.supplierInformationTypeRepository = supplierInformationTypeRepository;
//            this.supplierInformationCategoryRepository = supplierInformationCategoryRepository;
//            this.supplierInformationCoreCompanyRepository = supplierInformationCoreCompanyRepository;
//            this.caDefCountryRepository = caDefCountryRepository;
//            this.salesContactPersonRepository = salesContactPersonRepository;
//            this.salesDefBankInfoRepository = salesDefBankInfoRepository;
//            this.salesDefBankBranchInfoRepository = salesDefBankBranchInfoRepository;
//            this.salesDefBankAccountInformationRepository = salesDefBankAccountInformationRepository;
//            this.supplierInformationRepository = supplierInformationRepository;
//            this.salesSupplierBankAccountTempRepository = salesSupplierBankAccountTempRepository;
//            this.invDefSalesPersonRepository = invDefSalesPersonRepository;
//            this.hrmDesignationRepository = hrmDesignationRepository;
//        }

//        #endregion

//        #region Index

//        public async Task<IActionResult> Index(bool child = false)
//        {
//            // await Task.Delay(100);
//            SupplierInformationPageViewModel model = new SupplierInformationPageViewModel()
//            {
//                PageUrl = Url.Action(nameof(Index))
//            };

//            var list = await supplierInformationService.GetAllAsync();

//            model.SupplierInformationList = list ?? new List<SupplierInformationSetupViewModel>();

//            ViewBag.SupplierCatDD = new SelectList(supplierInformationCategoryRepository.All(), "SupplierCategoryId", "SupplierCategory");
//            ViewBag.SupplierTypeDD = new SelectList(supplierInformationOriginRepository.All(), "SupplierOriginId", "SupplierOrigin");
//            ViewBag.SupplierComDD = new SelectList(supplierInformationCoreCompanyRepository.All().Where(x=>x.CompanyForId=="03"), "CompanyId", "CompanyName");
//            ViewBag.SCountryDD = new SelectList(caDefCountryRepository.All(), "CountryId", "CountryName");
//            ViewBag.ContatPersonDD = new SelectList(salesContactPersonRepository.All(), "Cpid", "ContactPersonName");
//            ViewBag.SupplierBankDD = new SelectList(salesDefBankInfoRepository.All(), "BankId", "BankName");
//            ViewBag.SupplierBankBranchDD = new SelectList(salesDefBankBranchInfoRepository.All(), "BankBranchId", "BankBranchName");
//            ViewBag.SalesPersonDD = new SelectList(invDefSalesPersonRepository.All(), "SalesPersonId", "SalesPerson");
//            ViewBag.ActiveDD = new SelectList(supplierInformationRepository.All()

//        .Select(x => new
//        {
//            Value = x.Active,
//            Text = x.Active == "Y" ? "Y" : "N"
//        }).Distinct().ToList(),"Value","Text");

//            ViewBag.OptypeDD = new SelectList(
//                new[]
//                {
//                  new { Value = "Debit", Text = "Debit" },
//                  new { Value = "Credit", Text = "Credit" }
//                },
//                "Value",
//                "Text"
//            );

//            model.Setup = new SupplierInformationSetupViewModel
//            {
//                // Aeid = strMaxNO
//                // Aeid = nextCode,
//            };

//            if (child) return PartialView(model);

//            return View(model);
//        }

//        #endregion

//        #region Setup

//        public async Task<IActionResult> Setup(string id)
//        {
//            SupplierInformationSetupViewModel model = new SupplierInformationSetupViewModel();
//            var nextCode = commonService.GenerateCode("SupplierId", "RMG_Def_Supplier", "SUP", 6);

//            if (!string.IsNullOrEmpty(id))
//            {
//                model = await supplierInformationService.GetByIdAsync(id);
//                if (model == null)
//                {
//                    return NotFound();
//                }
//            }
//            else
//            {
//                // model.Aeid = strMaxNO;
//                model.SupplierId = nextCode;
//            }

//            ViewBag.SupplierCatDD = new SelectList(supplierInformationCategoryRepository.All(), "SupplierCategoryId", "SupplierCategory");
//            ViewBag.SupplierTypeDD = new SelectList(supplierInformationOriginRepository.All(), "SupplierOriginId", "SupplierOrigin");
//            ViewBag.SupplierComDD = new SelectList(supplierInformationCoreCompanyRepository.All(), "CompanyCode", "CompanyName");
//            ViewBag.SCountryDD = new SelectList(caDefCountryRepository.All(), "CountryId", "CountryName");
//            ViewBag.ContatPersonDD = new SelectList(salesContactPersonRepository.All(), "Cpid", "ContactPersonName");
//            ViewBag.SupplierBankDD = new SelectList(salesDefBankInfoRepository.All(), "BankId", "BankName");
//            ViewBag.SupplierBankBranchDD = new SelectList(salesDefBankBranchInfoRepository.All(), "BankBranchId", "BankBranchName");
//            // ViewBag.SalesPersonDD = new SelectList(salesContactPersonRepository.All(), "Cpid", "ContactPersonName");
//            ViewBag.ActiveDD = new SelectList(supplierInformationRepository.All().Select(x => new{
//            Value = x.Active,
//             Text = x.Active == "Y" ? "Y" : "N"
//            }).Distinct().ToList(), "Value", "Text");

//            ViewBag.OptypeDD = new SelectList(new[] {
//            new { Value = "Debit", Text = "Debit" },
//            new { Value = "Credit", Text = "Credit" }
//            }, "Value", "Text");

//            return PartialView($"_{nameof(Setup)}", model);
//        }

//        #endregion

//        #region GetById

//        [HttpGet]
//        public async Task<IActionResult> GetById(string code)
//        {
//            var result = await supplierInformationService.GetByIdAsync(code);
//            return Json(result);
//        }

//        #endregion

//        #region GetContactPersons

//        [HttpGet]
//        public JsonResult GetContactPersons()
//        {
//            try
//            {
//                var contactPersons = (from cp in salesContactPersonRepository.All()
//                                     join d in hrmDesignationRepository.All()
//                                     on cp.DesignationCode equals d.DesignationCode into desiGroup
//                                     from d in desiGroup.DefaultIfEmpty()
//                                     select new
//                                     {
//                                         cpid = cp.Cpid,
//                                         contactPersonName = cp.ContactPersonName,
//                                         designation = d.DesignationName,
//                                         phone = cp.ContactPersonMobile,
//                                         email = cp.ContactPersonEmail
//                                     }).ToList();

//                return Json(new { success = true, data = contactPersons });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = ex.Message });
//            }
//        }

//        #endregion

//        #region Post Update   

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Setup(SupplierInformationSetupViewModel modelVM)
//        {
//            try
//            {
//                if (!ModelState.IsValid)
//                {
//                    var errorMessage = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
//                    return Json(new { isSuccess = false, message = errorMessage });
//                }

//                modelVM.ToAudit(LoginInfo, modelVM.Tc > 0);
//                if (modelVM.Tc == 0)
//                {
//                    var hasSavePermission = await supplierInformationService.SavePermissionAsync(LoginInfo.AccessCode);
//                    if (hasSavePermission)
//                    {
//                        if (await supplierInformationService.IsExistAsync(modelVM.SupplierName, modelVM.Phone, modelVM.Email))
//                        {
//                            return Json(new { isSuccess = false, message = $"Already Exists!", isDuplicate = true });
//                        }

//                        await supplierInformationService.SaveAsync(modelVM);
//                        var nextCode = commonService.GenerateCode("SupplierId", "RMG_Def_Supplier", "SUP", 6);

//                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = nextCode });
//                    }
//                    else
//                    {
//                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
//                    }
//                }
//                else
//                {
//                    var hasUpdatePermission = await supplierInformationService.UpdatePermissionAsync(LoginInfo.AccessCode);
//                    if (hasUpdatePermission)
//                    {
//                        var result = await supplierInformationService.UpdateAsync(modelVM);

//                        var nextCode = commonService.GenerateCode("SupplierId", "RMG_Def_Supplier", "SUP", 6);
//                        return Json(new { isSuccess = result, message = "Updated Successfully.", lastCode = nextCode });
//                    }
//                    else
//                    {
//                        return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });
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

//            var hasPermission = await supplierInformationService.DeletePermissionAsync(LoginInfo.AccessCode);
//            if (!hasPermission)
//            {
//                return Json(new { success = false, message = "You have no access." });
//            }

//            bool success = await supplierInformationService.DeleteTab(ids);
//            if (success)
//            {
//                var nextCode = commonService.GenerateCode("SupplierId", "RMG_Def_Supplier", "SUP", 6);
//                return Json(new { success = true, message = "Deleted Successfully.", lastCode = nextCode });
//            }
//            else
//            {
//                return Json(new { success = false, message = "Deletion failed. Some entities may still exists." });
//            }
//        }

//        #endregion

//        #region TabeleLodaing

//        [HttpGet]
//        public async Task<IActionResult> GetTableData()
//        {
//            try
//            {
//                var list = await supplierInformationService.GetAllAsync();
//                return PartialView("_Grid", list);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }

//        #endregion

//        #region TabeleLodaing

//        [HttpGet]
//        public async Task<IActionResult> GetTableBankAccountInfoData()
//        {
//            try
//            {
//                var dtos = await supplierInformationService.GetTableBankAccountInfoDataAsync();

//                var result = dtos.Select(dto => new
//                {
//                    dto.Sbaid,
//                    dto.BankId, 
//                    dto.BankBranchId, 
//                    dto.AccountName,
//                    dto.BankBranchName,
//                    dto.BankName,
//                }).ToList();

//                return Json(result);
//            }
//            catch (Exception ex)
//            {               
//                return StatusCode(500, ex.Message);
//            }
//        }

//        #endregion

//        #region GenerateNewId
//        public async Task<IActionResult> GenerateNewId()
//        {
//            await Task.Delay(100);
//            //var nextCode = commonService.GenerateCode("SupplierId", "RMG_Def_Supplier", "SUP", 6);
//            var nextCode = await supplierInformationService.Autoid();
//            return Json(nextCode);
//        }

//        #endregion

//        #region Chake Degian

//        //public IActionResult Index()
//        //{
//        //    SupplierInformationPageViewModel model = new SupplierInformationPageViewModel
//        //    {
//        //        Setup = new SupplierInformationSetupViewModel()
//        //    };
//        //    return View(model);
//        //}

//        #endregion

//        #region Bank AccountInfo SaveEdit

//        [HttpPost]
//        public async Task<IActionResult> BankAccountInfoSaveEdit([FromBody] SalesSupplierBankAccountTempDto BankAccountInfoSaveEdit)
//        {
//            try
//            {
//                if (!ModelState.IsValid)
//                {
//                    var errorMessage = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
//                    return Json(new { isSuccess = false, message = errorMessage });
//                }

//                BankAccountInfoSaveEdit.ToAudit(LoginInfo, Convert.ToInt32(BankAccountInfoSaveEdit.Sbaid) > 0);
//                if (Convert.ToInt32(BankAccountInfoSaveEdit.Sbaid) == 0)
//                {
//                    var hasSavePermission = await supplierInformationService.SavePermissionAsync(LoginInfo.AccessCode);
//                    if (hasSavePermission)
//                    {
//                        if (await supplierInformationService.IsExistAsync(BankAccountInfoSaveEdit.SupplierId, BankAccountInfoSaveEdit.BankId, BankAccountInfoSaveEdit.BankBranchId))
//                        {
//                            return Json(new { isSuccess = false, message = $"Already Exists!", isDuplicate = true });
//                        }

//                        await supplierInformationService.BankAccountInfoSaveEditAsync(BankAccountInfoSaveEdit);
//                        var nextCode = commonService.GenerateCode("SupplierId", "RMG_Def_Supplier", "SUP", 6);

//                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = nextCode });
//                    }
//                    else
//                    {
//                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
//                    }
//                }
//                else
//                {
//                    var hasUpdatePermission = await supplierInformationService.UpdatePermissionAsync(LoginInfo.AccessCode);
//                    if (hasUpdatePermission)
//                    {
//                        var result = await supplierInformationService.BankAccountInfoSaveEditAsync(BankAccountInfoSaveEdit);

//                        var nextCode = commonService.GenerateCode("SupplierId", "RMG_Def_Supplier", "SUP", 6);
//                        return Json(new { isSuccess = result, message = "Updated Successfully.", lastCode = nextCode });
//                    }
//                    else
//                    {
//                        return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });
//                    }
//                }
//            }
//            catch (Exception )
//            {
//                throw;
//            }
//        }

//        #endregion

//        #region Bank AccountInfo Delete

//        [HttpPost]
//        public async Task<IActionResult> BankAccountInfoDelete([FromBody] string sbaid)
//        {
//            if (sbaid == null)
//            {
//                return BadRequest(new { success = false, message = "No IDs provided for delete." });
//            }

//            var hasPermission = await supplierInformationService.DeletePermissionAsync(LoginInfo.AccessCode);
//            if (!hasPermission)
//            {
//                return Json(new { success = false, message = "You have no access." });
//            }

//            bool success = await supplierInformationService.BankAccountInfoDeleteAsync(sbaid);
//            if (success)
//            {
//                var nextCode = commonService.GenerateCode("SupplierId", "RMG_Def_Supplier", "SUP", 6);
//                return Json(new { success = true, message = "Deleted Successfully.", lastCode = nextCode });
//            }
//            else
//            {
//                return Json(new { success = false, message = "Deletion failed. Some entities may still exists." });
//            }
//        }

//        #endregion

//        #region Bank AccountInfo Clear

//        [HttpGet]
//        public async Task<IActionResult> BankAccountInfoClearTableTemp()
//        {
//            try
//            {
//                var result = await supplierInformationService.BankAccountInfoClearTableTempAsync();
//                return Json(result);
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//        }

//        #endregion
//    }
//}

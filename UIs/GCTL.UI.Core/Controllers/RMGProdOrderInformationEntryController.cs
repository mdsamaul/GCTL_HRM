//using GCTL.Core.Data;
//using GCTL.Core.Helpers;
//using GCTL.Core.ViewModels.RMGProdOrderInformationEntry;
//using GCTL.Data.Models;
//using GCTL.Service.RMGProdOrderInformationEntry;
//using GCTL.UI.Core.ViewModels.RMGProdOrderInformationEntry;
//using GCTL.UI.Core.Views.RMGProdOrderInformationEntry;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;


//namespace GCTL.UI.Core.Controllers
//{
//    public class RMGProdOrderInformationEntryController : BaseController
//    {
//        private readonly IRMGProdOrderInformationEntryService rMGProdOrderInformationEntryService;
//        private readonly IRepository<RmgProdDefBuyerPhoto> buyerPhotoRepo;
//        private readonly IRepository<RmgProdDefBuyer> buyerRepo;
//        private readonly IRepository<RmgProdDefBrand> buyerBrandRepo;
//        private readonly IRepository<InvItemBrand> buyerBrandPhotoRepo;
//        private readonly IRepository<ProdDefStyle> styleRepo;
//        private readonly IRepository<RmgProdDefUnitType> unitTypeRepo;
//        private readonly IRepository<CaDefCurrency> currencyRepo;
//        private readonly IRepository<SalesDefBankInfo> bankRepo;
//        private readonly IRepository<SalesDefBankBranchInfo> bankBranchRepo;
//        private readonly IRepository<SalesContactPerson> buyerContactPersonRepo;
//        private readonly IRepository<RmgProdOrder> orderRepo;
//        private readonly IRepository<InvDefItem> itemRepo;
//        private readonly IRepository<SalesDefPaymentTerms> paymentTermsRepo;
//        private readonly IRepository<InvDefGarmentsTesing> garmentsTesingRepo;
//        private readonly IRepository<InvDefFebricTesting> fabricTypeRepo;
//        private readonly IRepository<RmgDefSupplier> supplierRepo;
//        private readonly IRepository<InvDefDeliveryMethod> deliveryRepo;
//        private readonly IRepository<InvDefPortInfo> portRepo;
//        private readonly IRepository<CaDefCountry> countryRepo;
//        private readonly IRepository<RmgProdDefColor> colorRepo;
//        private readonly IRepository<RmgProdDefSize> sizeRepo;
//        private readonly IRepository<RmgProdOrderDetails> prodOrderDetailsRepo;
//        private readonly IRepository<RmgProdTempColorSizeBreakup> colorSizeRepo;
//        private readonly IRepository<HrmEmployeeOfficialInfo> offiRepo;
//        private readonly IRepository<HrmEmployee> empRepo;
//        private readonly IRepository<HrmDefDesignation> desRepo;
//        private readonly IRepository<InvItemPhoto> itemImageRepo;
//        private readonly IRepository<RmgProdDefSeason> seasonRepo;

//        public RMGProdOrderInformationEntryController(
//            IRMGProdOrderInformationEntryService rMGProdOrderInformationEntryService,
//            IRepository<RmgProdDefBuyerPhoto> buyerPhotoRepo,
//            IRepository<RmgProdDefBuyer> buyerRepo,
//            IRepository<RmgProdDefBrand> buyerBrandRepo,
//            IRepository<InvItemBrand> buyerBrandPhotoRepo,
//            IRepository<ProdDefStyle> styleRepo,
//            IRepository<RmgProdDefSeason> seasonRepo,
//            IRepository<RmgProdDefUnitType> unitTypeRepo,
//            IRepository<CaDefCurrency> currencyRepo,
//            IRepository<SalesDefBankInfo> bankRepo,
//            IRepository<SalesDefBankBranchInfo> bankBranchRepo,
//            IRepository<SalesContactPerson> buyerContactPersonRepo,
//            IRepository<RmgProdOrder> orderRepo,
//            IRepository<InvDefItem> itemRepo,
//            IRepository<SalesDefPaymentTerms> paymentTermsRepo,
//            IRepository<InvDefGarmentsTesing> garmentsTesingRepo,
//            IRepository<InvDefFebricTesting> fabricTypeRepo,
//            IRepository<RmgDefSupplier> supplierRepo,
//            IRepository<InvDefDeliveryMethod> deliveryRepo,
//            IRepository<InvDefPortInfo> portRepo,
//            IRepository<CaDefCountry> countryRepo,
//            IRepository<RmgProdDefColor> colorRepo,
//            IRepository<RmgProdDefSize> sizeRepo,
//               IRepository<RmgProdOrderDetails> ProdOrderDetailsRepo,
//               IRepository<RmgProdTempColorSizeBreakup> colorSizeRepo,
//               IRepository<HrmEmployeeOfficialInfo> offiRepo,
//               IRepository<HrmEmployee> empRepo,
//               IRepository<HrmDefDesignation> desRepo,
//               IRepository<InvItemPhoto> itemImageRepo

//            )
//        {
//            this.rMGProdOrderInformationEntryService = rMGProdOrderInformationEntryService;
//            this.buyerRepo = buyerRepo;
//            this.buyerPhotoRepo = buyerPhotoRepo;
//            this.buyerBrandRepo = buyerBrandRepo;
//            this.buyerBrandPhotoRepo = buyerBrandPhotoRepo;
//            this.styleRepo = styleRepo;
//            this.unitTypeRepo = unitTypeRepo;
//            this.currencyRepo = currencyRepo;
//            this.bankRepo = bankRepo;
//            this.bankBranchRepo = bankBranchRepo;
//            this.buyerContactPersonRepo = buyerContactPersonRepo;
//            this.orderRepo = orderRepo;
//            this.itemRepo = itemRepo;
//            this.paymentTermsRepo = paymentTermsRepo;
//            this.garmentsTesingRepo = garmentsTesingRepo;
//            this.fabricTypeRepo = fabricTypeRepo;
//            this.supplierRepo = supplierRepo;
//            this.deliveryRepo = deliveryRepo;
//            this.portRepo = portRepo;
//            this.countryRepo = countryRepo;
//            this.colorRepo = colorRepo;
//            this.sizeRepo = sizeRepo;
//            this.prodOrderDetailsRepo = ProdOrderDetailsRepo;
//            this.colorSizeRepo = colorSizeRepo;
//            this.offiRepo = offiRepo;
//            this.empRepo = empRepo;
//            this.desRepo = desRepo;
//            this.itemImageRepo = itemImageRepo;
//            this.seasonRepo = seasonRepo;
//        }
//        public async Task<IActionResult> Index()
//        {
//            var hasPermission = await rMGProdOrderInformationEntryService.PagePermissionAsync(LoginInfo.AccessCode);

//            if (!hasPermission)

//            {

//                return RedirectToAction("Login", "Accounts");

//            }
//            ViewBag.buyerList = new SelectList(buyerRepo.All().Select(x => new { id = x.BuyerId, name = x.BuyerName }), "id", "name");
//            ViewBag.buyerBrandList = new SelectList(buyerBrandRepo.All().Select(x => new { id = x.BrandId, name = x.Name }), "id", "name");
//            ViewBag.styleList = new SelectList(styleRepo.All().Select(x => new { id = x.StyleId, name = x.Style }), "id", "name");
//            ViewBag.seactionList = new SelectList(seasonRepo.All().Select(x => new { id = x.SeasonId, name = x.Season }), "id", "name");
//            ViewBag.unitTypeList = new SelectList(unitTypeRepo.All().Select(x => new { id = x.UnitTypId, name = x.UnitTypeName }), "id", "name");
//            ViewBag.currentcyList = new SelectList(currencyRepo.All().Select(x => new { id = x.CurrencyId, name = x.ShortName }), "id", "name");
//            ViewBag.bankList = new SelectList(bankRepo.All().Select(x => new { id = x.BankId, name = x.BankName }), "id", "name");
//            ViewBag.bankBranchList = new SelectList(bankBranchRepo.All().Select(x => new { id = x.BankBranchId, name = x.BankBranchName }), "id", "name");
//            ViewBag.buyerContactList = new SelectList(buyerContactPersonRepo.All().Select(x => new { id = x.Cpid, name = x.ContactPersonName }), "id", "name");
//            ViewBag.intregraJobNoList = new SelectList(orderRepo.All().Select(x => new { id = x.IntegraJobno, name = x.IntegraJobno }), "id", "name");
//            ViewBag.itemRepoList = new SelectList(itemRepo.All().Select(x => new { id = x.ItemId, name = x.ItemName }), "id", "name");
//            ViewBag.paymentTermsRepoList = new SelectList(paymentTermsRepo.All().Select(x => new { id = x.PaymentTermsId, name = x.PaymentTermsName }), "id", "name");
//            ViewBag.garmentsTesingRepoList = new SelectList(garmentsTesingRepo.All().Select(x => new { id = x.GarmentsTestD, name = x.GarmentsTestName }), "id", "name");
//            ViewBag.fabricTypeRepoList = new SelectList(fabricTypeRepo.All().Select(x => new { id = x.FebricTestD, name = x.FebricTestName }), "id", "name");
//            ViewBag.supplierRepoList = new SelectList(supplierRepo.All().Select(x => new { id = x.SupplierId, name = x.SupplierName }), "id", "name");
//            ViewBag.deliveryRepoList = new SelectList(deliveryRepo.All().Select(x => new { id = x.DeliveryMethodId, name = x.DeliveryMethodName }), "id", "name");
//            ViewBag.portRepoList = new SelectList(portRepo.All().Select(x => new { id = x.PortId, name = x.PortName }), "id", "name");
//            ViewBag.colorRepoList = new SelectList(colorRepo.All().Select(x => new { id = x.ColorId, name = x.Color }), "id", "name");
//            ViewBag.sizeRepoList = new SelectList(sizeRepo.All().Select(x => new { id = x.SizeId, name = x.Size }), "id", "name");
//            ViewBag.ProdOrderDetailsRepoList = new SelectList(prodOrderDetailsRepo.All().Select(x => new { id = x.PurchaseOrder, name = x.PurchaseOrder }), "id", "name");

//            RMGProdOrderInformationEntryViewModel model = new RMGProdOrderInformationEntryViewModel()
//            {
//                PageUrl = Url.Action(nameof(Index)),
//            };
//            return View(model);
//        }
//        [HttpGet]
//        public async Task<IActionResult> LoadPoStyleJobLoad()
//        {
//            try
//            {
//                var styleList = styleRepo.All()
//                    .Select(x => new { id = x.StyleId, name = x.Style })
//                    .ToList();

//                var integraJobNoList = orderRepo.All()
//                    .Select(x => new { id = x.IntegraJobno, name = x.IntegraJobno })
//                    .Distinct()
//                    .ToList();

//                var poList = prodOrderDetailsRepo.All()
//                    .Select(x => new { id = x.PurchaseOrder, name = x.PurchaseOrder })
//                    .Distinct()
//                    .ToList();

//                return Json(new
//                {
//                    isSuccess = true,
//                    styleList = styleList,
//                    integraJobNoList = integraJobNoList,
//                    poList = poList
//                });
//            }
//            catch (Exception ex)
//            {
//                return Json(new
//                {
//                    isSuccess = false,
//                    message = ex.Message
//                });
//            }
//        }
//        [HttpGet]
//        public IActionResult ReloadViewData()
//        {
//            var integraJobNoList = orderRepo.All()
//                .Select(x => new { id = x.IntegraJobno, name = x.IntegraJobno })
//                .ToList();

//            return Json(new { integraJobNoList });
//        }


//        [HttpGet]
//        public async Task<IActionResult> EntryAutoId()
//        {
//            try
//            {
//                var id = await rMGProdOrderInformationEntryService.EntryAutoIdAsync();
//                return Json(id);
//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }
//        [HttpGet]
//        public async Task<IActionResult> IntegraJOBNoAuto()
//        {
//            try
//            {
//                var id = await rMGProdOrderInformationEntryService.IntegraJOBNoAutoAsync();
//                return Json(id);
//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }
//        [HttpPost]
//        public async Task<IActionResult> BuyerBrand([FromBody] string buyerId)
//        {
//            try
//            {
//                var BuyerImage = buyerPhotoRepo.All().Where(x => x.BuyerId == buyerId);
//                var BrandList = buyerBrandRepo.All().Where(x => x.BuyerId == buyerId).Select(c => new { id = c.BrandId, name = c.Name }).ToList();
//                return Json(new { buyerImage = BuyerImage, brandList = BrandList });
//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }
//        [HttpPost]
//        public async Task<IActionResult> BuyerBrandPhoto([FromBody] string buyerBrandId)
//        {
//            try
//            {
//                var BuyerImage = buyerBrandPhotoRepo.All().Where(x => x.BrandId == buyerBrandId);

//                return Json(BuyerImage);
//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }

//        [HttpPost]
//        public async Task<IActionResult> BuyerBankBranch([FromBody] string buyerBankId)
//        {
//            try
//            {
//                var BrandList = bankBranchRepo.All().Where(x => x.BankId == buyerBankId).Select(c => new { id = c.BankBranchId, name = c.BankBranchName, address = c.Address, swiftCode = c.Swiftcode }).ToList();
//                return Json(BrandList);
//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }
//        [HttpPost]
//        public async Task<IActionResult> IntJobNoByStyle([FromBody] string id)
//        {
//            try
//            {
//                var style = orderRepo.All().Where(x => x.IntegraJobno == id).Select(c => new { id = c.IntegraJobno, name = c.IntegraJobno, styleId = c.StyleId, c.UnitTypId, c.CurrencyIdFob, c.Fobamount }).FirstOrDefault();
//                return Json(style);
//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }

//        [HttpPost]
//        public async Task<IActionResult> itemAddress([FromBody] string productId)
//        {
//            try
//            {
//                var itemImage = itemImageRepo.All().Where(x => x.ItemId == productId);
//                var BrandList = itemRepo.All().Where(x => x.ItemId == productId).Select(c => new { id = c.ItemId, name = c.ItemName, address = c.TechnicalSpecification }).ToList();
//                return Json(new { itemImage = itemImage, brnadList = BrandList });
//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }
//        [HttpPost]
//        public async Task<IActionResult> BuyerBankBranchAddressSwiftCode([FromBody] string buyerBankBranchId)
//        {
//            try
//            {
//                var BrandList = bankBranchRepo.All().Where(x => x.BankBranchId == buyerBankBranchId).Select(c => new { id = c.BankBranchId, name = c.BankBranchName, address = c.Address, swiftCode = c.Swiftcode }).FirstOrDefault();
//                return Json(BrandList);
//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }
//        [HttpPost]
//        public async Task<IActionResult> OrderSaveEdit([FromBody] RMG_Prod_OrderDto fromData)
//        {
//            try
//            {


//                fromData.ToAudit(LoginInfo);
//                if (fromData.TC == 0)
//                {
//                    bool hasParmision = await rMGProdOrderInformationEntryService.SavePermissionAsync(LoginInfo.AccessCode);
//                    if (hasParmision)
//                    {
//                        var result = await rMGProdOrderInformationEntryService.OrderSaveEditAsync(fromData, LoginInfo.CompanyCode);
//                        return Json(new { isSuccess = result.isSuccess, message = result.message, data = result.data });
//                    }
//                    else
//                    {
//                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
//                    }
//                }
//                else
//                {
//                    var hasUpdatePermission = await rMGProdOrderInformationEntryService.UpdatePermissionAsync(LoginInfo.AccessCode);
//                    if (hasUpdatePermission)
//                    {
//                        var result = await rMGProdOrderInformationEntryService.OrderSaveEditAsync(fromData, LoginInfo.CompanyCode);
//                        return Json(new { isSuccess = result.isSuccess, message = result.message, data = result.data });
//                    }
//                    else
//                    {
//                        return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });
//                    }
//                }
//            }
//            catch (Exception)
//            {

//                throw;
//            }

//        }

//        [HttpPost]
//        public async Task<IActionResult> TotalQtyByJobId([FromBody] string id)
//        {
//            try
//            {
//                var totalQty = orderRepo.All().Where(x => x.IntegraJobno == id).Select(c => c.TotalOrderQuantity).FirstOrDefault();
//                var totalListQty = prodOrderDetailsRepo.All().Where(x => x.IntegraJobNo == id).Select(c => c.OrderQuantity).ToList();
//                var totalQtyList = 0;
//                if (totalListQty != null)
//                {
//                    foreach (var item in totalListQty)
//                    {
//                        totalQtyList += item;
//                    }
//                }

//                return Json(new { totalQty = totalQty, totalQtyList = totalQtyList });
//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }

//        [HttpPost]
//        public async Task<IActionResult> GetOrderList(string buyerId = null)
//        {
//            var draw = Convert.ToInt32(Request.Form["draw"]);
//            var start = Convert.ToInt32(Request.Form["start"]);
//            var length = Convert.ToInt32(Request.Form["length"]);
//            var searchValue = Request.Form["search[value]"].FirstOrDefault();

//            var filter = new DataTableFilter
//            {
//                Draw = draw,
//                Start = start,
//                Length = length,
//                SearchValue = searchValue,
//                buyerId = buyerId
//            };

//            var result = await rMGProdOrderInformationEntryService.GetPagedOrdersAsync(filter);
//            return Json(result);
//        }


//        [HttpPost]
//        public async Task<IActionResult> DeleteOrderInfo([FromBody] List<decimal> selectedIds)
//        {
//            try
//            {
//                var hasUpdatePermission = await rMGProdOrderInformationEntryService.DeletePermissionAsync(LoginInfo.AccessCode);
//                if (hasUpdatePermission)
//                {
//                    var result = await rMGProdOrderInformationEntryService.DeleteOrderInfoAsync(selectedIds);
//                    return Json(new { isSuccess = result.isSuccess, message = result.message, data = result });
//                }
//                else
//                {
//                    return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });
//                }
//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }
//        [HttpPost]
//        public async Task<IActionResult> DeleteOrderDetails([FromBody] List<decimal> selectedDetailsIds)
//        {
//            try
//            {
//                var hasUpdatePermission = await rMGProdOrderInformationEntryService.DeletePermissionAsync(LoginInfo.AccessCode);
//                if (hasUpdatePermission)
//                {
//                    var result = await rMGProdOrderInformationEntryService.DeleteOrderDetailsAsync(selectedDetailsIds);
//                    return Json(new { isSuccess = result.isSuccess, message = result.message, data = result });
//                }
//                else
//                {
//                    return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });
//                }
//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }

//        [HttpPost]
//        public async Task<IActionResult> DetailsSaveEdit([FromBody] RMG_Prod_OrderDetailsDto fromData)
//        {
//            try
//            {

//                fromData.ToAudit(LoginInfo);
//                if (fromData.TC == 0)
//                {
//                    bool hasParmision = await rMGProdOrderInformationEntryService.SavePermissionAsync(LoginInfo.AccessCode);
//                    if (hasParmision)
//                    {
//                        var result = await rMGProdOrderInformationEntryService.DetailsSaveEditAsync(fromData, LoginInfo.CompanyCode);
//                        return Json(new { isSuccess = result.isSuccess, message = result.message, data = result.data });
//                    }
//                    else
//                    {
//                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
//                    }
//                }
//                else
//                {
//                    var hasUpdatePermission = await rMGProdOrderInformationEntryService.UpdatePermissionAsync(LoginInfo.AccessCode);
//                    if (hasUpdatePermission)
//                    {
//                        var result = await rMGProdOrderInformationEntryService.DetailsSaveEditAsync(fromData, LoginInfo.CompanyCode);
//                        return Json(new { isSuccess = result.isSuccess, message = result.message, data = result.data });
//                    }
//                    else
//                    {
//                        return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });
//                    }
//                }
//            }
//            catch (Exception)
//            {

//                throw;
//            }

//        }
//        [HttpPost]
//        public async Task<IActionResult> GetOrderDetailsList(string integraJobNo = null)
//        {
//            try
//            {
//                var draw = Convert.ToInt32(Request.Form["draw"]);
//                var start = Convert.ToInt32(Request.Form["start"]);
//                var length = Convert.ToInt32(Request.Form["length"]);
//                var searchValue = Request.Form["search[value]"].FirstOrDefault();

//                var filter = new DataTableFilter
//                {
//                    Draw = draw,
//                    Start = start,
//                    Length = length,
//                    SearchValue = searchValue,
//                    IntegraJobNo = integraJobNo
//                };

//                var result = await rMGProdOrderInformationEntryService.GetPagedOrderDetailsAsync(filter);
//                return Json(result);
//            }
//            catch (Exception ex)
//            {
//                return Json(new { data = new List<object>(), recordsTotal = 0, recordsFiltered = 0 });
//            }
//        }



//        [HttpPost]
//        public async Task<IActionResult> SaveEditColorSizeBreakup([FromBody] RMG_Prod_Temp_ColorSizeBreakupDto fromData)
//        {
//            try
//            {

//                fromData.ToAudit(LoginInfo);
//                if (fromData.TC == 0)
//                {
//                    bool hasParmision = await rMGProdOrderInformationEntryService.SavePermissionAsync(LoginInfo.AccessCode);
//                    if (hasParmision)
//                    {
//                        var result = await rMGProdOrderInformationEntryService.SaveEditColorSizeBreakupAsync(fromData, LoginInfo.CompanyCode);
//                        return Json(new { isSuccess = result.isSuccess, message = result.message, data = result.data });
//                    }
//                    else
//                    {
//                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
//                    }
//                }
//                else
//                {
//                    var hasUpdatePermission = await rMGProdOrderInformationEntryService.UpdatePermissionAsync(LoginInfo.AccessCode);
//                    if (hasUpdatePermission)
//                    {
//                        var result = await rMGProdOrderInformationEntryService.SaveEditColorSizeBreakupAsync(fromData, LoginInfo.CompanyCode);
//                        return Json(new { isSuccess = result.isSuccess, message = result.message, data = result.data });
//                    }
//                    else
//                    {
//                        return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });
//                    }
//                }
//            }
//            catch (Exception)
//            {

//                throw;
//            }

//        }

//        [HttpPost]
//        public async Task<IActionResult> SaveEditColorSizeBreakupList([FromBody] RMG_Prod_Temp_ColorSizeBreakupDto fromData)
//        {
//            try
//            {

//                fromData.ToAudit(LoginInfo);
//                if (fromData.TC == 0)
//                {
//                    bool hasParmision = await rMGProdOrderInformationEntryService.SavePermissionAsync(LoginInfo.AccessCode);
//                    if (hasParmision)
//                    {
//                        var result = await rMGProdOrderInformationEntryService.SaveEditColorSizeBreakupListAsync(fromData, LoginInfo.CompanyCode);
//                        return Json(new { isSuccess = result.isSuccess, message = result.message, data = result.data });
//                    }
//                    else
//                    {
//                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
//                    }
//                }
//                else
//                {
//                    var hasUpdatePermission = await rMGProdOrderInformationEntryService.UpdatePermissionAsync(LoginInfo.AccessCode);
//                    if (hasUpdatePermission)
//                    {
//                        var result = await rMGProdOrderInformationEntryService.SaveEditColorSizeBreakupAsync(fromData, LoginInfo.CompanyCode);
//                        return Json(new { isSuccess = result.isSuccess, message = result.message, data = result.data });
//                    }
//                    else
//                    {
//                        return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });
//                    }
//                }
//            }
//            catch (Exception)
//            {

//                throw;
//            }

//        }

//        [HttpGet]
//        public async Task<IActionResult> GetColorSizeBreakups()
//        {
//            var colorList = await colorRepo.All()
//                .Select(x => new { id = x.ColorId, name = x.Color })
//                .ToListAsync();

//            var sizeList = await sizeRepo.All()
//                .Select(x => new { id = x.SizeId, name = x.Size })
//                .ToListAsync();

//            var unitTypeList = await unitTypeRepo.All()
//                .Select(x => new { id = x.UnitTypId, name = x.UnitTypeName })
//                .ToListAsync();

//            var data = await colorSizeRepo.All()
//                .Select(x => new
//                {
//                    x.Tc,
//                    x.BreakNo,
//                    ColorId = x.ColorId,
//                    ColorName = colorRepo.All()
//                        .Where(w => w.ColorId == x.ColorId)
//                        .Select(e => e.Color)
//                        .FirstOrDefault(),
//                    SizeId = x.SizeId,
//                    SizeName = sizeRepo.All()
//                        .Where(s => s.SizeId == x.SizeId)
//                        .Select(w => w.Size)
//                        .FirstOrDefault(),
//                    x.Quantity,
//                    UnitTypeId = x.UnitTypeId,
//                    UnitTypeName = unitTypeRepo.All()
//                        .Where(w => w.UnitTypId == x.UnitTypeId)
//                        .Select(e => e.UnitTypeName)
//                        .FirstOrDefault(),
//                    x.Remarks
//                })
//                .ToListAsync();

//            return Json(new
//            {
//                isSuccess = true,
//                data,
//                dropdowns = new
//                {
//                    colors = colorList,
//                    sizes = sizeList,
//                    units = unitTypeList
//                }
//            });
//        }


//        [HttpPost]
//        public async Task<IActionResult> UpdateColorSizeBreakups([FromBody] List<RMG_Prod_Temp_ColorSizeBreakupDto> dtos)
//        {
//            if (dtos == null || !dtos.Any())
//                return Json(new { isSuccess = false, message = "No data received!" });

//            var result = await rMGProdOrderInformationEntryService.UpdateColorSizeBreakupsAsync(dtos);

//            return Json(new { isSuccess = result, message = result ? "All rows updated successfully!" : "Update failed!" });
//        }


//        [HttpPost]
//        public async Task<IActionResult> DeleteColorSizeBreakup(decimal tc)
//        {
//            await colorSizeRepo.DeleteAsync(tc);
//            return Json(new { isSuccess = true, message = "Deleted successfully" });
//        }

//        [HttpPost]
//        public async Task<IActionResult> GetColorSizeBreakupList([FromForm] DataTableFilter filter)
//        {
//            var result = await rMGProdOrderInformationEntryService.GetPagedColorSizeBreakupsAsync(filter);
//            return Json(new
//            {
//                draw = result.Draw,
//                recordsTotal = result.RecordsTotal,
//                recordsFiltered = result.RecordsFiltered,
//                data = result.Data
//            });
//        }

//        [HttpPost]
//        public async Task<IActionResult> GetColorSizeBreakupIntegraJobNo([FromBody] string IJNo)
//        {
//            try
//            {
//                var result = orderRepo.All().Where(x => x.IntegraJobno == IJNo).FirstOrDefault();
//                return Json(result);
//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }

//        [HttpPost]
//        public async Task<IActionResult> PoIjobNoGetTemp([FromBody] OrderJobDto orderJobDto)
//        {
//            try
//            {
//                var (colorIds, sizeIds) = await rMGProdOrderInformationEntryService.PoIjobNoGetTempAsync(orderJobDto);

//                return Json(new
//                {
//                    isSuccess = true,
//                    colorIds = colorIds,
//                    sizeIds = sizeIds,
//                    message = "Data loaded successfully"
//                });
//            }
//            catch (Exception ex)
//            {
//                return Json(new
//                {
//                    isSuccess = false,
//                    message = ex.Message
//                });
//            }
//        }

//        [HttpPost]
//        public async Task<IActionResult> SaveFromTempToMain([FromBody] RMG_Prod_Temp_ColorSizeBreakupDto fromData)
//        {
//            if (fromData == null)
//                return Json(new { isSuccess = false, message = "Invalid Job No." });

//            await rMGProdOrderInformationEntryService.SaveFromTempToMainAsync(fromData);

//            return Json(new { isSuccess = true, message = "✅ Data saved to main table successfully!" });

//        }


//        [HttpPost]
//        public async Task<IActionResult> ClearTempData(string integraJobNo)
//        {


//            try
//            {
//                //await rMGProdOrderInformationEntryService.SaveTempDataToMainAsync(integraJobNo);
//                await rMGProdOrderInformationEntryService.ClearTempDataAsync(integraJobNo);
//                return Json(new { isSuccess = true, message = " Data transferred successfully!" });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { isSuccess = false, message = "❌ " + ex.Message });
//            }
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetmerchandiserContactPersonList()
//        {
//            try
//            {
//                var data = await rMGProdOrderInformationEntryService.GetMerchandiserContactPersonListAsync();

//                return Json(new
//                {
//                    isSuccess = true,
//                    message = " Data transferred successfully!",
//                    data = data
//                });
//            }
//            catch (Exception ex)
//            {
//                return Json(new
//                {
//                    isSuccess = false,
//                    message = ex.Message
//                });
//            }
//        }

//        public IActionResult GetPortList()
//        {
//            var data = portRepo.GetAll()
//                .Select(x => new
//                {
//                    id = x.PortId,
//                    portName = x.PortName,
//                    portType = deliveryRepo.All().Where(s => s.DeliveryMethodId == x.DeliveryMethodId).Select(d => d.DeliveryMethodName).FirstOrDefault() ?? "",
//                    address = x.PortAddress ?? "",
//                    country = countryRepo.All().Where(s => s.CountryId == x.CountryId).Select(d => d.CountryName).FirstOrDefault() ?? ""
//                }).ToList();

//            return Json(data);
//        }



//    }
//}

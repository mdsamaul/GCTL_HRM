//using GCTL.Core.Data;
//using GCTL.Core.Helpers;
//using GCTL.Core.ViewModels.SalesDefInvMainItem;
//using GCTL.Data.Models;
//using GCTL.Service.SALES_Def_Inv_MainItemGroupService;
//using GCTL.UI.Core.ViewModels.SalesDefInvMainItem;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;

//namespace GCTL.UI.Core.Controllers
//{
//    public class SALES_Def_Inv_MainItemGroupController : BaseController
//    {
//        private readonly ISALES_Def_Inv_MainItemGroup sALES_Def_Inv_MainItemGroup;
//        private readonly IRepository<RmgProdDefBuyer> buyerRepo;
//        private readonly IRepository<ProdDefStyle> stypeRepo;
//        private readonly IRepository<SalesDefInvMainItem> mainRepo;
//        private readonly IRepository<SalesDefInvSubItem> subRepo;
//        private readonly IRepository<RmgProdDefInvSubItem2> subRepo2;
//        private readonly IRepository<InvDefItem> itemRepo;
//        private readonly IRepository<InvDefItemType> itemTypeRepo;
//        private readonly IRepository<InvDefBookingItemType> itemBookingTypeRepo;
//        private readonly IRepository<InvDefSupplierOrigin> originRepo;
//        private readonly IRepository<CaDefCountry> countryRepo;
//        private readonly IRepository<CaDefCurrency> currencyRepo;
//        private readonly IRepository<RmgProdDefPackage> packageRepo;
//        private readonly IRepository<RmgProdDefUnitType> unitRepo;
//        private readonly IRepository<RmgDefSupplier> suplierRepo;
//        private readonly IRepository<CoreBranch> branchRepo;
//        private readonly IRepository<InvDefWarehouse> wareHouseRepo;
//        private readonly IRepository<HmsLtrvPeriod> periodRepo;


//        public SALES_Def_Inv_MainItemGroupController(
//            ISALES_Def_Inv_MainItemGroup sALES_Def_Inv_MainItemGroup,
//            IRepository<RmgProdDefBuyer> buyerRepo,
//            IRepository<ProdDefStyle> stypeRepo,
//            IRepository<SalesDefInvMainItem> mainRepo,
//            IRepository<SalesDefInvSubItem> subRepo,
//            IRepository<RmgProdDefInvSubItem2> subRepo2,
//            IRepository<InvDefItem> itemRepo,
//            IRepository<InvDefItemType> itemTypeRepo,
//            IRepository<InvDefBookingItemType> itemBookingTypeRepo,
//            IRepository<InvDefSupplierOrigin> originRepo,
//            IRepository<CaDefCountry> countryRepo,
//            IRepository<CaDefCurrency> currencyRepo,
//            IRepository<RmgProdDefPackage> packageRepo,
//            IRepository<RmgProdDefUnitType> unitRepo,
//            IRepository<RmgDefSupplier> suplierRepo,
//            IRepository<CoreBranch> branchRepo,
//            IRepository<InvDefWarehouse> wareHouseRepo,
//            IRepository<HmsLtrvPeriod> periodRepo
//            )
//        {
//            this.sALES_Def_Inv_MainItemGroup = sALES_Def_Inv_MainItemGroup;
//            this.buyerRepo = buyerRepo;
//            this.stypeRepo = stypeRepo;
//            this.mainRepo = mainRepo;
//            this.subRepo = subRepo;
//            this.subRepo2 = subRepo2;
//            this.itemRepo = itemRepo;
//            this.itemTypeRepo = itemTypeRepo;
//            this.itemBookingTypeRepo = itemBookingTypeRepo;
//            this.originRepo = originRepo;
//            this.countryRepo = countryRepo;
//            this.currencyRepo = currencyRepo;
//            this.packageRepo = packageRepo;
//            this.unitRepo = unitRepo;
//            this.suplierRepo = suplierRepo;
//            this.branchRepo = branchRepo;
//            this.wareHouseRepo = wareHouseRepo;
//            this.periodRepo = periodRepo;
//        }
//        public async Task<IActionResult> Index()
//        {
//            try
//            {
//                var hasPermission = await sALES_Def_Inv_MainItemGroup.PagePermissionAsync(LoginInfo.AccessCode);

//                if (!hasPermission)

//                {

//                    return RedirectToAction("Login", "Accounts");

//                }


//                ViewBag.Buyer = new SelectList(buyerRepo.All().Select(x => new { x.BuyerId, x.BuyerName }), "BuyerId", "BuyerName");
//                ViewBag.Style = new SelectList(stypeRepo.All().Select(x => new { x.StyleId, x.Style }), "StyleId", "Style");
//                ViewBag.MainGroup = new SelectList(mainRepo.All().Select(x => new { x.MainItemId, x.MainItemName }), "MainItemId", "MainItemName");
//                ViewBag.SubGroup = new SelectList(subRepo.All().Select(x => new { x.SubItemId, x.SubItemName }), "SubItemId", "SubItemName");
//                ViewBag.SubGroup2List = new SelectList(subRepo2.All().Select(x => new { x.SubItem2Id, x.SubItem2Name }), "SubItem2Id", "SubItem2Name");
//                ViewBag.ItemList = new SelectList(itemRepo.All().Select(x => new { x.ItemId, x.ItemName }), "ItemId", "ItemName");
//                ViewBag.ItemTypeList = new SelectList(itemBookingTypeRepo.All().Select(x => new { x.BookingItemTypeId, x.BookingItemType }), "BookingItemTypeId", "BookingItemType");
//                ViewBag.OriginList = new SelectList(originRepo.All().Select(x => new { x.SupplierOriginId, x.SupplierOrigin }), "SupplierOriginId", "SupplierOrigin");
//                ViewBag.CountryList = new SelectList(countryRepo.All().Select(x => new { x.CountryId, x.CountryName }), "CountryId", "CountryName");
//                ViewBag.CurrencyList = new SelectList(currencyRepo.All().Select(x => new { x.CurrencyId, x.ShortName }), "CurrencyId", "ShortName");
//                ViewBag.PackageList = new SelectList(packageRepo.All().Select(x => new { x.PackageId, x.PackageName }), "PackageId", "PackageName");
//                ViewBag.UnitTypeList = new SelectList(unitRepo.All().Select(x => new { x.UnitTypId, x.UnitTypeName }), "UnitTypId", "UnitTypeName");
//                ViewBag.SuplierList = new SelectList(suplierRepo.All().Select(x => new { x.SupplierId, x.SupplierName }), "SupplierId", "SupplierName");
//                ViewBag.BranchList = new SelectList(branchRepo.All().Select(x => new { x.BranchCode, x.BranchName }), "BranchCode", "BranchName");
//                ViewBag.WareHouseList = new SelectList(wareHouseRepo.All().Select(x => new { x.WarehouseId, x.WarehouseName }), "WarehouseId", "WarehouseName");
//                ViewBag.wPeriodList = new SelectList(periodRepo.All().Select(x => new { x.PeriodId, x.PeriodName }), "PeriodId", "PeriodName");
//                SalesDefInvMainItemDto model = new SalesDefInvMainItemDto()
//                {
//                    PageUrl = Url.Action(nameof(Index)),
//                };

//                return View(model);
//            }
//            catch (Exception)
//            {

//                throw;
//            }

//        }

//        [HttpPost]
//        public async Task<IActionResult> AdddEditMainSetup([FromBody] SalesDefInvMainItemSetupDto mainDto)
//        {
//            try
//            {
//                mainDto.ToAudit(LoginInfo);
//                if (mainDto.TC == 0)
//                {
//                    bool hasParmision = await sALES_Def_Inv_MainItemGroup.SavePermissionAsync(LoginInfo.AccessCode);
//                    if (hasParmision)
//                    {
//                        var result = await sALES_Def_Inv_MainItemGroup.AdddEditMainSetupAsync(mainDto, LoginInfo.CompanyCode);
//                        return Json(new { isSuccess = result.isSuccess, message = result.message });
//                    }
//                    else
//                    {
//                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
//                    }
//                }
//                else
//                {
//                    var hasUpdatePermission = await sALES_Def_Inv_MainItemGroup.UpdatePermissionAsync(LoginInfo.AccessCode);
//                    if (hasUpdatePermission)
//                    {
//                        var result = await sALES_Def_Inv_MainItemGroup.AdddEditMainSetupAsync(mainDto, LoginInfo.CompanyCode);
//                        return Json(new { isSuccess = result.isSuccess, message = result.message });
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
//        public async Task<IActionResult> AdddEditSubSetup([FromBody] SalesDefInvSubItemDto modelData)
//        {
//            try
//            {
//                modelData.ToAudit(LoginInfo);
//                if (modelData.TC == 0)
//                {
//                    bool hasParmision = await sALES_Def_Inv_MainItemGroup.SavePermissionAsync(LoginInfo.AccessCode);
//                    if (hasParmision)
//                    {
//                        var result = await sALES_Def_Inv_MainItemGroup.AdddEditSubSetupAsync(modelData, LoginInfo.CompanyCode);
//                        return Json(new { isSuccess = result.isSuccess, message = result.message });
//                    }
//                    else
//                    {
//                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
//                    }
//                }
//                else
//                {
//                    var hasUpdatePermission = await sALES_Def_Inv_MainItemGroup.UpdatePermissionAsync(LoginInfo.AccessCode);
//                    if (hasUpdatePermission)
//                    {
//                        var result = await sALES_Def_Inv_MainItemGroup.AdddEditSubSetupAsync(modelData, LoginInfo.CompanyCode);
//                        return Json(new { isSuccess = result.isSuccess, message = result.message });
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
//        public async Task<IActionResult> AdddEditSubTwoSetup([FromBody] RmgProdDefInvSubItem2Dto modelData)
//        {
//            try
//            {
//                modelData.ToAudit(LoginInfo);
//                if (modelData.TC == 0)
//                {
//                    bool hasParmision = await sALES_Def_Inv_MainItemGroup.SavePermissionAsync(LoginInfo.AccessCode);
//                    if (hasParmision)
//                    {
//                        var result = await sALES_Def_Inv_MainItemGroup.AdddEditSubTwoSetupAsync(modelData, LoginInfo.CompanyCode);
//                        return Json(new { isSuccess = result.isSuccess, message = result.message });
//                    }
//                    else
//                    {
//                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
//                    }
//                }
//                else
//                {
//                    var hasUpdatePermission = await sALES_Def_Inv_MainItemGroup.UpdatePermissionAsync(LoginInfo.AccessCode);
//                    if (hasUpdatePermission)
//                    {
//                        var result = await sALES_Def_Inv_MainItemGroup.AdddEditSubTwoSetupAsync(modelData, LoginInfo.CompanyCode);
//                        return Json(new { isSuccess = result.isSuccess, message = result.message });
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
//        public async Task<IActionResult> AdddEditItemInfoSetup([FromBody] InvDefItemDto modelData)
//        {
//            try
//            {
//                modelData.ToAudit(LoginInfo);
//                if (modelData.TC == 0)
//                {
//                    bool hasParmision = await sALES_Def_Inv_MainItemGroup.SavePermissionAsync(LoginInfo.AccessCode);
//                    if (hasParmision)
//                    {
//                        var result = await sALES_Def_Inv_MainItemGroup.AdddEditItemInfoSetupAsync(modelData, LoginInfo.CompanyCode);
//                        return Json(new { isSuccess = result.isSuccess, message = result.message });
//                    }
//                    else
//                    {
//                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
//                    }
//                }
//                else
//                {
//                    var hasUpdatePermission = await sALES_Def_Inv_MainItemGroup.UpdatePermissionAsync(LoginInfo.AccessCode);
//                    if (hasUpdatePermission)
//                    {
//                        var result = await sALES_Def_Inv_MainItemGroup.AdddEditItemInfoSetupAsync(modelData, LoginInfo.CompanyCode);
//                        return Json(new { isSuccess = result.isSuccess, message = result.message });
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
//        public async Task<IActionResult> StockLevelManagementSetup([FromBody] DefInvStockLevelManagementDto modelData)
//        {
//            try
//            {
//                modelData.ToAudit(LoginInfo);
//                if (modelData.TC == 0)
//                {
//                    bool hasParmision = await sALES_Def_Inv_MainItemGroup.SavePermissionAsync(LoginInfo.AccessCode);
//                    if (hasParmision)
//                    {
//                        var result = await sALES_Def_Inv_MainItemGroup.StockLevelManagementSetupAsync(modelData, LoginInfo.CompanyCode);
//                        return Json(new { isSuccess = result.isSuccess, message = result.message });
//                    }
//                    else
//                    {
//                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
//                    }
//                }
//                else
//                {
//                    var hasUpdatePermission = await sALES_Def_Inv_MainItemGroup.UpdatePermissionAsync(LoginInfo.AccessCode);
//                    if (hasUpdatePermission)
//                    {
//                        var result = await sALES_Def_Inv_MainItemGroup.StockLevelManagementSetupAsync(modelData, LoginInfo.CompanyCode);
//                        return Json(new { isSuccess = result.isSuccess, message = result.message });
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
//        public async Task<IActionResult> GetAutoAllId([FromBody] string tabName)
//        {
//            var resultId = await sALES_Def_Inv_MainItemGroup.GetAutoAllIdAsync(tabName);
//            return Json(resultId);
//        }


//        [HttpPost]
//        public async Task<IActionResult> LoadMainGroupData()
//        {
//            try
//            {
//                var draw = Request.Form["draw"].FirstOrDefault();
//                var start = Request.Form["start"].FirstOrDefault();
//                var length = Request.Form["length"].FirstOrDefault();
//                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
//                var sortColumnDir = Request.Form["order[0][dir]"].FirstOrDefault();
//                var searchValue = Request.Form["search[value]"].FirstOrDefault();

//                string sortColumn = "";
//                if (!string.IsNullOrEmpty(sortColumnIndex))
//                {
//                    switch (int.Parse(sortColumnIndex))
//                    {
//                        case 0:
//                            sortColumn = "MainItemId";
//                            break;
//                        case 1:
//                            sortColumn = "MainItemId";
//                            break;
//                        case 2:
//                            sortColumn = "MainItemName";
//                            break;
//                        case 3:
//                            sortColumn = "Description";
//                            break;
//                        default:
//                            sortColumn = "MainItemId";
//                            break;
//                    }
//                }

//                int pageSize = !string.IsNullOrEmpty(length) ? Convert.ToInt32(length) : 10;
//                int skip = !string.IsNullOrEmpty(start) ? Convert.ToInt32(start) : 0;

//                var (data, totalRecords) = await sALES_Def_Inv_MainItemGroup.GetMainGroup(sortColumn, sortColumnDir, searchValue, skip, pageSize);

//                var result = new
//                {
//                    draw = draw,
//                    recordsFiltered = totalRecords,
//                    recordsTotal = totalRecords,
//                    data = data
//                };

//                return Json(result);
//            }
//            catch (Exception ex)
//            {
//                return Json(new { error = ex.Message, draw = Request.Form["draw"].FirstOrDefault() });
//            }
//        }

//        //deleteItem
//        [HttpPost]
//        public async Task<IActionResult> DeleteItemUrl([FromBody] DeleteItemRequest modelData)
//        {
//            try
//            {
//                var hasUpdatePermission = await sALES_Def_Inv_MainItemGroup.DeletePermissionAsync(LoginInfo.AccessCode);
//                if (hasUpdatePermission)
//                {
//                    var result = await sALES_Def_Inv_MainItemGroup.DeleteAsync(modelData);
//                    return Json(new { isSuccess = result.isSuccess, message = result.message });
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
//        public async Task<IActionResult> ChangeAbleDropdown([FromBody] SelectDropdownFilterDto filterData)
//        {
//            try
//            {
//                var result = await sALES_Def_Inv_MainItemGroup.ChangeAbleDropdownAsync(filterData);
//                return Json(result);
//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }


//        [HttpGet]
//        public IActionResult LoadDropdowns()
//        {
//            try
//            {
//                var data = new
//                {
//                    //Buyer = buyerRepo.All().Select(x => new { x.BuyerId, x.BuyerName }).ToList(),
//                    //Style = stypeRepo.All().Select(x => new { x.StyleId, x.Style }).ToList(),
//                    MainGroup = mainRepo.All().Select(x => new { x.MainItemId, x.MainItemName }).ToList(),
//                    SubGroup = subRepo.All().Select(x => new { x.SubItemId, x.SubItemName }).ToList(),
//                    SubGroup2 = subRepo2.All().Select(x => new { x.SubItem2Id, x.SubItem2Name }).ToList(),
//                    Item = itemRepo.All().Select(x => new { x.ItemId, x.ItemName }).ToList(),
//                    //ItemType = itemTypeRepo.All().Select(x => new { x.ItemTypeId, x.ItemName }).ToList(),
//                    //Origin = originRepo.All().Select(x => new { x.SupplierOriginId, x.SupplierOrigin }).ToList(),
//                    //Country = countryRepo.All().Select(x => new { x.CountryId, x.CountryName }).ToList(),
//                    //Currency = currencyRepo.All().Select(x => new { x.CurrencyId, x.ShortName }).ToList(),
//                    //Package = packageRepo.All().Select(x => new { x.PackageId, x.PackageName }).ToList(),
//                    //UnitType = unitRepo.All().Select(x => new { x.UnitTypId, x.UnitTypeName }).ToList(),
//                    //Supplier = suplierRepo.All().Select(x => new { x.SupplierId, x.SupplierName }).ToList(),
//                    //Branch = branchRepo.All().Select(x => new { x.BranchCode, x.BranchName }).ToList(),
//                    //Warehouse = wareHouseRepo.All().Select(x => new { x.WarehouseId, x.WarehouseName }).ToList(),
//                };

//                return Json(data);
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//        }
//        [HttpPost]
//        public async Task<IActionResult> Upload(ItemPhotoDto dto)
//        {

//            try
//            {
//                dto.ToAudit(LoginInfo);
//                if (dto.AutoId == 0)
//                {
//                    bool hasParmision = await sALES_Def_Inv_MainItemGroup.SavePermissionAsync(LoginInfo.AccessCode);
//                    if (hasParmision)
//                    {
//                        if (dto.Photo == null || dto.Photo.Length == 0)
//                            return BadRequest("No file uploaded.");

//                        bool result = await sALES_Def_Inv_MainItemGroup.UploadPhotoAsync(dto);

//                        if (result)
//                            return Ok(new { success = true, message = "Photo uploaded successfully!" });
//                        else
//                            return BadRequest("Upload failed.");
//                    }
//                    else
//                    {
//                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
//                    }
//                }
//                else
//                {
//                    var hasUpdatePermission = await sALES_Def_Inv_MainItemGroup.UpdatePermissionAsync(LoginInfo.AccessCode);
//                    if (hasUpdatePermission)
//                    {

//                        if (dto.Photo == null || dto.Photo.Length == 0)
//                            return BadRequest("No file uploaded.");

//                        bool result = await sALES_Def_Inv_MainItemGroup.UploadPhotoAsync(dto);

//                        if (result)
//                            return Ok(new { success = true, message = "Photo uploaded successfully!" });
//                        else
//                            return BadRequest("Upload failed.");
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
//        public async Task<IActionResult> Delete(string itemId)
//        {

//            try
//            {
//                var hasUpdatePermission = await sALES_Def_Inv_MainItemGroup.DeletePermissionAsync(LoginInfo.AccessCode);
//                if (hasUpdatePermission)
//                {
//                    bool result = await sALES_Def_Inv_MainItemGroup.DeletePhotoAsync(itemId);
//                    if (result)
//                        return Ok(new { success = true, message = "Photo deleted successfully!" });
//                    else
//                        return BadRequest("Delete failed.");
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
//        [HttpGet]
//        public async Task<IActionResult> GetPhoto(string itemId)
//        {
//            var photo = await sALES_Def_Inv_MainItemGroup.GetPhotoByItemIdAsync(itemId);
//            if (photo == null || photo.PhotoBytes == null)
//            {
//                return Json(false);
//            }

//            return File(photo.PhotoBytes, photo.ImgType);
//        }
//        //[HttpPost("GetItemHierarchy")]
//        public async Task<IActionResult> GetItemHierarchy([FromBody] ItemHierarchyFilterDto filter)
//        {
//            var data = await sALES_Def_Inv_MainItemGroup.GetItemHierarchyAsync(filter);

//            return Ok(new
//            {
//                data
//            });
//        }
//    }
//}

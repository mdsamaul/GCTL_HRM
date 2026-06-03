//using Dapper;
//using GCTL.Core.Data;
//using GCTL.Core.ViewModels.Common;
//using GCTL.Core.ViewModels.SalesDefInvMainItem;
//using GCTL.Data.Models;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using System.Data;
//using System.Data.SqlClient;

//namespace GCTL.Service.SALES_Def_Inv_MainItemGroupService
//{
//    public class SALES_Def_Inv_MainItemGroup : AppService<InvDefItem>, ISALES_Def_Inv_MainItemGroup
//    {
//        private readonly IRepository<RmgProdDefBuyer> buyerRepo;
//        private readonly IRepository<ProdDefStyle> stypeRepo;
//        private readonly IRepository<SalesDefInvMainItem> mainRepo;
//        private readonly IRepository<SalesDefInvSubItem> subRepo;
//        private readonly IRepository<RmgProdDefInvSubItem2> subRepo2;
//        private readonly IRepository<InvDefItem> itemRepo;
//        private readonly IRepository<DefInvStockLevelManagement> stockManagementRepo;
//        private readonly IRepository<InvDefItemType> itemTypeRepo;
//        private readonly IRepository<InvDefBookingItemType> itemBookingTypeRepo;
//        private readonly IRepository<InvDefSupplierOrigin> originRepo;
//        private readonly IRepository<CaDefCountry> countryRepo;
//        private readonly IRepository<CaDefCurrency> currencyRepo;
//        private readonly IRepository<RmgProdDefPackage> packageRepo;
//        private readonly IRepository<RmgProdDefUnitType> unitRepo;
//        private readonly IRepository<RmgDefSupplier> suplierRepo;
//        private readonly IRepository<CoreBranch> branchRepo;
//        private readonly IRepository<CoreAccessCode> accessCodeRepository;
//        private readonly IRepository<InvItemPhoto> photoRepo;
//        private readonly IRepository<RmgDefSupplier> supplierRepo;
//        private readonly IRepository<InvDefWarehouse> wareHouseRepo;
//        private readonly string _connectionString;
//        public SALES_Def_Inv_MainItemGroup(
//            IRepository<RmgProdDefBuyer> buyerRepo,
//            IRepository<ProdDefStyle> stypeRepo,
//            IRepository<SalesDefInvMainItem> mainRepo,
//            IRepository<SalesDefInvSubItem> subRepo,
//            IRepository<RmgProdDefInvSubItem2> subRepo2,
//            IRepository<InvDefItem> itemRepo,
//            IRepository<DefInvStockLevelManagement> stockManagementRepo,
//            IRepository<InvDefItemType> itemTypeRepo,
//            IRepository<InvDefSupplierOrigin> originRepo,
//            IRepository<CaDefCountry> countryRepo,
//            IRepository<CaDefCurrency> currencyRepo,
//            IRepository<RmgProdDefPackage> packageRepo,
//            IRepository<RmgProdDefUnitType> unitRepo,
//            IRepository<RmgDefSupplier> suplierRepo,
//            IRepository<CoreBranch> branchRepo,
//            IRepository<CoreAccessCode> accessCodeRepository,
//            IRepository<InvItemPhoto> photoRepo,
//            IConfiguration configuration,
//            IRepository<RmgDefSupplier> supplierRepo,
//            IRepository<InvDefWarehouse> wareHouseRepo,
//            IRepository<InvDefBookingItemType> itemBookingTypeRepo) : base(itemRepo)
//        {
//            this.buyerRepo = buyerRepo;
//            this.stypeRepo = stypeRepo;
//            this.mainRepo = mainRepo;
//            this.subRepo = subRepo;
//            this.subRepo2 = subRepo2;
//            this.itemRepo = itemRepo;
//            this.stockManagementRepo = stockManagementRepo;
//            this.itemTypeRepo = itemTypeRepo;
//            this.originRepo = originRepo;
//            this.countryRepo = countryRepo;
//            this.currencyRepo = currencyRepo;
//            this.packageRepo = packageRepo;
//            this.unitRepo = unitRepo;
//            this.suplierRepo = suplierRepo;
//            this.branchRepo = branchRepo;
//            this.accessCodeRepository = accessCodeRepository;
//            this.photoRepo = photoRepo;
//            this.supplierRepo = supplierRepo;
//            this.wareHouseRepo = wareHouseRepo;
//            _connectionString = configuration.GetConnectionString("ApplicationDbConnection");
//            this.itemBookingTypeRepo = itemBookingTypeRepo;
//        }
//        private readonly string CreateSuccess = "Data saved successfully.";
//        private readonly string CreateFailed = "Data insertion failed.";
//        private readonly string UpdateSuccess = "Data updated successfully.";
//        private readonly string UpdateFailed = "Data update failed.";
//        private readonly string DeleteSuccess = "Data deleted successfully.";
//        private readonly string DeleteFailed = "Data deletion failed.";
//        private readonly string DataExists = "Data already exists.";

//        #region Permission all type

//        public async Task<bool> PagePermissionAsync(string accessCode)

//        {

//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "SALES Def Inv MainItem" && x.TitleCheck);

//        }

//        public async Task<bool> SavePermissionAsync(string accessCode)

//        {

//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "SALES Def Inv MainItem" && x.CheckAdd);

//        }

//        public async Task<bool> UpdatePermissionAsync(string accessCode)

//        {

//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "SALES Def Inv MainItem" && x.CheckEdit);

//        }

//        public async Task<bool> DeletePermissionAsync(string accessCode)

//        {

//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "SALES Def Inv MainItem" && x.CheckDelete);

//        }

//        #endregion

//        public async Task<(bool isSuccess, string message)> AdddEditMainSetupAsync(SalesDefInvMainItemSetupDto modelData, string companyCode)
//        {
//            try
//            {

//                if (modelData.MainItemID == null || modelData.MainItemID == "" || modelData.MainItemName == null || modelData.MainItemName == "")
//                {
//                    return (false, CreateFailed);
//                }
//                if (modelData.TC == 0)
//                {
//                    bool exists = mainRepo.All().Any(x => x.MainItemName == modelData.MainItemName);
//                    if (exists)
//                    {
//                        return (false, DataExists);
//                    }
//                    //create
//                    var result = new SalesDefInvMainItem
//                    {
//                        MainItemId = modelData.MainItemID ?? "",
//                        MainItemName = modelData.MainItemName ?? "",
//                        Description = modelData.Description ?? "",
//                        Ldate = modelData.Ldate,
//                        Lmac = modelData.Lmac ?? "",
//                        Lip = modelData.Lip ?? "",
//                        Luser = modelData.Luser ?? "",
//                        CompanyCode = companyCode ?? "",
//                    };
//                    await mainRepo.AddAsync(result);
//                    return (true, CreateSuccess);
//                }
//                else
//                {
//                    //update
//                    var mainItem = mainRepo.GetById(modelData.TC);
//                    if (mainItem == null)
//                    {
//                        return (false, UpdateFailed);
//                    }
//                    mainItem.MainItemId = modelData.MainItemID ?? "";
//                    mainItem.MainItemName = modelData.MainItemName ?? "";
//                    mainItem.Description = modelData.Description ?? "";
//                    mainItem.ModifyDate = DateTime.Now;
//                    await mainRepo.UpdateAsync(mainItem);
//                    return (true, UpdateSuccess);
//                }

//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }
//        public async Task<(bool isSuccess, string message)> AdddEditSubSetupAsync(SalesDefInvSubItemDto modelData, string companyCode)
//        {
//            try
//            {

//                if (modelData.SubItemID == null || modelData.SubItemID == "" || modelData.SubItemName == null || modelData.SubItemName == "")
//                {
//                    return (false, CreateFailed);
//                }
//                if (modelData.TC == 0)
//                {
//                    bool exists = subRepo.All().Any(x => x.SubItemName == modelData.SubItemName);
//                    if (exists)
//                    {
//                        return (false, DataExists);
//                    }
//                    //create
//                    var result = new SalesDefInvSubItem
//                    {
//                        SubItemId = modelData.SubItemID ?? "",
//                        MainItemId = modelData.MainItemID ?? "",
//                        SubItemName = modelData.SubItemName ?? "",
//                        Description = modelData.Description ?? "",
//                        Ldate = modelData.Ldate,
//                        Lmac = modelData.Lmac ?? "",
//                        Lip = modelData.Lip ?? "",
//                        Luser = modelData.Luser ?? "",
//                        CompanyCode = companyCode ?? "",
//                    };
//                    await subRepo.AddAsync(result);
//                    return (true, CreateSuccess);
//                }
//                else
//                {
//                    //update
//                    var item = subRepo.GetById(modelData.TC);
//                    if (item == null)
//                    {
//                        return (false, UpdateFailed);
//                    }
//                    item.SubItemId = modelData.SubItemID ?? "";
//                    item.MainItemId = modelData.MainItemID ?? "";
//                    item.SubItemName = modelData.SubItemName ?? "";
//                    item.Description = modelData.Description ?? "";
//                    item.ModifyDate = DateTime.Now;
//                    await subRepo.UpdateAsync(item);
//                    return (true, UpdateSuccess);
//                }

//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }
//        public async Task<(bool isSuccess, string message)> AdddEditSubTwoSetupAsync(RmgProdDefInvSubItem2Dto modelData, string companyCode)
//        {
//            try
//            {

//                if (modelData.SubItem2ID == null || modelData.SubItem2ID == "" || modelData.SubItem2Name == null || modelData.SubItem2Name == "")
//                {
//                    return (false, CreateFailed);
//                }
//                if (modelData.TC == 0)
//                {
//                    bool exists = subRepo2.All().Any(x => x.SubItem2Name == modelData.SubItem2Name);
//                    if (exists)
//                    {
//                        return (false, DataExists);
//                    }
//                    //create
//                    var result = new RmgProdDefInvSubItem2
//                    {
//                        SubItemId = modelData.SubItemID ?? "",
//                        MainItemId = modelData.MainItemID ?? "",
//                        SubItem2Id = modelData.SubItem2ID ?? "",
//                        SubItem2Name = modelData.SubItem2Name ?? "",
//                        Description = modelData.Description ?? "",
//                        Ldate = modelData.Ldate,
//                        Lmac = modelData.Lmac ?? "",
//                        Lip = modelData.Lip ?? "",
//                        Luser = modelData.Luser ?? "",
//                        CompanyCode = companyCode ?? "",
//                    };
//                    await subRepo2.AddAsync(result);
//                    return (true, CreateSuccess);
//                }
//                else
//                {
//                    //update
//                    var item = subRepo2.GetById(modelData.TC);
//                    if (item == null)
//                    {
//                        return (false, UpdateFailed);
//                    }
//                    item.SubItemId = modelData.SubItemID ?? "";
//                    item.MainItemId = modelData.MainItemID ?? "";
//                    item.SubItem2Id = modelData.SubItem2ID ?? "";
//                    item.SubItem2Name = modelData.SubItem2Name ?? "";
//                    item.Description = modelData.Description ?? "";
//                    item.ModifyDate = DateTime.Now;
//                    await subRepo2.UpdateAsync(item);
//                    return (true, UpdateSuccess);
//                }

//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }


//        public async Task<(bool isSuccess, string message)> AdddEditItemInfoSetupAsync(InvDefItemDto modelData, string companyCode)
//        {
//            try
//            {

//                if (modelData.ItemID == null || modelData.ItemID == ""
//                    || modelData.ItemName == null || modelData.ItemName == ""
//                    || modelData.ItemQuantity <= 0 || modelData.ItemPrice <= 0
//                    || modelData.ItemUnit == null || modelData.ItemID == ""
//                    || modelData.CurrencyId == null || modelData.CurrencyId == ""
//                    )
//                {
//                    return (false, CreateFailed);
//                }
//                if (modelData.TC == 0)
//                {
//                    bool exists = itemRepo.All().Any(x => x.ItemName == modelData.ItemName);
//                    if (exists)
//                    {
//                        return (false, DataExists);
//                    }
//                    //create
//                    var result = new InvDefItem
//                    {
//                        BuyerId = modelData.BuyerId ?? "",
//                        StyleId = modelData.StyleId ?? "",
//                        MainItemId = modelData.MainItemID ?? "",
//                        SubItemId = modelData.SubItemID ?? "",
//                        SubItem2Id = modelData.SubItem2ID ?? "",
//                        ItemTypeId = modelData.ItemTypeID ?? "",
//                        ItemId = modelData.ItemID ?? "",
//                        ItemName = modelData.ItemName ?? "",
//                        PrintName = modelData.PrintName ?? "",
//                        TechnicalSpecification = modelData.TechnicalSpecification ?? "",
//                        //discription= modelData.discription??"",
//                        ItemCode = modelData.ItemCode ?? "",
//                        Barcode = modelData.Barcode ?? "",
//                        OriginId = modelData.OriginId ?? "",
//                        ManufactureId = modelData.ManufactureId ?? "",
//                        PackageTypeId = modelData.PackageTypeId ?? "",
//                        PackageQuantity = modelData.PackageQuantity ?? 0,
//                        ItemQuantity = modelData.ItemQuantity ?? 0,
//                        ItemUnit = modelData.ItemUnit ?? "",
//                        ItemPrice = modelData.ItemPrice ?? 0,
//                        CurrencyId = modelData.CurrencyId ?? "",
//                        Discount = modelData.Discount ?? 0,
//                        TotalAmount = modelData.TotalAmount ?? 0,
//                        CurrencyId2 = modelData.CurrencyId2 ?? "",
//                        WarrantyStatus = modelData.WarrantyStatus ?? "",
//                        WarrantyTime = modelData.WarrantyTime ?? 0,
//                        WarrantyType = modelData.WarrantyType ?? "",
//                        BranchId = modelData.BranchID ?? "",
//                        SupplierId = modelData.SupplierId ?? "",
//                        EmployeeId = modelData.UserInfoEmployeeId ?? "" ?? "",
//                        //imag??""e
//                        Ldate = modelData.Ldate,
//                        Lmac = modelData.Lmac ?? "",
//                        Lip = modelData.Lip ?? "",
//                        Luser = modelData.Luser ?? "",
//                        CompanyCode = companyCode ?? "",
//                    };
//                    await itemRepo.AddAsync(result);
//                    return (true, CreateSuccess);
//                }
//                else
//                {
//                    //update
//                    var item = itemRepo.GetById(modelData.TC);
//                    if (item == null)
//                    {
//                        return (false, UpdateFailed);
//                    }

//                    item.BuyerId = modelData.BuyerId ?? "";
//                    item.StyleId = modelData.StyleId ?? "";
//                    item.MainItemId = modelData.MainItemID ?? "";
//                    item.SubItemId = modelData.SubItemID ?? "";
//                    item.SubItem2Id = modelData.SubItem2ID ?? "";
//                    item.ItemTypeId = modelData.ItemTypeID ?? "";
//                    item.ItemId = modelData.ItemID ?? "";
//                    item.ItemName = modelData.ItemName ?? "";
//                    item.PrintName = modelData.PrintName ?? "";
//                    item.TechnicalSpecification = modelData.TechnicalSpecification ?? "";
//                    item.ItemCode = modelData.ItemCode ?? "";
//                    item.Barcode = modelData.Barcode ?? "";
//                    item.OriginId = modelData.OriginId ?? "";
//                    item.ManufactureId = modelData.ManufactureId ?? "";
//                    item.PackageTypeId = modelData.PackageTypeId ?? "";
//                    item.PackageQuantity = modelData.PackageQuantity ?? 0;
//                    item.ItemQuantity = modelData.ItemQuantity ?? 0;
//                    item.ItemUnit = modelData.ItemUnit ?? "";
//                    item.ItemPrice = modelData.ItemPrice ?? 0;
//                    item.CurrencyId = modelData.CurrencyId ?? "";
//                    item.Discount = modelData.Discount ?? 0;
//                    item.TotalAmount = modelData.TotalAmount ?? 0;
//                    item.CurrencyId2 = modelData.CurrencyId2 ?? "";
//                    item.WarrantyStatus = modelData.WarrantyStatus ?? "";
//                    item.WarrantyTime = modelData.WarrantyTime ?? 0;
//                    item.WarrantyType = modelData.WarrantyType ?? "";
//                    item.BranchId = modelData.BranchID ?? "";
//                    item.SupplierId = modelData.SupplierId ?? "";
//                    item.EmployeeId = modelData.UserInfoEmployeeId ?? "";
//                    item.ModifyDate = DateTime.Now;
//                    await itemRepo.UpdateAsync(item);
//                    return (true, UpdateSuccess);
//                }

//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }

//        public async Task<(bool isSuccess, string message)> StockLevelManagementSetupAsync(DefInvStockLevelManagementDto modelData, string companyCode)
//        {
//            try
//            {

//                if (modelData.SLMID == null || modelData.SLMID == ""
//                    || modelData.ItemID == null || modelData.ItemID == ""
//                    || modelData.WarehouseID == null || modelData.SLMID == ""
//                    || modelData.InStock == null || modelData.InStock <= 0
//                    || modelData.StockValue == null || modelData.StockValue <= 0
//                    //|| modelData.MinStock == null || modelData.MinStock <= 0
//                    //|| modelData.MaxStock == null || modelData.MaxStock <= 0
//                    )
//                {
//                    return (false, CreateFailed);
//                }
//                if (modelData.TC == 0)
//                {
//                    //bool exists = stockManagementRepo.All().Any(x => x.ItemId == modelData.ItemID && x.WarehouseId == modelData.WarehouseID);
//                    //if (exists)
//                    //{
//                    //    return (false, DataExists);
//                    //}
//                    //create
//                    var result = new DefInvStockLevelManagement
//                    {
//                        Slmid = modelData.SLMID ?? "",
//                        ItemId = modelData.ItemID ?? "",
//                        WarehouseId = modelData.WarehouseID ?? "",
//                        InStock = modelData.InStock ?? 0,
//                        StockValue = modelData.StockValue ?? 0,
//                        ReorderLevel = modelData.ReorderLevel ?? 0,
//                        MaxStock = modelData.MaxStock ?? 0,
//                        MinStock = modelData.MinStock ?? 0,
//                        Description = modelData.Description ?? "",
//                        EmployeeId = modelData.UserInfoEmployeeId ?? "",
//                        Ldate = modelData.Ldate,
//                        Lmac = modelData.Lmac ?? "",
//                        Lip = modelData.Lip ?? "",
//                        Luser = modelData.Luser ?? "",
//                        CompanyCode = companyCode ?? "",
//                    };
//                    await stockManagementRepo.AddAsync(result);
//                    return (true, CreateSuccess);
//                }
//                else
//                {
//                    //update
//                    var item = stockManagementRepo.GetById(modelData.TC);
//                    if (item == null)
//                    {
//                        return (false, UpdateFailed);
//                    }

//                    item.Slmid = modelData.SLMID ?? "";
//                    item.ItemId = modelData.ItemID ?? "";
//                    item.WarehouseId = modelData.WarehouseID ?? "";
//                    item.InStock = modelData.InStock ?? 0;
//                    item.StockValue = modelData.StockValue ?? 0;
//                    item.ReorderLevel = modelData.ReorderLevel ?? 0;
//                    item.MaxStock = modelData.MaxStock ?? 0;
//                    item.MinStock = modelData.MinStock ?? 0;
//                    item.EmployeeId = modelData.UserInfoEmployeeId ?? "";
//                    item.Description = modelData.Description ?? "";
//                    item.ModifyDate = DateTime.Now;
//                    await stockManagementRepo.UpdateAsync(item);
//                    return (true, UpdateSuccess);
//                }

//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }








//        public async Task<string> GetAutoAllIdAsync(string tabName)
//        {
//            try
//            {
//                int lastId = 0;

//                if (tabName == "Main Group")
//                {
//                    lastId = Convert.ToInt32(mainRepo.All()
//                                     .OrderByDescending(x => x.Tc)
//                                     .Select(c => c.MainItemId)
//                                     .FirstOrDefault());
//                    return GetAutoAllName(3, lastId);
//                }
//                else if (tabName == "Sub Group")
//                {
//                    lastId = Convert.ToInt32(subRepo.All()
//                                    .OrderByDescending(x => x.Tc)
//                                    .Select(c => c.SubItemId)
//                                    .FirstOrDefault());
//                    return GetAutoAllName(3, lastId);
//                }
//                else if (tabName == "Sub Group - 2")
//                {
//                    lastId = Convert.ToInt32(subRepo2.All()
//                                     .OrderByDescending(x => x.Tc)
//                                     .Select(c => c.SubItem2Id)
//                                     .FirstOrDefault());
//                    return GetAutoAllName(3, lastId);
//                }
//                else if (tabName == "Item Information")
//                {
//                    lastId = Convert.ToInt32(itemRepo.All()
//                                     .OrderByDescending(x => x.Tc)
//                                     .Select(c => c.ItemId)
//                                     .FirstOrDefault());
//                    return GetAutoAllName(8, lastId);
//                }
//                else if (tabName == "Stock Level Management")
//                {
//                    lastId = Convert.ToInt32(stockManagementRepo.All()
//                                                .OrderByDescending(x => x.Tc)
//                                                .Select(c => c.Slmid)
//                                                .FirstOrDefault());
//                    return GetAutoAllName(3, lastId);
//                }

//                return string.Empty;
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//        }

//        public string GetAutoAllName(int d, int lastId)
//        {
//            int nextId = (lastId == 0) ? 1 : lastId + 1;
//            return nextId.ToString($"D{d}");
//        }


//        // Service Method
//        public async Task<(List<SalesDefInvMainItemSetupDto> data, int totalRecords)>
// GetMainGroup(string sortColumn, string sortColumnDir, string searchValue, int skip, int pageSize)
//        {
//            var query = mainRepo.All();

//            // Apply search filter first
//            if (!string.IsNullOrEmpty(searchValue))
//            {
//                query = query.Where(x => x.MainItemId.Contains(searchValue) ||
//                                         x.MainItemName.Contains(searchValue) ||
//                                         x.Description.Contains(searchValue));
//            }

//            // Get total count after filtering
//            int totalRecords = await query.CountAsync();

//            // Apply sorting using manual approach instead of EF.Property
//            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
//            {
//                switch (sortColumn.ToLower())
//                {
//                    case "mainitemid":
//                        query = sortColumnDir == "asc"
//                            ? query.OrderBy(x => x.MainItemId)
//                            : query.OrderByDescending(x => x.MainItemId);
//                        break;
//                    case "mainitemname":
//                        query = sortColumnDir == "asc"
//                            ? query.OrderBy(x => x.MainItemName)
//                            : query.OrderByDescending(x => x.MainItemName);
//                        break;
//                    case "description":
//                        query = sortColumnDir == "asc"
//                            ? query.OrderBy(x => x.Description)
//                            : query.OrderByDescending(x => x.Description);
//                        break;
//                    default:
//                        query = query.OrderBy(x => x.MainItemId); // Default sort
//                        break;
//                }
//            }
//            else
//            {
//                // Default sorting when no sort specified
//                query = query.OrderBy(x => x.MainItemId);
//            }

//            // Apply pagination and projection
//            var data = await query
//                .Skip(skip)
//                .Take(pageSize)
//                .Select(m => new SalesDefInvMainItemSetupDto
//                {
//                    TC = m.Tc,
//                    MainItemID = m.MainItemId,
//                    MainItemName = m.MainItemName,
//                    Description = m.Description,
//                    showCreateDate = m.Ldate.HasValue ? m.Ldate.Value.ToString("dd/MM/yyyy") : "",
//                    showModifyDate = m.ModifyDate.HasValue ? m.ModifyDate.Value.ToString("dd/MM/yyyy") : "",
//                }).ToListAsync();

//            return (data, totalRecords);
//        }
//        // Service Method


//        //  public async Task<(List<DefInvStockLevelManagementDto> data, int totalRecords)> LoadStockLevelManagementDataAsync(
//        //string sortColumn, string sortColumnDir, string searchValue, int skip, int pageSize)
//        //  {

//        //      try
//        //      {
//        //          var query = stockManagementRepo.All();

//        //          // Apply search filter first
//        //          if (!string.IsNullOrEmpty(searchValue))
//        //          {
//        //              query = query.Where(x => x.Slmid.Contains(searchValue) ||
//        //                                       x.ItemId.Contains(searchValue) ||
//        //                                       x.WarehouseId.Contains(searchValue) ||
//        //                                       x.InStock.ToString().Contains(searchValue) ||
//        //                                       x.StockValue.ToString().Contains(searchValue) ||
//        //                                       x.ReorderLevel.ToString().Contains(searchValue) ||
//        //                                       x.MaxStock.ToString().Contains(searchValue) ||
//        //                                       x.MinStock.ToString().Contains(searchValue) ||
//        //                                       x.Description.Contains(searchValue)
//        //                                       );
//        //          }

//        //          // Get total count after filtering
//        //          int totalRecords = await query.CountAsync();

//        //          // Sorting
//        //          if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
//        //          {
//        //              switch (sortColumn)
//        //              {
//        //                  case "SLMIID":
//        //                      query = sortColumnDir == "asc"
//        //                          ? query.OrderBy(x => x.Slmid)
//        //                          : query.OrderByDescending(x => x.Slmid);
//        //                      break;
//        //                  case "ItemName":
//        //                      query = sortColumnDir == "asc"
//        //                          ? query.OrderBy(x => x.ItemId)
//        //                          : query.OrderByDescending(x => x.ItemId);
//        //                      break;
//        //                  case "WareHouseName":
//        //                      query = sortColumnDir == "asc"
//        //                          ? query.OrderBy(x => x.WarehouseId)
//        //                          : query.OrderByDescending(x => x.WarehouseId);
//        //                      break;
//        //                  case "InStock":
//        //                      query = sortColumnDir == "asc"
//        //                          ? query.OrderBy(x => x.InStock)
//        //                          : query.OrderByDescending(x => x.InStock);
//        //                      break;
//        //                  case "PrintName":
//        //                      query = sortColumnDir == "asc"
//        //                          ? query.OrderBy(x => x.StockValue)
//        //                          : query.OrderByDescending(x => x.StockValue);
//        //                      break;
//        //                  case "ReorderLevel":
//        //                      query = sortColumnDir == "asc"
//        //                          ? query.OrderBy(x => x.ReorderLevel)
//        //                          : query.OrderByDescending(x => x.ReorderLevel);
//        //                      break;
//        //                  case "MaxStock":
//        //                      query = sortColumnDir == "asc"
//        //                          ? query.OrderBy(x => x.MaxStock)
//        //                          : query.OrderByDescending(x => x.MaxStock);
//        //                      break;
//        //                  case "MinStock":
//        //                      query = sortColumnDir == "asc"
//        //                          ? query.OrderBy(x => x.MinStock)
//        //                          : query.OrderByDescending(x => x.MinStock);
//        //                      break;
//        //                  case "Desctiption":
//        //                      query = sortColumnDir == "asc"
//        //                          ? query.OrderBy(x => x.Description)
//        //                          : query.OrderByDescending(x => x.Description);
//        //                      break;
//        //                  default:
//        //                      query = query.OrderBy(x => x.ItemId);
//        //                      break;
//        //              }
//        //          }
//        //          else
//        //          {
//        //              query = query.OrderBy(x => x.ItemId);
//        //          }

//        //          var data = await query
//        //              .Skip(skip)
//        //              .Take(pageSize)
//        //              .Select(m => new DefInvStockLevelManagementDto
//        //              {
//        //                  TC = m.Tc,
//        //                  SLMID = m.Slmid,
//        //                  ItemID = m.ItemId,
//        //                  ItemName = itemRepo.All().Where(x => x.ItemId == m.ItemId).Select(m => m.ItemName).FirstOrDefault(),
//        //                  WarehouseID = m.WarehouseId,
//        //                  WarehouseName = wareHouseRepo.All().Where(x => x.WarehouseId == m.WarehouseId).Select(m => m.WarehouseName).FirstOrDefault(),
//        //                  InStock = m.InStock,
//        //                  StockValue = m.StockValue,
//        //                  ReorderLevel = m.ReorderLevel,
//        //                  MaxStock = m.MaxStock,
//        //                  MinStock = m.MinStock,
//        //                  Description = m.Description,
//        //                  EmployeeID = m.EmployeeId,
//        //                  Ldate = m.Ldate,
//        //                  Lmac = m.Lmac,
//        //                  Lip = m.Lip,
//        //                  Luser = m.Luser,
//        //                  //CompanyCode = m.CompanyCode,
//        //                  //ModifyDate = m.ModifyDate,

//        //                  // Extra display fields from relations

//        //                  showCreateDate = m.Ldate.HasValue ? m.Ldate.Value.ToString("dd/MM/yyyy") : "",
//        //                  showModifyDate = m.ModifyDate.HasValue ? m.ModifyDate.Value.ToString("dd/MM/yyyy") : "",
//        //              }).ToListAsync();

//        //          return (data, totalRecords);
//        //      }
//        //      catch (Exception)
//        //      {

//        //          throw;
//        //      }

//        //  }


//        public async Task<(bool isSuccess, string message)> DeleteAsync(DeleteItemRequest modelData)
//        {
//            try
//            {
//                if (modelData.ItemIdList == null || !modelData.ItemIdList.Any())
//                {
//                    return (false, DeleteFailed);
//                }

//                bool isDeleted = false;

//                switch (modelData.TabName)
//                {
//                    case "Main Group":
//                        isDeleted = await DeletedItem(mainRepo, modelData.ItemIdList, false);
//                        break;
//                    case "Sub Group":
//                        isDeleted = await DeletedItem(subRepo, modelData.ItemIdList, false);
//                        break;
//                    case "Sub Group - 2":
//                        isDeleted = await DeletedItem(subRepo2, modelData.ItemIdList, false);
//                        break;
//                    case "Item Information":
//                        isDeleted = await DeletedItem(itemRepo, modelData.ItemIdList, false);
//                        break;
//                    case "Stock Level Management":
//                        isDeleted = await DeletedItem(stockManagementRepo, modelData.ItemIdList, true);
//                        break;
//                    default:
//                        return (false, DeleteFailed);
//                }

//                return isDeleted ? (true, DeleteSuccess) : (false, DeleteFailed);
//            }
//            catch (Exception)
//            {
//                return (false, DeleteFailed);
//            }
//        }


//        public async Task<bool> DeletedItem<T>(IRepository<T> repoName, List<string> itemList, bool isInt) where T : class
//        {
//            try
//            {
//                foreach (var id in itemList)
//                {
//                    object key;

//                    if (isInt)
//                        key = Convert.ToInt32(id);
//                    else
//                        key = Convert.ToDecimal(id);

//                    var entity = await repoName.GetByIdAsync(key);

//                    if (entity != null)
//                        await repoName.DeleteAsync(entity);
//                }
//                return true;
//            }
//            catch (Exception)
//            {
//                return false;
//            }
//        }

//        public async Task<SelectDropdownResultDto> ChangeAbleDropdownAsync(SelectDropdownFilterDto filterData)
//        {
//            try
//            {
//                var result = new SelectDropdownResultDto();
//                if (filterData.DropdownId != null || filterData.DropdownName != null || filterData.DropdownId != "" || filterData.DropdownName != "")
//                {
//                    switch (filterData.DropdownName)
//                    {
//                        case "MainItem":
//                            result = mainRepo.All().Where(x => x.MainItemId == filterData.DropdownId).Select(s => new SelectDropdownResultDto()
//                            {
//                                DeopdownId = s.MainItemId,
//                                DeopdownName = s.MainItemName,
//                                DescriptionName = s.Description,
//                                DropdownName = filterData.DropdownName
//                            }).FirstOrDefault();
//                            break;
//                        case "SubItem":
//                            result = subRepo.All().Where(x => x.SubItemId == filterData.DropdownId).Select(s => new SelectDropdownResultDto()
//                            {
//                                DeopdownId = s.SubItemId,
//                                DeopdownName = s.SubItemName,
//                                DescriptionName = s.Description,
//                                DropdownName = filterData.DropdownName
//                            }).FirstOrDefault();
//                            break;
//                        case "Sub2Item":
//                            result = subRepo2.All().Where(x => x.SubItem2Id == filterData.DropdownId).Select(s => new SelectDropdownResultDto()
//                            {
//                                DeopdownId = s.SubItem2Id,
//                                DeopdownName = s.SubItem2Name,
//                                DescriptionName = s.Description,
//                                DropdownName = filterData.DropdownName
//                            }).FirstOrDefault();
//                            break;
//                        case "supplier":
//                            result = supplierRepo.All().Where(x => x.SupplierId == filterData.DropdownId).Select(s => new SelectDropdownResultDto()
//                            {
//                                DeopdownId = s.SupplierId,
//                                DeopdownName = s.SupplierName,
//                                DescriptionName = s.Address,
//                                DropdownName = filterData.DropdownName
//                            }).FirstOrDefault();
//                            break;

//                        default:

//                            break;
//                    }


//                    return result;
//                }
//                return result;
//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }

//        //public async Task<bool> UploadPhotoAsync(ItemPhotoDto dto)
//        //{
//        //    using var stream = new MemoryStream();
//        //    await dto.Photo.CopyToAsync(stream);
//        //    byte[] imageBytes = stream.ToArray();

//        //    using var conn = new SqlConnection(_connectionString);
//        //    using var cmd = new SqlCommand(@"INSERT INTO Inv_ItemPhoto 
//        //        (ItemID, Photo, ImgType, ImgSize, CompanyCode, EmployeeID)
//        //        VALUES (@ItemID, @Photo, @ImgType, @ImgSize, @CompanyCode, @EmployeeID)", conn);

//        //    cmd.Parameters.AddWithValue("@ItemID", dto.ItemID);
//        //    cmd.Parameters.AddWithValue("@Photo", imageBytes);
//        //    cmd.Parameters.AddWithValue("@ImgType", dto.Photo.ContentType);
//        //    cmd.Parameters.AddWithValue("@ImgSize", dto.Photo.Length);
//        //    cmd.Parameters.AddWithValue("@CompanyCode", dto.CompanyCode ?? (object)DBNull.Value);
//        //    cmd.Parameters.AddWithValue("@EmployeeID", dto.EmployeeID);

//        //    await conn.OpenAsync();
//        //    int rows = await cmd.ExecuteNonQueryAsync();
//        //    return rows > 0;
//        //}

//        public async Task<bool> UploadPhotoAsync(ItemPhotoDto dto)
//        {
//            using var stream = new MemoryStream();
//            await dto.Photo.CopyToAsync(stream);
//            byte[] imageBytes = stream.ToArray();

//            using var conn = new SqlConnection(_connectionString);
//            await conn.OpenAsync();

//            // check if photo exists for ItemID
//            using (var checkCmd = new SqlCommand("SELECT COUNT(*) FROM Inv_ItemPhoto WHERE ItemID=@ItemID", conn))
//            {
//                checkCmd.Parameters.AddWithValue("@ItemID", dto.ItemID);
//                int count = (int)await checkCmd.ExecuteScalarAsync();

//                if (count > 0)
//                {
//                    // update
//                    using var updateCmd = new SqlCommand(@"
//                UPDATE Inv_ItemPhoto
//                SET Photo=@Photo, ImgType=@ImgType, ImgSize=@ImgSize, CompanyCode=@CompanyCode, EmployeeID=@EmployeeID
//                WHERE ItemID=@ItemID", conn);

//                    updateCmd.Parameters.AddWithValue("@ItemID", dto.ItemID);
//                    updateCmd.Parameters.AddWithValue("@Photo", imageBytes);
//                    updateCmd.Parameters.AddWithValue("@ImgType", dto.Photo.ContentType);
//                    updateCmd.Parameters.AddWithValue("@ImgSize", dto.Photo.Length);
//                    updateCmd.Parameters.AddWithValue("@CompanyCode", dto.CompanyCode ?? (object)DBNull.Value);
//                    updateCmd.Parameters.AddWithValue("@EmployeeID", dto.EmployeeID ?? "");

//                    int rows = await updateCmd.ExecuteNonQueryAsync();
//                    return rows > 0;
//                }
//                else
//                {
//                    // insert
//                    using var insertCmd = new SqlCommand(@"
//                INSERT INTO Inv_ItemPhoto (ItemID, Photo, ImgType, ImgSize, CompanyCode, EmployeeID)
//                VALUES (@ItemID, @Photo, @ImgType, @ImgSize, @CompanyCode, @EmployeeID)", conn);

//                    insertCmd.Parameters.AddWithValue("@ItemID", dto.ItemID);
//                    insertCmd.Parameters.AddWithValue("@Photo", imageBytes);
//                    insertCmd.Parameters.AddWithValue("@ImgType", dto.Photo.ContentType);
//                    insertCmd.Parameters.AddWithValue("@ImgSize", dto.Photo.Length);
//                    insertCmd.Parameters.AddWithValue("@CompanyCode", dto.CompanyCode ?? (object)DBNull.Value);
//                    insertCmd.Parameters.AddWithValue("@EmployeeID", dto.EmployeeID ?? "");

//                    int rows = await insertCmd.ExecuteNonQueryAsync();
//                    return rows > 0;
//                }
//            }
//        }

//        public async Task<ItemPhotoDto> GetPhotoByItemIdAsync(string itemId)
//        {
//            using var conn = new SqlConnection(_connectionString);
//            await conn.OpenAsync();

//            using var cmd = new SqlCommand(@"
//        SELECT TOP 1 ItemID, Photo, ImgType, ImgSize, CompanyCode, EmployeeID
//        FROM Inv_ItemPhoto
//        WHERE ItemID = @ItemID", conn);

//            cmd.Parameters.AddWithValue("@ItemID", itemId);

//            using var reader = await cmd.ExecuteReaderAsync();
//            if (await reader.ReadAsync())
//            {
//                return new ItemPhotoDto
//                {
//                    ItemID = reader["ItemID"].ToString(),
//                    PhotoBytes = reader["Photo"] as byte[],
//                    ImgType = reader["ImgType"].ToString(),
//                    ImgSize = Convert.ToInt64(reader["ImgSize"]),
//                    CompanyCode = reader["CompanyCode"] == DBNull.Value ? null : reader["CompanyCode"].ToString(),
//                    EmployeeID = reader["EmployeeID"].ToString()
//                };
//            }

//            return null;
//        }

//        public async Task<bool> DeletePhotoAsync(string itemId)
//        {
//            try
//            {
//                var itemPhoto = photoRepo.All().Where(x => x.ItemId == itemId).FirstOrDefault();
//                if (itemPhoto != null)
//                {
//                    await photoRepo.DeleteAsync(itemPhoto);
//                    return true;
//                }
//                else
//                {
//                    return false;
//                }
//            }
//            catch (Exception)
//            {

//                throw;
//            }
//            //using var conn = new SqlConnection(_connectionString);
//            //using var cmd = new SqlCommand("DELETE FROM Inv_ItemPhoto WHERE autoId=@id", conn);
//            //cmd.Parameters.AddWithValue("@id", itemId);

//            //await conn.OpenAsync();
//            //int rows = await cmd.ExecuteNonQueryAsync();
//            //return rows > 0;
//        }

//        public async Task<IEnumerable<CommonSelectModel>> SelectionGetItemTypeTitleAsync()
//        {
//            return await unitRepo.All()
//                     .Select(x => new CommonSelectModel
//                     {
//                         Code = x.UnitTypId,
//                         Name = x.UnitTypeName,
//                     })
//                     .ToListAsync();
//        }



//        public async Task<ItemHierarchyResponseDto> GetItemHierarchyAsync(ItemHierarchyFilterDto filter)
//        {
//            try
//            {
//                using var conn = new SqlConnection(_connectionString);

//                // === Stored Procedure Call (via Dapper) ===
//                var result = await conn.QueryAsync<dynamic>(
//                    "Get_ItemHierarchyData",
//                    new
//                    {
//                        MainId = filter.MainId,
//                        SubId = filter.SubId,
//                        Sub2Id = filter.Sub2Id,
//                        ItemId = filter.ItemId,
//                        StockItemId = filter.StockItemId
//                    },
//                    commandType: CommandType.StoredProcedure
//                );

//                var response = new ItemHierarchyResponseDto();

//                // === Cache repositories to dictionary (better performance) ===
//                var mainItems = mainRepo.All()
//                    .GroupBy(x => x.MainItemId)
//                    .Select(g => g.First())
//                    .ToDictionary(x => x.MainItemId, x => x.MainItemName);

//                var subItems = subRepo.All()
//                    .GroupBy(x => x.SubItemId)
//                    .Select(g => g.First())
//                    .ToDictionary(x => x.SubItemId, x => x.SubItemName);

//                var buyers = buyerRepo.All()
//                    .ToDictionary(x => x.BuyerId, x => x.BuyerName);

//                var itemTypes = itemBookingTypeRepo.All()
//                    .ToDictionary(x => x.BookingItemTypeId, x => x.BookingItemType);

//                var units = unitRepo.All()
//                    .ToDictionary(x => x.UnitTypId, x => x.UnitTypeName);

//                var styles = stypeRepo.All()
//                    .ToDictionary(x => x.StyleId, x => x.Style);

//                var itemsDict = itemRepo.All()
//                    .ToDictionary(x => x.ItemId, x => x.ItemName);

//                var warehouses = wareHouseRepo.All()
//                    .ToDictionary(x => x.WarehouseId, x => x.WarehouseName);

//                // === Process Stored Procedure Result ===
//                foreach (var row in result)
//                {
//                    // === Main Group ===
//                    if (row.mainMainItemID != null)
//                    {
//                        response.MainGroupList.Add(new SalesDefInvMainItemSetupDto
//                        {
//                            TC = row.mainTC,
//                            MainItemID = row.mainMainItemID,
//                            MainItemName = row.mainMainItemName,
//                            Description = row.mainDescription,
//                            showCreateDate = row.mainLDate != null
//                                            ? ((DateTime)row.mainLDate).ToString("dd/MM/yyyy")
//                                            : "",
//                            showModifyDate = row.mainModifyDate != null
//                                            ? ((DateTime)row.mainModifyDate).ToString("dd/MM/yyyy")
//                                            : ""
//                        });
//                    }

//                    // === Sub Group ===
//                    if (row.subSubItemID != null)
//                    {
//                        response.SubGroupList.Add(new SalesDefInvSubItemDto
//                        {
//                            TC = row.subTC,
//                            MainItemID = row.subMainItemID,
//                            MainItemName = row.subMainItemID != null && mainItems.ContainsKey(row.subMainItemID)
//                                            ? mainItems[row.subMainItemID]
//                                            : "",
//                            SubItemID = row.subSubItemID,
//                            SubItemName = row.subSubItemName,
//                            Description = row.subDescription,
//                            showCreateDate = row.subLDate != null
//                                            ? ((DateTime)row.subLDate).ToString("dd/MM/yyyy")
//                                            : "",
//                            showModifyDate = row.subModifyDate != null
//                                            ? ((DateTime)row.subModifyDate).ToString("dd/MM/yyyy")
//                                            : ""
//                        });
//                    }

//                    // === Sub2 Group ===
//                    if (row.sub2SubItem2ID != null)
//                    {
//                        response.Sub2GroupList.Add(new RmgProdDefInvSubItem2Dto
//                        {
//                            TC = row.sub2TC,
//                            MainItemID = row.sub2MainItemID,
//                            MainItemName = row.sub2MainItemID != null && mainItems.ContainsKey(row.sub2MainItemID)
//                                            ? mainItems[row.sub2MainItemID]
//                                            : "",
//                            SubItemID = row.sub2SubItemID,
//                            SubItemName = row.sub2SubItemID != null && subItems.ContainsKey(row.sub2SubItemID)
//                                            ? subItems[row.sub2SubItemID]
//                                            : "",
//                            SubItem2ID = row.sub2SubItem2ID,
//                            SubItem2Name = row.sub2SubItem2Name,
//                            Description = row.sub2Description,
//                            showCreateDate = row.sub2LDate != null
//                                            ? ((DateTime)row.sub2LDate).ToString("dd/MM/yyyy")
//                                            : "",
//                            showModifyDate = row.sub2ModifyDate != null
//                                            ? ((DateTime)row.sub2ModifyDate).ToString("dd/MM/yyyy")
//                                            : ""
//                        });
//                    }

//                    // === Item List ===
//                    if (row.itemItemID != null)
//                    {
//                        response.ItemList.Add(new InvDefItemDto
//                        {
//                            TC = row.itemTC,
//                            BuyerId = row.itemBuyerId,
//                            StyleId = row.itemStyleId,
//                            MainItemID = row.itemMainItemID,
//                            SubItemID = row.itemSubItemID,
//                            SubItem2ID = row.itemSubItem2ID,
//                            ItemTypeID = row.itemItemTypeID,

//                            ItemID = row.itemItemID,
//                            ItemName = row.itemItemName,
//                            PrintName = row.itemPrintName,
//                            ItemCode = row.itemItemCode,
//                            Barcode = row.itemBarcode,
//                            OriginId = row.itemOriginId,
//                            ManufactureId = row.itemManufactureId,
//                            PackageTypeId = row.itemPackageTypeId,
//                            PackageQuantity = row.itemPackageQuantity,
//                            ItemQuantity = row.itemItemQuantity,
//                            ItemUnit = row.itemItemUnit,
//                            ItemPrice = row.itemItemPrice,
//                            CurrencyId = row.itemCurrencyId,
//                            CurrencyId2 = row.itemCurrencyId2,
//                            Discount = row.itemDiscount,
//                            TotalAmount = row.itemTotalAmount,
//                            WarrantyStatus = row.itemWarrantyStatus,
//                            WarrantyTime = row.itemWarrantyTime,
//                            WarrantyType = row.itemWarrantyType,
//                            BranchID = row.itemBranchID,
//                            SupplierId = row.itemSupplierId,
//                            EmployeeID = row.itemEmployeeId,
//                            Ldate = row.itemLdate,
//                            Lmac = row.itemLmac,
//                            Lip = row.itemLip,
//                            Luser = row.itemLuser,
//                            TechnicalSpecification = row.itemTechnicalSpecification,
//                            // === Extra Display fields ===
//                            BuyerName = row.itemBuyerId != null && buyers.ContainsKey(row.itemBuyerId)
//                                        ? buyers[row.itemBuyerId]
//                                        : "",
//                            ItemTypeName = row.itemItemTypeID != null && itemTypes.ContainsKey(row.itemItemTypeID)
//                                        ? itemTypes[row.itemItemTypeID]
//                                        : "",
//                            ItemUnitName = row.itemItemUnit != null && units.ContainsKey(row.itemItemUnit)
//                                        ? units[row.itemItemUnit]
//                                        : "",
//                            StyleName = row.itemStyleId != null && styles.ContainsKey(row.itemStyleId)
//                                        ? styles[row.itemStyleId]
//                                        : "",
//                            showCreateDate = row.itemLDate != null
//                                            ? ((DateTime)row.itemLDate).ToString("dd/MM/yyyy")
//                                            : "",
//                            showModifyDate = row.itemModifyDate != null
//                                            ? ((DateTime)row.itemModifyDate).ToString("dd/MM/yyyy")
//                                            : ""
//                        });
//                    }

//                    // === Stock List ===
//                    if (row.stockItemID != null)
//                    {
//                        response.StockItemList.Add(new DefInvStockLevelManagementDto
//                        {
//                            TC = row.stockTC,
//                            SLMID = row.stockSLMID,
//                            ItemID = row.stockItemID,
//                            ItemName = row.stockItemID != null && itemsDict.ContainsKey(row.stockItemID)
//                                        ? itemsDict[row.stockItemID]
//                                        : "",
//                            WarehouseID = row.stockWarehouseID,
//                            WarehouseName = row.stockWarehouseID != null && warehouses.ContainsKey(row.stockWarehouseID)
//                                        ? warehouses[row.stockWarehouseID]
//                                        : "",
//                            InStock = row.stockInStock,
//                            StockValue = row.stockStockValue,
//                            ReorderLevel = row.stockReorderLevel,
//                            MaxStock = row.stockMaxStock,
//                            MinStock = row.stockMinStock,
//                            Description = row.stockDescription,
//                            EmployeeID = row.stockEmployeeId,
//                            Ldate = row.stockLdate,
//                            Lmac = row.stockLmac,
//                            Lip = row.stockLip,
//                            Luser = row.stockLuser,
//                            showCreateDate = row.stockLDate != null
//                                            ? ((DateTime)row.stockLDate).ToString("dd/MM/yyyy")
//                                            : "",
//                            showModifyDate = row.stockModifyDate != null
//                                            ? ((DateTime)row.stockModifyDate).ToString("dd/MM/yyyy")
//                                            : ""
//                        });
//                    }
//                }

//                // === Remove Duplicates ===
//                response.MainGroupList = response.MainGroupList
//                    .GroupBy(x => x.MainItemID)
//                    .Select(g => g.First())
//                    .ToList();

//                response.SubGroupList = response.SubGroupList
//                    .GroupBy(x => x.SubItemID)
//                    .Select(g => g.First())
//                    .ToList();

//                response.Sub2GroupList = response.Sub2GroupList
//                    .GroupBy(x => x.SubItem2ID)
//                    .Select(g => g.First())
//                    .ToList();

//                response.ItemList = response.ItemList
//                    .GroupBy(x => x.ItemID)
//                    .Select(g => g.First())
//                    .ToList();

//                response.StockItemList = response.StockItemList
//                    .GroupBy(x => x.ItemID)
//                    .Select(g => g.First())
//                    .ToList();

//                return response;
//            }
//            catch (Exception ex)
//            {
//                throw;
//            }
//        }




//    }

//}



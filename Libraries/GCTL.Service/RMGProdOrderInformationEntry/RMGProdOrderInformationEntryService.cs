//using GCTL.Core.Data;
//using GCTL.Core.ViewModels.RMGProdOrderInformationEntry;
//using GCTL.Data.Models;
//using GCTL.UI.Core.Views.RMGProdOrderInformationEntry;
//using Microsoft.EntityFrameworkCore;

//namespace GCTL.Service.RMGProdOrderInformationEntry
//{
//    public class RMGProdOrderInformationEntryService : AppService<RmgProdOrder>, IRMGProdOrderInformationEntryService
//    {
//        private readonly IRepository<RmgProdOrder> orderRepo;
//        private readonly IRepository<CoreAccessCode> accessCodeRepository;
//        private readonly IRepository<RmgProdOrderDetails> prodOrderDetailsRepo;
//        private readonly IRepository<RmgProdTempColorSizeBreakup> tempColorSizeBreakupRepo;
//        private readonly IRepository<RmgProdTempListColorSizeBreakup> tempListColorSizeBreakupRepo;
//        private readonly IRepository<HrmEmployeeOfficialInfo> offiRepo;
//        private readonly IRepository<HrmEmployee> empRepo;
//        private readonly IRepository<HrmDefDesignation> desRepo;
//        private readonly IRepository<InvDefItem> itemRepo;
//        private readonly IRepository<RmgProdDefUnitType> unitTypeRepo;
//        private readonly IRepository<InvDefPortInfo> portRepo;
//        private readonly IRepository<RmgProdDefBuyer> buyerRepo;
//        private readonly IRepository<RmgProdDefBrand> buyerBrandRepo;
//        private readonly IRepository<ProdDefStyle> styleRepo;
//        private readonly IRepository<RmgProdDefSeason> seasonRepo;
//        private readonly IRepository<CaDefCurrency> currencyRepo;

//        //private readonly string _connectionString;

//        public RMGProdOrderInformationEntryService(
//            IRepository<RmgProdOrder> orderRepo,
//            IRepository<CoreAccessCode> accessCodeRepository,
//            IRepository<RmgProdOrderDetails> ProdOrderDetailsRepo,
//            IRepository<RmgProdTempColorSizeBreakup> TempColorSizeBreakupRepo,
//            IRepository<RmgProdTempListColorSizeBreakup> TempListColorSizeBreakupRepo,
//            IRepository<HrmEmployeeOfficialInfo> offiRepo,
//            IRepository<HrmEmployee> empRepo,
//            IRepository<HrmDefDesignation> desRepo,
//            IRepository<InvDefItem> itemRepo,
//            IRepository<RmgProdDefUnitType> unitTypeRepo,
//             IRepository<InvDefPortInfo> portRepo,
//             IRepository<RmgProdDefBuyer> buyerRepo,
//            IRepository<RmgProdDefBrand> buyerBrandRepo,
//            IRepository<ProdDefStyle> styleRepo,
//            IRepository<RmgProdDefSeason> seasonRepo,
//             IRepository<CaDefCurrency> currencyRepo
//            //IConfiguration configuration
//            ) : base(orderRepo)
//        {
//            this.orderRepo = orderRepo;
//            this.accessCodeRepository = accessCodeRepository;
//            prodOrderDetailsRepo = ProdOrderDetailsRepo;
//            tempColorSizeBreakupRepo = TempColorSizeBreakupRepo;
//            tempListColorSizeBreakupRepo = TempListColorSizeBreakupRepo;
//            this.offiRepo = offiRepo;
//            this.empRepo = empRepo;
//            this.desRepo = desRepo;
//            this.itemRepo = itemRepo;
//            this.unitTypeRepo = unitTypeRepo;
//            this.portRepo = portRepo;
//            this.buyerRepo = buyerRepo;
//            this.buyerBrandRepo = buyerBrandRepo;
//            this.styleRepo = styleRepo;
//            this.seasonRepo = seasonRepo;
//            this.currencyRepo = currencyRepo;
//            //this.configuration = configuration;
//            //_connectionString = configuration.GetConnectionString("ApplicationDbConnection");
//        }

//        private readonly string CreateSuccess = "Data saved successfully.";
//        private readonly string CreateFailed = "Data insertion failed.";
//        private readonly string UpdateSuccess = "Data updated successfully.";
//        private readonly string UpdateFailed = "Data update failed.";
//        private readonly string DeleteSuccess = "Data deleted successfully.";
//        private readonly string DeleteFailed = "Data deletion failed.";
//        private readonly string DataExists = "Data already exists.";


//        public async Task<bool> PagePermissionAsync(string accessCode)

//        {

//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "RMG Prod Order Information" && x.TitleCheck);

//        }

//        public async Task<bool> SavePermissionAsync(string accessCode)

//        {

//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "RMG Prod Order Information" && x.CheckAdd);

//        }

//        public async Task<bool> UpdatePermissionAsync(string accessCode)

//        {

//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "RMG Prod Order Information" && x.CheckEdit);

//        }

//        public async Task<bool> DeletePermissionAsync(string accessCode)

//        {

//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "RMG Prod Order Information" && x.CheckDelete);

//        }



//        public async Task<string> EntryAutoIdAsync()
//        {
//            try
//            {
//                var orderLastId = orderRepo.All().OrderByDescending(x => x.Tc).Select(c => c.OrderId).FirstOrDefault();
//                int newOrderId = 0;
//                if (orderLastId != null)
//                {
//                    newOrderId = Convert.ToInt32(orderLastId) + 1;
//                }
//                else
//                {
//                    newOrderId = 1;
//                }
//                return newOrderId.ToString("D6");
//            }
//            catch (Exception)
//            {

//                throw;
//            }


//        }
//        public async Task<string> IntegraJOBNoAutoAsync()
//        {
//            try
//            {
//                var orderId = EntryAutoIdAsync();
//                var IJNo = "IABL_" + DateTime.Now.Year + "/" + orderId.Result;
//                return IJNo;

//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }

//        public async Task<PagedResult<RMG_Prod_OrderDto>> GetPagedOrdersAsync(DataTableFilter filter)
//        {
//            var query = orderRepo.All();

//            // 🔹 Filter by IntegraJOBNo if provided
//            if (!string.IsNullOrEmpty(filter.buyerId))
//            {
//                query = query.Where(x => x.BuyerId == filter.buyerId);
//            }

//            // 🔍 Search Filter
//            if (!string.IsNullOrEmpty(filter.SearchValue))
//            {
//                query = query.Where(x =>
//                    x.OrderId.Contains(filter.SearchValue) ||
//                    x.BuyerId.Contains(filter.SearchValue) ||
//                    x.StyleId.Contains(filter.SearchValue) ||
//                    x.BuyerBrand.Contains(filter.SearchValue));
//            }

//            var totalRecords = await query.CountAsync();

//            var rawData = await query
//                .Skip(filter.Start)
//                .Take(filter.Length)
//                .ToListAsync();

//            var data = rawData.Select(o => new RMG_Prod_OrderDto
//            {
//                TC = o.Tc,
//                OrderId = o.OrderId,
//                Date = o.Date,
//                BuyerId = o.BuyerId,
//                BuyerName = buyerRepo.All().Where(x => x.BuyerId == o.BuyerId).Select(s => s.BuyerName).FirstOrDefault() ?? "",
//                BuyerOrderNo = o.BuyerOrderNo,
//                BuyerOrderDate = o.BuyerOrderDate,
//                MasterPurchaseOrder = o.MasterPurchaseOrder,
//                MPO_Date = o.MpoDate,
//                SeasonId = o.SeasonId,
//                SeasonName = seasonRepo.All().Where(x => x.SeasonId == o.SeasonId).Select(s => s.Season).FirstOrDefault() ?? "",
//                SeasonYear = o.SeasonYear,
//                SupplierId = o.SupplierId,
//                TotalOrderQuantity = o.TotalOrderQuantity,
//                TotalOrderQuantityDis = (o.TotalOrderQuantity != null) ? Convert.ToInt32(o.TotalOrderQuantity) + " " + unitTypeRepo.All().Where(x => x.UnitTypId == o.UnitTypId).Select(s => s.UnitTypeName).FirstOrDefault() : "",
//                UnitTypID = o.UnitTypId,
//                TotalPrice = o.TotalPrice,
//                CurrencyId = o.CurrencyId,
//                PaymentTerm = o.PaymentTerm,
//                BuyerBankId = o.BuyerBankId,
//                BuyerBranchId = o.BuyerBranchId,
//                CompanyOwnBankId = o.CompanyOwnBankId,
//                CompanyOwnBranchId = o.CompanyOwnBranchId,
//                BuContatPerson = !string.IsNullOrEmpty(o.BuContatPerson)
//                           ? o.BuContatPerson.Split(',').ToList()
//                           : new List<string>(),
//                BuDesignation1 = o.BuDesignation1,
//                Buphone = o.Buphone,
//                BuEmail = o.BuEmail,
//                MerContatPerson = o.MerContatPerson,
//                MerDesignation1 = o.MerDesignation1,
//                Merphone = o.Merphone,
//                MerEmail = o.MerEmail,
//                BuyerDeclaration = o.BuyerDeclaration,
//                InspectionInfo = o.InspectionInfo,
//                Remarks = o.Remarks,
//                EmployeeID = o.EmployeeId,
//                IntegraJOBNo = o.IntegraJobno,
//                POStatusId = o.PostatusId,
//                BuyerBrand = o.BuyerBrand,
//                BuyerBrandName = buyerBrandRepo.All().Where(x => x.BrandId == o.BuyerBrand).Select(s => s.Name).FirstOrDefault() ?? "",
//                StyleId = o.StyleId,
//                StyleName = styleRepo.All().Where(x => x.StyleId == o.StyleId).Select(s => s.Style).FirstOrDefault() ?? "",
//                OrderDate = o.OrderDate,
//                BuyerSwiftCode = o.BuyerSwiftCode,
//                CompanySwiftCode = o.CompanySwiftCode,
//                MerchandiserContactId = !string.IsNullOrEmpty(o.MerchandiserContactId)
//                           ? o.MerchandiserContactId.Split(',').ToList()
//                           : new List<string>(),
//                StylePOWise = o.StylePowise,
//                FOBAmount = o.Fobamount,
//                FOBAmountDis = (o.Fobamount != null) ? Convert.ToInt32(o.Fobamount) + " " + currencyRepo.All().Where(x => x.CurrencyId == o.CurrencyIdFob).Select(s => s.ShortName).FirstOrDefault() : "",
//                CurrencyId_FOB = o.CurrencyIdFob,
//                //CurrencyId_FOBDis = (o.CurrencyIdFob != null) ? o.CurrencyIdFob + " " + currencyRepo.All().Where(x => x.CurrencyId == o.CurrencyId).Select(s => s.CurrencyName).FirstOrDefault() : "",
//                //UnitTyp = (o.CurrencyIdFob != null) ? o.CurrencyIdFob + " " + unitTypeRepo.All().Where(x => x.UnitTypId == o.UnitTypId).Select(s => s.UnitTypeName).FirstOrDefault() : "",
//                ShowCreateDate = o.Ldate.HasValue ? o.Ldate.Value.ToString("dd/MM/yyyy") : "",
//                ShowModifyDate = o.ModifyDate.HasValue ? o.ModifyDate.Value.ToString("dd/MM/yyyy") : ""
//            }).ToList();

//            return new PagedResult<RMG_Prod_OrderDto>
//            {
//                Draw = filter.Draw,
//                RecordsTotal = totalRecords,
//                RecordsFiltered = totalRecords,
//                Data = data
//            };
//        }



//        public async Task<(bool isSuccess, string message, object data)> OrderSaveEditAsync(RMG_Prod_OrderDto fromData, string companyCode)
//        {
//            try
//            {
//                if (fromData == null)
//                    return (false, "Invalid order data.", null);

//                // 🔍 Basic validation
//                if (string.IsNullOrWhiteSpace(fromData.BuyerId))
//                    return (false, "Buyer ID is required.", null);

//                if (string.IsNullOrWhiteSpace(fromData.IntegraJOBNo))
//                    return (false, "Integra Job No is required.", null);

//                if (fromData.TotalOrderQuantity == null || fromData.TotalOrderQuantity <= 0)
//                    return (false, "Total Order Quantity must be greater than zero.", null);

//                // ✅ Convert arrays to string
//                string buContacts = fromData.BuContatPerson != null && fromData.BuContatPerson.Any()
//                    ? string.Join(",", fromData.BuContatPerson.Select(x => $"{x}"))
//                    : "";

//                string merchContacts = fromData.MerchandiserContactId != null && fromData.MerchandiserContactId.Any()
//                    ? string.Join(",", fromData.MerchandiserContactId.Select(x => $"{x}"))
//                    : "";

//                // ✅ Helper function for null-safe strings
//                string Safe(string? val) => val ?? "";

//                if (fromData.TC == 0)
//                {

//                    var jobNo = IntegraJOBNoAutoAsync();
//                    // ✅ Save (Insert)
//                    var entity = new RmgProdOrder
//                    {
//                        OrderId = Safe(fromData.OrderId),
//                        Date = (DateTime)(fromData.Date == default ? DateTime.Now : fromData.Date),
//                        BuyerId = Safe(fromData.BuyerId),
//                        BuyerOrderNo = Safe(fromData.BuyerOrderNo),
//                        BuyerOrderDate = fromData.BuyerOrderDate ?? DateTime.Now,
//                        MasterPurchaseOrder = Safe(fromData.MasterPurchaseOrder),
//                        MpoDate = (DateTime)(fromData.MPO_Date == default ? DateTime.Now : fromData.MPO_Date),
//                        SeasonId = Safe(fromData.SeasonId),
//                        SeasonYear = Safe(fromData.SeasonYear),
//                        SupplierId = Safe(fromData.SupplierId),
//                        TotalOrderQuantity = fromData.TotalOrderQuantity ?? 0,
//                        UnitTypId = Safe(fromData.UnitTypID),
//                        TotalPrice = fromData.TotalPrice ?? 0,
//                        CurrencyId = Safe(fromData.CurrencyId),
//                        PaymentTerm = Safe(fromData.PaymentTerm),
//                        BuyerBankId = Safe(fromData.BuyerBankId),
//                        BuyerBranchId = Safe(fromData.BuyerBranchId),
//                        CompanyOwnBankId = Safe(fromData.CompanyOwnBankId),
//                        CompanyOwnBranchId = Safe(fromData.CompanyOwnBranchId),
//                        BuContatPerson = buContacts,
//                        BuDesignation1 = Safe(fromData.BuDesignation1),
//                        Buphone = Safe(fromData.Buphone),
//                        BuEmail = Safe(fromData.BuEmail),
//                        MerContatPerson = Safe(fromData.MerContatPerson),
//                        MerDesignation1 = Safe(fromData.MerDesignation1),
//                        Merphone = Safe(fromData.Merphone),
//                        MerEmail = Safe(fromData.MerEmail),
//                        BuyerDeclaration = Safe(fromData.BuyerDeclaration),
//                        InspectionInfo = Safe(fromData.InspectionInfo),
//                        Remarks = Safe(fromData.Remarks),
//                        //IntegraJobno = Safe(fromData.IntegraJOBNo),                     
//                        IntegraJobno = Safe(jobNo.Result),
//                        PostatusId = Safe(fromData.POStatusId),
//                        BuyerBrand = Safe(fromData.BuyerBrand),
//                        StyleId = Safe(fromData.StyleId),
//                        OrderDate = fromData.OrderDate ?? DateTime.Now,
//                        BuyerSwiftCode = Safe(fromData.BuyerSwiftCode),
//                        CompanySwiftCode = Safe(fromData.CompanySwiftCode),
//                        MerchandiserContactId = merchContacts,
//                        StylePowise = Safe(fromData.StylePOWise),
//                        Fobamount = fromData.FOBAmount ?? 0,
//                        CurrencyIdFob = Safe(fromData.CurrencyId_FOB),
//                        Luser = fromData.Luser,
//                        Ldate = fromData.Ldate,
//                        Lmac = fromData.Lmac,
//                        Lip = fromData.Lip,
//                        CompanyCode = companyCode,
//                        EmployeeId = Safe(fromData.EmployeeID),
//                    };

//                    await orderRepo.AddAsync(entity);

//                    return (true, CreateSuccess, entity);
//                }
//                else
//                {
//                    // ✅ Edit (Update)
//                    var entity = await orderRepo.All().FirstOrDefaultAsync(x => x.Tc == fromData.TC);
//                    if (entity == null)
//                        return (false, "Order not found for update.", null);

//                    entity.Date = (DateTime)(fromData.Date == default ? DateTime.Now : fromData.Date);
//                    entity.BuyerId = Safe(fromData.BuyerId);
//                    entity.BuyerOrderNo = Safe(fromData.BuyerOrderNo);
//                    entity.BuyerOrderDate = fromData.BuyerOrderDate ?? DateTime.Now;
//                    entity.MasterPurchaseOrder = Safe(fromData.MasterPurchaseOrder);
//                    entity.MpoDate = (DateTime)(fromData.MPO_Date == default ? DateTime.Now : fromData.MPO_Date);
//                    entity.SeasonId = Safe(fromData.SeasonId);
//                    entity.SeasonYear = Safe(fromData.SeasonYear);
//                    entity.SupplierId = Safe(fromData.SupplierId);
//                    entity.TotalOrderQuantity = fromData.TotalOrderQuantity ?? 0;
//                    entity.UnitTypId = Safe(fromData.UnitTypID);
//                    entity.TotalPrice = fromData.TotalPrice ?? 0;
//                    entity.CurrencyId = Safe(fromData.CurrencyId);
//                    entity.PaymentTerm = Safe(fromData.PaymentTerm);
//                    entity.BuyerBankId = Safe(fromData.BuyerBankId);
//                    entity.BuyerBranchId = Safe(fromData.BuyerBranchId);
//                    entity.CompanyOwnBankId = Safe(fromData.CompanyOwnBankId);
//                    entity.CompanyOwnBranchId = Safe(fromData.CompanyOwnBranchId);
//                    entity.BuContatPerson = buContacts;
//                    entity.BuDesignation1 = Safe(fromData.BuDesignation1);
//                    entity.Buphone = Safe(fromData.Buphone);
//                    entity.BuEmail = Safe(fromData.BuEmail);
//                    entity.MerContatPerson = Safe(fromData.MerContatPerson);
//                    entity.MerDesignation1 = Safe(fromData.MerDesignation1);
//                    entity.Merphone = Safe(fromData.Merphone);
//                    entity.MerEmail = Safe(fromData.MerEmail);
//                    entity.BuyerDeclaration = Safe(fromData.BuyerDeclaration);
//                    entity.InspectionInfo = Safe(fromData.InspectionInfo);
//                    entity.Remarks = Safe(fromData.Remarks);
//                    entity.IntegraJobno = Safe(fromData.IntegraJOBNo);
//                    entity.PostatusId = Safe(fromData.POStatusId);
//                    entity.BuyerBrand = Safe(fromData.BuyerBrand);
//                    entity.StyleId = Safe(fromData.StyleId);
//                    entity.OrderDate = fromData.OrderDate ?? DateTime.Now;
//                    entity.BuyerSwiftCode = Safe(fromData.BuyerSwiftCode);
//                    entity.CompanySwiftCode = Safe(fromData.CompanySwiftCode);
//                    entity.MerchandiserContactId = merchContacts;
//                    entity.StylePowise = Safe(fromData.StylePOWise);
//                    entity.Fobamount = fromData.FOBAmount ?? 0;
//                    entity.CurrencyIdFob = Safe(fromData.CurrencyId_FOB);
//                    entity.ModifyDate = DateTime.Now;
//                    entity.CompanyCode = companyCode;
//                    entity.EmployeeId = Safe(fromData.EmployeeID);

//                    await orderRepo.UpdateAsync(entity);

//                    return (true, UpdateSuccess, entity);
//                }
//            }
//            catch (Exception ex)
//            {
//                return (false, $"Error saving order: {ex.Message}", null);
//            }
//        }



//        public async Task<PagedResult<RMG_Prod_OrderDetailsDto>> GetPagedOrderDetailsAsync(DataTableFilter filter)
//        {
//            var query = prodOrderDetailsRepo.All()
//                .Select(o => new RMG_Prod_OrderDetailsDto
//                {
//                    TC = o.Tc,
//                    DetailOrderId = o.DetailOrderId,
//                    OrderId = o.OrderId,
//                    Date = o.Date,
//                    ProductId = o.ProductId,
//                    ProductName = itemRepo.All().Where(x => x.ItemId == o.ProductId).Select(d => d.ItemName).FirstOrDefault(),
//                    Description = o.Description,
//                    BrandId = o.BrandId,
//                    Style = o.Style,
//                    RefNo = o.RefNo,
//                    HSCode = o.Hscode,
//                    PurchaseOrder = o.PurchaseOrder,
//                    PODate = o.Podate,
//                    OrderQuantity = o.OrderQuantity,
//                    POUnitTypID = o.PounitTypId,
//                    POUnitTyp = unitTypeRepo.All().Where(x => x.UnitTypId == o.PounitTypId).Select(c => c.UnitTypeName).FirstOrDefault(),
//                    UnitPrice = o.UnitPrice,
//                    CurrencyId = o.CurrencyId,
//                    TotalAmount = o.TotalAmount,
//                    MaterialInfo = o.MaterialInfo,
//                    PrintingInstruction = o.PrintingInstruction,
//                    WashingInstruction = o.WashingInstruction,
//                    LabelInstruction = o.LabelInstruction,
//                    PackagingInstruction = o.PackagingInstruction,
//                    OtherInstruction = o.OtherInstruction,
//                    DeliveryDate = o.DeliveryDate,
//                    DeliveryAddress = o.DeliveryAddress,
//                    DeliveryTerm = o.DeliveryTerm,
//                    DeliveryMethod = o.DeliveryMethod,
//                    PortOfLoading = o.PortOfLoading,
//                    PortOfLoadingName = portRepo.All().Where(x => x.PortId == o.PortOfLoading).Select(s => s.PortName).FirstOrDefault(),
//                    PortOfDischarge = o.PortOfDischarge,
//                    PortOfDischargeName = portRepo.All().Where(x => x.PortId == o.PortOfDischarge).Select(s => s.PortName).FirstOrDefault(),
//                    SupplierId = o.SupplierId,
//                    PaymentTermsId = o.PaymentTermsId,
//                    GarmentsTesting = o.GarmentsTesting,
//                    GarmentsInstruction = o.GarmentsInstruction,
//                    GarmentReminderDay = o.GarmentReminderDay,
//                    GarmentReminderType = o.GarmentReminderType,
//                    GarmnetRemainderMail = o.GarmnetRemainderMail,
//                    IsGarmentTestRecieved = o.IsGarmentTestRecieved,
//                    GarmentTestAttachment = o.GarmentTestAttachment,
//                    FebricTesting = o.FebricTesting,
//                    FebricInstruction = o.FebricInstruction,
//                    FebricReminderDay = o.FebricReminderDay,
//                    FebricReminderType = o.FebricReminderType,
//                    FebricRemainderMail = o.FebricRemainderMail,
//                    IsFebricTestRecieved = o.IsFebricTestRecieved,
//                    FebricTestAttachment = o.FebricTestAttachment,
//                    TransportNo = o.TransportNo,
//                    IntegraJobNO = o.IntegraJobNo,
//                    MasterPurchaseOrder = o.MasterPurchaseOrder,
//                    Percentage1 = o.Percentage1,
//                    DeliveryMethod2 = o.DeliveryMethod2,
//                    Percentage2 = o.Percentage2,
//                    DeliveryMethod3 = o.DeliveryMethod3,
//                    Percentage3 = o.Percentage3,
//                    XFactoryDate = o.XfactoryDate,
//                    ShowCreateDate = o.Ldate.HasValue ? o.Ldate.Value.ToString("dd/MM/yyyy") : "",
//                    ShowModifyDate = o.ModifyDate.HasValue ? o.ModifyDate.Value.ToString("dd/MM/yyyy") : ""
//                });

//            if (!string.IsNullOrEmpty(filter.IntegraJobNo))
//            {
//                query = query.Where(x => x.IntegraJobNO == filter.IntegraJobNo);
//            }

//            if (!string.IsNullOrEmpty(filter.SearchValue))
//            {
//                var search = filter.SearchValue.ToLower();
//                query = query.Where(x =>
//                    (x.DetailOrderId ?? "").ToLower().Contains(search) ||
//                    (x.PurchaseOrder ?? "").ToLower().Contains(search) ||
//                    (x.ProductId ?? "").ToLower().Contains(search) ||
//                    (x.Description ?? "").ToLower().Contains(search) ||
//                    (x.SupplierId ?? "").ToLower().Contains(search) ||
//                    (x.IntegraJobNO ?? "").ToLower().Contains(search)
//                );
//            }

//            var totalRecords = await query.CountAsync();
//            var data = await query.Skip(filter.Start).Take(filter.Length).ToListAsync();

//            return new PagedResult<RMG_Prod_OrderDetailsDto>
//            {
//                Draw = filter.Draw,
//                RecordsTotal = totalRecords,
//                RecordsFiltered = totalRecords,
//                Data = data
//            };
//        }


//        public async Task<(bool isSuccess, string message, object data)> DetailsSaveEditAsync(RMG_Prod_OrderDetailsDto fromData, string companyCode)
//        {
//            try
//            {
//                if (fromData == null)
//                    return (false, "Invalid order data.", null);

//                string orderIdPart = "";
//                if (!string.IsNullOrWhiteSpace(fromData.IntegraJobNO))
//                {
//                    var parts = fromData.IntegraJobNO.Split('/');
//                    if (parts.Length > 1)
//                    {
//                        orderIdPart = parts[1];
//                    }
//                }
//                var lastDetailsOrderId = await prodOrderDetailsRepo.All().Where(x => x.OrderId == orderIdPart).OrderByDescending(x => x.DetailOrderId)
//                    .Select(c => c.DetailOrderId)
//                    .FirstOrDefaultAsync();

//                string nextDetailsOrderId;
//                if (string.IsNullOrEmpty(lastDetailsOrderId))
//                {
//                    nextDetailsOrderId = $"{orderIdPart}001";
//                }
//                else
//                {
//                    var suffix = lastDetailsOrderId.Substring(lastDetailsOrderId.Length - 3);
//                    if (int.TryParse(suffix, out int num))
//                    {
//                        num++;
//                        nextDetailsOrderId = $"{orderIdPart}{num.ToString("D3")}";
//                    }
//                    else
//                    {
//                        nextDetailsOrderId = $"{orderIdPart}001";
//                    }
//                }

//                if (fromData.IntegraJobNO == null || fromData.ProductId == null || fromData.OrderQuantity == null || fromData.OrderQuantity < 0 || fromData.DeliveryDate == null || fromData.XFactoryDate == null)
//                {
//                    return (false, CreateFailed, null);
//                }
//                // Helper for safe null string
//                string Safe(string? val) => val ?? "";

//                if (fromData.TC == 0)
//                {
//                    // ✅ CREATE (INSERT)
//                    var entity = new RmgProdOrderDetails
//                    {
//                        DetailOrderId = Safe(nextDetailsOrderId),
//                        OrderId = Safe(orderIdPart),
//                        Date = fromData.Date ?? DateTime.Now,
//                        ProductId = Safe(fromData.ProductId),
//                        Description = Safe(fromData.Description),
//                        BrandId = Safe(fromData.BrandId),
//                        Style = Safe(fromData.Style),
//                        RefNo = Safe(fromData.RefNo),
//                        Hscode = Safe(fromData.HSCode),
//                        PurchaseOrder = Safe(fromData.PurchaseOrder),
//                        Podate = fromData.PODate ?? DateTime.Now,
//                        OrderQuantity = fromData.OrderQuantity ?? 0,
//                        PounitTypId = Safe(fromData.POUnitTypID),
//                        UnitPrice = fromData.UnitPrice ?? 0,
//                        CurrencyId = Safe(fromData.CurrencyId),
//                        TotalAmount = fromData.TotalAmount ?? 0,
//                        MaterialInfo = Safe(fromData.MaterialInfo),
//                        PrintingInstruction = Safe(fromData.PrintingInstruction),
//                        WashingInstruction = Safe(fromData.WashingInstruction),
//                        LabelInstruction = Safe(fromData.LabelInstruction),
//                        PackagingInstruction = Safe(fromData.PackagingInstruction),
//                        OtherInstruction = Safe(fromData.OtherInstruction),
//                        DeliveryDate = fromData.DeliveryDate ?? DateTime.Now,
//                        DeliveryAddress = Safe(fromData.DeliveryAddress),
//                        DeliveryTerm = Safe(fromData.DeliveryTerm),
//                        DeliveryMethod = Safe(fromData.DeliveryMethod),
//                        PortOfLoading = Safe(fromData.PortOfLoading),
//                        PortOfDischarge = Safe(fromData.PortOfDischarge),
//                        SupplierId = Safe(fromData.SupplierId),
//                        PaymentTermsId = Safe(fromData.PaymentTermsId),
//                        GarmentsTesting = Safe(fromData.GarmentsTesting),
//                        GarmentsInstruction = Safe(fromData.GarmentsInstruction),
//                        GarmentReminderDay = Safe(fromData.GarmentReminderDay),
//                        GarmentReminderType = Safe(fromData.GarmentReminderType),
//                        GarmnetRemainderMail = Safe(fromData.GarmnetRemainderMail),
//                        IsGarmentTestRecieved = Safe(fromData.IsGarmentTestRecieved),
//                        GarmentTestAttachment = Safe(fromData.GarmentTestAttachment),
//                        FebricTesting = Safe(fromData.FebricTesting),
//                        FebricInstruction = Safe(fromData.FebricInstruction),
//                        FebricReminderDay = Safe(fromData.FebricReminderDay),
//                        FebricReminderType = Safe(fromData.FebricReminderType),
//                        FebricRemainderMail = Safe(fromData.FebricRemainderMail),
//                        IsFebricTestRecieved = Safe(fromData.IsFebricTestRecieved),
//                        FebricTestAttachment = Safe(fromData.FebricTestAttachment),
//                        TransportNo = Safe(fromData.TransportNo),
//                        IntegraJobNo = Safe(fromData.IntegraJobNO),
//                        MasterPurchaseOrder = Safe(fromData.MasterPurchaseOrder),
//                        Percentage1 = fromData.Percentage1 ?? 0,
//                        DeliveryMethod2 = Safe(fromData.DeliveryMethod2),
//                        Percentage2 = fromData.Percentage2 ?? 0,
//                        DeliveryMethod3 = Safe(fromData.DeliveryMethod3),
//                        Percentage3 = fromData.Percentage3 ?? 0,
//                        XfactoryDate = fromData.XFactoryDate,
//                        CompanyCode = companyCode,
//                        Luser = fromData.Luser,
//                        Ldate = fromData.Ldate,
//                        Lip = fromData.Lip,
//                        Lmac = fromData.Lmac
//                    };

//                    await prodOrderDetailsRepo.AddAsync(entity);
//                    return (true, CreateSuccess, entity);
//                }
//                else
//                {
//                    // ✅ UPDATE (EDIT)
//                    var entity = await prodOrderDetailsRepo.All()
//                        .FirstOrDefaultAsync(x => x.Tc == fromData.TC);

//                    if (entity == null)
//                        return (false, "Order not found for update.", null);

//                    //entity.DetailOrderId = Safe(fromData.DetailOrderId);
//                    //entity.OrderId = Safe(fromData.OrderId);
//                    entity.Date = fromData.Date ?? entity.Date;
//                    entity.ProductId = Safe(fromData.ProductId);
//                    entity.Description = Safe(fromData.Description);
//                    entity.BrandId = Safe(fromData.BrandId);
//                    entity.Style = Safe(fromData.Style);
//                    entity.RefNo = Safe(fromData.RefNo);
//                    entity.Hscode = Safe(fromData.HSCode);
//                    entity.PurchaseOrder = Safe(fromData.PurchaseOrder);
//                    entity.Podate = fromData.PODate ?? entity.Podate;
//                    entity.OrderQuantity = fromData.OrderQuantity ?? entity.OrderQuantity;
//                    entity.PounitTypId = Safe(fromData.POUnitTypID);
//                    entity.UnitPrice = fromData.UnitPrice ?? entity.UnitPrice;
//                    entity.CurrencyId = Safe(fromData.CurrencyId);
//                    entity.TotalAmount = fromData.TotalAmount ?? entity.TotalAmount;
//                    entity.MaterialInfo = Safe(fromData.MaterialInfo);
//                    entity.PrintingInstruction = Safe(fromData.PrintingInstruction);
//                    entity.WashingInstruction = Safe(fromData.WashingInstruction);
//                    entity.LabelInstruction = Safe(fromData.LabelInstruction);
//                    entity.PackagingInstruction = Safe(fromData.PackagingInstruction);
//                    entity.OtherInstruction = Safe(fromData.OtherInstruction);
//                    entity.DeliveryDate = fromData.DeliveryDate ?? entity.DeliveryDate;
//                    entity.DeliveryAddress = Safe(fromData.DeliveryAddress);
//                    entity.DeliveryTerm = Safe(fromData.DeliveryTerm);
//                    entity.DeliveryMethod = Safe(fromData.DeliveryMethod);
//                    entity.PortOfLoading = Safe(fromData.PortOfLoading);
//                    entity.PortOfDischarge = Safe(fromData.PortOfDischarge);
//                    entity.SupplierId = Safe(fromData.SupplierId);
//                    entity.PaymentTermsId = Safe(fromData.PaymentTermsId);
//                    entity.GarmentsTesting = Safe(fromData.GarmentsTesting);
//                    entity.GarmentsInstruction = Safe(fromData.GarmentsInstruction);
//                    entity.GarmentReminderDay = Safe(fromData.GarmentReminderDay);
//                    entity.GarmentReminderType = Safe(fromData.GarmentReminderType);
//                    entity.GarmnetRemainderMail = Safe(fromData.GarmnetRemainderMail);
//                    entity.IsGarmentTestRecieved = Safe(fromData.IsGarmentTestRecieved);
//                    entity.GarmentTestAttachment = Safe(fromData.GarmentTestAttachment);
//                    entity.FebricTesting = Safe(fromData.FebricTesting);
//                    entity.FebricInstruction = Safe(fromData.FebricInstruction);
//                    entity.FebricReminderDay = Safe(fromData.FebricReminderDay);
//                    entity.FebricReminderType = Safe(fromData.FebricReminderType);
//                    entity.FebricRemainderMail = Safe(fromData.FebricRemainderMail);
//                    entity.IsFebricTestRecieved = Safe(fromData.IsFebricTestRecieved);
//                    entity.FebricTestAttachment = Safe(fromData.FebricTestAttachment);
//                    entity.TransportNo = Safe(fromData.TransportNo);
//                    entity.IntegraJobNo = Safe(fromData.IntegraJobNO);
//                    entity.MasterPurchaseOrder = Safe(fromData.MasterPurchaseOrder);
//                    entity.Percentage1 = fromData.Percentage1 ?? entity.Percentage1;
//                    entity.DeliveryMethod2 = Safe(fromData.DeliveryMethod2);
//                    entity.Percentage2 = fromData.Percentage2 ?? entity.Percentage2;
//                    entity.DeliveryMethod3 = Safe(fromData.DeliveryMethod3);
//                    entity.Percentage3 = fromData.Percentage3 ?? entity.Percentage3;
//                    entity.XfactoryDate = fromData.XFactoryDate ?? entity.XfactoryDate;
//                    entity.ModifyDate = DateTime.Now;

//                    await prodOrderDetailsRepo.UpdateAsync(entity);
//                    return (true, UpdateSuccess, entity);
//                }
//            }
//            catch (Exception ex)
//            {
//                return (false, $"Error saving order: {ex.Message}", null);
//            }
//        }


//        public async Task<(bool isSuccess, string message, object data)> SaveEditColorSizeBreakupAsync(RMG_Prod_Temp_ColorSizeBreakupDto dto, string companyCode) //tt
//        {
//            try
//            {
//                if (dto.ColorIds == null || dto.ColorIds.Count == 0)
//                    return (false, "No colors selected.", null);
//                if (dto.SizeIds == null || dto.SizeIds.Count == 0)
//                    return (false, "No sizes selected.", null);

//                var orderDetailsNo = prodOrderDetailsRepo.All()
//                    .Where(x => x.IntegraJobNo == dto.IntegraJOBNo && x.PurchaseOrder == dto.DetailOrderId)
//                    .Select(s => s.DetailOrderId)
//                    .FirstOrDefault();

//                if (orderDetailsNo == null)
//                    return (false, "Order details not found.", null);

//                // ✅ Get last BreakNo
//                var lastBreakNo = await tempColorSizeBreakupRepo.All()
//                    .Where(x => x.DetailOrderId == orderDetailsNo)
//                    .OrderByDescending(x => x.BreakNo)
//                    .Select(x => x.BreakNo)
//                    .FirstOrDefaultAsync();

//                int lastNumber = 0;
//                if (!string.IsNullOrEmpty(lastBreakNo) && lastBreakNo.Contains('_'))
//                {
//                    var parts = lastBreakNo.Split('_');
//                    int.TryParse(parts.Last(), out lastNumber);
//                }

//                // ✅ Get existing combinations for this DetailOrderId
//                var existingCombinations = await tempColorSizeBreakupRepo.All()
//                    .Where(x => x.DetailOrderId == orderDetailsNo)
//                    .Select(x => new { x.ColorId, x.SizeId })
//                    .ToListAsync();

//                var newEntities = new List<RmgProdTempColorSizeBreakup>();
//                int counter = lastNumber + 1;

//                // ✅ Generate color × size combinations
//                foreach (var colorId in dto.ColorIds)
//                {
//                    foreach (var sizeId in dto.SizeIds)
//                    {
//                        // Check if this combination already exists
//                        bool alreadyExists = existingCombinations.Any(e => e.ColorId == colorId && e.SizeId == sizeId);
//                        if (alreadyExists)
//                            continue; // Skip existing combination (no update)

//                        string newBreakNo = $"{orderDetailsNo}_{counter.ToString("D5")}";

//                        newEntities.Add(new RmgProdTempColorSizeBreakup
//                        {
//                            DetailOrderId = orderDetailsNo,
//                            BreakNo = newBreakNo,
//                            ColorId = colorId,
//                            SizeId = sizeId,
//                            Quantity = 0,
//                            UnitTypeId = dto.UnitTypeId,
//                            Remarks = dto.Remarks,
//                            IntegraJobno = dto.IntegraJOBNo,
//                            CompanyCode = companyCode,
//                            Luser = dto.Luser,
//                            Ldate = dto.Ldate,
//                            Lip = dto.Lip,
//                            Lmac = dto.Lmac
//                        });

//                        counter++;
//                    }
//                }

//                if (newEntities.Count > 0)
//                {
//                    await tempColorSizeBreakupRepo.AddRangeAsync(newEntities);
//                    return (true, $"Added {newEntities.Count} new color-size combinations.", newEntities);
//                }
//                else
//                {
//                    return (true, "All selected color-size combinations already exist. No new data added.", null);
//                }
//            }
//            catch (Exception ex)
//            {
//                return (false, $"Error saving breakup: {ex.Message}", null);
//            }
//        }


//        public async Task<(bool isSuccess, string message, object data)> SaveEditColorSizeBreakupListAsync(
//    RMG_Prod_Temp_ColorSizeBreakupDto dto, string companyCode)
//        {
//            try
//            {
//                if (dto.ColorIds == null || dto.ColorIds.Count == 0)
//                    return (false, "No colors selected.", null);
//                if (dto.SizeIds == null || dto.SizeIds.Count == 0)
//                    return (false, "No sizes selected.", null);

//                // ✅ Get OrderDetailsNo
//                var orderDetailsNo = await prodOrderDetailsRepo.All()
//                    .Where(x => x.IntegraJobNo == dto.IntegraJOBNo && x.PurchaseOrder == dto.PONo)
//                    .Select(s => s.DetailOrderId)
//                    .FirstOrDefaultAsync();

//                if (orderDetailsNo == null)
//                    return (false, "Order details not found.", null);

//                // ✅ Get Last BreakNo for sequencing
//                var lastBreakNo = await tempColorSizeBreakupRepo.All()
//                    .Where(x => x.DetailOrderId == orderDetailsNo)
//                    .OrderByDescending(x => x.BreakNo)
//                    .Select(x => x.BreakNo)
//                    .FirstOrDefaultAsync();

//                int lastNumber = 0;
//                if (!string.IsNullOrEmpty(lastBreakNo) && lastBreakNo.Contains('_'))
//                {
//                    var parts = lastBreakNo.Split('_');
//                    int.TryParse(parts.Last(), out lastNumber);
//                }

//                // ✅ Get existing combinations (to skip duplicates)
//                var existingCombos = await tempColorSizeBreakupRepo.All()
//                    .Where(x => x.DetailOrderId == orderDetailsNo)
//                    .Select(x => new { x.ColorId, x.SizeId })
//                    .ToListAsync();

//                var newEntities = new List<RmgProdTempColorSizeBreakup>();
//                int counter = lastNumber + 1;

//                foreach (var colorId in dto.ColorIds)
//                {
//                    foreach (var sizeId in dto.SizeIds)
//                    {
//                        bool alreadyExists = existingCombos.Any(c => c.ColorId == colorId && c.SizeId == sizeId);
//                        if (alreadyExists)
//                            continue; // ❌ skip duplicate

//                        string newBreakNo = $"{orderDetailsNo}_{counter.ToString("D5")}";

//                        newEntities.Add(new RmgProdTempColorSizeBreakup
//                        {
//                            DetailOrderId = orderDetailsNo,
//                            BreakNo = newBreakNo,
//                            ColorId = colorId,
//                            SizeId = sizeId,
//                            Quantity = 0,
//                            UnitTypeId = dto.UnitTypeId,
//                            Remarks = dto.Remarks,
//                            IntegraJobno = dto.IntegraJOBNo,
//                            CompanyCode = companyCode,
//                            Luser = dto.Luser,
//                            Ldate = dto.Ldate,
//                            Lip = dto.Lip,
//                            Lmac = dto.Lmac
//                        });

//                        counter++;
//                    }
//                }

//                if (!newEntities.Any())
//                    return (false, "All selected color-size combinations already exist.", null);

//                await tempColorSizeBreakupRepo.AddRangeAsync(newEntities);

//                return (true, "Color-size breakup saved successfully.", newEntities);
//            }
//            catch (Exception ex)
//            {
//                return (false, $"Error saving breakup: {ex.Message}", null);
//            }
//        }


//        public async Task<PagedResult<RMG_Prod_Temp_ColorSizeBreakupDto>> GetPagedColorSizeBreakupsAsync(DataTableFilter filter)
//        {
//            var query = tempListColorSizeBreakupRepo.All()
//                .Select(o => new RMG_Prod_Temp_ColorSizeBreakupDto
//                {
//                    TC = o.Tc,
//                    BreakNo = o.BreakNo,
//                    DetailOrderId = o.DetailOrderId,
//                    ColorId = o.ColorId,
//                    SizeId = o.SizeId,
//                    Quantity = o.Quantity,
//                    UnitTypeId = o.UnitTypeId,
//                    Remarks = o.Remarks
//                });

//            // 🔍 Global Search
//            if (!string.IsNullOrEmpty(filter.SearchValue))
//            {
//                var keyword = filter.SearchValue.ToLower();
//                query = query.Where(x =>
//                    (x.BreakNo ?? "").ToLower().Contains(keyword) ||
//                    (x.DetailOrderId ?? "").ToLower().Contains(keyword) ||
//                    (x.ColorId ?? "").ToLower().Contains(keyword) ||
//                    (x.SizeId ?? "").ToLower().Contains(keyword) ||
//                    (x.UnitTypeId ?? "").ToLower().Contains(keyword) ||
//                    (x.Remarks ?? "").ToLower().Contains(keyword));
//            }

//            var totalRecords = await query.CountAsync();

//            // 🧾 Paging
//            var data = await query
//                .Skip(filter.Start)
//                .Take(filter.Length)
//                .ToListAsync();

//            return new PagedResult<RMG_Prod_Temp_ColorSizeBreakupDto>
//            {
//                Draw = filter.Draw,
//                RecordsTotal = totalRecords,
//                RecordsFiltered = totalRecords,
//                Data = data
//            };
//        }

//        public async Task<(bool isSuccess, string message, object data)> DeleteOrderInfoAsync(List<decimal> selectedIds)
//        {
//            try
//            {
//                if (selectedIds == null || !selectedIds.Any())
//                    return (false, "No records selected to delete.", null);

//                // Example: fetch records to delete
//                var ordersToDelete = await orderRepo.All()
//                    .Where(o => selectedIds.Contains(o.Tc))
//                    .ToListAsync();

//                if (!ordersToDelete.Any())
//                    return (false, "No matching records found.", null);

//                await orderRepo.DeleteRangeAsync(ordersToDelete);

//                return (true, "Selected orders deleted successfully.", null);
//            }
//            catch (Exception ex)
//            {
//                // log exception if needed
//                return (false, $"Delete failed: {ex.Message}", null);
//            }
//        }
//        public async Task<(bool isSuccess, string message, object data)> DeleteOrderDetailsAsync(List<decimal> selectedIds)
//        {
//            try
//            {
//                if (selectedIds == null || !selectedIds.Any())
//                    return (false, "No records selected to delete.", null);

//                // Example: fetch records to delete
//                var detailsToDelete = await prodOrderDetailsRepo.All()
//                    .Where(o => selectedIds.Contains(o.Tc))
//                    .ToListAsync();

//                if (!detailsToDelete.Any())
//                    return (false, "No matching records found.", null);

//                await prodOrderDetailsRepo.DeleteRangeAsync(detailsToDelete);

//                return (true, "Selected orders deleted successfully.", null);
//            }
//            catch (Exception ex)
//            {
//                // log exception if needed
//                return (false, $"Delete failed: {ex.Message}", null);
//            }
//        }

//        public async Task<bool> UpdateColorSizeBreakupsAsync(List<RMG_Prod_Temp_ColorSizeBreakupDto> dtos)
//        {
//            foreach (var dto in dtos)
//            {
//                var entity = await tempColorSizeBreakupRepo.All().FirstOrDefaultAsync(x => x.Tc == dto.TC);
//                if (entity != null)
//                {
//                    entity.ColorId = dto.ColorId;
//                    entity.SizeId = dto.SizeId;
//                    entity.Quantity = dto.Quantity;
//                    entity.UnitTypeId = dto.UnitTypeId;
//                    entity.Remarks = dto.Remarks;

//                    await tempColorSizeBreakupRepo.UpdateAsync(entity);
//                }
//            }

//            return true;
//        }
//        public async Task SaveFromTempToMainAsync(RMG_Prod_Temp_ColorSizeBreakupDto fromData)
//        {

//            await tempListColorSizeBreakupRepo.BeginTransactionAsync();

//            try
//            {
//                var detailsOrderId = await prodOrderDetailsRepo.All().Where(x => x.IntegraJobNo == fromData.IntegraJOBNo && x.PurchaseOrder == fromData.PONo).Select(w => w.DetailOrderId).FirstOrDefaultAsync();
//                // 1. Main table old data delete
//                var existing = await tempListColorSizeBreakupRepo.All().Where(x => x.IntegraJobno == fromData.IntegraJOBNo && x.DetailOrderId == detailsOrderId).ToListAsync();
//                foreach (var item in existing)
//                    await tempListColorSizeBreakupRepo.DeleteAsync(item);
//                //await tempListColorSizeBreakupRepo.Add();

//                // 2. Temp data main table এ insert
//                var tempData = await tempColorSizeBreakupRepo.All().Where(x => x.IntegraJobno == fromData.IntegraJOBNo).ToListAsync();
//                foreach (var t in tempData)
//                {
//                    var main = new RmgProdTempListColorSizeBreakup
//                    {
//                        IntegraJobno = t.IntegraJobno,
//                        //PONo = t.PONo,
//                        BreakNo = t.BreakNo,
//                        DetailOrderId = t.DetailOrderId,
//                        ColorId = t.ColorId,
//                        SizeId = t.SizeId,
//                        Quantity = t.Quantity,
//                        UnitTypeId = t.UnitTypeId,
//                        Remarks = t.Remarks
//                    };
//                    await tempListColorSizeBreakupRepo.AddAsync(main);
//                }
//                //await tempListColorSizeBreakupRepo.AddAsync();
//                await tempColorSizeBreakupRepo.DeleteRangeAsync(tempData);
//                // 3. Commit
//                await tempListColorSizeBreakupRepo.CommitTransactionAsync();
//            }
//            catch
//            {
//                await tempListColorSizeBreakupRepo.RollbackTransactionAsync();
//                throw;
//            }

//        }


//        public async Task ClearTempDataAsync(string integraJobNo)
//        {
//            try
//            {
//                var tempData = await tempColorSizeBreakupRepo.GetAllAsync();
//                var itemsToDelete = tempData.Where(x => x.IntegraJobno == integraJobNo).ToList();

//                foreach (var item in itemsToDelete)
//                {
//                    await tempColorSizeBreakupRepo.DeleteAsync(item);
//                }

//                await tempColorSizeBreakupRepo.AllAsync();
//            }
//            catch (Exception ex)
//            {
//                throw new Exception("Error clearing temp data: " + ex.Message, ex);
//            }
//        }

//        public async Task<(List<string> colorIds, List<string> sizeIds)> PoIjobNoGetTempAsync(OrderJobDto orderJobDto)
//        {
//            try
//            {
//                var exTempData = tempColorSizeBreakupRepo.All().ToList();
//                if (exTempData != null && exTempData.Any())
//                {
//                    await tempColorSizeBreakupRepo.DeleteRangeAsync(exTempData);
//                }

//                // 🔹 Database order fetch
//                var data = await prodOrderDetailsRepo.All()
//                    .Where(x => x.PurchaseOrder == orderJobDto.PoId.ToString()
//                                && x.IntegraJobNo == orderJobDto.IJobNo)
//                    .Select(x => new
//                    {
//                        x.OrderId,
//                        x.PurchaseOrder,
//                        x.ProductId,
//                        x.IntegraJobNo,
//                        x.DetailOrderId
//                    })
//                    .FirstOrDefaultAsync();

//                if (data == null)
//                {
//                    return (new List<string>(), new List<string>());
//                }

//                // 🔹 Temp data fetch
//                var tempData = await tempListColorSizeBreakupRepo.All()
//                    .Where(x => x.DetailOrderId == data.DetailOrderId && x.IntegraJobno == data.IntegraJobNo)
//                    .ToListAsync();

//                if (tempData.Any())
//                {
//                    var tempDataToAdd = tempData.Select(x => new RmgProdTempColorSizeBreakup
//                    {
//                        BreakNo = x.BreakNo,
//                        DetailOrderId = x.DetailOrderId,
//                        IntegraJobno = x.IntegraJobno,
//                        ColorId = x.ColorId,
//                        SizeId = x.SizeId,
//                        Quantity = x.Quantity,
//                        CompanyCode = x.CompanyCode,
//                        Ldate = x.Ldate,
//                        Lip = x.Lip,
//                        Lmac = x.Lmac,
//                        Luser = x.Luser,
//                        Remarks = x.Remarks,
//                        UnitTypeId = x.UnitTypeId,
//                    }).ToList();

//                    await tempColorSizeBreakupRepo.AddRangeAsync(tempDataToAdd);

//                    // ✅ Get unique ColorIds and SizeIds
//                    var uniqueColorIds = tempData
//                        .Select(x => x.ColorId)
//                        .Distinct()
//                        .Where(x => !string.IsNullOrEmpty(x))
//                        .ToList();

//                    var uniqueSizeIds = tempData
//                        .Select(x => x.SizeId)
//                        .Distinct()
//                        .Where(x => !string.IsNullOrEmpty(x))
//                        .ToList();

//                    return (uniqueColorIds, uniqueSizeIds);
//                }
//                else
//                {
//                    Console.WriteLine("⚠️ No temp color/size breakup data found");
//                    return (new List<string>(), new List<string>());
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine("❌ Error in PoIjobNoGetTempAsync: " + ex.Message);
//                throw;
//            }
//        }



//        public async Task<List<MerchandiserContactPersonDto>> GetMerchandiserContactPersonListAsync()
//        {

//            var query = from offi in offiRepo.All()
//                        join emp in empRepo.All() on offi.EmployeeId equals emp.EmployeeId
//                        join des in desRepo.All() on offi.DesignationCode equals des.DesignationCode
//                        where des.DesignationCode == "004"
//                        select new MerchandiserContactPersonDto
//                        {
//                            EmployeeId = emp.EmployeeId,
//                            FullName = emp.FirstName + " " + emp.LastName,
//                            DesignationName = des.DesignationName,
//                            MobileNo = offi.MobileNo,
//                            Email = offi.Email
//                        };

//            return await query.ToListAsync();
//        }
//    }
//}

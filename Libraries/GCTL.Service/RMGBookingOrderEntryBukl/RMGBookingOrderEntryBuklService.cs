//using GCTL.Core.Data;
//using GCTL.Core.ViewModels.RMGBookingOrderEntryBukl;
//using GCTL.Data.Models;
//using GCTL.Service.Common;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using SixLabors.ImageSharp;

//namespace GCTL.Service.RMGBookingOrderEntryBukl
//{
//    public class RMGBookingOrderEntryBuklService : AppService<RmgBookingOrder>, IRMGBookingOrderEntryBuklService
//    {
//        private readonly IRepository<RmgBookingOrder> boRepo;
//        private readonly IRepository<CoreAccessCode> accessCodeRepository;
//        private readonly IRepository<InvDefItem> itemRepo;
//        private readonly IRepository<ProdDefStyle> styleRepo;
//        private readonly IRepository<RmgProdDefColor> colorRepo;
//        private readonly IRepository<RmgProdDefUnitType> unitRepo;
//        private readonly IRepository<RmgProdDefSize> sizeRepo;
//        private readonly IRepository<RmgProdDefThreadCount> threadCountRepo;
//        private readonly IRepository<CaDefCurrency> currenciesRepo;
//        private readonly IRepository<RmgInvBookingReceivedDetailsCarton> cartonRepo;
//        private readonly IRepository<RmgInvBookingReceivedDetailsButton> buttonRepo;
//        private readonly IRepository<RmgInvBookingReceivedDetailsExtra> extraRepo;
//        private readonly IRepository<RmgInvBookingReceivedDetailsFebric> febricRepo;
//        private readonly IRepository<RmgInvBookingReceivedDetailsPoly> polyRepo;
//        private readonly IRepository<RmgInvBookingReceivedDetailsThread> threadRepo;
//        private readonly IRepository<RmgInvBookingReceivedDetailsExtraTemp> extraTempRepo;
//        private readonly IRepository<RmgInvBookingReceivedDetailsButtonTemp> buttonTempRepo;
//        private readonly IRepository<RmgInvBookingReceivedDetailsFebricTemp> febricTempRepo;
//        private readonly IRepository<RmgInvBookingReceivedDetailsCartonTemp> cartonTempRepo;
//        private readonly IRepository<RmgInvBookingReceivedDetailsPolyTemp> polyTempRepo;
//        private readonly IRepository<RmgInvBookingReceivedDetailsThreadTemp> threadTempRepo;
//        private readonly IRepository<InvDefBookingItemType> bTypeRepo;
//        private readonly IRepository<RmgProdDefBuyer> buyerRepo;
//        private readonly IRepository<RmgDefSupplier> supplierRepo;
//        private readonly IRepository<CaDefCountry> countryRepo;
//        private readonly IRepository<HrmEmployee2> empRepo;
//        private readonly IRepository<HrmEmployeeOfficialInfo> empOffiRepo;
//        private readonly IRepository<RmgProdDefDeliveryMethod> deliveryRepo;
//        private readonly IRepository<SalesDefPaymentTerms> paymentTermRepo;
//        private readonly IRepository<HrmDefDesignation> degRepo;
//        private readonly IRepository<RmgCostingInfo> costingRepo;
//        private readonly IRepository<InvDefBookingItemType> bookTypeRepo;
//        private readonly ICommonService commonService;
//        private readonly string _connectionString;

//        public RMGBookingOrderEntryBuklService(
//            IRepository<RmgBookingOrder> boRepo,
//            IRepository<CoreAccessCode> accessCodeRepository,
//            IRepository<InvDefItem> itemRepo,
//            IRepository<ProdDefStyle> styleRepo,
//            IRepository<RmgProdDefColor> colorRepo,
//            IRepository<RmgProdDefUnitType> unitRepo,
//            IRepository<RmgProdDefSize> sizeRepo,
//            IRepository<RmgProdDefThreadCount> threadCountRepo,
//            IRepository<CaDefCurrency> currenciesRepo,
//            IRepository<RmgInvBookingReceivedDetailsCarton> cartonRepo,
//            IRepository<RmgInvBookingReceivedDetailsButton> buttonRepo,
//            IRepository<RmgInvBookingReceivedDetailsButtonTemp> buttonTempRepo,
//            IRepository<RmgInvBookingReceivedDetailsExtra> extraRepo,
//            IRepository<RmgInvBookingReceivedDetailsExtraTemp> extraTempRepo,
//            IRepository<RmgInvBookingReceivedDetailsFebric> febricRepo,
//            IRepository<RmgInvBookingReceivedDetailsFebricTemp> febricTempRepo,
//            IRepository<RmgInvBookingReceivedDetailsCartonTemp> cartonTempRepo,
//            IRepository<RmgInvBookingReceivedDetailsPoly> polyRepo,
//            IRepository<RmgInvBookingReceivedDetailsPolyTemp> polyTempRepo,
//            IRepository<RmgInvBookingReceivedDetailsThread> threadRepo,
//            IRepository<RmgInvBookingReceivedDetailsThreadTemp> threadTempRepo,
//            IRepository<InvDefBookingItemType> bTypeRepo,
//            IRepository<RmgProdDefBuyer> buyerRepo,
//            IRepository<RmgDefSupplier> supplierRepo,
//            IRepository<CaDefCountry> countryRepo,
//            IRepository<HrmEmployee2> empRepo,
//            IRepository<HrmEmployeeOfficialInfo> empOffiRepo,
//            IRepository<RmgProdDefDeliveryMethod> deliveryRepo,
//            IRepository<SalesDefPaymentTerms> paymentTermRepo,
//            IRepository<HrmDefDesignation> degRepo,
//            IRepository<RmgCostingInfo> costingRepo,
//            IRepository<InvDefBookingItemType> bookTypeRepo,
//            IConfiguration configuration,
//            ICommonService commonService

//            ) : base(boRepo)
//        {
//            this.boRepo = boRepo;
//            this.accessCodeRepository = accessCodeRepository;
//            this.itemRepo = itemRepo;
//            this.styleRepo = styleRepo;
//            this.colorRepo = colorRepo;
//            this.unitRepo = unitRepo;
//            this.sizeRepo = sizeRepo;
//            this.threadCountRepo = threadCountRepo;
//            this.currenciesRepo = currenciesRepo;
//            this.cartonRepo = cartonRepo;
//            this.buttonRepo = buttonRepo;
//            this.buttonTempRepo = buttonTempRepo;
//            this.extraRepo = extraRepo;
//            this.extraTempRepo = extraTempRepo;
//            this.febricRepo = febricRepo;
//            this.febricTempRepo = febricTempRepo;
//            this.cartonTempRepo = cartonTempRepo;
//            this.polyRepo = polyRepo;
//            this.polyTempRepo = polyTempRepo;
//            this.threadRepo = threadRepo;
//            this.threadTempRepo = threadTempRepo;
//            this.bTypeRepo = bTypeRepo;
//            this.buyerRepo = buyerRepo;
//            this.supplierRepo = supplierRepo;
//            this.countryRepo = countryRepo;
//            this.empRepo = empRepo;
//            this.empOffiRepo = empOffiRepo;
//            this.deliveryRepo = deliveryRepo;
//            this.paymentTermRepo = paymentTermRepo;
//            this.degRepo = degRepo;
//            this.costingRepo = costingRepo;
//            this.bookTypeRepo = bookTypeRepo;
//            this.commonService = commonService;
//            _connectionString = configuration.GetConnectionString("ApplicationDbConnection");
//        }

//        private readonly string CreateSuccess = "Data saved successfully.";
//        private readonly string CreateFailed = "Data insertion failed.";
//        private readonly string UpdateSuccess = "Data updated successfully.";
//        private readonly string UpdateFailed = "Data update failed.";
//        private readonly string DeleteSuccess = "Data deleted successfully.";
//        private readonly string DeleteFailed = "Data deletion failed.";
//        private readonly string DataExists = "Data already exists.";




//        #region Duplicate Check 

//        public async Task<bool> IsExistByCodeAsync(string code)
//        {
//            return await boRepo.All().AnyAsync(x => x.BookinOrderNo == code);
//        }

//        public async Task<bool> IsExistAsync(string name)
//        {
//            return await boRepo.All().AnyAsync(x => x.BookinOrderNo == name);
//        }

//        public async Task<bool> IsExistAsync(string employeeCode, string phone, string email)
//        {
//            var result = boRepo.All().FirstOrDefault(e => e.BookinOrderNo == employeeCode);

//            return await boRepo.All().AnyAsync(x => x.BookinOrderNo == employeeCode && x.BookinOrderNo == phone && x.BookinOrderNo == email);
//        }

//        #endregion

//        #region Permission all type

//        public async Task<bool> PagePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "RMG Booking Order Bukl" && x.TitleCheck);
//        }

//        public async Task<bool> SavePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "RMG Booking Order Bukl" && x.CheckAdd);
//        }

//        public async Task<bool> UpdatePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "RMG Booking Order Bukl" && x.CheckEdit);
//        }

//        public async Task<bool> DeletePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "RMG Booking Order Bukl" && x.CheckDelete);
//        }

//        #endregion


//        public async Task<(bool isSuccess, string message)> SaveBookingAsync(RMGBookingOrderEntryBuklDto dto, string companyCode)
//        {

//            try
//            {

//                try
//                {
//                    if (dto.BookingType == null || !dto.BookingType.Any())
//                        return (false, CreateFailed);


//                    switch (dto.BookingType)
//                    {
//                        case "04":
//                            await CartonBookingData();
//                            break;
//                        case "07":
//                            await ThreadBookingData();
//                            break;
//                        case "03":
//                            await PolyBookingData();
//                            break;
//                        case "02":
//                            await ButtonBookingData();
//                            break;
//                        case "01":
//                            await FebricBookingData();
//                            break;
//                        default:
//                            await ExtraBookingData();
//                            break;
//                    }

//                }
//                catch (Exception ex)
//                {

//                    return (false, $"Details Save Failed: {CreateFailed}. Error: {ex.Message}");
//                }


//                if (dto == null || string.IsNullOrEmpty(dto.BookinOrderNo))
//                {
//                    return (false, "Booking Order No cannot be empty!");
//                }

//                if (dto.Tc == 0) // *** CREATE ***
//                {
//                    // Check duplicate BookingOrderNo
//                    bool exists = await IsExistByCodeAsync(dto.BookinOrderNo);
//                    if (exists)
//                        return (false, "This Booking Order No already exists!");





//                    foreach (var costingId in dto.SelectedCostingIds)
//                    {
//                        var y = DateTime.Now.ToString("yyyy");
//                        var CItem = costingRepo.All().Where(x => x.CostingId == costingId).FirstOrDefault();
//                        if (CItem != null)
//                        {
//                            var entity = new RmgBookingOrder
//                            {

//                                BookinOrderNo = commonService.GenerateNextCode("BookinOrderNO", "RMG_BookingOrder", 3, "FAWI-" + y + "-") ?? "",
//                                BookinDate = (DateTime)dto.BookinDate,
//                                BuyerId = CItem.BuyerId ?? "",
//                                StyleId = CItem.StyleId ?? "",
//                                MasterPurchaseOrder = CItem.MasterPurchaseOrder ?? "",
//                                PoNo = CItem.PoNo ?? "",
//                                IntegraJobNo = CItem.IntegraJobNo ?? "",
//                                PurchasedOfficer = dto.PurchasedOfficer ?? "",
//                                Remarks = dto.Remarks ?? "",
//                                EmployeId = dto.UserInfoEmployeeId ?? "",
//                                CompanyId = companyCode ?? "",
//                                DeliveryDate = dto.DeliveryDate,
//                                DeliveryAddress = dto.DeliveryAddress ?? "",
//                                DeliveryMethod = dto.DeliveryMethod ?? "",
//                                PaymentTerms = dto.PaymentTerms ?? "",
//                                TermsCondition = dto.TermsCondition ?? "",
//                                BookingType = dto.BookingType ?? "",
//                                BookingEntryType = dto.BookingEntryType ?? "",
//                                WarehouseId = dto.WarehouseId ?? "",
//                                Pino = dto.Pino ?? "",
//                                Pidate = dto.Pidate,
//                                Pivalue = dto.Pivalue,
//                                PicurrencyId = dto.PicurrencyId ?? "",
//                                SupplierId = dto.SupplierId ?? "",
//                                Mrbpid = dto.Mrbpid ?? "",
//                                EnterFromPageName = dto.EnterFromPageName ?? "",
//                                PifilePath = dto.PifilePath ?? "",
//                                Ldate = dto.Ldate ?? null,
//                                Lmac = dto.Lmac ?? "",
//                                Lip = dto.Lip ?? "",
//                                Luser = dto.Luser ?? "",

//                            };

//                            await boRepo.AddAsync(entity);
//                        }
//                    }


//                    //await transaction.CommitAsync();
//                    return (true, CreateSuccess);
//                }
//                else // *** UPDATE ***
//                {
//                    var entity = await boRepo.All().FirstOrDefaultAsync(x => x.BookinOrderNo == dto.BookinOrderNo);

//                    if (entity == null)
//                        return (false, "Booking entry not found!");

//                    entity.BookinDate = (DateTime)dto.BookinDate;
//                    entity.PurchasedOfficer = dto.PurchasedOfficer ?? "";
//                    entity.Remarks = dto.Remarks ?? "";
//                    entity.CompanyId = companyCode ?? "";
//                    entity.DeliveryDate = dto.DeliveryDate;
//                    entity.DeliveryAddress = dto.DeliveryAddress ?? "";
//                    entity.DeliveryMethod = dto.DeliveryMethod ?? "";
//                    entity.PaymentTerms = dto.PaymentTerms ?? "";
//                    entity.TermsCondition = dto.TermsCondition ?? "";
//                    entity.BookingEntryType = dto.BookingEntryType ?? "";
//                    entity.EmployeId = dto.UserInfoEmployeeId ?? "";
//                    entity.WarehouseId = dto.WarehouseId ?? "";
//                    entity.Pidate = dto.Pidate;
//                    entity.Pivalue = dto.Pivalue;
//                    entity.PicurrencyId = dto.PicurrencyId ?? "";
//                    entity.SupplierId = dto.SupplierId ?? "";
//                    entity.Mrbpid = dto.Mrbpid ?? "";
//                    entity.EnterFromPageName = dto.EnterFromPageName ?? "";
//                    entity.PifilePath = dto.PifilePath ?? "";
//                    entity.ModifyDate = DateTime.Now;

//                    await boRepo.UpdateAsync(entity);

//                    return (true, "Booking updated successfully");
//                }

//            }
//            catch (Exception ex)
//            {
//                // await transaction.RollbackAsync(); 
//                return (false, $"Error: {ex.Message}");
//            }
//        }



//        // --- 1. Carton Booking Data (Provided by User, Minor Fix) ---

//        private async Task<List<RmgInvBookingReceivedDetailsCarton>> CartonBookingData()
//        {
//            List<RmgInvBookingReceivedDetailsCarton> newCartonBookings = new();

//            try
//            {
//                var tempData = cartonTempRepo.All().ToList();
//                if (!tempData.Any())
//                    return newCartonBookings;

//                // 🔹 Load existing main data once
//                var existingMainData = cartonRepo.All().ToList();

//                // 🔹 Safe Slno
//                var sNo = existingMainData.Any()
//                    ? existingMainData.Max(x => x.Slno) + 1
//                    : 1;

//                var deleteList = new List<RmgInvBookingReceivedDetailsCarton>();

//                foreach (var tempItem in tempData)
//                {
//                    // 🔍 Find duplicates in main table
//                    var duplicates = existingMainData.Where(x =>
//                        x.PoNo == tempItem.PoNo &&
//                        x.IntegraJobNo == tempItem.IntegraJobNo &&
//                        x.ItemId == tempItem.ItemId
//                    ).ToList();

//                    if (duplicates.Any())
//                    {
//                        deleteList.AddRange(duplicates);
//                    }

//                    // ✅ Fresh insert object
//                    var mainItem = new RmgInvBookingReceivedDetailsCarton
//                    {
//                        PurchaseReceiveNo = tempItem.PurchaseReceiveNo,
//                        PoNo = tempItem.PoNo,
//                        IntegraJobNo = tempItem.IntegraJobNo,
//                        Slno = sNo++,

//                        ItemId = tempItem.ItemId,
//                        ItemDescription = tempItem.ItemDescription,

//                        OrderQty = tempItem.OrderQty,
//                        OrderUnitId = tempItem.OrderUnitId,
//                        RequiredQty = tempItem.RequiredQty,
//                        RequiredQtyUnitId = tempItem.RequiredQtyUnitId,
//                        Consumption = tempItem.Consumption,
//                        ConsumptionUnitId = tempItem.ConsumptionUnitId,

//                        UnitPrice = tempItem.UnitPrice,
//                        TotalPrice = tempItem.TotalPrice,

//                        ColorId = tempItem.ColorId,
//                        SizeId = tempItem.SizeId,
//                        Refcode = tempItem.Refcode,

//                        CartonLeangth = tempItem.CartonLeangth,
//                        LeangthUnitId = tempItem.LeangthUnitId,
//                        CartonWidth = tempItem.CartonWidth,
//                        WidthUnitId = tempItem.WidthUnitId,
//                        CatonHeight = tempItem.CatonHeight,
//                        HeightUnitId = tempItem.HeightUnitId,

//                        CartonPercent = int.TryParse(tempItem.CartonPercent, out int cartonPercent)
//                            ? cartonPercent
//                            : 0,

//                        TotalReceivedQty = tempItem.TotalReceivedQty,
//                        CurrentReceiveQty = tempItem.CurrentReceiveQty,
//                        ReceivedUnitPrice = tempItem.ReceivedUnitPrice,
//                        TotalReceivedQtyPre = tempItem.TotalReceivedQtyPre,
//                        PendingReceiveQty = tempItem.PendingReceiveQty,
//                        PendingReceiveQtyPre = tempItem.PendingReceiveQtyPre,

//                        Brdid = tempItem.Brdid,
//                        ReceivedUnitType = tempItem.ReceivedUnitType,
//                        CurrencyId = tempItem.CurrencyId,
//                        Remarks = tempItem.Remarks,
//                        EmployeeId = tempItem.EmployeeId
//                    };

//                    newCartonBookings.Add(mainItem);
//                }

//                // 🗑 Delete duplicates FIRST
//                if (deleteList.Any())
//                    await cartonRepo.DeleteRangeAsync(deleteList);

//                // ➕ Insert fresh data
//                await cartonRepo.AddRangeAsync(newCartonBookings);

//                // 🧹 Clear temp table
//                await cartonTempRepo.DeleteRangeAsync(tempData);

//                return newCartonBookings;
//            }
//            catch (Exception ex)
//            {
//                Console.Error.WriteLine($"Error saving carton booking data: {ex.Message}");
//                throw;
//            }
//        }


//        // --- 2. Thread Booking Data (Provided by User, Complete) ---


//        private async Task<List<RmgInvBookingReceivedDetailsThread>> ThreadBookingData()
//        {
//            List<RmgInvBookingReceivedDetailsThread> newThreadBookings = new();

//            try
//            {
//                var tempData = threadTempRepo.All().ToList();
//                if (!tempData.Any())
//                    return newThreadBookings;

//                // 🔹 Load existing main data once
//                var existingMainData = threadRepo.All().ToList();

//                // 🔹 Safe Slno
//                var sNo = existingMainData.Any()
//                    ? existingMainData.Max(x => x.Slno) + 1
//                    : 1;

//                var deleteList = new List<RmgInvBookingReceivedDetailsThread>();

//                foreach (var tempItem in tempData)
//                {
//                    // 🔍 Find duplicates in main table
//                    var duplicates = existingMainData.Where(x =>
//                        x.PoNo == tempItem.PoNo &&
//                        x.IntegraJobNo == tempItem.IntegraJobNo &&
//                        x.ItemId == tempItem.ItemId
//                    ).ToList();

//                    if (duplicates.Any())
//                    {
//                        deleteList.AddRange(duplicates);
//                    }

//                    // ✅ Fresh insert object
//                    var mainItem = new RmgInvBookingReceivedDetailsThread
//                    {
//                        PurchaseReceiveNo = tempItem.PurchaseReceiveNo,
//                        PoNo = tempItem.PoNo,
//                        IntegraJobNo = tempItem.IntegraJobNo,
//                        Slno = sNo++,

//                        Brdid = tempItem.Brdid,
//                        ItemId = tempItem.ItemId,
//                        ColorId = tempItem.ColorId,

//                        FebricDetail = tempItem.FebricDetail,
//                        ThreadColorId = tempItem.ThreadColorId,
//                        ThreadCountId = tempItem.ThreadCountId,
//                        Refcodepantone = tempItem.Refcodepantone,
//                        ThreadReqUnit = tempItem.ThreadReqUnit,
//                        Threadpercent = tempItem.Threadpercent,

//                        OrderQty = tempItem.OrderQty,
//                        QtyUnitId = tempItem.QtyUnitId,
//                        Consumption = tempItem.Consumption,
//                        ConsumtionUnitId = tempItem.ConsumtionUnitId,
//                        TotalQty = tempItem.TotalQty,
//                        TotalQtyUnitId = tempItem.TotalQtyUnitId,
//                        ReqQty = tempItem.ReqQty,

//                        UnitPrice = tempItem.UnitPrice,
//                        TotalPrice = tempItem.TotalPrice,
//                        CurrencyId = tempItem.CurrencyId,

//                        TotalReceivedQty = tempItem.TotalReceivedQty,
//                        CurrentReceiveQty = tempItem.CurrentReceiveQty,
//                        ReceivedUnitType = tempItem.ReceivedUnitType,
//                        ReceivedUnitPrice = tempItem.ReceivedUnitPrice,
//                        TotalReceivedQtyPre = tempItem.TotalReceivedQtyPre,
//                        PendingReceiveQty = tempItem.PendingReceiveQty,
//                        PendingReceiveQtyPre = tempItem.PendingReceiveQtyPre,

//                        Remarks = tempItem.Remarks,
//                        EmployeeId = tempItem.EmployeeId
//                    };

//                    newThreadBookings.Add(mainItem);
//                }

//                // 🗑 Delete duplicates FIRST
//                if (deleteList.Any())
//                    await threadRepo.DeleteRangeAsync(deleteList);

//                // ➕ Insert fresh data
//                await threadRepo.AddRangeAsync(newThreadBookings);

//                // 🧹 Clear temp table
//                await threadTempRepo.DeleteRangeAsync(tempData);

//                return newThreadBookings;
//            }
//            catch (Exception ex)
//            {
//                Console.Error.WriteLine($"Error saving thread booking data: {ex.Message}");
//                throw;
//            }
//        }

//        // --- 3. Poly Booking Data (Case "03") ---

//        private async Task<List<RmgInvBookingReceivedDetailsPoly>> PolyBookingData()
//        {
//            List<RmgInvBookingReceivedDetailsPoly> newPolyBookings = new();

//            try
//            {
//                var tempData = polyTempRepo.All().ToList();
//                if (!tempData.Any())
//                    return newPolyBookings;

//                // 🔹 Load existing main data once
//                var existingMainData = polyRepo.All().ToList();

//                // 🔹 Safe SerialNo
//                var sNo = existingMainData.Any()
//                    ? existingMainData.Max(x => x.SerialNo) + 1
//                    : 1;

//                var deleteList = new List<RmgInvBookingReceivedDetailsPoly>();

//                foreach (var tempItem in tempData)
//                {
//                    // 🔍 Find duplicate rows
//                    var duplicates = existingMainData.Where(x =>
//                        x.PoNo == tempItem.PoNo &&
//                        x.IntegraJobNo == tempItem.IntegraJobNo &&
//                        x.ItemId == tempItem.ItemId
//                    ).ToList();

//                    if (duplicates.Any())
//                    {
//                        deleteList.AddRange(duplicates);
//                    }

//                    // ✅ Fresh insert object
//                    var mainItem = new RmgInvBookingReceivedDetailsPoly
//                    {
//                        PurchaseReceiveNo = tempItem.PurchaseReceiveNo,
//                        PoNo = tempItem.PoNo,
//                        IntegraJobNo = tempItem.IntegraJobNo,
//                        SerialNo = sNo++,

//                        Brdid = tempItem.Brdid,
//                        ItemId = tempItem.ItemId,
//                        ItemDescription = tempItem.ItemDescription,
//                        ColorId = tempItem.ColorId,
//                        RefernceCode = tempItem.RefernceCode,

//                        Length = tempItem.Length,
//                        LengthUnitId = tempItem.LengthUnitId,
//                        Width = tempItem.Width,
//                        WidthUnitId = tempItem.WidthUnitId,
//                        Flap = tempItem.Flap,
//                        FlapUnitId = tempItem.FlapUnitId,
//                        Guest = tempItem.Guest,
//                        GuestUnitId = tempItem.GuestUnitId,

//                        GarmentQty = tempItem.GarmentQty,
//                        GarmentQtyUnitId = tempItem.GarmentQtyUnitId,
//                        Consumption = tempItem.Consumption,
//                        ConsumptionUnitId = tempItem.ConsumptionUnitId,
//                        TotalQty = tempItem.TotalQty,
//                        TotalQtyUnitId = tempItem.TotalQtyUnitId,
//                        Percentage = tempItem.Percentage,

//                        TotalReceivedQty = tempItem.TotalReceivedQty,
//                        CurrentReceiveQty = tempItem.CurrentReceiveQty,
//                        ReceivedUnitType = tempItem.ReceivedUnitType,
//                        UnitPrice = tempItem.UnitPrice,
//                        ReceivedUnitPrice = tempItem.ReceivedUnitPrice,
//                        TotalPrice = tempItem.TotalPrice,
//                        CurrencyId = tempItem.CurrencyId,
//                        TotalReceivedQtyPre = tempItem.TotalReceivedQtyPre,
//                        PendingReceiveQty = tempItem.PendingReceiveQty,
//                        PendingReceiveQtyPre = tempItem.PendingReceiveQtyPre,

//                        Remarks = tempItem.Remarks,
//                        EmployeeId = tempItem.EmployeeId
//                    };

//                    newPolyBookings.Add(mainItem);
//                }

//                // 🗑 Delete duplicates first
//                if (deleteList.Any())
//                    await polyRepo.DeleteRangeAsync(deleteList);

//                // ➕ Insert fresh data
//                await polyRepo.AddRangeAsync(newPolyBookings);

//                // 🧹 Clear temp table
//                await polyTempRepo.DeleteRangeAsync(tempData);

//                return newPolyBookings;
//            }
//            catch (Exception ex)
//            {
//                Console.Error.WriteLine($"Error saving poly booking data: {ex.Message}");
//                throw;
//            }
//        }


//        // --- 4. Button Booking Data (Case "02") ---

//        private async Task<List<RmgInvBookingReceivedDetailsButton>> ButtonBookingData()
//        {
//            List<RmgInvBookingReceivedDetailsButton> newButtonBookings = new();

//            try
//            {
//                var tempData = buttonTempRepo.All().ToList();
//                if (!tempData.Any())
//                    return newButtonBookings;

//                // 🔹 Load main data ONCE
//                var existingMainData = buttonRepo.All().ToList();

//                // 🔹 Safe Serial start
//                var sNo = existingMainData.Any()
//                    ? existingMainData.Max(x => x.SerialNo) + 1
//                    : 1;

//                var deleteList = new List<RmgInvBookingReceivedDetailsButton>();

//                foreach (var tempItem in tempData)
//                {
//                    // 🔥 IMPORTANT: Find ALL duplicates (not FirstOrDefault)
//                    var duplicates = existingMainData.Where(x =>
//                        x.PurchaseReceiveNo == tempItem.PurchaseReceiveNo &&
//                        x.PoNo == tempItem.PoNo &&
//                        x.IntegraJobNo == tempItem.IntegraJobNo &&
//                        x.ItemId == tempItem.ItemId
//                    ).ToList();

//                    if (duplicates.Any())
//                    {
//                        deleteList.AddRange(duplicates);
//                    }

//                    // ✅ Prepare new row
//                    newButtonBookings.Add(new RmgInvBookingReceivedDetailsButton
//                    {
//                        PurchaseReceiveNo = tempItem.PurchaseReceiveNo,
//                        PoNo = tempItem.PoNo,
//                        IntegraJobNo = tempItem.IntegraJobNo,
//                        SerialNo = sNo++,

//                        Brdid = tempItem.Brdid,
//                        ItemId = tempItem.ItemId,
//                        Description = tempItem.Description,
//                        FabricColorId = tempItem.FabricColorId,
//                        ColorId = tempItem.ColorId,
//                        SizeId = tempItem.SizeId,
//                        Idno = tempItem.Idno,

//                        GermentQty = tempItem.GermentQty,
//                        GermentsQtyUnitId = tempItem.GermentsQtyUnitId,
//                        Consumption = tempItem.Consumption,
//                        ConsumptionUnitId = tempItem.ConsumptionUnitId,
//                        TotalQty = tempItem.TotalQty,
//                        TotalQtyUnitId = tempItem.TotalQtyUnitId,
//                        OrderQty = tempItem.TotalQty.HasValue ? (int)Math.Ceiling((double)tempItem.TotalQty.Value) : 0,
//                        OrderQtyUnitId = tempItem.OrderQtyUnitId,
//                        Percentage = tempItem.Percentage,

//                        TotalReceivedQty = tempItem.TotalReceivedQty,
//                        CurrentReceiveQty = tempItem.CurrentReceiveQty,
//                        ReceivedUnitType = tempItem.ReceivedUnitType,
//                        UnitPrice = tempItem.UnitPrice,
//                        ReceivedUnitPrice = tempItem.ReceivedUnitPrice,
//                        TotalPrice = tempItem.TotalPrice,
//                        CurrencyId = tempItem.CurrencyId,
//                        TotalReceivedQtyPre = tempItem.TotalReceivedQtyPre,
//                        PendingReceiveQty = tempItem.PendingReceiveQty,
//                        PendingReceiveQtyPre = tempItem.PendingReceiveQtyPre,

//                        Remarks = tempItem.Remarks,
//                        EmployeeId = tempItem.EmployeeId
//                    });
//                }

//                // 🗑️ STEP-1: DELETE FIRST
//                if (deleteList.Any())
//                    await buttonRepo.DeleteRangeAsync(deleteList);

//                // ➕ STEP-2: ADD AFTER DELETE
//                await buttonRepo.AddRangeAsync(newButtonBookings);

//                // 🧹 STEP-3: CLEAR TEMP
//                await buttonTempRepo.DeleteRangeAsync(tempData);

//                return newButtonBookings;
//            }
//            catch (Exception ex)
//            {
//                Console.Error.WriteLine($"Error saving button booking data: {ex.Message}");
//                throw;
//            }
//        }



//        // --- 5. Febric Booking Data (Case "01") ---



//        private async Task<List<RmgInvBookingReceivedDetailsFebric>> FebricBookingData()
//        {
//            List<RmgInvBookingReceivedDetailsFebric> newFebricBookings = new();

//            try
//            {
//                var tempData = febricTempRepo.All().ToList();
//                if (!tempData.Any())
//                    return newFebricBookings;

//                // 🔹 Load existing main data once
//                var existingMainData = febricRepo.All().ToList();

//                // 🔹 Safe Slno
//                var sNo = existingMainData.Any()
//                    ? existingMainData.Max(x => x.Slno) + 1
//                    : 1;

//                var deleteList = new List<RmgInvBookingReceivedDetailsFebric>();

//                foreach (var tempItem in tempData)
//                {
//                    // 🔍 Find duplicate rows in main table
//                    var duplicates = existingMainData.Where(x =>
//                        x.PoNo == tempItem.PoNo &&
//                        x.IntegraJobNo == tempItem.IntegraJobNo &&
//                        x.ItemId == tempItem.ItemId
//                    ).ToList();

//                    if (duplicates.Any())
//                    {
//                        deleteList.AddRange(duplicates);
//                    }

//                    // ✅ Fresh insert object
//                    var mainItem = new RmgInvBookingReceivedDetailsFebric
//                    {
//                        PurchaseReceiveNo = tempItem.PurchaseReceiveNo,
//                        PoNo = tempItem.PoNo,
//                        IntegraJobNo = tempItem.IntegraJobNo,
//                        Slno = sNo++,

//                        Brdid = tempItem.Brdid,
//                        ColorId = tempItem.ColorId,
//                        FabricItemId = tempItem.FabricItemId,
//                        ItemId = tempItem.ItemId,
//                        FebricDetails = tempItem.FebricDetails,
//                        Refcode = tempItem.Refcode,

//                        //OrderQty = tempItem.OrderQty,
//                        OrderQty = tempItem.OrderQty.HasValue ? (int)Math.Ceiling((double)tempItem.OrderQty.Value) : 0,
//                        QtyUnit = tempItem.QtyUnit,
//                        Consumption = tempItem.Consumption,
//                        ConsumtionUnit = tempItem.ConsumtionUnit,
//                        TotalFebricQty = tempItem.TotalFebricQty,
//                        Percentage = tempItem.Percentage,

//                        TotalReceivedQty = tempItem.TotalReceivedQty,
//                        CurrentReceiveQty = tempItem.CurrentReceiveQty,
//                        ReceivedUnitType = tempItem.ReceivedUnitType,
//                        UnitPrice = tempItem.UnitPrice,
//                        ReceivedUnitPrice = tempItem.ReceivedUnitPrice,
//                        TotalPrice = tempItem.TotalPrice,
//                        CurrencyId = tempItem.CurrencyId,
//                        TotalReceivedQtyPre = tempItem.TotalReceivedQtyPre,
//                        PendingReceiveQty = tempItem.PendingReceiveQty,
//                        PendingReceiveQtyPre = tempItem.PendingReceiveQtyPre,

//                        EmployeeId = tempItem.EmployeeId
//                    };

//                    newFebricBookings.Add(mainItem);
//                }

//                // 🗑 Delete duplicates first
//                if (deleteList.Any())
//                    await febricRepo.DeleteRangeAsync(deleteList);

//                // ➕ Insert fresh data
//                await febricRepo.AddRangeAsync(newFebricBookings);

//                // 🧹 Clear temp table
//                await febricTempRepo.DeleteRangeAsync(tempData);

//                return newFebricBookings;
//            }
//            catch (Exception ex)
//            {
//                Console.Error.WriteLine($"Error saving febric booking data: {ex.Message}");
//                throw;
//            }
//        }

//        // --- 6. Extra Booking Data (Default) ---

//        private async Task<List<RmgInvBookingReceivedDetailsExtra>> ExtraBookingData()
//        {
//            List<RmgInvBookingReceivedDetailsExtra> newExtraBookings = new();

//            try
//            {
//                var tempData = extraTempRepo.All().ToList();
//                if (!tempData.Any())
//                    return newExtraBookings;

//                // 🔹 Load existing main data once
//                var existingMainData = extraRepo.All().ToList();

//                // 🔹 Safe Slno
//                var sNo = existingMainData.Any()
//                    ? existingMainData.Max(x => x.Slno) + 1
//                    : 1;

//                var deleteList = new List<RmgInvBookingReceivedDetailsExtra>();

//                foreach (var tempItem in tempData)
//                {
//                    // 🔍 Find duplicates in main table
//                    var duplicates = existingMainData.Where(x =>
//                        x.PoNo == tempItem.PoNo &&
//                        x.IntegraJobNo == tempItem.IntegraJobNo &&
//                        x.ItemId == tempItem.ItemId
//                    ).ToList();

//                    if (duplicates.Any())
//                    {
//                        deleteList.AddRange(duplicates);
//                    }

//                    // ✅ Create fresh row
//                    var mainItem = new RmgInvBookingReceivedDetailsExtra
//                    {
//                        PurchaseReceiveNo = tempItem.PurchaseReceiveNo,
//                        PoNo = tempItem.PoNo,
//                        IntegraJobNo = tempItem.IntegraJobNo,
//                        Slno = sNo++,

//                        Brdid = tempItem.Brdid,
//                        FabricColorId = tempItem.FabricColorId,
//                        ItemId = tempItem.ItemId,
//                        Description = tempItem.Description,
//                        ColorId = tempItem.ColorId,

//                        //OrderQty = tempItem.OrderQty,
//                        OrderQty = tempItem.TotalQty.HasValue ? (int)Math.Ceiling((double)tempItem.TotalQty.Value) : 0,
//                        OrderQtyIunitD = tempItem.OrderQtyIunitD,
//                        Consumption = tempItem.Consumption,
//                        ConsumptionUnitId = tempItem.ConsumptionUnitId,
//                        TotalQty = tempItem.TotalQty,
//                        TotalQtyUnitId = tempItem.TotalQtyUnitId,
//                        ReqQty = tempItem.ReqQty,
//                        ReqQtyUnitId = tempItem.ReqQtyUnitId,
//                        Percentage = tempItem.Percentage,

//                        TotalReceivedQty = tempItem.TotalReceivedQty,
//                        CurrentReceiveQty = tempItem.CurrentReceiveQty,
//                        ReceivedUnitType = tempItem.ReceivedUnitType,
//                        UnitPrice = tempItem.UnitPrice,
//                        ReceivedUnitPrice = tempItem.ReceivedUnitPrice,
//                        TotalPrice = tempItem.TotalPrice,
//                        CurrencyId = tempItem.CurrencyId,
//                        TotalReceivedQtyPre = tempItem.TotalReceivedQtyPre,
//                        PendingReceiveQty = tempItem.PendingReceiveQty,
//                        PendingReceiveQtyPre = tempItem.PendingReceiveQtyPre,

//                        Remarks = tempItem.Remarks,
//                        EmployeeId = tempItem.EmployeeId
//                    };

//                    newExtraBookings.Add(mainItem);
//                }

//                // 🗑 Delete duplicates FIRST
//                if (deleteList.Any())
//                    await extraRepo.DeleteRangeAsync(deleteList);

//                // ➕ Insert fresh data
//                await extraRepo.AddRangeAsync(newExtraBookings);

//                // 🧹 Clear temp table
//                await extraTempRepo.DeleteRangeAsync(tempData);

//                return newExtraBookings;
//            }
//            catch (Exception ex)
//            {
//                Console.Error.WriteLine($"Error saving extra booking data: {ex.Message}");
//                throw;
//            }
//        }


//        public async Task<(IEnumerable<object> data, int total, int filtered)> GetBookingListAsync(
//        int start, int length, string search, string sortColumn, string sortDir)
//        {
//            var query = boRepo.All();

//            int totalData = await query.CountAsync();

//            // Searching
//            if (!string.IsNullOrEmpty(search))
//            {
//                query = query.Where(x =>
//                    x.BookinOrderNo.Contains(search) ||
//                    x.SupplierId.Contains(search) ||
//                    x.StyleId.Contains(search) ||
//                    x.PoNo.Contains(search));
//            }

//            int filteredData = await query.CountAsync();

//            // Sorting
//            query = sortColumn switch
//            {
//                "BookinOrderNo" => (sortDir == "asc") ? query.OrderBy(x => x.BookinOrderNo) : query.OrderByDescending(x => x.BookinOrderNo),
//                "BookinDate" => (sortDir == "asc") ? query.OrderBy(x => x.BookinDate) : query.OrderByDescending(x => x.BookinDate),
//                "SupplierId" => (sortDir == "asc") ? query.OrderBy(x => x.SupplierId) : query.OrderByDescending(x => x.SupplierId),
//                _ => query.OrderByDescending(x => x.Tc)
//            };

//            // Paging
//            var data = await query
//                .Skip(start)
//                .Take(length)
//                .Select(x => new
//                {
//                    Tc = x.Tc,
//                    BookingOrderNo = x.BookinOrderNo,
//                    BookingDate = x.BookinDate.ToString("dd/MM/yyyy"),
//                    BuyerId = x.BuyerId,
//                    BuyerName = buyerRepo.All().Where(s => s.BuyerId == x.BuyerId).Select(c => c.BuyerName).FirstOrDefault(),
//                    StyleId = x.StyleId,
//                    StyleName = styleRepo.All().Where(s => s.StyleId == x.StyleId).Select(c => c.Style).FirstOrDefault(),
//                    MasterPurchaseOrder = x.MasterPurchaseOrder,
//                    PoNo = x.PoNo,
//                    IntegraJobNo = x.IntegraJobNo,
//                    PurchasedOfficer = x.PurchasedOfficer,
//                    Remarks = x.Remarks,
//                    Luser = x.Luser,
//                    Ldate = x.Ldate.HasValue ? x.Ldate.Value.ToString("dd/MM/yyyy") : "",
//                    Lip = x.Lip,
//                    Lmac = x.Lmac,
//                    ModifyDate = x.ModifyDate.HasValue ? x.ModifyDate.Value.ToString("dd/MM/yyyy") : "",
//                    EmployeId = x.EmployeId,
//                    CompanyId = x.CompanyId,
//                    DeliveryDate = x.DeliveryDate.HasValue ? x.DeliveryDate.Value.ToString("dd/MM/yyyy") : "",
//                    DeliveryAddress = x.DeliveryAddress,
//                    DeliveryMethod = x.DeliveryMethod,
//                    PaymentTerms = x.PaymentTerms,
//                    TermsCondition = x.TermsCondition,
//                    BookingType = x.BookingType,
//                    BookingTypeName = bookTypeRepo.All().Where(s => s.BookingItemTypeId == x.BookingType).Select(c => c.BookingItemType).FirstOrDefault(),
//                    BookingEntryType = x.BookingEntryType,
//                    WarehouseId = x.WarehouseId,
//                    Pino = x.Pino,
//                    Pidate = x.Pidate.HasValue ? x.Pidate.Value.ToString("dd/MM/yyyy") : "",
//                    Pivalue = x.Pivalue,
//                    PicurrencyId = x.PicurrencyId,
//                    SupplierId = x.SupplierId,
//                    SupplierName = supplierRepo.All().Where(s => s.SupplierId == x.SupplierId).Select(c => c.SupplierName).FirstOrDefault(),
//                    Mrbpid = x.Mrbpid,
//                    EnterFromPageName = x.EnterFromPageName,
//                    PifilePath = x.PifilePath
//                }).ToListAsync();

//            return (data, totalData, filteredData);
//        }

//        public async Task<(bool isSuccess, string message, object data)> GetBookingItemTypesAsync(string id)
//        {
//            try
//            {
//                var bookingData = boRepo.All().FirstOrDefault(x => x.BookinOrderNo == id);

//                if (bookingData == null)
//                    return (false, "Booking data not found.", null);

//                try
//                {
//                    if (string.IsNullOrEmpty(bookingData.BookingType))
//                        return (false, CreateFailed, null);

//                    object data = null;

//                    switch (bookingData.BookingType)
//                    {
//                        case "04":
//                            data = await GetCartonBookingDataList(bookingData);
//                            break;
//                        case "07":
//                            data = await GetThreadBookingDataList(bookingData);
//                            break;
//                        case "03":
//                            data = await GetPolyBookingDataList(bookingData);
//                            break;
//                        case "02":
//                            data = await GetButtonBookingDataList(bookingData);
//                            break;
//                        case "01":
//                            data = await GetFebricBookingDataList(bookingData);
//                            break;
//                        default:
//                            data = await GetExtraBookingDataList(bookingData);
//                            break;
//                    }

//                    return (true, "Booking items copied to temp successfully.", data);
//                }
//                catch (Exception ex)
//                {
//                    return (false, $"Details Save Failed. Error: {ex.Message}", null);
//                }
//            }
//            catch (Exception ex)
//            {
//                return (false, $"Unexpected error: {ex.Message}", null);
//            }
//        }


//        public async Task<List<object>> GetCartonBookingDataList(RmgBookingOrder bookingData)
//        {
//            try
//            {
//                // 1️⃣ Get main records
//                var mainRecords = cartonRepo.All()
//                    .Where(x => x.PoNo == bookingData.PoNo && x.IntegraJobNo == bookingData.IntegraJobNo)
//                    .ToList();

//                if (!mainRecords.Any())
//                    return new List<object>();

//                // 2️⃣ Clear temp table
//                var existingTemp = cartonTempRepo.All().ToList();
//                if (existingTemp.Any())
//                    await cartonTempRepo.DeleteRangeAsync(existingTemp);

//                // 3️⃣ Copy main → temp
//                var newCartonTempData = mainRecords.Select(mainItem =>
//                {
//                    var orderQty = mainItem.OrderQty.HasValue ? (int)Math.Ceiling((double)mainItem.OrderQty.Value) : 0;

//                    return new RmgInvBookingReceivedDetailsCartonTemp
//                    {
//                        PurchaseReceiveNo = mainItem.PurchaseReceiveNo,
//                        ItemId = mainItem.ItemId,
//                        ItemDescription = mainItem.ItemDescription,
//                        OrderQty = orderQty,
//                        OrderUnitId = mainItem.OrderUnitId,
//                        RequiredQty = mainItem.RequiredQty,
//                        RequiredQtyUnitId = mainItem.RequiredQtyUnitId,
//                        ConsumptionUnitId = mainItem.ConsumptionUnitId,
//                        Consumption = mainItem.Consumption,
//                        UnitPrice = mainItem.UnitPrice,
//                        TotalPrice = mainItem.TotalPrice,
//                        PoNo = mainItem.PoNo,
//                        IntegraJobNo = mainItem.IntegraJobNo,
//                        Slno = mainItem.Slno,
//                        ColorId = mainItem.ColorId,
//                        SizeId = mainItem.SizeId,
//                        Refcode = mainItem.Refcode,
//                        CartonLeangth = mainItem.CartonLeangth,
//                        LeangthUnitId = mainItem.LeangthUnitId,
//                        CartonWidth = mainItem.CartonWidth,
//                        WidthUnitId = mainItem.WidthUnitId,
//                        CatonHeight = mainItem.CatonHeight,
//                        HeightUnitId = mainItem.HeightUnitId,
//                        CartonPercent = mainItem.CartonPercent?.ToString() ?? "0",
//                        TotalReceivedQty = mainItem.TotalReceivedQty,
//                        CurrentReceiveQty = mainItem.CurrentReceiveQty,
//                        ReceivedUnitPrice = mainItem.ReceivedUnitPrice,
//                        TotalReceivedQtyPre = mainItem.TotalReceivedQtyPre,
//                        PendingReceiveQty = mainItem.PendingReceiveQty,
//                        PendingReceiveQtyPre = mainItem.PendingReceiveQtyPre,
//                        Brdid = mainItem.Brdid,
//                        ReceivedUnitType = mainItem.ReceivedUnitType,
//                        CurrencyId = mainItem.CurrencyId,
//                        Remarks = mainItem.Remarks,
//                        EmployeeId = mainItem.EmployeeId
//                    };
//                }).ToList();

//                await cartonTempRepo.AddRangeAsync(newCartonTempData);

//                // 4️⃣ Return shaped projection for AJAX
//                var tempList = cartonTempRepo.All().ToList();  // now IDs are populated

//                var result = tempList.Select(x =>
//                {
//                    var garmentQty = x.OrderQty;
//                    var consumption = x.Consumption ?? 0;
//                    var totalQty = x.RequiredQty ?? 0;

//                    return new
//                    {
//                        x.Id,
//                        x.PoNo,
//                        x.ItemId,
//                        x.ItemDescription,
//                        x.ColorId,
//                        x.SizeId,
//                        x.CartonLeangth,
//                        x.LeangthUnitId,
//                        x.CartonWidth,
//                        x.WidthUnitId,
//                        x.CatonHeight,
//                        x.HeightUnitId,
//                        GarmentQty = garmentQty,
//                        OrderQty = garmentQty,
//                        GarmentQtyUnitId = x.OrderUnitId,
//                        Consumption = consumption,
//                        ConsumptionUnitId = x.ConsumptionUnitId,
//                        TotalQty = totalQty,
//                        TotalQtyUnitId = x.RequiredQtyUnitId,
//                        Percentage = x.CartonPercent,
//                        UnitPrice = x.UnitPrice,
//                        ReceivedUnitPrice = x.ReceivedUnitPrice,
//                        TotalPrice = x.TotalPrice,
//                        CurrencyId = x.CurrencyId,
//                        Remarks = x.Remarks
//                    };
//                }).Cast<object>().ToList();

//                return result;
//            }
//            catch (Exception ex)
//            {
//                Console.Error.WriteLine($"Error copying carton booking data to temp: {ex.Message}");
//                throw;
//            }
//        }

//        public async Task<List<object>> GetThreadBookingDataList(RmgBookingOrder bookingData)
//        {
//            var resultList = new List<object>();
//            var newThreadTempData = new List<RmgInvBookingReceivedDetailsThreadTemp>();

//            try
//            {
//                // 1️⃣ Get main records
//                var mainRecords = threadRepo.All()
//                    .Where(x => x.PoNo == bookingData.PoNo
//                             && x.IntegraJobNo == bookingData.IntegraJobNo)
//                    .ToList();

//                if (!mainRecords.Any())
//                    return resultList;

//                // 2️⃣ Clear temp table
//                var existingTemp = threadTempRepo.All().ToList();
//                if (existingTemp.Any())
//                    await threadTempRepo.DeleteRangeAsync(existingTemp);

//                // 3️⃣ Copy main → temp (but don’t build resultList yet)
//                foreach (var mainItem in mainRecords)
//                {
//                    var orderQty = mainItem.TotalQty.HasValue
//                        ? (int)Math.Ceiling((double)mainItem.TotalQty.Value)
//                        : 0;

//                    var tempItem = new RmgInvBookingReceivedDetailsThreadTemp
//                    {
//                        PurchaseReceiveNo = mainItem.PurchaseReceiveNo,
//                        PoNo = mainItem.PoNo,
//                        IntegraJobNo = mainItem.IntegraJobNo,
//                        Slno = mainItem.Slno,
//                        Brdid = mainItem.Brdid,
//                        ItemId = mainItem.ItemId,
//                        ColorId = mainItem.ColorId,
//                        FebricDetail = mainItem.FebricDetail,
//                        ThreadColorId = mainItem.ThreadColorId,
//                        ThreadCountId = mainItem.ThreadCountId,
//                        Refcodepantone = mainItem.Refcodepantone,
//                        ThreadReqUnit = mainItem.ThreadReqUnit,
//                        Threadpercent = mainItem.Threadpercent,
//                        OrderQty = orderQty,
//                        QtyUnitId = mainItem.QtyUnitId,
//                        Consumption = mainItem.Consumption,
//                        ConsumtionUnitId = mainItem.ConsumtionUnitId,
//                        TotalQty = mainItem.TotalQty,
//                        TotalQtyUnitId = mainItem.TotalQtyUnitId,
//                        ReqQty = mainItem.ReqQty,
//                        UnitPrice = mainItem.UnitPrice,
//                        TotalPrice = mainItem.TotalPrice,
//                        CurrencyId = mainItem.CurrencyId,
//                        TotalReceivedQty = mainItem.TotalReceivedQty,
//                        CurrentReceiveQty = mainItem.CurrentReceiveQty,
//                        ReceivedUnitType = mainItem.ReceivedUnitType,
//                        ReceivedUnitPrice = mainItem.ReceivedUnitPrice,
//                        TotalReceivedQtyPre = mainItem.TotalReceivedQtyPre,
//                        PendingReceiveQty = mainItem.PendingReceiveQty,
//                        PendingReceiveQtyPre = mainItem.PendingReceiveQtyPre,
//                        Remarks = mainItem.Remarks,
//                        EmployeeId = mainItem.EmployeeId
//                    };

//                    newThreadTempData.Add(tempItem);
//                }

//                // 4️⃣ Save temp table so Ids are generated
//                await threadTempRepo.AddRangeAsync(newThreadTempData);

//                // 5️⃣ Now build resultList with real Ids
//                foreach (var tempItem in newThreadTempData)
//                {
//                    var orderQty = tempItem.OrderQty;
//                    var consumption = tempItem.Consumption ?? 0;
//                    var totalQty = tempItem.TotalQty ?? 0;

//                    resultList.Add(new
//                    {
//                        // ===== DB Fields =====
//                        tempItem.Id,
//                        tempItem.PurchaseReceiveNo,
//                        tempItem.PoNo,
//                        tempItem.IntegraJobNo,
//                        tempItem.Slno,
//                        tempItem.Brdid,
//                        tempItem.ItemId,
//                        tempItem.ColorId,
//                        tempItem.FebricDetail,
//                        tempItem.ThreadColorId,
//                        tempItem.ThreadCountId,
//                        tempItem.Refcodepantone,
//                        tempItem.ThreadReqUnit,
//                        tempItem.Threadpercent,
//                        tempItem.QtyUnitId,
//                        tempItem.ConsumtionUnitId,
//                        tempItem.TotalQtyUnitId,
//                        tempItem.ReqQty,
//                        tempItem.UnitPrice,
//                        tempItem.TotalPrice,
//                        tempItem.CurrencyId,
//                        tempItem.TotalReceivedQty,
//                        tempItem.CurrentReceiveQty,
//                        tempItem.ReceivedUnitType,
//                        tempItem.ReceivedUnitPrice,
//                        tempItem.TotalReceivedQtyPre,
//                        tempItem.PendingReceiveQty,
//                        tempItem.PendingReceiveQtyPre,
//                        tempItem.Remarks,
//                        tempItem.EmployeeId,

//                        // ===== Business / Calculated =====
//                        OrderQty = orderQty,
//                        Consumption = consumption,
//                        TotalQty = totalQty,
//                        ReceiveQty = tempItem.CurrentReceiveQty ?? 0,
//                        PendingQty = tempItem.PendingReceiveQty ?? 0,
//                        GarmentQty = tempItem.OrderQty // or mainItem.OrderQty if needed
//                    });
//                }

//                return resultList;
//            }
//            catch (Exception ex)
//            {
//                Console.Error.WriteLine($"Error copying thread booking data to temp: {ex.Message}");
//                throw;
//            }
//        }
//        public async Task<List<object>> GetPolyBookingDataList(RmgBookingOrder bookingData)
//        {
//            var resultList = new List<object>();
//            var newPolyTempData = new List<RmgInvBookingReceivedDetailsPolyTemp>();

//            try
//            {
//                // 1️⃣ Get main records
//                var mainRecords = polyRepo.All()
//                    .Where(x => x.PoNo == bookingData.PoNo && x.IntegraJobNo == bookingData.IntegraJobNo)
//                    .ToList();

//                if (!mainRecords.Any())
//                    return resultList;

//                // 2️⃣ Clear temp
//                var existingTemp = polyTempRepo.All().ToList();
//                if (existingTemp.Any())
//                    await polyTempRepo.DeleteRangeAsync(existingTemp);

//                // 3️⃣ Copy main → temp (but don’t build resultList yet)
//                foreach (var mainItem in mainRecords)
//                {
//                    var tempItem = new RmgInvBookingReceivedDetailsPolyTemp
//                    {
//                        PurchaseReceiveNo = mainItem.PurchaseReceiveNo,
//                        PoNo = mainItem.PoNo,
//                        IntegraJobNo = mainItem.IntegraJobNo,
//                        SerialNo = mainItem.SerialNo,
//                        Brdid = mainItem.Brdid,
//                        ItemId = mainItem.ItemId,
//                        ItemDescription = mainItem.ItemDescription,
//                        ColorId = mainItem.ColorId,
//                        RefernceCode = mainItem.RefernceCode,
//                        Length = mainItem.Length,
//                        LengthUnitId = mainItem.LengthUnitId,
//                        Width = mainItem.Width,
//                        WidthUnitId = mainItem.WidthUnitId,
//                        Flap = mainItem.Flap,
//                        FlapUnitId = mainItem.FlapUnitId,
//                        Guest = mainItem.Guest,
//                        GuestUnitId = mainItem.GuestUnitId,
//                        GarmentQty = mainItem.GarmentQty,
//                        GarmentQtyUnitId = mainItem.GarmentQtyUnitId,
//                        Consumption = mainItem.Consumption,
//                        ConsumptionUnitId = mainItem.ConsumptionUnitId,
//                        TotalQty = mainItem.TotalQty,
//                        TotalQtyUnitId = mainItem.TotalQtyUnitId,
//                        Percentage = mainItem.Percentage,
//                        TotalReceivedQty = mainItem.TotalReceivedQty,
//                        CurrentReceiveQty = mainItem.CurrentReceiveQty,
//                        ReceivedUnitType = mainItem.ReceivedUnitType,
//                        UnitPrice = mainItem.UnitPrice,
//                        ReceivedUnitPrice = mainItem.ReceivedUnitPrice,
//                        TotalPrice = mainItem.TotalPrice,
//                        CurrencyId = mainItem.CurrencyId,
//                        TotalReceivedQtyPre = mainItem.TotalReceivedQtyPre,
//                        PendingReceiveQty = mainItem.PendingReceiveQty,
//                        PendingReceiveQtyPre = mainItem.PendingReceiveQtyPre,
//                        Remarks = mainItem.Remarks,
//                        EmployeeId = mainItem.EmployeeId,
//                    };

//                    newPolyTempData.Add(tempItem);
//                }

//                // 4️⃣ Save temp table so Ids are generated
//                await polyTempRepo.AddRangeAsync(newPolyTempData);

//                // 5️⃣ Now build resultList with real Ids
//                foreach (var tempItem in newPolyTempData)
//                {
//                    var garmentQty = tempItem.GarmentQty ?? 0;
//                    var consumption = tempItem.Consumption ?? 0;
//                    var totalQty = garmentQty * consumption;

//                    resultList.Add(new
//                    {
//                        // ===== DB Fields =====
//                        tempItem.Id,
//                        tempItem.PurchaseReceiveNo,
//                        tempItem.PoNo,
//                        tempItem.IntegraJobNo,
//                        tempItem.SerialNo,
//                        tempItem.Brdid,
//                        tempItem.ItemId,
//                        tempItem.ItemDescription,
//                        tempItem.ColorId,
//                        tempItem.RefernceCode,
//                        tempItem.Length,
//                        tempItem.LengthUnitId,
//                        tempItem.Width,
//                        tempItem.WidthUnitId,
//                        tempItem.Flap,
//                        tempItem.FlapUnitId,
//                        tempItem.Guest,
//                        tempItem.GuestUnitId,
//                        tempItem.GarmentQty,
//                        tempItem.GarmentQtyUnitId,
//                        tempItem.Consumption,
//                        tempItem.ConsumptionUnitId,
//                        tempItem.TotalQty,
//                        tempItem.TotalQtyUnitId,
//                        tempItem.Percentage,
//                        tempItem.TotalReceivedQty,
//                        tempItem.CurrentReceiveQty,
//                        tempItem.ReceivedUnitType,
//                        tempItem.UnitPrice,
//                        tempItem.ReceivedUnitPrice,
//                        tempItem.TotalPrice,
//                        tempItem.CurrencyId,
//                        tempItem.TotalReceivedQtyPre,
//                        tempItem.PendingReceiveQty,
//                        tempItem.PendingReceiveQtyPre,
//                        tempItem.Remarks,
//                        tempItem.EmployeeId,

//                        // ===== Business calculations =====
//                        TotalCalculatedQty = totalQty,
//                        GarmentQtyCalculated = garmentQty,
//                        UnitPriceOriginal = tempItem.UnitPrice,
//                        UnitPriceReceived = tempItem.ReceivedUnitPrice,
//                        PendingQty = tempItem.PendingReceiveQty ?? 0,
//                        ReceiveQty = tempItem.CurrentReceiveQty ?? 0
//                    });
//                }

//                return resultList;
//            }
//            catch (Exception ex)
//            {
//                Console.Error.WriteLine($"Error copying poly booking data to temp: {ex.Message}");
//                throw;
//            }
//        }


//        public async Task<List<object>> GetButtonBookingDataList(RmgBookingOrder bookingData)
//        {
//            var resultList = new List<object>();
//            var newButtonTempData = new List<RmgInvBookingReceivedDetailsButtonTemp>();

//            try
//            {
//                // 1️⃣ Get main records
//                var mainRecords = buttonRepo.All()
//                    .Where(x => x.PoNo == bookingData.PoNo && x.IntegraJobNo == bookingData.IntegraJobNo)
//                    .ToList();

//                if (!mainRecords.Any())
//                    return resultList;

//                // 2️⃣ Clear temp
//                var existingTemp = buttonTempRepo.All().ToList();
//                if (existingTemp.Any())
//                    await buttonTempRepo.DeleteRangeAsync(existingTemp);

//                // 3️⃣ Copy main → temp (but don’t build resultList yet)
//                foreach (var mainItem in mainRecords)
//                {
//                    var tempItem = new RmgInvBookingReceivedDetailsButtonTemp
//                    {
//                        PurchaseReceiveNo = mainItem.PurchaseReceiveNo,
//                        PoNo = mainItem.PoNo,
//                        IntegraJobNo = mainItem.IntegraJobNo,
//                        SerialNo = mainItem.SerialNo,
//                        Brdid = mainItem.Brdid,
//                        ItemId = mainItem.ItemId,
//                        Description = mainItem.Description,
//                        FabricColorId = mainItem.FabricColorId,
//                        ColorId = mainItem.ColorId,
//                        SizeId = mainItem.SizeId,
//                        Idno = mainItem.Idno,
//                        GermentQty = mainItem.GermentQty,
//                        GermentsQtyUnitId = mainItem.GermentsQtyUnitId,
//                        Consumption = mainItem.Consumption,
//                        ConsumptionUnitId = mainItem.ConsumptionUnitId,
//                        TotalQty = mainItem.TotalQty,
//                        TotalQtyUnitId = mainItem.TotalQtyUnitId,
//                        OrderQty = mainItem.TotalQty.HasValue ? (int)Math.Ceiling((double)mainItem.TotalQty.Value) : 0,
//                        OrderQtyUnitId = mainItem.OrderQtyUnitId,
//                        Percentage = mainItem.Percentage,
//                        TotalReceivedQty = mainItem.TotalReceivedQty,
//                        CurrentReceiveQty = mainItem.CurrentReceiveQty,
//                        ReceivedUnitType = mainItem.ReceivedUnitType,
//                        UnitPrice = mainItem.UnitPrice,
//                        ReceivedUnitPrice = mainItem.ReceivedUnitPrice,
//                        TotalPrice = mainItem.TotalPrice,
//                        CurrencyId = mainItem.CurrencyId,
//                        TotalReceivedQtyPre = mainItem.TotalReceivedQtyPre,
//                        PendingReceiveQty = mainItem.PendingReceiveQty,
//                        PendingReceiveQtyPre = mainItem.PendingReceiveQtyPre,
//                        Remarks = mainItem.Remarks,
//                        EmployeeId = mainItem.EmployeeId,
//                    };

//                    newButtonTempData.Add(tempItem);
//                }

//                // 4️⃣ Save temp table so Ids are generated
//                await buttonTempRepo.AddRangeAsync(newButtonTempData);

//                // 5️⃣ Now build resultList with real Ids
//                foreach (var tempItem in newButtonTempData)
//                {
//                    var orderQty = tempItem.OrderQty;
//                    var consumption = tempItem.Consumption ?? 0;
//                    var totalQty = orderQty * consumption;

//                    resultList.Add(new
//                    {
//                        // ===== DB Fields =====
//                        tempItem.Id,
//                        tempItem.PurchaseReceiveNo,
//                        tempItem.PoNo,
//                        tempItem.IntegraJobNo,
//                        tempItem.SerialNo,
//                        tempItem.Brdid,
//                        tempItem.ItemId,
//                        tempItem.Description,
//                        tempItem.FabricColorId,
//                        tempItem.ColorId,
//                        tempItem.SizeId,
//                        tempItem.Idno,
//                        tempItem.GermentQty,
//                        tempItem.GermentsQtyUnitId,
//                        tempItem.Consumption,
//                        tempItem.ConsumptionUnitId,
//                        tempItem.TotalQty,
//                        tempItem.TotalQtyUnitId,
//                        tempItem.OrderQty,
//                        tempItem.OrderQtyUnitId,
//                        tempItem.Percentage,
//                        tempItem.TotalReceivedQty,
//                        tempItem.CurrentReceiveQty,
//                        tempItem.ReceivedUnitType,
//                        tempItem.UnitPrice,
//                        tempItem.ReceivedUnitPrice,
//                        tempItem.TotalPrice,
//                        tempItem.CurrencyId,
//                        tempItem.TotalReceivedQtyPre,
//                        tempItem.PendingReceiveQty,
//                        tempItem.PendingReceiveQtyPre,
//                        tempItem.Remarks,
//                        tempItem.EmployeeId,

//                        // ===== Business Calculations =====
//                        TotalCalculatedQty = totalQty,
//                        GarmentQty = orderQty,
//                        UnitPriceOriginal = tempItem.UnitPrice,
//                        UnitPriceReceived = tempItem.ReceivedUnitPrice,
//                        PendingQty = tempItem.PendingReceiveQty ?? 0,
//                        ReceiveQty = tempItem.CurrentReceiveQty ?? 0
//                    });
//                }

//                return resultList;
//            }
//            catch (Exception ex)
//            {
//                Console.Error.WriteLine($"Error copying button booking data to temp: {ex.Message}");
//                throw;
//            }
//        }
//        public async Task<List<object>> GetFebricBookingDataList(RmgBookingOrder bookingData)
//        {
//            var resultList = new List<object>();
//            var newFebricTempData = new List<RmgInvBookingReceivedDetailsFebricTemp>();

//            try
//            {
//                // 1️⃣ Get main records
//                var mainRecords = febricRepo.All()
//                    .Where(x => x.PoNo == bookingData.PoNo &&
//                                x.IntegraJobNo == bookingData.IntegraJobNo)
//                    .ToList();

//                if (!mainRecords.Any())
//                    return resultList;

//                // 2️⃣ Clear temp
//                var existingTemp = febricTempRepo.All().ToList();
//                if (existingTemp.Any())
//                    await febricTempRepo.DeleteRangeAsync(existingTemp);

//                // 3️⃣ Copy main → temp (but don’t build resultList yet)
//                foreach (var mainItem in mainRecords)
//                {
//                    var tempItem = new RmgInvBookingReceivedDetailsFebricTemp
//                    {
//                        PurchaseReceiveNo = mainItem.PurchaseReceiveNo,
//                        PoNo = mainItem.PoNo,
//                        IntegraJobNo = mainItem.IntegraJobNo,
//                        Slno = mainItem.Slno,
//                        Brdid = mainItem.Brdid,
//                        ColorId = mainItem.ColorId,
//                        FabricItemId = mainItem.FabricItemId,
//                        ItemId = mainItem.ItemId,
//                        FebricDetails = mainItem.FebricDetails,
//                        Refcode = mainItem.Refcode,
//                        OrderQty = mainItem.OrderQty.HasValue
//                            ? (int)Math.Ceiling((double)mainItem.OrderQty.Value)
//                            : 0,
//                        QtyUnit = mainItem.QtyUnit,
//                        Consumption = mainItem.Consumption,
//                        ConsumtionUnit = mainItem.ConsumtionUnit,
//                        TotalFebricQty = mainItem.TotalFebricQty,
//                        Percentage = mainItem.Percentage,
//                        TotalReceivedQty = mainItem.TotalReceivedQty,
//                        CurrentReceiveQty = mainItem.CurrentReceiveQty,
//                        ReceivedUnitType = mainItem.ReceivedUnitType,
//                        UnitPrice = mainItem.UnitPrice,
//                        ReceivedUnitPrice = mainItem.ReceivedUnitPrice,
//                        TotalPrice = mainItem.TotalPrice,
//                        CurrencyId = mainItem.CurrencyId,
//                        TotalReceivedQtyPre = mainItem.TotalReceivedQtyPre,
//                        PendingReceiveQty = mainItem.PendingReceiveQty,
//                        PendingReceiveQtyPre = mainItem.PendingReceiveQtyPre,
//                        EmployeeId = mainItem.EmployeeId,
//                    };

//                    newFebricTempData.Add(tempItem);
//                }

//                // 4️⃣ Save temp table so Ids are generated
//                await febricTempRepo.AddRangeAsync(newFebricTempData);

//                // 5️⃣ Now build resultList with real Ids
//                foreach (var tempItem in newFebricTempData)
//                {
//                    var consumption = tempItem.Consumption ?? 0;
//                    var garmentQty = tempItem.OrderQty;
//                    var totalQty = garmentQty * consumption;

//                    resultList.Add(new
//                    {
//                        // ===== Original fields =====
//                        tempItem.Id,
//                        tempItem.PurchaseReceiveNo,
//                        tempItem.PoNo,
//                        tempItem.IntegraJobNo,
//                        tempItem.Slno,
//                        tempItem.Brdid,
//                        tempItem.ColorId,
//                        tempItem.FabricItemId,
//                        tempItem.ItemId,
//                        tempItem.FebricDetails,
//                        tempItem.Refcode,
//                        tempItem.QtyUnit,
//                        tempItem.ConsumtionUnit,
//                        tempItem.TotalFebricQty,
//                        tempItem.Percentage,
//                        tempItem.TotalReceivedQty,
//                        tempItem.CurrentReceiveQty,
//                        tempItem.ReceivedUnitType,
//                        tempItem.UnitPrice,
//                        tempItem.ReceivedUnitPrice,
//                        tempItem.TotalPrice,
//                        tempItem.CurrencyId,
//                        tempItem.TotalReceivedQtyPre,
//                        tempItem.PendingReceiveQty,
//                        tempItem.PendingReceiveQtyPre,
//                        tempItem.EmployeeId,

//                        // ===== Business values =====
//                        GarmentQty = garmentQty,
//                        Consumption = consumption,
//                        Description = tempItem.FebricDetails,
//                        TotalQty = totalQty,
//                        OrderQty = (int)Math.Ceiling((decimal)totalQty),
//                        ReceiveQty = tempItem.CurrentReceiveQty ?? 0,
//                        PendingQty = tempItem.PendingReceiveQty ?? 0,
//                        UnitPriceOriginal = tempItem.UnitPrice,
//                        UnitPriceReceived = tempItem.ReceivedUnitPrice
//                    });
//                }

//                return resultList;
//            }
//            catch (Exception ex)
//            {
//                Console.Error.WriteLine($"Error copying fabric booking data to temp: {ex.Message}");
//                throw;
//            }
//        }


//        public async Task<List<object>> GetExtraBookingDataList(RmgBookingOrder bookingData)
//        {
//            var resultList = new List<object>();
//            var newExtraTempData = new List<RmgInvBookingReceivedDetailsExtraTemp>();

//            try
//            {
//                // 1️⃣ Get main records with JOIN
//                var mainRecords =
//                    (from bx in extraRepo.All()
//                     join di in itemRepo.All()
//                        on bx.ItemId equals di.ItemId
//                     join bit in bTypeRepo.All()
//                        on di.ItemTypeId equals bit.BookingItemTypeId
//                     where bx.PoNo == bookingData.PoNo
//                        && bx.IntegraJobNo == bookingData.IntegraJobNo
//                        && bit.BookingItemTypeId == bookingData.BookingType
//                     select new
//                     {
//                         BookingExtra = bx,
//                         Item = di,
//                         BookingItemType = bit
//                     }).ToList();

//                if (!mainRecords.Any())
//                    return resultList;

//                // 2️⃣ Clear temp table
//                var existingTemp = extraTempRepo.All().ToList();
//                if (existingTemp.Any())
//                    await extraTempRepo.DeleteRangeAsync(existingTemp);

//                // 3️⃣ Copy main → temp (but don’t build resultList yet)
//                foreach (var data in mainRecords)
//                {
//                    var bx = data.BookingExtra;

//                    var orderQty = bx.TotalQty.HasValue
//                        ? (int)Math.Ceiling((double)bx.TotalQty.Value)
//                        : 0;

//                    var consumption = bx.Consumption ?? 0;
//                    var totalQty = bx.TotalQty ?? 0;

//                    var tempItem = new RmgInvBookingReceivedDetailsExtraTemp
//                    {
//                        PurchaseReceiveNo = bx.PurchaseReceiveNo,
//                        PoNo = bx.PoNo,
//                        IntegraJobNo = bx.IntegraJobNo,
//                        Slno = bx.Slno,
//                        Brdid = bx.Brdid,
//                        FabricColorId = bx.FabricColorId,
//                        ItemId = bx.ItemId,
//                        Description = bx.Description,
//                        ColorId = bx.ColorId,
//                        OrderQty = orderQty,
//                        OrderQtyIunitD = bx.OrderQtyIunitD,
//                        Consumption = bx.Consumption,
//                        ConsumptionUnitId = bx.ConsumptionUnitId,
//                        TotalQty = bx.TotalQty,
//                        TotalQtyUnitId = bx.TotalQtyUnitId,
//                        ReqQty = bx.ReqQty,
//                        ReqQtyUnitId = bx.ReqQtyUnitId,
//                        Percentage = bx.Percentage,
//                        TotalReceivedQty = bx.TotalReceivedQty,
//                        CurrentReceiveQty = bx.CurrentReceiveQty,
//                        ReceivedUnitType = bx.ReceivedUnitType,
//                        UnitPrice = bx.UnitPrice,
//                        ReceivedUnitPrice = bx.ReceivedUnitPrice,
//                        TotalPrice = bx.TotalPrice,
//                        CurrencyId = bx.CurrencyId,
//                        TotalReceivedQtyPre = bx.TotalReceivedQtyPre,
//                        PendingReceiveQty = bx.PendingReceiveQty,
//                        PendingReceiveQtyPre = bx.PendingReceiveQtyPre,
//                        Remarks = bx.Remarks,
//                        EmployeeId = bx.EmployeeId
//                    };

//                    newExtraTempData.Add(tempItem);
//                }

//                // 4️⃣ Save temp table so Ids are generated
//                await extraTempRepo.AddRangeAsync(newExtraTempData);

//                // 5️⃣ Now build resultList with real Ids
//                foreach (var tempItem in newExtraTempData)
//                {
//                    var consumption = tempItem.Consumption ?? 0;
//                    var totalQty = tempItem.TotalQty ?? 0;

//                    resultList.Add(new
//                    {
//                        // ===== DB Fields =====
//                        tempItem.Id,
//                        tempItem.PurchaseReceiveNo,
//                        tempItem.PoNo,
//                        tempItem.IntegraJobNo,
//                        tempItem.Slno,
//                        tempItem.Brdid,
//                        tempItem.FabricColorId,
//                        tempItem.ItemId,
//                        tempItem.Description,
//                        tempItem.ColorId,
//                        tempItem.OrderQtyIunitD,
//                        tempItem.ConsumptionUnitId,
//                        tempItem.TotalQtyUnitId,
//                        tempItem.ReqQtyUnitId,
//                        tempItem.Percentage,
//                        tempItem.TotalReceivedQty,
//                        tempItem.CurrentReceiveQty,
//                        tempItem.ReceivedUnitType,
//                        tempItem.UnitPrice,
//                        tempItem.ReceivedUnitPrice,
//                        tempItem.TotalPrice,
//                        tempItem.CurrencyId,
//                        tempItem.TotalReceivedQtyPre,
//                        tempItem.PendingReceiveQty,
//                        tempItem.PendingReceiveQtyPre,
//                        tempItem.Remarks,
//                        tempItem.EmployeeId,

//                        // ===== Business / Calculated =====
//                        GarmentQty = tempItem.OrderQty,
//                        Consumption = consumption,
//                        TotalQty = totalQty * consumption,
//                        OrderQty = (int)Math.Ceiling(totalQty * consumption),
//                        ReceiveQty = tempItem.CurrentReceiveQty ?? 0,
//                        PendingQty = tempItem.PendingReceiveQty ?? 0
//                    });
//                }

//                return resultList;
//            }
//            catch (Exception ex)
//            {
//                Console.Error.WriteLine($"Error copying extra booking data to temp: {ex.Message}");
//                throw;
//            }
//        }


//        public async Task<(bool success, string message)> DeleteBookingOrderAsync(List<decimal> deleteBookingIds)
//        {
//            try
//            {
//                var orders = boRepo.All().Where(o => deleteBookingIds.Contains(o.Tc)).ToList();

//                if (!orders.Any())
//                    return (false, DeleteFailed);

//                await boRepo.DeleteRangeAsync(orders);

//                return (true, DeleteSuccess);
//            }
//            catch (Exception ex)
//            {
//                return (false, $"Error occurred: {ex.Message}");
//            }
//        }

//    }
//}

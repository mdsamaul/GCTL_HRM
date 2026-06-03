//using GCTL.Core.Data;
//using GCTL.Core.Helpers;
//using GCTL.Core.ViewModels.RMGBookingOrderEntryBukl;
//using GCTL.Data.Models;
//using GCTL.Service.Common;
//using GCTL.Service.RMGBookingOrderEntryBukl;
//using GCTL.UI.Core.ViewModels.RMGBookingOrderEntryBukl;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.Data.SqlClient;
//using Microsoft.EntityFrameworkCore;

//namespace GCTL.UI.Core.Controllers
//{
//    public class RMGBookingOrderEntryBuklController : BaseController
//    {
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
//        private readonly IRepository<RmgInvBookingReceivedDetailsButtonTemp> buttonTempRepo;
//        private readonly IRepository<RmgInvBookingReceivedDetailsExtraTemp> extraTempRepo;
//        private readonly IRepository<RmgInvBookingReceivedDetailsFebricTemp> febricTempRepo;
//        private readonly IRepository<RmgInvBookingReceivedDetailsCartonTemp> cartonTempRepo;
//        private readonly IRepository<RmgInvBookingReceivedDetailsPolyTemp> polyTempRepo;
//        private readonly IRepository<RmgInvBookingReceivedDetailsThreadTemp> threadTempRepo;
//        private readonly IRepository<RmgInvBookingReceivedDetailsPoly> polyRepo;
//        private readonly IRepository<RmgInvBookingReceivedDetailsThread> threadRepo;
//        private readonly IRepository<InvDefBookingItemType> bTypeRepo;
//        private readonly IRepository<RmgProdDefBuyer> buyerRepo;
//        private readonly IRepository<RmgDefSupplier> supplierRepo;
//        private readonly IRepository<CaDefCountry> countryRepo;
//        private readonly IRepository<HrmEmployee2> empRepo;
//        private readonly IRepository<HrmEmployeeOfficialInfo> empOffiRepo;
//        private readonly IRepository<RmgProdDefDeliveryMethod> deliveryRepo;
//        private readonly IRepository<SalesDefPaymentTerms> paymentTermRepo;
//        private readonly IRepository<HrmDefDesignation> degRepo;
//        private readonly IRepository<RmgTermsCondition> termConditionRepo;
//        private readonly IRepository<CoreCompany> comRepo;
//        private readonly ICommonService commonService;
//        private readonly IRMGBookingOrderEntryBuklService rmgBookingOrderEntryBuklService;
//        private readonly string _connectionString;




//        public RMGBookingOrderEntryBuklController(
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
//            IRepository<RmgTermsCondition> termConditionRepo,
//            IRepository<CoreCompany> comRepo,
//            IConfiguration configuration,
//            ICommonService commonService,
//            IRMGBookingOrderEntryBuklService rmgBookingOrderEntryBuklService
//            )
//        {
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
//            this.termConditionRepo = termConditionRepo;
//            this.comRepo = comRepo;
//            this.commonService = commonService;
//            this.rmgBookingOrderEntryBuklService = rmgBookingOrderEntryBuklService;
//            _connectionString = configuration.GetConnectionString("ApplicationDbConnection");
//        }
//        public async Task<IActionResult> Index()
//        {
//            var hasPermission = await rmgBookingOrderEntryBuklService.PagePermissionAsync(LoginInfo.AccessCode);

//            if (!hasPermission)

//            {

//                return RedirectToAction("Login", "Accounts");

//            }
//            ViewBag.BookingTypeList = new SelectList(bTypeRepo.All().Select(x => new { id = x.BookingItemTypeId, name = x.BookingItemType }), "id", "name");
//            ViewBag.SupplierList = new SelectList(supplierRepo.All().Select(x => new { id = x.SupplierId, name = x.SupplierName }), "id", "name");
//            ViewBag.CountryList = new SelectList(countryRepo.All().Select(x => new { id = x.CountryId, name = x.CountryName }), "id", "name");
//            ViewBag.CurrencyList = new SelectList(currenciesRepo.All().Select(x => new { id = x.CurrencyId, name = x.ShortName }), "id", "name");
//            ViewBag.pTermList = new SelectList(paymentTermRepo.All().Select(x => new { id = x.PaymentTermsId, name = x.PaymentTermsName }), "id", "name");
//            ViewBag.termConditionList = new SelectList(termConditionRepo.All().Select(x => new { id = x.TermsConditionId, name = x.TermsConditionName }), "id", "name");
//            ViewBag.deliveryList = new SelectList(deliveryRepo.All().Select(x => new { id = x.DeliveryMethodId, name = x.DeliveryMethod }), "id", "name");
//            ViewBag.ComAddress = comRepo.All().Select(x => x.Address1).FirstOrDefault();
//            var empList = new List<EmployeeDto>();

//            string query = @"
//                SELECT 
//                    emp.EmployeeID,
//                    emp.FirstName + ' ' + emp.LastName AS FullName
//                FROM HRM_EmployeeOfficialInfo empOff
//                LEFT JOIN HRM_Employee emp 
//                       ON emp.EmployeeID = empOff.EmployeeID
//                LEFT JOIN HRM_Def_Designation des 
//                       ON des.DesignationCode = empOff.DesignationCode
//                WHERE des.DesignationCode = @DesignationCode";

//            using (var con = new SqlConnection(_connectionString))
//            using (var cmd = new SqlCommand(query, con))
//            {
//                cmd.Parameters.AddWithValue("@DesignationCode", "028");

//                await con.OpenAsync();
//                using (var rdr = await cmd.ExecuteReaderAsync())
//                {
//                    while (await rdr.ReadAsync())
//                    {
//                        empList.Add(new EmployeeDto
//                        {
//                            EmployeeID = rdr["EmployeeID"].ToString(),
//                            FullName = rdr["FullName"].ToString()
//                        });
//                    }
//                }
//            }

//            ViewBag.EmpList = new SelectList(empList, "EmployeeID", "FullName");




//            RMGBookingOrderEntryBuklViewModel model = new RMGBookingOrderEntryBuklViewModel()
//            {
//                PageUrl = Url.Action(nameof(Index)),
//                bookingReceivedDetailsThreadSetup = new BookingReceivedDetailsThreadDto()
//            };
//            return View(model);
//        }

//        [HttpGet]
//        public async Task<IActionResult> BookingOrderAutoId()
//        {
//            try
//            {
//                var y = DateTime.Now.Year;
//                var bookingOrderId = commonService.GenerateNextCode("BookinOrderNO", "RMG_BookingOrder", 3, "FAWI-" + y + "-");
//                return Json(new { data = bookingOrderId });
//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }


//        [HttpGet]
//        public async Task<IActionResult> GetSupplierDetails(string supplierId)
//        {
//            if (string.IsNullOrEmpty(supplierId))
//            {
//                return BadRequest(new { success = false, message = "SupplierId is required" });
//            }

//            // Example: fetch supplier details from repository or database
//            var supplier = await supplierRepo.GetByIdAsync(supplierId);

//            if (supplier == null)
//            {
//                return NotFound(new { success = false, message = "Supplier not found" });
//            }

//            // Return JSON data
//            return Json(new
//            {
//                success = true,
//                id = supplier.SupplierId,
//                name = supplier.SupplierName,
//                address = supplier.Address,
//                countryId = supplier.CountryId,
//            });
//        }


//        [HttpPost]
//        public async Task<IActionResult> LoadBookingTable([FromBody] ItemTypeFilterDto dto)
//        {
//            try
//            {
//                if (dto.CostingId[0] == "edit")
//                {
//                    return Json(new { success = true, MessagePack = "" });
//                }
//                if (dto.CostingId == null && !dto.CostingId.Any())
//                {
//                    return Json(new { success = false, message = "No costing selected" });
//                }

//                var dropdownData = await GetDropdownData();
//                List<object> data;

//                switch (dto.BookingType)
//                {
//                    case "04":
//                        data = await GetCartonBookingData(dto);
//                        break;
//                    case "07":
//                        data = await GetThreadBookingData(dto);
//                        break;
//                    case "03":
//                        data = await GetPolyBookingData(dto);
//                        break;
//                    case "02":
//                        data = await GetButtonBookingData(dto);
//                        break;
//                    case "01":
//                        data = (await GetFebricBookingData(dto)).Cast<object>().ToList();
//                        break;
//                    default:
//                        data = (await GetExtraBookingData(dto)).Cast<object>().ToList();
//                        break;
//                }

//                return Json(new { success = true, data, dropdownData });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = ex.Message });
//            }
//        }




//        private async Task<object> GetDropdownData()
//        {
//            // Fetch dropdown data from your database
//            var items = await itemRepo.All()
//                .Select(x => new { id = x.ItemId, name = x.ItemName })
//                .ToListAsync();

//            var colors = await colorRepo.All()
//                .Select(x => new { id = x.ColorId, name = x.Color })
//                .ToListAsync();

//            var units = await unitRepo.All()
//                .Select(x => new { id = x.UnitTypId, name = x.UnitTypeName })
//                .ToListAsync();

//            var sizes = await sizeRepo.All()
//                .Select(x => new { id = x.SizeId, name = x.Size })
//                .ToListAsync();

//            var threadCounts = await threadCountRepo.All()
//                .Select(x => new { id = x.ThreadCountId, name = x.ThreadCountName })
//                .ToListAsync();

//            var currencies = await currenciesRepo.All()
//                .Select(x => new { id = x.CurrencyId, name = x.ShortName })
//                .ToListAsync();

//            return new
//            {
//                items = items,
//                colors = colors,
//                units = units,
//                sizes = sizes,
//                threadCounts = threadCounts,
//                currencies = currencies
//            };
//        }

//        // ==================== Carton Methods ====================


//        private async Task<List<object>> GetCartonBookingData(ItemTypeFilterDto dto)
//        {
//            await cartonTempRepo.DeleteRangeAsync(cartonTempRepo.All().ToList());

//            var costingIds = dto.CostingId;

//            string inClause = string.Join(",", costingIds.Select((x, i) => $"@cid{i}"));

//            string query = $@"
//    SELECT
//           cd.Id, cd.CostingDetailsID, cd.CostingID AS DetailCostingID, cd.SLNO,
//            cd.BookingItemTypeID, cd.ItemID, cd.Description, cd.Width, cd.ColorID,
//            cd.SupplierID, cd.PoNo AS DetailPoNo, cd.Quantity, cd.Consumption, cd.Extra,
//            cd.TotalQuantity, cd.TotalQuantityUnit, cd.UnitPrice, cd.TotalPrice,
//            cd.TotalPriceCurrencyId,

//            ci.IntegraJobNO ,
//            ci.StyleID ,
//            ci.PoNo ,
//            ci.MasterPurchaseOrder 
//    FROM RMG_CostingInfo ci
//    JOIN RMG_CostingDetails cd ON ci.CostingID = cd.CostingID
//    JOIN Inv_Def_Item di ON di.ItemID = cd.ItemID
//    WHERE ci.CostingID IN ({inClause})
//      AND di.ItemTypeID = @BookingType";

//            using var con = new SqlConnection(_connectionString);
//            using var cmd = new SqlCommand(query, con);

//            for (int i = 0; i < costingIds.Count; i++)
//                cmd.Parameters.AddWithValue($"@cid{i}", costingIds[i]);

//            cmd.Parameters.AddWithValue("@BookingType", dto.BookingType);

//            await con.OpenAsync();
//            using var rdr = await cmd.ExecuteReaderAsync();

//            while (await rdr.ReadAsync())
//            {
//                // Helper function for safe conversion
//                decimal SafeToDecimal(object obj) => obj == DBNull.Value ? 0M : Convert.ToDecimal(obj);
//                int SafeToInt(object obj) => obj == DBNull.Value ? 0 : Convert.ToInt32(obj);
//                string SafeToString(object obj) => obj == DBNull.Value ? "" : obj.ToString();

//                await cartonTempRepo.AddAsync(new RmgInvBookingReceivedDetailsCartonTemp
//                {
//                    PurchaseReceiveNo = await GenerateAutoCartonBooking(),

//                    // --- Properties from Reader ---
//                    ItemId = SafeToString(rdr["ItemID"]),
//                    ItemDescription = SafeToString(rdr["Description"]),
//                    OrderQty = SafeToDecimal(rdr["Quantity"]),
//                    OrderUnitId = SafeToString(rdr["TotalQuantityUnit"]),
//                    RequiredQty = SafeToDecimal(rdr["TotalQuantity"]),
//                    RequiredQtyUnitId = SafeToString(rdr["TotalQuantityUnit"]),
//                    ConsumptionUnitId = SafeToString(rdr["TotalQuantityUnit"]),
//                    Consumption = SafeToDecimal(rdr["Consumption"]),
//                    UnitPrice = SafeToDecimal(rdr["UnitPrice"]),
//                    TotalPrice = SafeToDecimal(rdr["TotalPrice"]),
//                    PoNo = SafeToString(rdr["PoNo"]),
//                    IntegraJobNo = SafeToString(rdr["IntegraJobNO"]),
//                    Slno = SafeToInt(rdr["SLNO"]),

//                    // --- Properties explicitly set to "" or default 0 ---
//                    ColorId = "",
//                    SizeId = "",
//                    Refcode = "",
//                    CartonLeangth = "",
//                    LeangthUnitId = "",
//                    CartonWidth = "",
//                    WidthUnitId = "",
//                    CatonHeight = "",
//                    HeightUnitId = "",
//                    //ConsumptionUnitId = "",
//                    CartonPercent = "",

//                    // Set other nullable properties to 0 (since they are not read from the query)
//                    TotalReceivedQty = 0M,
//                    CurrentReceiveQty = 0M,
//                    ReceivedUnitPrice = 0M,
//                    TotalReceivedQtyPre = 0M,
//                    PendingReceiveQty = 0M,
//                    PendingReceiveQtyPre = 0M,

//                    // Set other string properties to "" (since they are not read from the query)
//                    Brdid = "",
//                    ReceivedUnitType = "",
//                    CurrencyId = "",
//                    Remarks = "",
//                    EmployeeId = "",
//                });
//            }

//            return cartonTempRepo.All()
//                .Select(x => new
//                {
//                    id = x.Id,
//                    poNo = x.PoNo,
//                    itemID = x.ItemId,
//                    description = x.ItemDescription,
//                    colorID = x.ColorId,
//                    sizeID = x.SizeId,
//                    cartonLength = x.CartonLeangth,
//                    leangthUnitID = x.LeangthUnitId,
//                    cartonWidth = x.CartonWidth,
//                    widthUnitID = x.WidthUnitId,
//                    catonHeight = x.CatonHeight,
//                    heightUnitID = x.HeightUnitId,
//                    garmentQty = x.OrderQty,
//                    orderQty = (int)Math.Ceiling((decimal)(x.OrderQty ?? 1) * (x.Consumption ?? 1)),
//                    garmentQtyUnitID = x.OrderUnitId,
//                    consumption = x.Consumption,
//                    consumptionUnitID = x.ConsumptionUnitId,
//                    totalQty = x.OrderQty * x.Consumption,
//                    totalQtyUnitID = x.RequiredQtyUnitId,
//                    percentage = x.CartonPercent,
//                    unitPrice = x.UnitPrice,
//                    totalPrice = (int)Math.Ceiling((decimal)(x.OrderQty ?? 1) * (x.Consumption ?? 1)) * (x.UnitPrice ?? 1),
//                    currencyID = x.CurrencyId,
//                    remarks = x.Remarks
//                })
//                .Cast<object>()
//                .ToList();
//        }

//        public async Task<string> GenerateAutoCartonBooking()
//        {
//            var getYear = DateTime.Now.Year.ToString();
//            var prefix = "PRN-" + getYear + "-";
//            //PRN - 001
//            string lastCode = null;

//            using (SqlConnection con = new SqlConnection(_connectionString))
//            {
//                await con.OpenAsync();

//                // First check Temp table
//                string queryTemp = @"SELECT MAX(PurchaseReceiveNo) 
//                             FROM RMG_Inv_BookingReceivedDetails_CartonTemp 
//                             WHERE PurchaseReceiveNo LIKE @prefix + '%'";
//                using (SqlCommand cmd = new SqlCommand(queryTemp, con))
//                {
//                    cmd.Parameters.AddWithValue("@prefix", prefix);
//                    var result = await cmd.ExecuteScalarAsync();
//                    lastCode = result?.ToString();
//                }

//                // If nothing found in Temp, check main table
//                if (string.IsNullOrEmpty(lastCode))
//                {
//                    string queryMain = @"SELECT MAX(PurchaseReceiveNo) 
//                                 FROM RMG_Inv_BookingReceivedDetails_Carton 
//                                 WHERE PurchaseReceiveNo LIKE @prefix + '%'";
//                    using (SqlCommand cmd = new SqlCommand(queryMain, con))
//                    {
//                        cmd.Parameters.AddWithValue("@prefix", prefix);
//                        var result = await cmd.ExecuteScalarAsync();
//                        lastCode = result?.ToString();
//                    }
//                }
//            }

//            int nextNumber = 1;
//            if (!string.IsNullOrEmpty(lastCode))
//            {
//                // Extract numeric part after the prefix
//                string numberPart = lastCode.Substring(prefix.Length);
//                if (int.TryParse(numberPart, out int currentNumber))
//                {
//                    nextNumber = currentNumber + 1;
//                }
//            }

//            // Pad with zeros to length 6
//            string nextCode = prefix + nextNumber.ToString().PadLeft(6, '0');
//            return nextCode;
//        }

//        // ==================== Thread Methods ====================

//        private async Task<List<object>> GetThreadBookingData(ItemTypeFilterDto dto)
//        {
//            // ============================
//            // CLEAR OLD TEMP DATA
//            // ============================
//            var exTempThread = threadTempRepo.All().ToList();
//            if (exTempThread.Any())
//            {
//                await threadTempRepo.DeleteRangeAsync(exTempThread);
//            }

//            // ============================
//            // BUILD IN CLAUSE FOR CostingId LIST
//            // ============================
//            var costingIds = dto.CostingId;
//            if (costingIds == null || !costingIds.Any())
//                return new List<object>();

//            // Using Dapper's style parameterization is safer, but sticking to your current style:
//            string inClause = string.Join(",", costingIds.Select((x, i) => $"@cid{i}"));

//            // ============================
//            // SQL QUERY (Thread) - NO CHANGE NEEDED HERE
//            // ============================
//            string query = $@"
//SELECT 
//    cd.Id, cd.CostingDetailsID, cd.CostingID AS DetailCostingID, cd.SLNO,
//    cd.BookingItemTypeID, cd.ItemID, cd.Description, cd.Width, cd.ColorID,
//    cd.SupplierID, cd.PoNo AS DetailPoNo, cd.Quantity, cd.Consumption, cd.Extra,
//    cd.TotalQuantity, cd.TotalQuantityUnit, cd.UnitPrice, cd.TotalPrice,
//    cd.TotalPriceCurrencyId,

//    ci.IntegraJobNO AS CiIntegraJob,
//    ci.StyleID AS CiStyleID,
//    ci.PoNo AS CiPoNo,
//    ci.MasterPurchaseOrder AS CiMasterPo
//FROM RMG_CostingInfo ci
//LEFT JOIN RMG_CostingDetails cd ON ci.CostingID = cd.CostingID
//LEFT JOIN Inv_Def_Item di ON di.ItemID = cd.ItemID
//LEFT JOIN Inv_Def_BookingItemType dit ON dit.BookingItemTypeID = di.ItemTypeID
//WHERE ci.CostingID IN ({inClause})
//    AND di.ItemTypeID = @BookingType";

//            using var con = new SqlConnection(_connectionString);
//            using var cmd = new SqlCommand(query, con);

//            // ============================
//            // ADD PARAMETERS
//            // ============================
//            for (int i = 0; i < costingIds.Count; i++)
//            {
//                cmd.Parameters.AddWithValue($"@cid{i}", costingIds[i]);
//            }

//            cmd.Parameters.AddWithValue("@BookingType", dto.BookingType);

//            await con.OpenAsync();
//            using var rdr = await cmd.ExecuteReaderAsync();

//            // ============================
//            // INSERT INTO THREAD TEMP TABLE (Updated Conversion Logic)
//            // ============================
//            while (await rdr.ReadAsync())
//            {
//                var temp = new RmgInvBookingReceivedDetailsThreadTemp
//                {
//                    PurchaseReceiveNo = await GenerateAutoThreadBooking(),

//                    // --- Read from SQL Query ---
//                    // Strings (Safe using Helper function or direct ToString())
//                    ItemId = SafeToString(rdr["ItemID"]),
//                    ColorId = SafeToString(rdr["ColorID"]),
//                    TotalQtyUnitId = SafeToString(rdr["TotalQuantityUnit"]),
//                    CurrencyId = SafeToString(rdr["TotalPriceCurrencyId"]),
//                    IntegraJobNo = SafeToString(rdr["CiIntegraJob"]),
//                    PoNo = SafeToString(rdr["CiPoNo"]),

//                    // Integers (Safe Conversion)
//                    Slno = SafeToInt(rdr["SLNO"]),

//                    // Decimals (Safe Conversion)
//                    OrderQty = SafeToDecimal(rdr["Quantity"]),
//                    Consumption = SafeToDecimal(rdr["Consumption"]),
//                    TotalQty = SafeToDecimal(rdr["TotalQuantity"]),
//                    UnitPrice = SafeToDecimal(rdr["UnitPrice"]),
//                    TotalPrice = SafeToDecimal(rdr["TotalPrice"]),

//                    // --- Default/Empty Properties (Not in current SQL query) ---
//                    Brdid = "",
//                    FebricDetail = "",
//                    ThreadColorId = "",
//                    QtyUnitId = SafeToString(rdr["TotalQuantityUnit"]),
//                    ConsumtionUnitId = SafeToString(rdr["TotalQuantityUnit"]),
//                    ThreadCountId = "",
//                    Refcodepantone = "",
//                    ReqQty = 0M,
//                    ThreadReqUnit = "",
//                    Threadpercent = "",

//                    // --- Default Numeric Properties ---
//                    TotalReceivedQty = 0M,
//                    CurrentReceiveQty = 0M,
//                    ReceivedUnitPrice = 0M,
//                    TotalReceivedQtyPre = 0M,
//                    PendingReceiveQty = 0M,
//                    PendingReceiveQtyPre = 0M,

//                    // --- Default String Properties ---
//                    ReceivedUnitType = "",
//                    Remarks = "",
//                    EmployeeId = "",
//                };

//                await threadTempRepo.AddAsync(temp);
//            }

//            var data = await threadTempRepo.All()
//                .Select(x => new
//                {
//                    id = x.Id,
//                    purchaseReceiveNo = x.PurchaseReceiveNo,
//                    itemID = x.ItemId,
//                    description = x.FebricDetail,
//                    colorID = x.ColorId,
//                    slno = x.Slno,
//                    orderQty = (int)Math.Ceiling((decimal)x.TotalQty),
//                    qtyUnitId = x.QtyUnitId,
//                    consumption = x.Consumption,
//                    consumtionUnitId = x.ConsumtionUnitId,
//                    totalQty = x.TotalQty,
//                    totalQtyUnitId = x.TotalQtyUnitId,
//                    totalReceivedQty = x.TotalReceivedQty,
//                    currentReceiveQty = x.CurrentReceiveQty,
//                    pendingReceiveQty = x.PendingReceiveQty,
//                    garmentQty = x.OrderQty,
//                    unitPrice = x.UnitPrice,
//                    receivedUnitPrice = x.ReceivedUnitPrice,
//                    totalPrice = x.TotalPrice,
//                    currencyId = x.CurrencyId,
//                    remarks = x.Remarks,
//                    employeeId = x.EmployeeId,
//                    integraJobNo = x.IntegraJobNo,
//                    poNo = x.PoNo
//                })
//                .ToListAsync();

//            return data.Cast<object>().ToList();
//        }


//        private static string SafeToString(object value)
//        {
//            return value == null || value == DBNull.Value ? "" : value.ToString();
//        }

//        private static decimal? SafeToDecimal(object value)
//        {
//            if (value == null || value == DBNull.Value)
//                return null;

//            if (decimal.TryParse(value.ToString(), out decimal result))
//                return result;

//            try
//            {
//                return Convert.ToDecimal(value);
//            }
//            catch
//            {
//                return null;
//            }
//        }

//        private static int? SafeToInt(object value)
//        {
//            if (value == null || value == DBNull.Value)
//                return null;

//            if (int.TryParse(value.ToString(), out int result))
//                return result;

//            try
//            {
//                return Convert.ToInt32(value);
//            }
//            catch
//            {
//                return null;
//            }
//        }



//        public async Task<string> GenerateAutoThreadBooking()
//        {
//            var getYear = DateTime.Now.Year.ToString();
//            var prefix = "POR_" + getYear + "_";

//            string lastCode = null;

//            using (SqlConnection con = new SqlConnection(_connectionString))
//            {
//                await con.OpenAsync();

//                // First check Temp table
//                string queryTemp = @"SELECT MAX(PurchaseReceiveNo) 
//                             FROM RMG_Inv_BookingReceivedDetails_ThreadTemp 
//                             WHERE PurchaseReceiveNo LIKE @prefix + '%'";
//                using (SqlCommand cmd = new SqlCommand(queryTemp, con))
//                {
//                    cmd.Parameters.AddWithValue("@prefix", prefix);
//                    var result = await cmd.ExecuteScalarAsync();
//                    lastCode = result?.ToString();
//                }

//                // If nothing found in Temp, check main table
//                if (string.IsNullOrEmpty(lastCode))
//                {
//                    string queryMain = @"SELECT MAX(PurchaseReceiveNo) 
//                                 FROM RMG_Inv_BookingReceivedDetails_Thread 
//                                 WHERE PurchaseReceiveNo LIKE @prefix + '%'";
//                    using (SqlCommand cmd = new SqlCommand(queryMain, con))
//                    {
//                        cmd.Parameters.AddWithValue("@prefix", prefix);
//                        var result = await cmd.ExecuteScalarAsync();
//                        lastCode = result?.ToString();
//                    }
//                }
//            }

//            int nextNumber = 1;
//            if (!string.IsNullOrEmpty(lastCode))
//            {
//                string numberPart = lastCode.Substring(prefix.Length);
//                if (int.TryParse(numberPart, out int currentNumber))
//                {
//                    nextNumber = currentNumber + 1;
//                }
//            }

//            // Pad with zeros to length 6
//            string nextCode = prefix + nextNumber.ToString().PadLeft(6, '0');
//            return nextCode;
//        }

//        // ==================== Poly Methods ====================

//        private async Task<List<object>> GetPolyBookingData(ItemTypeFilterDto dto)
//        {

//            var exTempPoly = polyTempRepo.All().ToList();
//            if (exTempPoly.Any())
//            {
//                await polyTempRepo.DeleteRangeAsync(exTempPoly);
//            }

//            var costingIds = dto.CostingId;
//            if (costingIds == null || !costingIds.Any())
//                return new List<object>();

//            string inClause = string.Join(",", costingIds.Select((x, i) => $"@cid{i}"));

//            // ============================
//            // SQL QUERY (POLY)
//            // ============================

//            string query = $@"
//SELECT 
//    cd.Id, cd.CostingDetailsID, cd.CostingID AS DetailCostingID, cd.SLNO,
//    cd.BookingItemTypeID, cd.ItemID, cd.Description, cd.Width, cd.ColorID,
//    cd.SupplierID, cd.PoNo AS DetailPoNo, cd.Quantity, cd.Consumption, cd.Extra,
//    cd.TotalQuantity, cd.TotalQuantityUnit, cd.UnitPrice, cd.TotalPrice,
//    cd.TotalPriceCurrencyId,

//    ci.IntegraJobNO AS CiIntegraJob,
//    ci.StyleID AS CiStyleID,
//    ci.PoNo AS CiPoNo,
//    ci.MasterPurchaseOrder AS CiMasterPo
//FROM RMG_CostingInfo ci
//LEFT JOIN RMG_CostingDetails cd ON ci.CostingID = cd.CostingID
//LEFT JOIN Inv_Def_Item di ON di.ItemID = cd.ItemID
//LEFT JOIN Inv_Def_BookingItemType dit ON dit.BookingItemTypeID = di.ItemTypeID
//WHERE ci.CostingID IN ({inClause})
//    AND di.ItemTypeID = @BookingType";

//            using var con = new SqlConnection(_connectionString);
//            using var cmd = new SqlCommand(query, con);

//            // ============================
//            // ADD PARAMETERS
//            // ============================
//            for (int i = 0; i < costingIds.Count; i++)
//            {
//                cmd.Parameters.AddWithValue($"@cid{i}", costingIds[i]);
//            }

//            cmd.Parameters.AddWithValue("@BookingType", dto.BookingType);

//            await con.OpenAsync();
//            using var rdr = await cmd.ExecuteReaderAsync();

//            while (await rdr.ReadAsync())
//            {
//                var temp = new RmgInvBookingReceivedDetailsPolyTemp
//                {
//                    PurchaseReceiveNo = await GenerateAutoPolyBooking(),

//                    ItemId = SafeToString(rdr["ItemID"]),
//                    ItemDescription = SafeToString(rdr["Description"]),
//                    ColorId = SafeToString(rdr["ColorID"]),
//                    Width = SafeToString(rdr["Width"]),

//                    GarmentQty = SafeToDecimal(rdr["Quantity"]),
//                    Consumption = SafeToDecimal(rdr["Consumption"]),
//                    TotalQty = SafeToDecimal(rdr["TotalQuantity"]),
//                    TotalQtyUnitId = SafeToString(rdr["TotalQuantityUnit"]),

//                    UnitPrice = SafeToDecimal(rdr["UnitPrice"]),
//                    TotalPrice = SafeToDecimal(rdr["TotalPrice"]),
//                    CurrencyId = SafeToString(rdr["TotalPriceCurrencyId"]),

//                    IntegraJobNo = SafeToString(rdr["CiIntegraJob"]),
//                    PoNo = SafeToString(rdr["CiPoNo"]),

//                    SerialNo = SafeToInt(rdr["SLNO"]),

//                    Brdid = "",
//                    RefernceCode = "",
//                    Length = "",
//                    LengthUnitId = "",
//                    WidthUnitId = "",
//                    Flap = "",
//                    FlapUnitId = "",
//                    Guest = "",
//                    GuestUnitId = "",
//                    GarmentQtyUnitId = SafeToString(rdr["TotalQuantityUnit"]),
//                    ConsumptionUnitId = SafeToString(rdr["TotalQuantityUnit"]),
//                    Percentage = "",

//                    TotalReceivedQty = 0M,
//                    CurrentReceiveQty = 0M,
//                    ReceivedUnitType = "",
//                    ReceivedUnitPrice = 0M,
//                    TotalReceivedQtyPre = 0M,
//                    PendingReceiveQty = 0M,
//                    PendingReceiveQtyPre = 0M,

//                    Remarks = "",
//                    EmployeeId = "",
//                };

//                await polyTempRepo.AddAsync(temp);
//            }

//            var data = await polyTempRepo.All()
//                .Select(x => new
//                {
//                    id = x.Id,
//                    purchaseReceiveNo = x.PurchaseReceiveNo,
//                    brdId = x.Brdid,
//                    serialNo = x.SerialNo,

//                    itemID = x.ItemId,
//                    description = x.ItemDescription,
//                    colorID = x.ColorId,

//                    referenceCode = x.RefernceCode,

//                    length = x.Length,
//                    lengthUnitID = x.LengthUnitId,

//                    width = x.Width,
//                    widthUnitID = x.WidthUnitId,

//                    flap = x.Flap,
//                    flapUnitID = x.FlapUnitId,

//                    guest = x.Guest,
//                    guestUnitID = x.GuestUnitId,

//                    garmentQty = x.GarmentQty,
//                    garmentQtyUnitID = x.GarmentQtyUnitId,
//                    OrderQty = (int)Math.Ceiling((decimal)x.TotalQty),

//                    consumption = x.Consumption,
//                    consumptionUnitID = x.ConsumptionUnitId,

//                    totalQty = x.TotalQty,
//                    totalQtyUnitID = x.TotalQtyUnitId,

//                    percentage = x.Percentage,

//                    totalReceivedQty = x.TotalReceivedQty,
//                    currentReceiveQty = x.CurrentReceiveQty,
//                    receivedUnitType = x.ReceivedUnitType,

//                    unitPrice = x.UnitPrice,
//                    receivedUnitPrice = x.ReceivedUnitPrice,
//                    totalPrice = x.TotalPrice,
//                    currencyID = x.CurrencyId,

//                    remarks = x.Remarks,
//                    employeeId = x.EmployeeId,

//                    totalReceivedQtyPre = x.TotalReceivedQtyPre,
//                    pendingReceiveQty = x.PendingReceiveQty,
//                    pendingReceiveQtyPre = x.PendingReceiveQtyPre,

//                    integraJobNo = x.IntegraJobNo,
//                    poNo = x.PoNo
//                })
//                .ToListAsync();

//            return data.Cast<object>().ToList();
//        }



//        public async Task<string> GenerateAutoPolyBooking()
//        {
//            var getYear = DateTime.Now.Year.ToString();
//            var prefix = "POR_" + getYear + "_";

//            string lastCode = null;

//            using (SqlConnection con = new SqlConnection(_connectionString))
//            {
//                await con.OpenAsync();

//                string queryTemp = @"SELECT MAX(PurchaseReceiveNo) 
//                             FROM RMG_Inv_BookingReceivedDetails_PolyTemp 
//                             WHERE PurchaseReceiveNo LIKE @prefix + '%'";
//                using (SqlCommand cmd = new SqlCommand(queryTemp, con))
//                {
//                    cmd.Parameters.AddWithValue("@prefix", prefix);
//                    var result = await cmd.ExecuteScalarAsync();
//                    lastCode = result?.ToString();
//                }
//                if (string.IsNullOrEmpty(lastCode))
//                {
//                    string queryMain = @"SELECT MAX(PurchaseReceiveNo) 
//                                 FROM RMG_Inv_BookingReceivedDetails_Poly 
//                                 WHERE PurchaseReceiveNo LIKE @prefix + '%'";
//                    using (SqlCommand cmd = new SqlCommand(queryMain, con))
//                    {
//                        cmd.Parameters.AddWithValue("@prefix", prefix);
//                        var result = await cmd.ExecuteScalarAsync();
//                        lastCode = result?.ToString();
//                    }
//                }
//            }

//            int nextNumber = 1;
//            if (!string.IsNullOrEmpty(lastCode))
//            {
//                string numberPart = lastCode.Substring(prefix.Length);
//                if (int.TryParse(numberPart, out int currentNumber))
//                {
//                    nextNumber = currentNumber + 1;
//                }
//            }
//            string nextCode = prefix + nextNumber.ToString().PadLeft(6, '0');
//            return nextCode;
//        }

//        // ==================== Button Methods ====================


//        private async Task<List<object>> GetButtonBookingData(ItemTypeFilterDto dto)
//        {
//            try
//            {
//                var exTempButton = buttonTempRepo.All().ToList();
//                if (exTempButton.Any())
//                {
//                    await buttonTempRepo.DeleteRangeAsync(exTempButton);
//                }

//                var costingIds = dto.CostingId;
//                if (costingIds == null || !costingIds.Any())
//                    return new List<object>();

//                string inClause = string.Join(",", costingIds.Select((x, i) => $"@cid{i}"));

//                string query = $@"
//SELECT 
//    cd.Id,
//    cd.SLNO,
//    cd.ItemID,
//    cd.Description,
//    cd.ColorID,    
//    cd.Quantity AS GermentQty,
//    cd.TotalQuantity AS TotalQty,
//    cd.TotalQuantityUnit AS TotalQtyUnitId,
//    cd.Consumption,
//    cd.TotalQuantityUnit AS ConsumptionUnitId, -- ⚠️ ConsumptionUnitId-এর জন্য TotalQuantityUnit ধরে নিলাম
//    cd.Extra AS Percentage,
//    cd.Quantity AS CurrentReceiveQty,
//    cd.UnitPrice,
//    cd.UnitPrice AS ReceivedUnitPrice,
//    cd.TotalPrice,
//    cd.TotalPriceCurrencyId AS CurrencyId,
//    cd.SupplierID AS EmployeeId,
//    cd.Quantity AS TotalReceivedQtyPre,
//    0 AS PendingReceiveQty,
//    0 AS PendingReceiveQtyPre,
//    ci.IntegraJobNO AS IntegraJobNo,
//    ci.PoNo AS PoNo,
//    ci.StyleID AS FabricColorId
//FROM RMG_CostingInfo ci
//LEFT JOIN RMG_CostingDetails cd ON ci.CostingID = cd.CostingID
//LEFT JOIN Inv_Def_Item di ON di.ItemID = cd.ItemID
//LEFT JOIN Inv_Def_BookingItemType dit ON dit.BookingItemTypeID = di.ItemTypeID
//WHERE ci.CostingID IN ({inClause})
//    AND di.ItemTypeID = @BookingType";

//                using var con = new SqlConnection(_connectionString);
//                using var cmd = new SqlCommand(query, con);
//                for (int i = 0; i < costingIds.Count; i++)
//                {
//                    cmd.Parameters.AddWithValue($"@cid{i}", costingIds[i]);
//                }

//                cmd.Parameters.AddWithValue("@BookingType", dto.BookingType);

//                await con.OpenAsync();
//                using var rdr = await cmd.ExecuteReaderAsync();

//                while (await rdr.ReadAsync())
//                {
//                    var temp = new RmgInvBookingReceivedDetailsButtonTemp
//                    {
//                        PurchaseReceiveNo = await GenerateAutoButtonId(),

//                        SerialNo = SafeToInt(rdr["SLNO"]),
//                        ItemId = SafeToString(rdr["ItemID"]),
//                        Description = SafeToString(rdr["Description"]),
//                        ColorId = SafeToString(rdr["ColorID"]),

//                        GermentQty = SafeToDecimal(rdr["GermentQty"]),
//                        TotalQty = SafeToDecimal(rdr["TotalQty"]),
//                        TotalQtyUnitId = SafeToString(rdr["TotalQtyUnitId"]),
//                        Consumption = SafeToDecimal(rdr["Consumption"]),

//                        ConsumptionUnitId = SafeToString(rdr["ConsumptionUnitId"]),

//                        Percentage = SafeToString(rdr["Percentage"]),
//                        CurrentReceiveQty = SafeToDecimal(rdr["CurrentReceiveQty"]),

//                        UnitPrice = SafeToDecimal(rdr["UnitPrice"]),
//                        ReceivedUnitPrice = SafeToDecimal(rdr["ReceivedUnitPrice"]),
//                        TotalPrice = SafeToDecimal(rdr["TotalPrice"]),
//                        CurrencyId = SafeToString(rdr["CurrencyId"]),

//                        TotalReceivedQtyPre = SafeToDecimal(rdr["TotalReceivedQtyPre"]),
//                        PendingReceiveQty = SafeToDecimal(rdr["PendingReceiveQty"]),
//                        PendingReceiveQtyPre = SafeToDecimal(rdr["PendingReceiveQtyPre"]),

//                        IntegraJobNo = SafeToString(rdr["IntegraJobNo"]),
//                        PoNo = SafeToString(rdr["PoNo"]),
//                        EmployeeId = SafeToString(rdr["EmployeeId"]),

//                        Brdid = "",
//                        FabricColorId = SafeToString(rdr["FabricColorId"]),
//                        SizeId = "",
//                        Idno = "",
//                        GermentsQtyUnitId = SafeToString(rdr["TotalQtyUnitId"]),
//                        OrderQty = SafeToDecimal(rdr["TotalQty"]),
//                        OrderQtyUnitId = SafeToString(rdr["TotalQtyUnitId"]),
//                        ReceivedUnitType = SafeToString(rdr["TotalQtyUnitId"]),
//                        Remarks = "",
//                        TotalReceivedQty = 0M,
//                    };

//                    await buttonTempRepo.AddAsync(temp);
//                }

//                var items = buttonTempRepo.All()
//                    .Select(x => new
//                    {
//                        Id = x.Id,
//                        PurchaseReceiveNo = x.PurchaseReceiveNo,
//                        brdId = x.Brdid,
//                        serialNo = x.SerialNo,
//                        fabricColorId = x.FabricColorId,

//                        // Item Details
//                        itemID = x.ItemId,
//                        description = x.Description,
//                        colorID = x.ColorId,
//                        sizeId = x.SizeId,
//                        idno = x.Idno,

//                        // Quantity
//                        garmentQty = x.GermentQty,
//                        garmentsQtyUnitId = x.GermentsQtyUnitId,
//                        consumption = x.Consumption,
//                        consumptionUnitId = x.ConsumptionUnitId,
//                        totalQty = x.TotalQty,
//                        totalQtyUnitId = x.TotalQtyUnitId,
//                        OrderQty = x.OrderQty,
//                        orderQtyUnitId = x.OrderQtyUnitId,
//                        Percentage = x.Percentage,

//                        // Received/Price
//                        totalReceivedQty = x.TotalReceivedQty,
//                        currentReceiveQty = x.CurrentReceiveQty,
//                        receivedUnitType = x.ReceivedUnitType,
//                        UnitPrice = x.UnitPrice,
//                        receivedUnitPrice = x.ReceivedUnitPrice,
//                        TotalPrice = x.TotalPrice,
//                        currencyId = x.CurrencyId,

//                        // Other
//                        remarks = x.Remarks,
//                        employeeId = x.EmployeeId,
//                        totalReceivedQtyPre = x.TotalReceivedQtyPre,
//                        pendingReceiveQty = x.PendingReceiveQty,
//                        pendingReceiveQtyPre = x.PendingReceiveQtyPre,
//                        IntegraJobNO = x.IntegraJobNo,
//                        PoNo = x.PoNo,
//                    })
//                    .ToList();

//                return items.Cast<object>().ToList();
//            }
//            catch (Exception ex)
//            {
//                Console.Error.WriteLine($"Error retrieving button booking data: {ex.Message}");
//                throw;
//            }
//        }


//        public async Task<string> GenerateAutoButtonId()
//        {
//            var getYear = DateTime.Now.Year.ToString();
//            var prefix = "POR_" + getYear + "_";

//            string lastCode = null;

//            using (SqlConnection con = new SqlConnection(_connectionString))
//            {
//                await con.OpenAsync();

//                string queryTemp = @"SELECT MAX(PurchaseReceiveNo) 
//                             FROM RMG_Inv_BookingReceivedDetails_ButtonTemp 
//                             WHERE PurchaseReceiveNo LIKE @prefix + '%'";
//                using (SqlCommand cmd = new SqlCommand(queryTemp, con))
//                {
//                    cmd.Parameters.AddWithValue("@prefix", prefix);
//                    var result = await cmd.ExecuteScalarAsync();
//                    lastCode = result?.ToString();
//                }

//                if (string.IsNullOrEmpty(lastCode))
//                {
//                    string queryMain = @"SELECT MAX(PurchaseReceiveNo) 
//                                 FROM RMG_Inv_BookingReceivedDetails_Button
//                                 WHERE PurchaseReceiveNo LIKE @prefix + '%'";
//                    using (SqlCommand cmd = new SqlCommand(queryMain, con))
//                    {
//                        cmd.Parameters.AddWithValue("@prefix", prefix);
//                        var result = await cmd.ExecuteScalarAsync();
//                        lastCode = result?.ToString();
//                    }
//                }
//            }

//            int nextNumber = 1;
//            if (!string.IsNullOrEmpty(lastCode))
//            {
//                string numberPart = lastCode.Substring(prefix.Length);
//                if (int.TryParse(numberPart, out int currentNumber))
//                {
//                    nextNumber = currentNumber + 1;
//                }
//            }

//            string nextCode = prefix + nextNumber.ToString().PadLeft(6, '0');
//            return nextCode;
//        }
//        // ==================== Extra Methods ====================


//        private async Task<List<object>> GetExtraBookingData(ItemTypeFilterDto dto)
//        {
//            var exTempExtra = extraTempRepo.All().ToList();
//            if (exTempExtra.Any())
//            {
//                await extraTempRepo.DeleteRangeAsync(exTempExtra);
//            }
//            var costingIds = dto.CostingId;
//            if (costingIds == null || !costingIds.Any())
//                return new List<object>();

//            string inClause = string.Join(",", costingIds.Select((x, i) => $"@cid{i}"));


//            string query = $@"
//SELECT 
//    cd.Id,
//    cd.SLNO,
//    cd.ItemID,
//    cd.Description,
//    cd.ColorID,
//    cd.Quantity,
//    cd.Consumption,
//    cd.TotalQuantity,
//    cd.UnitPrice,
//    cd.TotalPrice,
//    cd.Extra,
//    ci.IntegraJobNO AS IntegraJobNo,
//    ci.PoNo AS PoNo
//FROM RMG_CostingInfo ci
//LEFT JOIN RMG_CostingDetails cd ON ci.CostingID = cd.CostingID
//LEFT JOIN Inv_Def_Item di ON di.ItemID = cd.ItemID
//LEFT JOIN Inv_Def_BookingItemType dit ON dit.BookingItemTypeID = di.ItemTypeID
//WHERE ci.CostingID IN ({inClause})
//    AND di.ItemTypeID = @BookingType";

//            using var con = new SqlConnection(_connectionString);
//            using var cmd = new SqlCommand(query, con);

//            for (int i = 0; i < costingIds.Count; i++)
//            {
//                cmd.Parameters.AddWithValue($"@cid{i}", costingIds[i]);
//            }
//            cmd.Parameters.AddWithValue("@BookingType", dto.BookingType);

//            await con.OpenAsync();
//            using var rdr = await cmd.ExecuteReaderAsync();

//            while (await rdr.ReadAsync())
//            {
//                var temp = new RmgInvBookingReceivedDetailsExtraTemp
//                {
//                    PurchaseReceiveNo = await GenerateAutoExtraId(),

//                    // --- Core/Mapped Data from SQL ---
//                    Slno = SafeToInt(rdr["SLNO"]),
//                    ItemId = SafeToString(rdr["ItemID"]),
//                    ColorId = SafeToString(rdr["ColorID"]),
//                    Description = SafeToString(rdr["Description"]),

//                    OrderQty = SafeToDecimal(rdr["Quantity"]),
//                    ReqQty = SafeToDecimal(rdr["Quantity"]),
//                    Consumption = SafeToDecimal(rdr["Consumption"]),
//                    TotalQty = SafeToDecimal(rdr["TotalQuantity"]),

//                    ReceivedUnitPrice = SafeToDecimal(rdr["UnitPrice"]),
//                    UnitPrice = SafeToDecimal(rdr["UnitPrice"]),
//                    TotalPrice = SafeToDecimal(rdr["TotalPrice"]),

//                    Percentage = SafeToString(rdr["Extra"]),

//                    TotalReceivedQty = 0M,
//                    PendingReceiveQty = 0M,

//                    IntegraJobNo = SafeToString(rdr["IntegraJobNo"]),
//                    PoNo = SafeToString(rdr["PoNo"]),

//                    Brdid = "",
//                    FabricColorId = "",

//                    OrderQtyIunitD = "",
//                    ConsumptionUnitId = "",
//                    TotalQtyUnitId = "",
//                    ReqQtyUnitId = "",
//                    ReceivedUnitType = "",

//                    CurrencyId = "",
//                    Remarks = "",
//                    EmployeeId = "",
//                    TotalReceivedQtyPre = 0M,
//                    CurrentReceiveQty = 0M,
//                    PendingReceiveQtyPre = 0M,
//                };

//                await extraTempRepo.AddAsync(temp);
//            }

//            var items = extraTempRepo.All()
//                .Select(x => new
//                {
//                    Id = x.Id,
//                    PurchaseReceiveNo = x.PurchaseReceiveNo,
//                    brdId = x.Brdid,
//                    slno = x.Slno ?? 0,

//                    itemID = x.ItemId,
//                    fabricColorId = x.FabricColorId,
//                    description = x.Description,
//                    colorID = x.ColorId,

//                    OrderQty = (int)Math.Ceiling((decimal)x.TotalQty),
//                    orderQtyIunitD = x.OrderQtyIunitD,
//                    consumption = x.Consumption,
//                    consumptionUnitId = x.ConsumptionUnitId,
//                    totalQty = x.TotalQty,
//                    totalQtyUnitId = x.TotalQtyUnitId,
//                    reqQty = x.ReqQty,
//                    reqQtyUnitId = x.ReqQtyUnitId,
//                    Percentage = x.Percentage,

//                    totalReceivedQty = x.TotalReceivedQty,
//                    currentReceiveQty = x.CurrentReceiveQty,
//                    receivedUnitType = x.ReceivedUnitType,
//                    unitPrice = x.UnitPrice,
//                    receivedUnitPrice = x.ReceivedUnitPrice,
//                    totalPrice = x.TotalPrice,
//                    currencyId = x.CurrencyId,
//                    garmentQty = x.OrderQty,

//                    remarks = x.Remarks,
//                    employeeId = x.EmployeeId,
//                    totalReceivedQtyPre = x.TotalReceivedQtyPre,
//                    pendingReceiveQty = x.PendingReceiveQty,
//                    pendingReceiveQtyPre = x.PendingReceiveQtyPre,
//                    IntegraJobNO = x.IntegraJobNo,
//                    PoNo = x.PoNo
//                })
//                .ToList();

//            return items.Cast<object>().ToList();
//        }


//        public async Task<string> GenerateAutoExtraId()
//        {
//            var getYear = DateTime.Now.Year.ToString();
//            var prefix = "POR_" + getYear + "_";

//            string lastCode = null;

//            using (SqlConnection con = new SqlConnection(_connectionString))
//            {
//                await con.OpenAsync();

//                string queryTemp = @"SELECT MAX(PurchaseReceiveNo) 
//                             FROM RMG_Inv_BookingReceivedDetails_ExtraTemp 
//                             WHERE PurchaseReceiveNo LIKE @prefix + '%'";
//                using (SqlCommand cmd = new SqlCommand(queryTemp, con))
//                {
//                    cmd.Parameters.AddWithValue("@prefix", prefix);
//                    var result = await cmd.ExecuteScalarAsync();
//                    lastCode = result?.ToString();
//                }

//                if (string.IsNullOrEmpty(lastCode))
//                {
//                    string queryMain = @"SELECT MAX(PurchaseReceiveNo) 
//                                 FROM RMG_Inv_BookingReceivedDetails_Extra 
//                                 WHERE PurchaseReceiveNo LIKE @prefix + '%'";
//                    using (SqlCommand cmd = new SqlCommand(queryMain, con))
//                    {
//                        cmd.Parameters.AddWithValue("@prefix", prefix);
//                        var result = await cmd.ExecuteScalarAsync();
//                        lastCode = result?.ToString();
//                    }
//                }
//            }

//            int nextNumber = 1;
//            if (!string.IsNullOrEmpty(lastCode))
//            {
//                string numberPart = lastCode.Substring(prefix.Length);
//                if (int.TryParse(numberPart, out int currentNumber))
//                {
//                    nextNumber = currentNumber + 1;
//                }
//            }

//            string nextCode = prefix + nextNumber.ToString().PadLeft(6, '0');
//            return nextCode;
//        }

//        // ==================== Febric Methods ====================


//        private async Task<List<object>> GetFebricBookingData(ItemTypeFilterDto dto)
//        {
//            try
//            {
//                var exTempFebric = febricTempRepo.All().ToList();
//                if (exTempFebric.Any())
//                {
//                    await febricTempRepo.DeleteRangeAsync(exTempFebric);
//                }

//                var costingIds = dto.CostingId;
//                if (costingIds == null || !costingIds.Any())
//                    return new List<object>();

//                string inClause = string.Join(",", costingIds.Select((x, i) => $"@cid{i}"));

//                string query = $@"
//SELECT 
//    cd.Id,
//    cd.SLNO,
//    cd.ItemID,
//    cd.Description,
//    cd.ColorID,
//    cd.Quantity,
//    cd.Consumption,
//    cd.Extra,
//    cd.TotalQuantity,
//    cd.TotalQuantityUnit,
//    cd.UnitPrice,
//    cd.TotalPrice,
//    cd.TotalPriceCurrencyId AS CurrencyId,
//    cd.SupplierID AS EmployeeId, -- Assuming SupplierID can be used for EmployeeId temporarily

//    ci.IntegraJobNO AS IntegraJobNo,
//    ci.PoNo AS PoNo
//FROM RMG_CostingInfo ci
//LEFT JOIN RMG_CostingDetails cd ON ci.CostingID = cd.CostingID
//LEFT JOIN Inv_Def_Item di ON di.ItemID = cd.ItemID
//LEFT JOIN Inv_Def_BookingItemType dit ON dit.BookingItemTypeID = di.ItemTypeID
//WHERE ci.CostingID IN ({inClause})
//    AND di.ItemTypeID = @BookingType";

//                using var con = new SqlConnection(_connectionString);
//                using var cmd = new SqlCommand(query, con);

//                for (int i = 0; i < costingIds.Count; i++)
//                {
//                    cmd.Parameters.AddWithValue($"@cid{i}", costingIds[i]);
//                }
//                cmd.Parameters.AddWithValue("@BookingType", dto.BookingType);

//                await con.OpenAsync();
//                using var rdr = await cmd.ExecuteReaderAsync();


//                while (await rdr.ReadAsync())
//                {
//                    var temp = new RmgInvBookingReceivedDetailsFebricTemp
//                    {
//                        PurchaseReceiveNo = await GenerateAutoFebrickId(),

//                        Slno = SafeToInt(rdr["SLNO"]),
//                        ItemId = SafeToString(rdr["ItemID"]),
//                        FebricDetails = SafeToString(rdr["Description"]),
//                        ColorId = SafeToString(rdr["ColorID"]),

//                        OrderQty = SafeToDecimal(rdr["Quantity"]),
//                        Consumption = SafeToDecimal(rdr["Consumption"]),
//                        TotalFebricQty = SafeToDecimal(rdr["TotalQuantity"]),
//                        Percentage = SafeToDecimal(rdr["Extra"]),

//                        ReceivedUnitPrice = SafeToDecimal(rdr["UnitPrice"]),
//                        TotalPrice = SafeToDecimal(rdr["TotalPrice"]),

//                        CurrentReceiveQty = SafeToDecimal(rdr["Quantity"]),
//                        TotalReceivedQty = SafeToDecimal(rdr["Consumption"]),

//                        IntegraJobNo = SafeToString(rdr["IntegraJobNo"]),
//                        PoNo = SafeToString(rdr["PoNo"]),

//                        QtyUnit = SafeToString(rdr["TotalQuantityUnit"]),
//                        ConsumtionUnit = SafeToString(rdr["TotalQuantityUnit"]),

//                        Brdid = "",
//                        FabricItemId = dto.BookingType,
//                        Refcode = "",
//                        ReceivedUnitType = SafeToString(rdr["TotalQuantityUnit"]),
//                        CurrencyId = SafeToString(rdr["CurrencyId"]),
//                        EmployeeId = SafeToString(rdr["EmployeeId"]),

//                        PendingReceiveQty = 0M,
//                        TotalReceivedQtyPre = 0M,
//                        PendingReceiveQtyPre = 0M,
//                        UnitPrice = 0M
//                    };

//                    await febricTempRepo.AddAsync(temp);
//                }

//                var items = febricTempRepo.All()
//                    .Select(x => new
//                    {
//                        Id = x.Id,
//                        PurchaseReceiveNo = x.PurchaseReceiveNo,
//                        brdId = x.Brdid,
//                        slno = x.Slno ?? 0,

//                        // Item Details
//                        itemID = x.ItemId,
//                        fabricItemId = x.FabricItemId,
//                        description = x.FebricDetails,
//                        colorID = x.ColorId,
//                        refcode = x.Refcode,

//                        // Quantity
//                        OrderQty = x.OrderQty,
//                        qtyUnit = x.QtyUnit,
//                        consumption = x.Consumption,
//                        consumtionUnit = x.ConsumtionUnit,
//                        totalFebricQty = x.TotalFebricQty,
//                        percentage = x.Percentage,

//                        // Price & Received
//                        unitPrice = x.UnitPrice,
//                        receivedUnitPrice = x.ReceivedUnitPrice,
//                        totalPrice = x.TotalPrice,
//                        currencyId = x.CurrencyId,

//                        totalReceivedQty = x.TotalReceivedQty,
//                        currentReceiveQty = x.CurrentReceiveQty,
//                        receivedUnitType = x.ReceivedUnitType,
//                        totalReceivedQtyPre = x.TotalReceivedQtyPre,
//                        pendingReceiveQty = x.PendingReceiveQty,
//                        pendingReceiveQtyPre = x.PendingReceiveQtyPre,

//                        // Other
//                        employeeId = x.EmployeeId,
//                        integraJobNo = x.IntegraJobNo,
//                        poNo = x.PoNo
//                    })
//                    .ToList();

//                return items.Cast<object>().ToList();
//            }
//            catch (Exception ex)
//            {
//                Console.Error.WriteLine($"Error retrieving fabric booking data: {ex.Message}");
//                throw;
//            }
//        }




//        public async Task<string> GenerateAutoFebrickId()
//        {
//            var getYear = DateTime.Now.Year.ToString();
//            var prefix = "POR_" + getYear + "_";

//            string lastCode = null;

//            using (SqlConnection con = new SqlConnection(_connectionString))
//            {
//                await con.OpenAsync();

//                string queryTemp = @"SELECT MAX(PurchaseReceiveNo) 
//                             FROM RMG_Inv_BookingReceivedDetails_FebricTemp 
//                             WHERE PurchaseReceiveNo LIKE @prefix + '%'";
//                using (SqlCommand cmd = new SqlCommand(queryTemp, con))
//                {
//                    cmd.Parameters.AddWithValue("@prefix", prefix);
//                    var result = await cmd.ExecuteScalarAsync();
//                    lastCode = result?.ToString();
//                }

//                if (string.IsNullOrEmpty(lastCode))
//                {
//                    string queryMain = @"SELECT MAX(PurchaseReceiveNo) 
//                                 FROM RMG_Inv_BookingReceivedDetails_Febric 
//                                 WHERE PurchaseReceiveNo LIKE @prefix + '%'";
//                    using (SqlCommand cmd = new SqlCommand(queryMain, con))
//                    {
//                        cmd.Parameters.AddWithValue("@prefix", prefix);
//                        var result = await cmd.ExecuteScalarAsync();
//                        lastCode = result?.ToString();
//                    }
//                }
//            }

//            int nextNumber = 1;
//            if (!string.IsNullOrEmpty(lastCode))
//            {
//                string numberPart = lastCode.Substring(prefix.Length);
//                if (int.TryParse(numberPart, out int currentNumber))
//                {
//                    nextNumber = currentNumber + 1;
//                }
//            }

//            string nextCode = prefix + nextNumber.ToString().PadLeft(6, '0');
//            return nextCode;
//        }


//        [HttpPost]
//        public IActionResult GetPurchaseOrders()
//        {
//            var draw = Request.Form["draw"].FirstOrDefault();
//            var start = Request.Form["start"].FirstOrDefault();
//            var length = Request.Form["length"].FirstOrDefault();
//            var searchValue = Request.Form["search[value]"].FirstOrDefault();

//            // Filters
//            var poNo = Request.Form["poNo"].FirstOrDefault();
//            var style = Request.Form["style"].FirstOrDefault();
//            var buyer = Request.Form["buyer"].FirstOrDefault();
//            var masterPo = Request.Form["masterPo"].FirstOrDefault();
//            var funJobNo = Request.Form["funJobNo"].FirstOrDefault();

//            int pageSize = length != null ? Convert.ToInt32(length) : 10;
//            int skip = start != null ? Convert.ToInt32(start) : 0;

//            var list = new List<PurchaseOrderViewModel>();

//            using (SqlConnection con = new SqlConnection(_connectionString))
//            {
//                string query = @"
//       SELECT DISTINCT
//            ci.CostingID,
//            ci.StyleID,
//            st.Style,
//            ci.IntegraJobNO,
//            ci.BuyerID,
//            b.BuyerName,
//            ci.MasterPurchaseOrder,
//            ci.PoNo,
//            od.OrderQuantity
//        FROM RMG_CostingInfo ci        
//        left join RMG_Prod_OrderDetails od on od.PurchaseOrder= ci.PoNo
//        LEFT JOIN RMG_Prod_Def_Buyer b ON b.BuyerId = ci.BuyerID
//        LEFT JOIN Prod_Def_Style st ON ci.StyleID = st.StyleId
//        WHERE 1 = 1";


//                if (!string.IsNullOrEmpty(searchValue))
//                {
//                    query += @"
//            AND (
//                ci.PoNo LIKE @Search
//                OR st.Style LIKE @Search
//                OR b.BuyerName LIKE @Search
//                OR ci.MasterPurchaseOrder LIKE @Search
//                OR ci.IntegraJobNO LIKE @Search
//                OR CAST(od.OrderQuantity AS VARCHAR) LIKE @Search
//            )";
//                }

//                if (!string.IsNullOrEmpty(poNo))
//                    query += " AND ci.PoNo LIKE @PoNo";

//                if (!string.IsNullOrEmpty(style))
//                    query += " AND ci.StyleID = @Style";

//                if (!string.IsNullOrEmpty(buyer))
//                    query += " AND ci.BuyerID = @Buyer";

//                if (!string.IsNullOrEmpty(masterPo))
//                    query += " AND ci.MasterPurchaseOrder LIKE @MasterPo";

//                if (!string.IsNullOrEmpty(funJobNo))
//                    query += " AND ci.IntegraJobNO LIKE @FunJobNo";

//                SqlCommand cmd = new SqlCommand(query, con);

//                if (!string.IsNullOrEmpty(searchValue))
//                    cmd.Parameters.AddWithValue("@Search", "%" + searchValue + "%");

//                if (!string.IsNullOrEmpty(poNo))
//                    cmd.Parameters.AddWithValue("@PoNo", "%" + poNo + "%");

//                if (!string.IsNullOrEmpty(style))
//                    cmd.Parameters.AddWithValue("@Style", style);

//                if (!string.IsNullOrEmpty(buyer))
//                    cmd.Parameters.AddWithValue("@Buyer", buyer);

//                if (!string.IsNullOrEmpty(masterPo))
//                    cmd.Parameters.AddWithValue("@MasterPo", "%" + masterPo + "%");

//                if (!string.IsNullOrEmpty(funJobNo))
//                    cmd.Parameters.AddWithValue("@FunJobNo", "%" + funJobNo + "%");

//                con.Open();
//                SqlDataReader rdr = cmd.ExecuteReader();

//                while (rdr.Read())
//                {
//                    list.Add(new PurchaseOrderViewModel
//                    {
//                        Style = rdr["StyleID"].ToString(),
//                        CostingId = rdr["CostingId"].ToString(),
//                        StyleName = rdr["Style"].ToString(),
//                        FunJobNo = rdr["IntegraJobNO"].ToString(),
//                        Buyer = rdr["BuyerID"].ToString(),
//                        BuyerName = rdr["BuyerName"].ToString(),
//                        PoNo = rdr["PoNo"].ToString(),
//                        OrderQty = rdr["OrderQuantity"] == DBNull.Value ? null : Convert.ToInt32(rdr["OrderQuantity"]),
//                        MasterPo = rdr["MasterPurchaseOrder"].ToString()
//                    });
//                }
//            }

//            var recordsTotal = list.Count;

//            var data = pageSize == -1
//                ? list
//                : list.Skip(skip).Take(pageSize).ToList();

//            return Json(new
//            {
//                draw = draw,
//                recordsTotal = recordsTotal,
//                recordsFiltered = recordsTotal,
//                data = data
//            });
//        }




//        [HttpPost]
//        public JsonResult GetItemTypes([FromBody] List<string> CostingIds)
//        {
//            var list = new List<ItemTypeViewModel>();

//            if (CostingIds == null || !CostingIds.Any())
//                return Json(list);

//            using (SqlConnection con = new SqlConnection(_connectionString))
//            {
//                // Dynamic IN clause
//                var parameters = CostingIds
//                    .Select((id, index) => $"@id{index}")
//                    .ToArray();

//                string query = $@"
//            SELECT DISTINCT 
//                dit.BookingItemTypeID,
//                dit.BookingItemType
//            FROM RMG_CostingInfo ci
//            LEFT JOIN RMG_CostingDetails cd ON ci.CostingID = cd.CostingID
//            LEFT JOIN Inv_Def_Item di ON di.ItemID = cd.ItemID
//            LEFT JOIN Inv_Def_BookingItemType dit ON dit.BookingItemTypeID = di.ItemTypeID
//            WHERE cd.CostingID IN ({string.Join(",", parameters)})
//              AND (cd.PoNo IS NULL OR cd.PoNo = '')
//              AND dit.BookingItemTypeID IS NOT NULL
//              AND dit.BookingItemType IS NOT NULL";

//                SqlCommand cmd = new SqlCommand(query, con);

//                for (int i = 0; i < CostingIds.Count; i++)
//                {
//                    cmd.Parameters.AddWithValue(parameters[i], CostingIds[i]);
//                }

//                con.Open();
//                SqlDataReader rdr = cmd.ExecuteReader();

//                while (rdr.Read())
//                {
//                    list.Add(new ItemTypeViewModel
//                    {
//                        BookingItemTypeID = rdr["BookingItemTypeID"].ToString(),
//                        BookingItemType = rdr["BookingItemType"].ToString()
//                    });
//                }
//            }

//            return Json(list);
//        }


//        [HttpPost]
//        public async Task<IActionResult> SaveBooking([FromBody] RMGBookingOrderEntryBuklDto dto)
//        {
//            if (!ModelState.IsValid)
//            {
//                return BadRequest(ModelState);
//            }

//            var hasSavePermission = await rmgBookingOrderEntryBuklService.SavePermissionAsync(LoginInfo.AccessCode);

//            if (hasSavePermission)

//            {

//                dto.ToAudit(LoginInfo);
//                var (isSuccess, message) =
//                    await rmgBookingOrderEntryBuklService.SaveBookingAsync(dto, LoginInfo.CompanyCode);

//                if (isSuccess)
//                    return Ok(new { success = true, message });
//                return BadRequest(new { success = false, message });

//            }
//            else
//            {
//                return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
//            }

//        }


//        public async Task<IActionResult> GetBookingItemTypes(string id)
//        {
//            try
//            {
//                var dropdownData = await GetDropdownData();
//                var result = await rmgBookingOrderEntryBuklService.GetBookingItemTypesAsync(id);
//                return Json(new { success = result.isSuccess, message = result.message, data = result.data, dropdownData });

//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }

//        [HttpPost]
//        public async Task<IActionResult> GetBookingList()
//        {
//            var draw = Request.Form["draw"].FirstOrDefault();
//            var start = Convert.ToInt32(Request.Form["start"].FirstOrDefault());
//            var length = Convert.ToInt32(Request.Form["length"].FirstOrDefault());
//            var search = Request.Form["search[value]"].FirstOrDefault();

//            var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
//            var sortColumn = Request.Form[$"columns[{sortColumnIndex}][data]"].FirstOrDefault();
//            var sortDir = Request.Form["order[0][dir]"].FirstOrDefault();

//            var result = await rmgBookingOrderEntryBuklService.GetBookingListAsync(start, length, search, sortColumn, sortDir);

//            return Json(new
//            {
//                draw = draw,
//                recordsTotal = result.total,
//                recordsFiltered = result.filtered,
//                data = result.data
//            });
//        }

//        [HttpPost]
//        public async Task<IActionResult> UpdateBookingItem([FromBody] BookingItemUpdateDto itemDto)
//        {
//            // 1. Initial Validation
//            if (itemDto == null || itemDto.Id == 0)
//            {
//                return Json(new { success = false, message = "Invalid data or missing ID provided." });
//            }

//            try
//            {
//                var hasUpdatePermission = await rmgBookingOrderEntryBuklService.UpdatePermissionAsync(LoginInfo.AccessCode);

//                if (hasUpdatePermission)

//                {

//                    // 2. Determine the repository based on BookingType
//                    dynamic repo;

//                    switch (itemDto.BookingType)
//                    {
//                        case "04": repo = cartonTempRepo; break;
//                        case "07": repo = threadTempRepo; break;
//                        case "03": repo = polyTempRepo; break;
//                        case "02": repo = buttonTempRepo; break;
//                        case "01": repo = febricTempRepo; break;
//                        default: repo = extraTempRepo; break;
//                    }

//                    if (repo == null)
//                    {
//                        return Json(new { success = false, message = $"Configuration Error: Repository not found for Type {itemDto.BookingType}. Check Dependency Injection." });
//                    }

//                    // 3. Fetch the existing item
//                    var existingItem = await repo.GetByIdAsync(itemDto.Id);

//                    if (existingItem == null)
//                    {
//                        return Json(new { success = false, message = $"Item with Id {itemDto.Id} not found in database." });
//                    }

//                    //existingItem.ItemId = itemDto.ItemId;
//                    existingItem.ColorId = itemDto.ColorId;
//                    //existingItem.CurrencyId = itemDto.CurrencyId;
//                    //existingItem.PoNo = itemDto.PoNo;
//                    //existingItem.IntegraJobNo = itemDto.IntegraJobNo;
//                    existingItem.Remarks = itemDto.Remarks;
//                    //existingItem.ReceivedUnitPrice = itemDto.UnitPrice;
//                    //existingItem.TotalPrice = itemDto.TotalPrice;


//                    if (itemDto.BookingType == "01")
//                    {
//                        //existingItem.FebricDetails = itemDto.Description;
//                        //existingItem.Percentage = itemDto.Percentage; 

//                        // Quantity Mappings
//                        //existingItem.Consumption = itemDto.Consumption;
//                        //existingItem.ConsumtionUnit = itemDto.ConsumptionUnitID; 
//                        //existingItem.TotalFebricQty = itemDto.TotalQty;
//                        //existingItem.OrderQty = itemDto.OrderQty;
//                        //existingItem.QtyUnit = itemDto.OrderQtyUnitID; 
//                        // GarmentQty is not present in Febric model
//                    }
//                    // B. Carton (Type 04) - RmgInvBookingReceivedDetailsCartonTemp
//                    else if (itemDto.BookingType == "04")
//                    {
//                        //existingItem.ItemDescription = itemDto.Description;
//                        //existingItem.CartonPercent = itemDto.Percentage?.ToString(); 
//                        existingItem.SizeId = itemDto.SizeId;

//                        // Dimensions (Carton uses strings for dimensions in this model)
//                        existingItem.CartonLeangth = itemDto.CartonLength?.ToString();
//                        existingItem.LeangthUnitId = itemDto.LeangthUnitID;
//                        existingItem.CartonWidth = itemDto.CartonWidth?.ToString();
//                        existingItem.WidthUnitId = itemDto.WidthUnitID;
//                        existingItem.CatonHeight = itemDto.CatonHeight?.ToString();
//                        existingItem.HeightUnitId = itemDto.HeightUnitID;

//                        // Quantity Mappings (TotalQty maps to RequiredQty)
//                        //existingItem.Consumption = itemDto.Consumption;
//                        //existingItem.ConsumptionUnitId = itemDto.ConsumptionUnitID;
//                        //existingItem.RequiredQty = itemDto.TotalQty; 
//                        //existingItem.RequiredQtyUnitId = itemDto.TotalQtyUnitID; 
//                        //existingItem.OrderQty = itemDto.OrderQty;
//                        //existingItem.OrderUnitId = itemDto.OrderQtyUnitID; 
//                        // GarmentQty is not present in Carton model
//                    }
//                    // C. Thread (Type 07) - RmgInvBookingReceivedDetailsThreadTemp
//                    else if (itemDto.BookingType == "07")
//                    {
//                        //existingItem.FebricDetail = itemDto.Description;
//                        existingItem.ThreadCountId = itemDto.ThreadCountID;
//                        //existingItem.Threadpercent = itemDto.Percentage?.ToString();

//                        // Quantity Mappings
//                        //existingItem.Consumption = itemDto.Consumption;
//                        //existingItem.ConsumtionUnitId = itemDto.ConsumptionUnitID;
//                        //existingItem.TotalQty = itemDto.TotalQty;
//                        //existingItem.TotalQtyUnitId = itemDto.TotalQtyUnitID;
//                        //existingItem.OrderQty = itemDto.OrderQty;
//                        //existingItem.QtyUnitId = itemDto.OrderQtyUnitID;
//                        // GarmentQty is not present in Thread model
//                    }
//                    // D. Poly (Type 03) - RmgInvBookingReceivedDetailsPolyTemp
//                    else if (itemDto.BookingType == "03")
//                    {
//                        //existingItem.ItemDescription = itemDto.Description; 
//                        //existingItem.Percentage = itemDto.Percentage?.ToString(); 

//                        // Dimensions (Poly uses strings for dimensions in this model)
//                        existingItem.Length = itemDto.Length?.ToString();
//                        existingItem.LengthUnitId = itemDto.LengthUnitID;
//                        existingItem.Width = itemDto.Width?.ToString();
//                        existingItem.WidthUnitId = itemDto.WidthUnitID;
//                        existingItem.Flap = itemDto.Flap?.ToString();
//                        existingItem.FlapUnitId = itemDto.FlapUnitID;
//                        existingItem.Guest = itemDto.Guest?.ToString();
//                        existingItem.GuestUnitId = itemDto.GuestUnitID;

//                        // Quantity Mappings
//                        //existingItem.GarmentQty = itemDto.GarmentQty;
//                        //existingItem.GarmentQtyUnitId = itemDto.GarmentQtyUnitID;
//                        //existingItem.Consumption = itemDto.Consumption;
//                        //existingItem.ConsumptionUnitId = itemDto.ConsumptionUnitID;
//                        //existingItem.TotalQty = itemDto.TotalQty;
//                        //existingItem.TotalQtyUnitId = itemDto.TotalQtyUnitID;
//                        // OrderQty is not present in Poly model
//                    }
//                    // E. Button (Type 02) - RmgInvBookingReceivedDetailsButtonTemp
//                    else if (itemDto.BookingType == "02")
//                    {
//                        //existingItem.Description = itemDto.Description;
//                        //existingItem.Percentage = itemDto.Percentage?.ToString();

//                        // Quantity Mappings (Note: GarmentQty uses 'GermentQty' name)
//                        //existingItem.GermentQty = itemDto.GarmentQty; 
//                        //existingItem.GermentsQtyUnitId = itemDto.GarmentQtyUnitID;
//                        //existingItem.Consumption = itemDto.Consumption;
//                        //existingItem.ConsumptionUnitId = itemDto.ConsumptionUnitID;
//                        //existingItem.TotalQty = itemDto.TotalQty;
//                        //existingItem.TotalQtyUnitId = itemDto.TotalQtyUnitID;
//                        //existingItem.OrderQty = itemDto.OrderQty;
//                        //existingItem.OrderQtyUnitId = itemDto.OrderQtyUnitID;
//                    }
//                    // F. Extra (Default) - RmgInvBookingReceivedDetailsExtraTemp
//                    else // default booking type
//                    {
//                        //existingItem.Description = itemDto.Description;
//                        //existingItem.Percentage = itemDto.Percentage?.ToString();

//                        // Quantity Mappings (TotalQty maps to ReqQty)
//                        //existingItem.Consumption = itemDto.Consumption;
//                        //existingItem.ConsumptionUnitId = itemDto.ConsumptionUnitID;
//                        //existingItem.TotalQty = itemDto.TotalQty;
//                        //existingItem.TotalQtyUnitId = itemDto.TotalQtyUnitID;
//                        //existingItem.ReqQty = itemDto.OrderQty;
//                        //existingItem.ReqQtyUnitId = itemDto.OrderQtyUnitID;
//                        // GarmentQty is not present in Extra model
//                    }

//                    // 4. Update the item in the database
//                    await repo.UpdateAsync(existingItem);

//                    return Json(new { success = true, message = "Booking item updated successfully." });

//                }

//                else

//                {

//                    return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });

//                }



//            }
//            catch (Exception ex)
//            {
//                // Log the full exception (ex) in your server logs for better debugging
//                return Json(new { success = false, message = $"Database Update Failed: {ex.Message}. Check model definitions, especially for the '{itemDto.BookingType}' type." });
//            }
//        }

//        [HttpPost]
//        public async Task<IActionResult> DeleteBookingOrder([FromBody] List<decimal> DeleteBookingIds)
//        {
//            try
//            {
//                var hasPermission = await rmgBookingOrderEntryBuklService.DeletePermissionAsync(LoginInfo.AccessCode);

//                if (!hasPermission)

//                {

//                    return Json(new { success = false, message = "You have no access." });

//                }

//                var result = await rmgBookingOrderEntryBuklService.DeleteBookingOrderAsync(DeleteBookingIds);
//                return Json(new { success = result.success, message = result.message });
//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }
//    }

//    public class SaveBookingRequest
//    {
//        public string BookingType { get; set; }
//        public List<Dictionary<string, object>> BookingData { get; set; }
//    }
//}


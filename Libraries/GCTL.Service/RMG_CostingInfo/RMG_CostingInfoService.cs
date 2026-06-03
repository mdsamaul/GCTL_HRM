//using AutoMapper;
//using Dapper;
//using GCTL.Core.Data;
//using GCTL.Core.ViewModels.RMG_CostingInfo;
//using GCTL.Data.Models;
//using GCTL.Service.Common;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Data.SqlClient;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using OfficeOpenXml;
//using System.Data;

//namespace GCTL.Service.RMG_CostingInfo
//{
//    public class RMG_CostingInfoService : AppService<RmgCostingInfo>, IRMG_CostingInfoService
//    {
//        private readonly IRepository<RmgCostingInfo> costingInfoRepo;
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
//        private readonly IRepository<RmgBookingOrder> boRepo;
//        private readonly IRepository<RmgProdDefColor> colorRepo;
//        private readonly IRepository<RmgProdDefSize> sizeRepo;
//        private readonly IRepository<RmgCostingDetailsTemp> rmgCostingDetailsTempRepo;
//        private readonly IRepository<RmgCostingDetails> rmgCostingDetailsRepo;
//        private readonly IRepository<InvDefBookingItemType> itemBookingTypeRepo;
//        private readonly IRepository<RmgDefSupplier> supplieRepo;
//        private readonly IMapper mapper;
//        private readonly ICommonService commonService;
//        private readonly string _connectionString;

//        private readonly IRepository<RmgInvBookingReceivedDetailsCarton> cartonRepo;
//        private readonly IRepository<RmgInvBookingReceivedDetailsButton> buttonRepo;
//        private readonly IRepository<RmgInvBookingReceivedDetailsExtra> extraRepo;
//        private readonly IRepository<RmgInvBookingReceivedDetailsFebric> febricRepo;
//        private readonly IRepository<RmgInvBookingReceivedDetailsPoly> polyRepo;
//        private readonly IRepository<RmgInvBookingReceivedDetailsThread> threadRepo;

//        public RMG_CostingInfoService(
//            IRepository<RmgCostingInfo> costingInfoRepo,
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
//             IRepository<CaDefCurrency> currencyRepo,
//             IRepository<RmgBookingOrder> boRepo,
//             IRepository<RmgProdDefColor> colorRepo,
//             IRepository<RmgProdDefSize> sizeRepo,
//             IRepository<RmgCostingDetailsTemp> RmgCostingDetailsTempRepo,
//             IRepository<RmgCostingDetails> RmgCostingDetailsRepo,
//             IRepository<InvDefBookingItemType> itemBookingTypeRepo,
//            IConfiguration configuration,
//              IRepository<RmgDefSupplier> supplieRepo,
//            IMapper mapper,
//            ICommonService commonService,
//            IRepository<RmgInvBookingReceivedDetailsCarton> cartonRepo,
//            IRepository<RmgInvBookingReceivedDetailsButton> buttonRepo,
//            IRepository<RmgInvBookingReceivedDetailsExtra> extraRepo,
//            IRepository<RmgInvBookingReceivedDetailsFebric> febricRepo,
//            IRepository<RmgInvBookingReceivedDetailsPoly> polyRepo,
//            IRepository<RmgInvBookingReceivedDetailsThread> threadRepo

//            ) : base(costingInfoRepo)
//        {
//            this.costingInfoRepo = costingInfoRepo;
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
//            this.boRepo = boRepo;
//            this.colorRepo = colorRepo;
//            this.sizeRepo = sizeRepo;
//            rmgCostingDetailsTempRepo = RmgCostingDetailsTempRepo;
//            rmgCostingDetailsRepo = RmgCostingDetailsRepo;
//            this.itemBookingTypeRepo = itemBookingTypeRepo;
//            this.supplieRepo = supplieRepo;
//            this.cartonRepo = cartonRepo;
//            this.buttonRepo = buttonRepo;
//            this.extraRepo = extraRepo;
//            this.febricRepo = febricRepo;
//            this.polyRepo = polyRepo;
//            this.threadRepo = threadRepo;
//            this.mapper = mapper;
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

//        public async Task<bool> PagePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "RMG Costing Info" && x.TitleCheck);
//        }

//        public async Task<bool> SavePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "RMG Costing Info" && x.CheckAdd);
//        }

//        public async Task<bool> UpdatePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "RMG Costing Info" && x.CheckEdit);
//        }

//        public async Task<bool> DeletePermissionAsync(string accessCode)
//        {
//            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "RMG Costing Info" && x.CheckDelete);
//        }

//        public async Task<List<ProdOrderReportDto>> GetProdOrderReport(ProdOrderFilterDto filter)
//        {
//            using (var connection = new SqlConnection(_connectionString))
//            {
//                var parameters = new DynamicParameters();
//                parameters.Add("@BuyerId", string.IsNullOrWhiteSpace(filter.BuyerId) ? null : filter.BuyerId);
//                parameters.Add("@JobNo", string.IsNullOrWhiteSpace(filter.JobNo) ? null : filter.JobNo);
//                parameters.Add("@StyleId", string.IsNullOrWhiteSpace(filter.StyleId) ? null : filter.StyleId);
//                parameters.Add("@MPO", string.IsNullOrWhiteSpace(filter.MPO) ? null : filter.MPO);
//                parameters.Add("@PurchaseOrder", string.IsNullOrWhiteSpace(filter.PurchaseOrder) ? null : filter.PurchaseOrder);

//                var rawData = await connection.QueryAsync<ProdOrderReportRawDto>(
//                    "GetProdOrderReport",
//                    parameters,
//                    commandType: CommandType.StoredProcedure
//                );

//                var groupedData = rawData
//                    .GroupBy(x => new
//                    {
//                        x.BuyerId,
//                        x.IntegraJOBNo,
//                        x.StylePOWise,
//                        x.StyleId,
//                        x.MasterPurchaseOrder,
//                        x.PurchaseOrder,
//                        x.ProductId,
//                        x.PDescription,
//                        x.SupplierId,
//                        x.DeliveryDate,
//                        x.LUser,
//                        x.ProductName,
//                        x.StyleName,
//                        x.BuyerName
//                    })
//                    .Select(g => new ProdOrderReportDto
//                    {
//                        BuyerId = g.Key.BuyerId,
//                        BuyerName = g.Key.BuyerName,
//                        IntegraJOBNo = g.Key.IntegraJOBNo,
//                        StylePOWise = g.Key.StylePOWise,
//                        StyleId = g.Key.StyleId,
//                        StyleName = g.Key.StyleName,
//                        MasterPurchaseOrder = g.Key.MasterPurchaseOrder,
//                        PurchaseOrder = g.Key.PurchaseOrder,
//                        ProductId = g.Key.ProductId,
//                        ProductName = g.Key.ProductName,
//                        PDescription = g.Key.PDescription,
//                        SupplierId = g.Key.SupplierId,
//                        DeliveryDate = g.Key.DeliveryDate,
//                        LUser = g.Key.LUser,
//                        ColorSizeBreakups = g
//                            .Where(x => !string.IsNullOrWhiteSpace(x.ColorId) || !string.IsNullOrWhiteSpace(x.SizeId))
//                            .Select(x => new ColorSizeBreakupDto
//                            {
//                                StyleId = g.Key.StyleId,
//                                StyleName = styleRepo.All().Where(st => st.StyleId == g.Key.StyleId).Select(s => s.Style).FirstOrDefault(),
//                                ColorId = x.ColorId,
//                                ColorName = colorRepo.All().Where(c => c.ColorId == x.ColorId).Select(s => s.Color).FirstOrDefault(),
//                                SizeId = x.SizeId,
//                                SizeName = sizeRepo.All().Where(si => si.SizeId == x.SizeId).Select(s => s.Size).FirstOrDefault(),
//                                Quantity = x.Quantity
//                            })
//                            .ToList()
//                    })
//                    .ToList();

//                return groupedData;
//            }
//        }

//        public async Task<FilterOptionsDto> GetFilterOptions(ProdOrderFilterDto filter = null)
//        {
//            using (var connection = new SqlConnection(_connectionString))
//            {
//                var parameters = new DynamicParameters();
//                parameters.Add("@BuyerId", string.IsNullOrWhiteSpace(filter?.BuyerId) ? null : filter.BuyerId);
//                parameters.Add("@JobNo", string.IsNullOrWhiteSpace(filter?.JobNo) ? null : filter.JobNo);
//                parameters.Add("@StyleId", string.IsNullOrWhiteSpace(filter?.StyleId) ? null : filter.StyleId);
//                parameters.Add("@MPO", string.IsNullOrWhiteSpace(filter?.MPO) ? null : filter.MPO);
//                parameters.Add("@PurchaseOrder", string.IsNullOrWhiteSpace(filter?.PurchaseOrder) ? null : filter.PurchaseOrder);

//                var result = await connection.QueryAsync<ProdOrderReportRawDto>(
//                    "GetProdOrderReport",
//                    parameters,
//                    commandType: CommandType.StoredProcedure
//                );

//                var data = result.ToList();

//                return new FilterOptionsDto
//                {


//                    Buyers = data.Where(x => !string.IsNullOrWhiteSpace(x.BuyerId))
//                    .Select(x => x.BuyerId.Trim())
//                    .Distinct()
//                    .Select(id =>
//                    {
//                        var buyer = buyerRepo.All().FirstOrDefault(b => b.BuyerId == id);
//                        return new BuyerDto
//                        {
//                            Id = id,
//                            Name = buyer != null ? buyer.BuyerName : id
//                        };
//                    })
//                    .OrderBy(x => x.Name)
//                    .ToList(),


//                    JobNos = data
//                        .Where(x => !string.IsNullOrWhiteSpace(x.IntegraJOBNo))
//                        .Select(x => new JobNoDto { Id = x.IntegraJOBNo.Trim(), Name = x.IntegraJOBNo.Trim() })
//                        .GroupBy(x => x.Id)
//                        .Select(g => g.First())
//                        .OrderBy(x => x.Name)
//                        .ToList(),

//                    Styles = data
//                        .Where(x => !string.IsNullOrWhiteSpace(x.StyleId))
//                        .Select(x => x.StyleId.Trim())
//                        .Distinct()
//                        .Select(id =>
//                        {
//                            var style = styleRepo.All().FirstOrDefault(s => s.StyleId == id);
//                            return new StyleDto
//                            {
//                                Id = id,
//                                Name = style != null ? style.Style : id
//                            };
//                        })
//                        .OrderBy(x => x.Name)
//                        .ToList(),


//                    MasterPOs = data
//                        .Where(x => !string.IsNullOrWhiteSpace(x.MasterPurchaseOrder))
//                        .Select(x => new MasterPODto { Id = x.MasterPurchaseOrder.Trim(), Name = x.MasterPurchaseOrder.Trim() })
//                        .GroupBy(x => x.Id)
//                        .Select(g => g.First())
//                        .OrderBy(x => x.Name)
//                        .ToList(),

//                    PurchaseOrders = data
//                        .Where(x => !string.IsNullOrWhiteSpace(x.PurchaseOrder))
//                        .Select(x => new PurchaseOrderDto { Id = x.PurchaseOrder.Trim(), Name = x.PurchaseOrder.Trim() })
//                        .GroupBy(x => x.Id)
//                        .Select(g => g.First())
//                        .OrderBy(x => x.Name)
//                        .ToList()
//                };
//            }
//        }

//        public async Task<List<RmgCostingDetailsTempDto>> GetAllByCostingIdAsync(
//    string costingId,
//    bool clearTemp = true,
//    string username = null)
//        {
//            if (clearTemp)
//            {
//                // after temporary rows delete
//                //var existing = await rmgCostingDetailsTempRepo.All()
//                //    .Where(x => x.CostingId == costingId)
//                //    .ToListAsync();
//                var existing = await rmgCostingDetailsTempRepo.All()
//                    .ToListAsync();

//                await rmgCostingDetailsTempRepo.DeleteRangeAsync(existing);

//                // after auto-incremented ID generate
//                var firstGeneratedCode = commonService.GenerateNextCode(
//                                            "CostingDetailsId",
//                                            "RMG_CostingDetails",
//                                            8,
//                                            "CO_DL_");

//                int currentNumber = int.Parse(firstGeneratedCode.Replace("CO_DL_", ""));

//                // 10 rows auto-increment IDs with insert
//                for (int i = 1; i <= 10; i++)
//                {
//                    string nextId = "CO_DL_" + currentNumber.ToString("D8");
//                    currentNumber++;

//                    var entity = new RmgCostingDetailsTemp
//                    {
//                        CostingDetailsId = nextId,
//                        CostingId = costingId,
//                        Slno = i.ToString(),
//                        ItemId = "",
//                        Description = "",
//                        Width = "",
//                        ColorId = "",
//                        SupplierId = "",
//                        PoNo = "",
//                        Quantity = 0,
//                        Consumption = 0,
//                        Extra = 0,
//                        TotalQuantity = 0,
//                        TotalQuantityUnit = "",
//                        UnitPrice = 0,
//                        TotalPriceCurrencyId = "",
//                        TotalAmountShhkg = 0,
//                        TotalAmountBdt = 0,
//                        TotalAmountThb = 0,
//                        ResponsibleBy = "",
//                        Luser = username ?? "",
//                        TotalPrice = 0,
//                        BookingItemTypeId = ""
//                    };


//                    await rmgCostingDetailsTempRepo.AddAsync(entity);
//                }
//            }

//            var tempDetails = await rmgCostingDetailsTempRepo.All()
//                .Where(x => x.CostingId == costingId)
//                .OrderBy(x => x.Id)
//                .ToListAsync();

//            var dtoList = tempDetails.Select(detail => new RmgCostingDetailsTempDto
//            {
//                Id = detail.Id,
//                CostingDetailsId = detail.CostingDetailsId ?? "",
//                CostingId = detail.CostingId ?? "",
//                Slno = detail.Slno ?? "",
//                ItemId = detail.ItemId ?? "",
//                Description = detail.Description ?? "",
//                Width = detail.Width ?? "",
//                ColorId = detail.ColorId ?? "",
//                SupplierId = detail.SupplierId ?? "",
//                PoNo = detail.PoNo ?? "",
//                Quantity = detail.Quantity ?? 0,
//                Consumption = detail.Consumption ?? 0,
//                Extra = detail.Extra ?? 0,
//                TotalQuantity = detail.TotalQuantity ?? 0,
//                TotalQuantityUnit = detail.TotalQuantityUnit ?? "",
//                UnitPrice = detail.UnitPrice ?? 0,
//                TotalPriceCurrencyId = detail.TotalPriceCurrencyId ?? "",
//                TotalAmountShhkg = detail.TotalAmountShhkg ?? 0,
//                TotalAmountBdt = detail.TotalAmountBdt ?? 0,
//                TotalAmountThb = detail.TotalAmountThb ?? 0,
//                ResponsibleBy = detail.ResponsibleBy ?? ""
//            }).ToList();


//            return dtoList;
//        }




//        public async Task<RmgCostingDetailsTempDto> GetByIdAsync(string id)
//        {
//            var detail = await rmgCostingDetailsTempRepo.All().Where(x => x.CostingDetailsId == id).FirstOrDefaultAsync();
//            if (detail == null) return null;

//            return new RmgCostingDetailsTempDto
//            {
//                Id = detail.Id,
//                CostingId = detail.CostingId,
//                ItemId = detail.ItemId,
//                Description = detail.Description,
//                Width = detail.Width,
//                ColorId = detail.ColorId,
//                SupplierId = detail.SupplierId,
//                PoNo = detail.PoNo,
//                Quantity = detail.Quantity,
//                Consumption = detail.Consumption,
//                Extra = detail.Extra,
//                TotalQuantity = detail.TotalQuantity,
//                TotalQuantityUnit = detail.TotalQuantityUnit,
//                UnitPrice = detail.UnitPrice,
//                TotalPriceCurrencyId = detail.TotalPriceCurrencyId,
//                TotalAmountShhkg = detail.TotalAmountShhkg,
//                TotalAmountBdt = detail.TotalAmountBdt,
//                TotalAmountThb = detail.TotalAmountThb,
//                ResponsibleBy = detail.ResponsibleBy
//            };
//        }

//        public async Task<RmgCostingDetailsTempDto> AddAsync(RmgCostingDetailsTempDto dto)
//        {
//            try
//            {
//                var entity = new RmgCostingDetailsTemp
//                {
//                    CostingDetailsId = dto.CostingDetailsId,
//                    CostingId = dto.CostingId,
//                    Slno = dto.Slno ?? "1",
//                    BookingItemTypeId = dto.BookingItemTypeId ?? "",
//                    ItemId = dto.ItemId ?? "",
//                    Description = dto.Description ?? "",
//                    Width = dto.Width ?? "",
//                    ColorId = dto.ColorId ?? "",
//                    SupplierId = dto.SupplierId ?? "",
//                    PoNo = dto.PoNo ?? "",
//                    Quantity = dto.Quantity ?? 0,
//                    Consumption = dto.Consumption ?? 0,
//                    Extra = dto.Extra ?? 0,
//                    TotalQuantityUnit = dto.TotalQuantityUnit ?? "",
//                    UnitPrice = dto.UnitPrice ?? 0,
//                    TotalPriceCurrencyId = dto.TotalPriceCurrencyId ?? "",
//                    ResponsibleBy = dto.ResponsibleBy ?? "",
//                    Luser = "System"
//                };

//                CalculateRowTotals(entity);
//                await rmgCostingDetailsTempRepo.AddAsync(entity);

//                dto.Id = entity.Id;
//                dto.TotalQuantity = entity.TotalQuantity;
//                dto.TotalPrice = entity.TotalPrice;
//                dto.TotalAmountShhkg = entity.TotalAmountShhkg;
//                dto.TotalAmountBdt = entity.TotalAmountBdt;
//                dto.TotalAmountThb = entity.TotalAmountThb;

//                return dto;
//            }
//            catch (Exception ex)
//            {
//                var innerMsg = ex.InnerException?.Message ?? ex.Message;
//                throw new Exception($"Save error: {innerMsg}", ex);
//            }
//        }

//        public async Task<RmgCostingDetailsTempDto> UpdateAsync(RmgCostingDetailsTempDto dto)
//        {
//            var entity = await rmgCostingDetailsTempRepo.All().Where(x => x.Id.ToString() == dto.Id.ToString()).FirstOrDefaultAsync();
//            if (entity == null) throw new Exception("Record not found");

//            var itemTypeId = itemRepo.All().Where(x => x.ItemId == dto.ItemId).Select(s => s.ItemTypeId).FirstOrDefault();
//            entity.ItemId = dto.ItemId;
//            entity.BookingItemTypeId = itemTypeId ?? "";
//            entity.Description = dto.Description;
//            entity.Width = dto.Width;
//            entity.ColorId = dto.ColorId;
//            entity.SupplierId = dto.SupplierId;
//            entity.PoNo = dto.PoNo;
//            entity.Quantity = dto.Quantity;
//            entity.Consumption = dto.Consumption;
//            entity.Extra = dto.Extra;
//            entity.TotalQuantityUnit = dto.TotalQuantityUnit;
//            entity.UnitPrice = dto.UnitPrice;
//            entity.TotalPriceCurrencyId = dto.TotalPriceCurrencyId;
//            entity.ResponsibleBy = dto.ResponsibleBy;
//            entity.TotalAmountShhkg = dto.TotalAmountShhkg;
//            entity.TotalAmountBdt = dto.TotalAmountBdt;
//            entity.TotalAmountThb = dto.TotalAmountThb;
//            //CalculateRowTotals(entity);
//            await rmgCostingDetailsTempRepo.UpdateAsync(entity);

//            dto.TotalQuantity = entity.TotalQuantity;
//            dto.TotalPrice = entity.TotalPrice;
//            dto.TotalAmountShhkg = entity.TotalAmountShhkg;
//            dto.TotalAmountBdt = entity.TotalAmountBdt;
//            dto.TotalAmountThb = entity.TotalAmountThb;

//            return dto;
//        }

//        public async Task<bool> DeleteAsync(string id)
//        {
//            var entity = await rmgCostingDetailsTempRepo.All().Where(x => x.Id.ToString() == id).FirstOrDefaultAsync();
//            if (entity == null) return false;

//            await rmgCostingDetailsTempRepo.DeleteAsync(entity);
//            return true;
//        }

//        public async Task<bool> DeleteByCostingIdAsync(string costingId)
//        {
//            var entities = await rmgCostingDetailsTempRepo.All()
//                .Where(x => x.CostingId == costingId)
//                .ToListAsync();
//            await rmgCostingDetailsTempRepo.DeleteRangeAsync(entities);

//            return true;
//        }
//        public async Task<RmgCostingSummaryDto> CalculateSummaryAsync(string costingId, decimal damagePercent, decimal interestPercent, decimal cmAndProfit, decimal handlingCharge, decimal productionUpchargePercent)
//        {
//            var details = await rmgCostingDetailsTempRepo.All()
//                .Where(x => x.CostingId == costingId)
//                .ToListAsync();

//            var summary = new RmgCostingSummaryDto();

//            //  amount calculate
//            var totalShhkg = details.Sum(x => x.TotalAmountShhkg ?? 0);
//            var totalBdt = details.Sum(x => x.TotalAmountBdt ?? 0);
//            var totalThb = details.Sum(x => x.TotalAmountThb ?? 0);

//            summary.SubTotalShhkg = totalShhkg;
//            summary.SubTotalBdt = totalBdt;
//            summary.SubTotalThb = totalThb;
//            summary.SubTotal = totalShhkg + totalBdt + totalThb;

//            // Total Gar Qty ( last row  quantity)
//            var totalGarQty = details
//                .OrderByDescending(x => x.CostingDetailsId)
//                .Select(s => s.Quantity)
//                .FirstOrDefault() ?? 0;

//            // Sub Total (per Gar. Qty) - 
//            if (totalGarQty > 0)
//            {
//                summary.SubTotalPerGarQtyShhkg = totalShhkg / totalGarQty;
//                summary.SubTotalPerGarQtyBdt = totalBdt / totalGarQty;
//                summary.SubTotalPerGarQtyThb = totalThb / totalGarQty;
//            }

//            // Damage % - 
//            summary.DamagePercent = damagePercent;
//            summary.DamageAmountShhkg = summary.SubTotalPerGarQtyShhkg * (damagePercent / 100);
//            summary.DamageAmountBdt = summary.SubTotalPerGarQtyBdt * (damagePercent / 100);
//            summary.DamageAmountThb = summary.SubTotalPerGarQtyThb * (damagePercent / 100);

//            // Interest/Overhead % - 
//            summary.InterestOverheadPercent = interestPercent;
//            summary.InterestOverheadAmountShhkg = summary.SubTotalPerGarQtyShhkg * (interestPercent / 100);
//            summary.InterestOverheadAmountBdt = summary.SubTotalPerGarQtyBdt * (interestPercent / 100);
//            summary.InterestOverheadAmountThb = summary.SubTotalPerGarQtyThb * (interestPercent / 100);

//            // Total (Sub Total + Damage + Interest)
//            summary.TotalShhkg = summary.SubTotalPerGarQtyShhkg + summary.DamageAmountShhkg + summary.InterestOverheadAmountShhkg;
//            summary.TotalBdt = summary.SubTotalPerGarQtyBdt + summary.DamageAmountBdt + summary.InterestOverheadAmountBdt;
//            summary.TotalThb = summary.SubTotalPerGarQtyThb + summary.DamageAmountThb + summary.InterestOverheadAmountThb;

//            // Material Cost
//            summary.TotalMaterialCostOverseas = summary.TotalShhkg;
//            summary.TotalMaterialCostBangladesh = summary.TotalBdt;
//            //summary.TotalMaterialCostBkk = summary.TotalThb * 1.2m; // +20%
//            summary.TotalMaterialCostBkk = summary.TotalThb;

//            // User inputs
//            summary.CmAndProfit = cmAndProfit;
//            summary.HandlingCharge = handlingCharge;
//            summary.ProductionUpchargePercent = productionUpchargePercent;
//            //summary.ProductionUpcharge = (summary.TotalMaterialCostOverseas + summary.TotalMaterialCostBangladesh + summary.TotalMaterialCostBkk) * (productionUpchargePercent / 100);
//            summary.ProductionUpcharge = productionUpchargePercent;

//            // FF Price
//            summary.FfPrice = summary.TotalMaterialCostOverseas +
//                              summary.TotalMaterialCostBangladesh +
//                              summary.TotalMaterialCostBkk +
//                              summary.CmAndProfit +
//                              summary.HandlingCharge +
//                              summary.ProductionUpcharge;

//            // Grand Total
//            summary.GrandTotal = summary.FfPrice * totalGarQty;

//            return summary;
//        }

//        private void CalculateRowTotals(RmgCostingDetailsTemp entity)
//        {
//            if (entity.Quantity.HasValue && entity.Consumption.HasValue)
//            {
//                var baseTotal = entity.Quantity.Value * entity.Consumption.Value;
//                var extraPercent = entity.Extra ?? 0;
//                entity.TotalQuantity = baseTotal * (1 + extraPercent / 100);
//            }

//            if (entity.TotalQuantity.HasValue && entity.UnitPrice.HasValue)
//            {
//                entity.TotalPrice = entity.TotalQuantity.Value * entity.UnitPrice.Value;
//            }

//            if (entity.TotalPrice.HasValue && !string.IsNullOrEmpty(entity.TotalPriceCurrencyId))
//            {
//                entity.TotalAmountShhkg = 0;
//                entity.TotalAmountBdt = 0;
//                entity.TotalAmountThb = 0;

//                switch (entity.TotalPriceCurrencyId.ToUpper())
//                {
//                    case "USD":
//                    case "HKD":
//                        entity.TotalAmountShhkg = entity.TotalPrice;
//                        break;
//                    case "BDT":
//                        entity.TotalAmountBdt = entity.TotalPrice;
//                        break;
//                    case "THB":
//                        entity.TotalAmountThb = entity.TotalPrice;
//                        break;
//                }
//            }
//        }

//        // ========== EXCEL PREVIEW WITH EPPLUS ==========
//        public async Task<List<RmgCostingDetailsTempDto>> PreviewExcelAsync(IFormFile file)
//        {
//            if (file == null || file.Length == 0)
//                throw new Exception("File is empty");

//            var ext = Path.GetExtension(file.FileName).ToLower();
//            if (ext != ".xlsx" && ext != ".xls")
//                throw new Exception("Only Excel files (.xlsx, .xls) are supported");

//            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

//            var dtoList = new List<RmgCostingDetailsTempDto>();

//            using (var stream = new MemoryStream())
//            {
//                await file.CopyToAsync(stream);
//                stream.Position = 0;

//                using (var package = new ExcelPackage(stream))
//                {
//                    var worksheet = package.Workbook.Worksheets.FirstOrDefault();
//                    if (worksheet == null)
//                        throw new Exception("Worksheet not found in Excel file");

//                    int rowCount = worksheet.Dimension?.End.Row ?? 0;
//                    if (rowCount < 2)
//                        throw new Exception("Excel file has no data rows");

//                    for (int row = 2; row <= rowCount; row++)
//                    {
//                        var Slno = worksheet.Cells[row, 1].Text?.Trim();
//                        if (string.IsNullOrWhiteSpace(Slno)) continue;

//                        var dto = new RmgCostingDetailsTempDto
//                        {
//                            Slno = Slno,
//                            ItemName = worksheet.Cells[row, 2].Text?.Trim(),
//                            Description = worksheet.Cells[row, 3].Text?.Trim(),
//                            Width = worksheet.Cells[row, 4].Text?.Trim(),
//                            ColorName = worksheet.Cells[row, 5].Text?.Trim(),
//                            SupplierName = worksheet.Cells[row, 6].Text?.Trim(),
//                            PoNo = worksheet.Cells[row, 7].Text?.Trim(),
//                            Quantity = ParseDecimal(worksheet.Cells[row, 8].Value),
//                            Consumption = ParseDecimal(worksheet.Cells[row, 9].Value),
//                            TotalQuantity = ParseDecimal(worksheet.Cells[row, 10].Value),
//                            UnitName = worksheet.Cells[row, 11].Text?.Trim(),
//                            UnitPrice = ParseDecimal(worksheet.Cells[row, 12].Value),
//                            ResponsibleByName = worksheet.Cells[row, 13].Text?.Trim()
//                        };

//                        dtoList.Add(dto);
//                    }
//                }
//            }

//            return dtoList;
//        }

//        private decimal ParseDecimal(object value)
//        {
//            if (value == null)
//                return 0;

//            var str = value.ToString()?.Trim();

//            if (string.IsNullOrEmpty(str))
//                return 0;

//            decimal.TryParse(str, out decimal result);
//            return result;
//        }

//        private void CalculateExcelRowTotals(RmgCostingDetailsTemp entity)
//        {
//            entity.TotalQuantity =
//                (entity.Quantity * entity.Consumption) * 1.0m;

//            entity.TotalPrice =
//                entity.TotalQuantity * entity.UnitPrice;

//            entity.TotalAmountShhkg = 0;
//            entity.TotalAmountBdt = 0;
//            entity.TotalAmountThb = 0;

//            switch (entity.ResponsibleBy?.Trim().ToUpper())
//            {
//                case "BKK":
//                    entity.TotalAmountShhkg = entity.TotalPrice;
//                    entity.TotalPriceCurrencyId = "002";
//                    break;

//                case "FF":
//                    entity.TotalAmountBdt = entity.TotalPrice;
//                    entity.TotalPriceCurrencyId = "001";
//                    break;

//                case "THB":
//                    entity.TotalAmountThb = entity.TotalPrice;
//                    entity.TotalPriceCurrencyId = "003";
//                    break;

//                default:
//                    entity.TotalAmountShhkg = entity.TotalPrice;
//                    entity.TotalPriceCurrencyId = "002";
//                    break;
//            }
//        }

//        public async Task<bool> ImportExcelAsync(IFormFile file, string costingId, string username)
//        {
//            if (file == null || file.Length == 0)
//                throw new Exception("File is empty");

//            var ext = Path.GetExtension(file.FileName);
//            if (!ext.Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
//                throw new Exception("Only .xlsx files are supported");

//            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

//            var importList = new List<RmgCostingDetailsTemp>();

//            using var stream = new MemoryStream();
//            await file.CopyToAsync(stream);
//            stream.Position = 0;

//            using var package = new ExcelPackage(stream);
//            var sheet = package.Workbook.Worksheets.FirstOrDefault();
//            if (sheet == null)
//                throw new Exception("Worksheet not found");

//            int rowCount = sheet.Dimension.End.Row;
//            int colCount = sheet.Dimension.End.Column;

//            // ===============================
//            // 1️⃣ HEADER MAP (CASE-INSENSITIVE)
//            // ===============================
//            var headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

//            for (int col = 1; col <= colCount; col++)
//            {
//                var header = sheet.Cells[1, col].Text?.Trim();
//                if (!string.IsNullOrEmpty(header) && !headerMap.ContainsKey(header))
//                    headerMap.Add(header, col);
//            }

//            int Col(string name)
//            {
//                var key = headerMap.Keys.FirstOrDefault(x =>
//                    x.Replace(" ", "").Equals(name.Replace(" ", ""), StringComparison.OrdinalIgnoreCase));

//                if (key == null)
//                    throw new Exception($"Required column '{name}' not found");

//                return headerMap[key];
//            }

//            // ===============================
//            // 2️⃣ LOOKUP DICTIONARIES (IGNORE CASE)
//            // ===============================
//            var items = (await itemRepo.All().ToListAsync())
//                .Where(x => !string.IsNullOrWhiteSpace(x.ItemName))
//                .GroupBy(x => x.ItemName.Trim(), StringComparer.OrdinalIgnoreCase)
//                .ToDictionary(g => g.Key, g => g.First().ItemId, StringComparer.OrdinalIgnoreCase);

//            var colors = (await colorRepo.All().ToListAsync())
//                .Where(x => !string.IsNullOrWhiteSpace(x.Color))
//                .GroupBy(x => x.Color.Trim(), StringComparer.OrdinalIgnoreCase)
//                .ToDictionary(g => g.Key, g => g.First().ColorId, StringComparer.OrdinalIgnoreCase);

//            var suppliers = (await supplieRepo.All().ToListAsync())
//                .Where(x => !string.IsNullOrWhiteSpace(x.SupplierName))
//                .GroupBy(x => x.SupplierName.Trim(), StringComparer.OrdinalIgnoreCase)
//                .ToDictionary(g => g.Key, g => g.First().SupplierId, StringComparer.OrdinalIgnoreCase);

//            var units = (await unitTypeRepo.All().ToListAsync())
//                .Where(x => !string.IsNullOrWhiteSpace(x.UnitTypeName))
//                .GroupBy(x => x.UnitTypeName.Trim(), StringComparer.OrdinalIgnoreCase)
//                .ToDictionary(g => g.Key, g => g.First().UnitTypId, StringComparer.OrdinalIgnoreCase);

//            // ===============================
//            // 3️⃣ ID GENERATION
//            // ===============================
//            var firstCode = commonService.GenerateNextCode(
//                "CostingDetailsId", "RMG_CostingDetails", 8, "CO_DL_");

//            int currentNo = int.Parse(firstCode.Replace("CO_DL_", ""));
//            int sl = 1;

//            // ===============================
//            // 4️⃣ ROW LOOP
//            // ===============================
//            for (int row = 2; row <= rowCount; row++)
//            {
//                var material = sheet.Cells[row, Col("MATERIAL")].Text?.Trim();
//                if (string.IsNullOrWhiteSpace(material)) continue;

//                items.TryGetValue(material, out var itemId);

//                var colorName = sheet.Cells[row, Col("Color")].Text?.Trim();
//                colors.TryGetValue(colorName ?? "", out var colorId);

//                var supplierName = sheet.Cells[row, Col("SUPPLIER")].Text?.Trim();
//                suppliers.TryGetValue(supplierName ?? "", out var supplierId);

//                var unitName = sheet.Cells[row, Col("UNIT")].Text?.Trim();
//                units.TryGetValue(unitName ?? "", out var unitId);

//                var responsible = sheet.Cells[row, Col("RESPONSIBLE By")].Text?.Trim();

//                // Get BookingItemTypeId
//                var bookingTypeId = itemRepo.All().Where(x => x.ItemId == itemId).Select(x => x.ItemTypeId).FirstOrDefault();

//                var entity = new RmgCostingDetailsTemp
//                {
//                    CostingDetailsId = $"CO_DL_{currentNo:D8}",
//                    CostingId = costingId,
//                    Slno = sl.ToString(),
//                    ItemId = itemId ?? "",
//                    BookingItemTypeId = bookingTypeId ?? "",
//                    Description = sheet.Cells[row, Col("DESCRIPTION")].Text?.Trim(),
//                    Width = sheet.Cells[row, Col("Width")].Text?.Trim(),
//                    ColorId = colorId ?? "",
//                    SupplierId = supplierId ?? "",
//                    PoNo = sheet.Cells[row, Col("P/O No.")].Text?.Trim(),
//                    Quantity = ParseDecimal(sheet.Cells[row, Col("ORD QTY")].Value),
//                    Consumption = ParseDecimal(sheet.Cells[row, Col("QTY/GMT")].Value),
//                    Extra = 0,
//                    TotalQuantityUnit = unitId ?? "",
//                    UnitPrice = ParseDecimal(sheet.Cells[row, Col("Unit Price")].Value),
//                    ResponsibleBy = responsible,
//                    Luser = username
//                };

//                CalculateExcelRowTotals(entity);
//                importList.Add(entity);

//                sl++;
//                currentNo++;
//            }

//            if (!importList.Any())
//                throw new Exception("No valid data found");

//            // ===============================
//            // 5️⃣ DELETE + BULK INSERT
//            // ===============================
//            await rmgCostingDetailsTempRepo.DeleteRangeAsync(
//                await rmgCostingDetailsTempRepo.All()
//                .Where(x => x.CostingId == costingId)
//                .ToListAsync());

//            await BulkInsertAsync(importList);
//            return true;
//        }

//        private async Task BulkInsertAsync(List<RmgCostingDetailsTemp> list)
//        {
//            using var con = new SqlConnection(_connectionString);
//            await con.OpenAsync();

//            using var bulk = new SqlBulkCopy(con);
//            bulk.DestinationTableName = "RMG_CostingDetailsTemp";
//            bulk.BatchSize = 1000;
//            bulk.BulkCopyTimeout = 300;

//            // ✅ COLUMN MAPPINGS - MATCHING COMMENTED CODE
//            bulk.ColumnMappings.Add("CostingDetailsID", "CostingDetailsID");
//            bulk.ColumnMappings.Add("CostingID", "CostingID");
//            bulk.ColumnMappings.Add("SLNO", "SLNO");
//            bulk.ColumnMappings.Add("BookingItemTypeID", "BookingItemTypeID");
//            bulk.ColumnMappings.Add("ItemID", "ItemID");
//            bulk.ColumnMappings.Add("Description", "Description");
//            bulk.ColumnMappings.Add("Width", "Width");
//            bulk.ColumnMappings.Add("ColorID", "ColorID");
//            bulk.ColumnMappings.Add("SupplierID", "SupplierID");
//            bulk.ColumnMappings.Add("PoNo", "PoNo");
//            bulk.ColumnMappings.Add("Quantity", "Quantity");
//            bulk.ColumnMappings.Add("Consumption", "Consumption");
//            bulk.ColumnMappings.Add("Extra", "Extra");
//            bulk.ColumnMappings.Add("TotalQuantity", "TotalQuantity");
//            bulk.ColumnMappings.Add("TotalQuantityUnit", "TotalQuantityUnit");
//            bulk.ColumnMappings.Add("UnitPrice", "UnitPrice");
//            bulk.ColumnMappings.Add("TotalPrice", "TotalPrice");
//            bulk.ColumnMappings.Add("TotalPriceCurrencyId", "TotalPriceCurrencyId");
//            bulk.ColumnMappings.Add("TotalAmountSHHKG", "TotalAmountSHHKG");
//            bulk.ColumnMappings.Add("TotalAmountBDT", "TotalAmountBDT");
//            bulk.ColumnMappings.Add("TotalAmountTHB", "TotalAmountTHB");
//            bulk.ColumnMappings.Add("ResponsibleBy", "ResponsibleBy");
//            bulk.ColumnMappings.Add("LUser", "LUser");

//            var table = new DataTable();

//            // ✅ DATATABLE COLUMNS - MATCHING COMMENTED CODE
//            table.Columns.Add("CostingDetailsID", typeof(string));
//            table.Columns.Add("CostingID", typeof(string));
//            table.Columns.Add("SLNO", typeof(string));
//            table.Columns.Add("BookingItemTypeID", typeof(string));
//            table.Columns.Add("ItemID", typeof(string));
//            table.Columns.Add("Description", typeof(string));
//            table.Columns.Add("Width", typeof(string));
//            table.Columns.Add("ColorID", typeof(string));
//            table.Columns.Add("SupplierID", typeof(string));
//            table.Columns.Add("PoNo", typeof(string));
//            table.Columns.Add("Quantity", typeof(decimal));
//            table.Columns.Add("Consumption", typeof(decimal));
//            table.Columns.Add("Extra", typeof(decimal));
//            table.Columns.Add("TotalQuantity", typeof(decimal));
//            table.Columns.Add("TotalQuantityUnit", typeof(string));
//            table.Columns.Add("UnitPrice", typeof(decimal));
//            table.Columns.Add("TotalPrice", typeof(decimal));
//            table.Columns.Add("TotalPriceCurrencyId", typeof(string));
//            table.Columns.Add("TotalAmountSHHKG", typeof(decimal));
//            table.Columns.Add("TotalAmountBDT", typeof(decimal));
//            table.Columns.Add("TotalAmountTHB", typeof(decimal));
//            table.Columns.Add("ResponsibleBy", typeof(string));
//            table.Columns.Add("LUser", typeof(string));

//            foreach (var x in list)
//            {
//                var row = table.NewRow();

//                row["CostingDetailsID"] = x.CostingDetailsId ?? "";
//                row["CostingID"] = x.CostingId ?? "";
//                row["SLNO"] = x.Slno ?? "";
//                row["BookingItemTypeID"] = x.BookingItemTypeId ?? "";
//                row["ItemID"] = x.ItemId ?? "";
//                row["Description"] = x.Description ?? "";
//                row["Width"] = x.Width ?? "";
//                row["ColorID"] = x.ColorId ?? "";
//                row["SupplierID"] = x.SupplierId ?? "";
//                row["PoNo"] = x.PoNo ?? "";

//                // ✅ DECIMAL VALUES
//                row["Quantity"] = x.Quantity;
//                row["Consumption"] = x.Consumption;
//                row["Extra"] = x.Extra;
//                row["TotalQuantity"] = x.TotalQuantity;
//                row["UnitPrice"] = x.UnitPrice;
//                row["TotalPrice"] = x.TotalPrice;
//                row["TotalAmountSHHKG"] = x.TotalAmountShhkg;
//                row["TotalAmountBDT"] = x.TotalAmountBdt;
//                row["TotalAmountTHB"] = x.TotalAmountThb;

//                row["TotalQuantityUnit"] = x.TotalQuantityUnit ?? "";
//                row["TotalPriceCurrencyId"] = x.TotalPriceCurrencyId ?? "";
//                row["ResponsibleBy"] = x.ResponsibleBy ?? "";
//                row["LUser"] = x.Luser ?? "";

//                table.Rows.Add(row);
//            }

//            await bulk.WriteToServerAsync(table);
//        }

//        public async Task<(bool isSuccess, string message, object data)> CreateUpdateAsync(RmgCostingInfoDto model, string companyCode)
//        {
//            try
//            {
//                RmgCostingInfo entity;
//                string mess = "";

//                model.EntryDate = model.EntryDate == default ? DateTime.Now : model.EntryDate;
//                model.ShipmentDate = model.ShipmentDate == default ? DateTime.Now : model.ShipmentDate;

//                // ===========================
//                // ADD
//                // ===========================
//                if (model.AutoId == 0)
//                {
//                    bool isExist = costingInfoRepo.All().Any(x =>
//                        x.IntegraJobNo == model.IntegraJobNo &&
//                        x.StyleId == model.StyleId &&
//                        x.PoNo == model.PoNo);

//                    if (isExist)
//                        return (false, DataExists, null);

//                    entity = new RmgCostingInfo
//                    {
//                        CostingId = model.CostingId ?? Guid.NewGuid().ToString(),
//                        EntryDate = model.EntryDate,
//                        BuyerId = model.BuyerId ?? "",
//                        StyleId = model.StyleId ?? "",
//                        MasterPurchaseOrder = model.MasterPurchaseOrder ?? "",
//                        PoNo = model.PoNo ?? "",
//                        IntegraJobNo = model.IntegraJobNo ?? "",
//                        ExportLcnoSc = model.ExportLcnoSc ?? "",
//                        ShipmentDate = model.ShipmentDate,
//                        FactorySuplier = model.FactorySuplier ?? "",
//                        IssuedBy = model.IssuedBy ?? "",
//                        CheckedBy = model.CheckedBy ?? "",
//                        SubTotalAmountShhkg = model.SubTotalAmountShhkg,
//                        SubTotalAmountBdt = model.SubTotalAmountBdt,
//                        SubTotalAmountThb = model.SubTotalAmountThb,
//                        DamagePercentage = model.DamagePercentage,
//                        DamageAmountShhkg = model.DamageAmountShhkg,
//                        DamageAmountBdt = model.DamageAmountBdt,
//                        DamageAmountThb = model.DamageAmountThb,
//                        InterestOverheadPercentage = model.InterestOverheadPercentage,
//                        InterestOverheadShhkg = model.InterestOverheadShhkg,
//                        InterestOverheadBdt = model.InterestOverheadBdt,
//                        InterestOverheadThb = model.InterestOverheadThb,
//                        TotalAmountShhkg = model.TotalAmountShhkg,
//                        TotalAmountBdt = model.TotalAmountBdt,
//                        TotalAmountThb = model.TotalAmountThb,
//                        TotalMaterialCostOverseas = model.TotalMaterialCostOverseas,
//                        TotalMaterialCostBdt = model.TotalMaterialCostBdt,
//                        TotalMaterialCostBkk = model.TotalMaterialCostBkk,
//                        CmandProfit = model.CmandProfit,
//                        HandlingCharge = model.HandlingCharge,
//                        ProductionUpCharge = model.ProductionUpCharge,
//                        GrandTotal = model.GrandTotal,
//                        Ffprice = model.Ffprice,
//                        SubTotalByPerPcsShhkg = model.SubTotalByPerPcsShhkg ?? 0,
//                        SubTotalByPerPcsBdt = model.SubTotalByPerPcsBdt ?? 0,
//                        SubTotalByPerPcsThb = model.SubTotalByPerPcsThb ?? 0,
//                        HandlingChargePerUnit = model.HandlingChargePerUnit ?? 0,
//                        CmprofitUperUnit = model.CmprofitUperUnit ?? 0,
//                        CompanyCode = companyCode,
//                        EmployeId = model.UserInfoEmployeeId ?? "",
//                        Luser = model.Luser ?? "",
//                        Ldate = DateTime.Now,
//                        Lip = model.Lip ?? "",
//                        Lmac = model.Lmac ?? ""
//                    };

//                    await costingInfoRepo.AddAsync(entity);
//                    mess = CreateSuccess;
//                }
//                else
//                {
//                    // ===========================
//                    // UPDATE
//                    // ===========================
//                    entity = await costingInfoRepo.GetByIdAsync(model.AutoId);
//                    if (entity == null)
//                        return (false, "Record not found", null);

//                    entity.ModifyDate = DateTime.Now;
//                    entity.BuyerId = model.BuyerId ?? "";
//                    entity.StyleId = model.StyleId ?? "";
//                    entity.MasterPurchaseOrder = model.MasterPurchaseOrder ?? "";
//                    entity.PoNo = model.PoNo ?? "";
//                    entity.IntegraJobNo = model.IntegraJobNo ?? "";
//                    entity.ExportLcnoSc = model.ExportLcnoSc ?? "";
//                    entity.ShipmentDate = model.ShipmentDate;
//                    entity.FactorySuplier = model.FactorySuplier ?? "";
//                    entity.IssuedBy = model.IssuedBy ?? "";
//                    entity.CheckedBy = model.CheckedBy ?? "";
//                    entity.SubTotalAmountShhkg = model.SubTotalAmountShhkg;
//                    entity.SubTotalAmountBdt = model.SubTotalAmountBdt;
//                    entity.SubTotalAmountThb = model.SubTotalAmountThb;
//                    entity.DamagePercentage = model.DamagePercentage;
//                    entity.DamageAmountShhkg = model.DamageAmountShhkg;
//                    entity.DamageAmountBdt = model.DamageAmountBdt;
//                    entity.DamageAmountThb = model.DamageAmountThb;
//                    entity.InterestOverheadPercentage = model.InterestOverheadPercentage;
//                    entity.InterestOverheadShhkg = model.InterestOverheadShhkg;
//                    entity.InterestOverheadBdt = model.InterestOverheadBdt;
//                    entity.InterestOverheadThb = model.InterestOverheadThb;
//                    entity.TotalAmountShhkg = model.TotalAmountShhkg;
//                    entity.TotalAmountBdt = model.TotalAmountBdt;
//                    entity.TotalAmountThb = model.TotalAmountThb;
//                    entity.TotalMaterialCostOverseas = model.TotalMaterialCostOverseas;
//                    entity.TotalMaterialCostBdt = model.TotalMaterialCostBdt;
//                    entity.TotalMaterialCostBkk = model.TotalMaterialCostBkk;
//                    entity.CmandProfit = model.CmandProfit;
//                    entity.HandlingCharge = model.HandlingCharge;
//                    entity.ProductionUpCharge = model.ProductionUpCharge;
//                    entity.GrandTotal = model.GrandTotal;
//                    entity.Ffprice = model.Ffprice;

//                    await costingInfoRepo.UpdateAsync(entity);
//                    mess = UpdateSuccess;

//                    var oldDetails = await rmgCostingDetailsRepo.All()
//                        .Where(x => x.CostingId == entity.CostingId)
//                        .ToListAsync();
//                    if (oldDetails.Any())
//                        await rmgCostingDetailsRepo.DeleteRangeAsync(oldDetails);

//                    var oldBookings = await boRepo.All()
//                        .Where(x => x.IntegraJobNo == entity.IntegraJobNo && x.PoNo == entity.PoNo)
//                        .ToListAsync();
//                    if (oldBookings.Any())
//                        await boRepo.DeleteRangeAsync(oldBookings);

//                    var oldExtraDetails = await extraRepo.All()
//                        .Where(x => x.IntegraJobNo == entity.IntegraJobNo && x.PoNo == entity.PoNo)
//                        .ToListAsync();
//                    if (oldExtraDetails.Any())
//                        await extraRepo.DeleteRangeAsync(oldExtraDetails);

//                    var oldButtonDetails = await buttonRepo.All()
//                        .Where(x => x.IntegraJobNo == entity.IntegraJobNo && x.PoNo == entity.PoNo)
//                        .ToListAsync();
//                    if (oldButtonDetails.Any())
//                        await buttonRepo.DeleteRangeAsync(oldButtonDetails);

//                    var oldCartonDetails = await cartonRepo.All()
//                        .Where(x => x.IntegraJobNo == entity.IntegraJobNo && x.PoNo == entity.PoNo)
//                        .ToListAsync();
//                    if (oldCartonDetails.Any())
//                        await cartonRepo.DeleteRangeAsync(oldCartonDetails);

//                    var oldFebricDetails = await febricRepo.All()
//                        .Where(x => x.IntegraJobNo == entity.IntegraJobNo && x.PoNo == entity.PoNo)
//                        .ToListAsync();
//                    if (oldFebricDetails.Any())
//                        await febricRepo.DeleteRangeAsync(oldFebricDetails);

//                    var oldPolyDetails = await polyRepo.All()
//                        .Where(x => x.IntegraJobNo == entity.IntegraJobNo && x.PoNo == entity.PoNo)
//                        .ToListAsync();
//                    if (oldPolyDetails.Any())
//                        await polyRepo.DeleteRangeAsync(oldPolyDetails);

//                    var oldThredDetails = await threadRepo.All()
//                        .Where(x => x.IntegraJobNo == entity.IntegraJobNo && x.PoNo == entity.PoNo)
//                        .ToListAsync();
//                    if (oldThredDetails.Any())
//                        await threadRepo.DeleteRangeAsync(oldThredDetails);
//                }

//                // ===========================
//                // TEMP → MAIN DETAILS
//                // ===========================
//                var tempData = await rmgCostingDetailsTempRepo.All()
//                    .Where(x => x.CostingId == entity.CostingId)
//                    .ToListAsync();

//                var details = tempData.Select(t => new RmgCostingDetails
//                {
//                    CostingDetailsId = t.CostingDetailsId ?? "",
//                    CostingId = t.CostingId ?? "",
//                    Slno = t.Slno ?? "",
//                    BookingItemTypeId = t.BookingItemTypeId ?? "",
//                    ItemId = t.ItemId ?? "",
//                    Description = t.Description ?? "",
//                    Width = t.Width ?? "",
//                    ColorId = t.ColorId ?? "",
//                    SupplierId = t.SupplierId ?? "",
//                    PoNo = t.PoNo ?? "",
//                    Quantity = t.Quantity ?? 0,
//                    Consumption = t.Consumption ?? 0,
//                    Extra = t.Extra ?? 0,
//                    TotalQuantity = t.TotalQuantity ?? 0,
//                    TotalQuantityUnit = t.TotalQuantityUnit ?? "",
//                    UnitPrice = t.UnitPrice ?? 0,
//                    TotalPrice = t.TotalPrice ?? 0,
//                    TotalPriceCurrencyId = t.TotalPriceCurrencyId ?? "",
//                    TotalAmountBdt = t.TotalAmountBdt ?? 0,
//                    TotalAmountThb = t.TotalAmountThb ?? 0,
//                    TotalAmountShhkg = t.TotalAmountShhkg ?? 0,
//                    ResponsibleBy = t.ResponsibleBy ?? "",
//                    BookinOrderNo = t.BookingItemTypeId ?? "",
//                    Luser = model.Luser
//                }).ToList();

//                if (details.Any())
//                    await rmgCostingDetailsRepo.AddRangeAsync(details);

//                // ===========================
//                // BOOKING (PO WISE)
//                // ===========================
//                var poWiseGroups = tempData
//                    .Where(x => !string.IsNullOrEmpty(x.PoNo))
//                    .GroupBy(x => x.PoNo)
//                    .ToList();

//                foreach (var group in poWiseGroups)
//                {
//                    var items = group.ToList();
//                    string bookingType = items.First().BookingItemTypeId;
//                    var bookingItems = items.Where(x => x.BookingItemTypeId == bookingType).ToList();
//                    string bookingPoNo = group.Key;
//                    string currentPoNo = model.PoNo;

//                    switch (bookingType)
//                    {
//                        case "04": await CartonBookingData(bookingItems, currentPoNo); break;
//                        case "07": await ThreadBookingData(bookingItems, currentPoNo); break;
//                        case "03": await PolyBookingData(bookingItems, currentPoNo); break;
//                        case "02": await ButtonBookingData(bookingItems, currentPoNo); break;
//                        case "01": await FebricBookingData(bookingItems, currentPoNo); break;
//                        default: await ExtraBookingData(bookingItems, currentPoNo); break;
//                    }

//                    var booking = new RmgBookingOrder
//                    {
//                        BookinOrderNo = bookingPoNo,
//                        BookinDate = model.EntryDate,
//                        BuyerId = model.BuyerId ?? "",
//                        StyleId = model.StyleId ?? "",
//                        MasterPurchaseOrder = model.MasterPurchaseOrder ?? "",
//                        PoNo = model.PoNo ?? "",
//                        IntegraJobNo = model.IntegraJobNo ?? "",
//                        BookingType = bookingType,
//                        SupplierId = items[0].SupplierId ?? "",
//                        EmployeId = model.UserInfoEmployeeId ?? "",
//                        CompanyId = companyCode,
//                        Luser = model.Luser ?? "",
//                        Ldate = DateTime.Now,
//                        Lip = model.Lip ?? "",
//                        Lmac = model.Lmac ?? ""
//                    };

//                    await boRepo.AddAsync(booking);
//                }

//                return (true, mess, entity);
//            }
//            catch (Exception ex)
//            {
//                return (false, ex.Message, null);
//            }
//        }





//        private async Task CartonBookingData(List<RmgCostingDetailsTemp> bookingData, string PoNum)
//        {
//            try
//            {
//                if (!bookingData.Any()) return;

//                var slno = cartonRepo.All().Count() + 1;

//                foreach (var b in bookingData)
//                {
//                    var existing = cartonRepo.All()
//                        .Where(x => x.PoNo == b.PoNo && x.ItemId == b.ItemId)
//                        .ToList();

//                    if (existing.Any())
//                        await cartonRepo.DeleteRangeAsync(existing);

//                    var entity = new RmgInvBookingReceivedDetailsCarton
//                    {
//                        PurchaseReceiveNo = await GenerateAutoCartonBooking(),
//                        ItemId = b.ItemId,
//                        ItemDescription = b.Description?.Length > 250
//                            ? b.Description.Substring(0, 250)
//                            : b.Description,

//                        OrderQty = b.Quantity,
//                        OrderUnitId = b.TotalQuantityUnit,
//                        RequiredQty = b.Quantity,
//                        RequiredQtyUnitId = b.TotalQuantityUnit,
//                        ConsumptionUnitId = b.TotalQuantityUnit,
//                        Consumption = b.Consumption,
//                        UnitPrice = b.UnitPrice,
//                        TotalPrice = b.TotalPrice,
//                        PoNo = PoNum,

//                        IntegraJobNo = costingInfoRepo.All()
//                            .Where(x => x.CostingId == b.CostingId)
//                            .Select(s => s.IntegraJobNo)
//                            .FirstOrDefault(),

//                        Slno = slno++,
//                        ColorId = b.ColorId,
//                        CurrencyId = b.TotalPriceCurrencyId,

//                        CartonPercent = 0m,
//                        TotalReceivedQty = 0m,
//                        CurrentReceiveQty = 0m,
//                        ReceivedUnitPrice = 0m,
//                        TotalReceivedQtyPre = 0m,
//                        PendingReceiveQty = 0m,
//                        PendingReceiveQtyPre = 0m
//                    };

//                    await cartonRepo.AddAsync(entity);
//                }
//            }
//            catch (Exception)
//            {

//                throw;
//            }
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
//                             FROM RMG_Inv_BookingReceivedDetails_Carton 
//                             WHERE PurchaseReceiveNo LIKE @prefix + '%'";
//                using (SqlCommand cmd = new SqlCommand(queryTemp, con))
//                {
//                    cmd.Parameters.AddWithValue("@prefix", prefix);
//                    var result = await cmd.ExecuteScalarAsync();
//                    lastCode = result?.ToString();
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

//        private async Task ThreadBookingData(List<RmgCostingDetailsTemp> bookingData, string PoNO)
//        {
//            try
//            {
//                if (!bookingData.Any()) return;

//                var slno = threadRepo.All().Count() + 1;
//                var prn = await GenerateAutoThreadBooking();

//                foreach (var b in bookingData)
//                {
//                    var existing = threadRepo.All()
//                        .Where(x => x.PoNo == b.PoNo && x.ItemId == b.ItemId)
//                        .ToList();

//                    if (existing.Any())
//                        await threadRepo.DeleteRangeAsync(existing);

//                    var entity = new RmgInvBookingReceivedDetailsThread
//                    {
//                        PurchaseReceiveNo = prn ?? string.Empty,
//                        PoNo = PoNO ?? string.Empty,
//                        IntegraJobNo = costingInfoRepo.All()
//        .Where(x => x.CostingId == b.CostingId)
//        .Select(x => x.IntegraJobNo)
//        .FirstOrDefault() ?? string.Empty,

//                        Slno = slno++,

//                        ItemId = b.ItemId ?? string.Empty,
//                        ColorId = b.ColorId ?? string.Empty,

//                        OrderQty = b.Quantity ?? 0m,
//                        QtyUnitId = b.TotalQuantityUnit ?? string.Empty,

//                        Consumption = b.Consumption ?? 0m,
//                        ConsumtionUnitId = b.TotalQuantityUnit ?? string.Empty,

//                        TotalQty = b.TotalQuantity ?? 0m,
//                        TotalQtyUnitId = b.TotalQuantityUnit ?? string.Empty,

//                        ReqQty = 0m,
//                        ThreadReqUnit = string.Empty,
//                        Threadpercent = string.Empty,

//                        TotalReceivedQty = 0m,
//                        CurrentReceiveQty = 0m,
//                        ReceivedUnitType = string.Empty,

//                        UnitPrice = b.UnitPrice ?? 0m,
//                        ReceivedUnitPrice = 0m,
//                        TotalPrice = b.TotalPrice ?? 0m,
//                        CurrencyId = b.TotalPriceCurrencyId ?? string.Empty,

//                        Remarks = string.Empty,
//                        EmployeeId = string.Empty,

//                        TotalReceivedQtyPre = 0m,
//                        PendingReceiveQty = 0m,
//                        PendingReceiveQtyPre = 0m,

//                        Brdid = string.Empty,
//                        FebricDetail = string.Empty,
//                        ThreadColorId = string.Empty,
//                        ThreadCountId = string.Empty,
//                        Refcodepantone = string.Empty
//                    };

//                    await threadRepo.AddAsync(entity);
//                }
//            }
//            catch (Exception)
//            {

//                throw;
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
//                             FROM RMG_Inv_BookingReceivedDetails_Thread 
//                             WHERE PurchaseReceiveNo LIKE @prefix + '%'";
//                using (SqlCommand cmd = new SqlCommand(queryTemp, con))
//                {
//                    cmd.Parameters.AddWithValue("@prefix", prefix);
//                    var result = await cmd.ExecuteScalarAsync();
//                    lastCode = result?.ToString();
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
//        private async Task PolyBookingData(List<RmgCostingDetailsTemp> bookingData, string PoNO)
//        {
//            try
//            {
//                if (!bookingData.Any()) return;

//                var slno = polyRepo.All().Count() + 1;
//                var prn = await GenerateAutoPolyBooking();

//                foreach (var b in bookingData)
//                {
//                    var existing = polyRepo.All()
//                        .Where(x => x.PoNo == b.PoNo && x.ItemId == b.ItemId)
//                        .ToList();

//                    if (existing.Any())
//                        await polyRepo.DeleteRangeAsync(existing);

//                    var entity = new RmgInvBookingReceivedDetailsPoly
//                    {
//                        PurchaseReceiveNo = prn ?? string.Empty,
//                        PoNo = PoNO ?? string.Empty,
//                        IntegraJobNo = costingInfoRepo.All()
//        .Where(x => x.CostingId == b.CostingId)
//        .Select(x => x.IntegraJobNo)
//        .FirstOrDefault() ?? string.Empty,

//                        SerialNo = slno++,

//                        ItemId = b.ItemId ?? string.Empty,
//                        ItemDescription = b.Description ?? string.Empty,
//                        ColorId = b.ColorId ?? string.Empty,

//                        GarmentQty = b.Quantity ?? 0m,
//                        GarmentQtyUnitId = b.TotalQuantityUnit ?? string.Empty,

//                        Consumption = b.Consumption ?? 0m,
//                        ConsumptionUnitId = b.TotalQuantityUnit ?? string.Empty,

//                        TotalQty = b.TotalQuantity ?? 0m,
//                        TotalQtyUnitId = b.TotalQuantityUnit ?? string.Empty,

//                        Percentage = string.Empty,

//                        TotalReceivedQty = 0m,
//                        CurrentReceiveQty = 0m,
//                        ReceivedUnitType = string.Empty,

//                        UnitPrice = b.UnitPrice ?? 0m,
//                        ReceivedUnitPrice = 0m,
//                        TotalPrice = b.TotalPrice ?? 0m,
//                        CurrencyId = b.TotalPriceCurrencyId ?? string.Empty,

//                        Remarks = string.Empty,
//                        EmployeeId = string.Empty,

//                        TotalReceivedQtyPre = 0m,
//                        PendingReceiveQty = 0m,
//                        PendingReceiveQtyPre = 0m,

//                        Brdid = string.Empty,
//                        RefernceCode = string.Empty,
//                        Length = string.Empty,
//                        LengthUnitId = string.Empty,
//                        Width = string.Empty,
//                        WidthUnitId = string.Empty,
//                        Flap = string.Empty,
//                        FlapUnitId = string.Empty,
//                        Guest = string.Empty,
//                        GuestUnitId = string.Empty
//                    };

//                    await polyRepo.AddAsync(entity);
//                }
//            }
//            catch (Exception)
//            {

//                throw;
//            }
//        }
//        public async Task<string> GenerateAutoPolyBooking()
//        {
//            var getYear = DateTime.Now.Year.ToString();
//            var prefix = "POR_" + getYear + "_";

//            string lastCode = null;

//            using (SqlConnection con = new SqlConnection(_connectionString))
//            {
//                await con.OpenAsync();

//                // First check Temp table
//                string queryTemp = @"SELECT MAX(PurchaseReceiveNo) 
//                     FROM RMG_Inv_BookingReceivedDetails_Poly 
//                     WHERE PurchaseReceiveNo LIKE @prefix + '%'";
//                using (SqlCommand cmd = new SqlCommand(queryTemp, con))
//                {
//                    cmd.Parameters.AddWithValue("@prefix", prefix);
//                    var result = await cmd.ExecuteScalarAsync();
//                    lastCode = result?.ToString();
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
//        private async Task ButtonBookingData(List<RmgCostingDetailsTemp> bookingData, string PoNO)
//        {
//            if (!bookingData.Any()) return;

//            var slno = buttonRepo.All().Count() + 1;
//            var prn = await GenerateAutoButtonId();

//            foreach (var b in bookingData)
//            {
//                var existing = buttonRepo.All()
//                    .Where(x => x.PoNo == b.PoNo && x.ItemId == b.ItemId)
//                    .ToList();

//                if (existing.Any())
//                    await buttonRepo.DeleteRangeAsync(existing);

//                var entity = new RmgInvBookingReceivedDetailsButton
//                {
//                    PurchaseReceiveNo = prn ?? string.Empty,
//                    PoNo = PoNO ?? string.Empty,
//                    IntegraJobNo = costingInfoRepo.All()
//        .Where(x => x.CostingId == b.CostingId)
//        .Select(x => x.IntegraJobNo)
//        .FirstOrDefault() ?? string.Empty,

//                    SerialNo = slno++,

//                    ItemId = b.ItemId ?? string.Empty,
//                    Description = b.Description ?? string.Empty,
//                    ColorId = b.ColorId ?? string.Empty,

//                    GermentQty = b.Quantity ?? 0m,
//                    GermentsQtyUnitId = b.TotalQuantityUnit ?? string.Empty,

//                    Consumption = b.Consumption ?? 0m,
//                    ConsumptionUnitId = b.TotalQuantityUnit ?? string.Empty,

//                    TotalQty = b.TotalQuantity ?? 0m,
//                    TotalQtyUnitId = b.TotalQuantityUnit ?? string.Empty,

//                    OrderQty = b.TotalQuantity ?? 0m,
//                    OrderQtyUnitId = b.TotalQuantityUnit ?? string.Empty,

//                    Percentage = string.Empty,

//                    TotalReceivedQty = 0m,
//                    CurrentReceiveQty = 0m,
//                    ReceivedUnitType = string.Empty,

//                    UnitPrice = b.UnitPrice ?? 0m,
//                    ReceivedUnitPrice = 0m,
//                    TotalPrice = b.TotalPrice ?? 0m,
//                    CurrencyId = b.TotalPriceCurrencyId ?? string.Empty,

//                    Remarks = string.Empty,
//                    EmployeeId = string.Empty,

//                    TotalReceivedQtyPre = 0m,
//                    PendingReceiveQty = 0m,
//                    PendingReceiveQtyPre = 0m,

//                    Brdid = string.Empty,
//                    FabricColorId = string.Empty,
//                    SizeId = string.Empty,
//                    Idno = string.Empty
//                };
//                await buttonRepo.AddAsync(entity);
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

//                // First check Temp table
//                string queryTemp = @"SELECT MAX(PurchaseReceiveNo) 
//                     FROM RMG_Inv_BookingReceivedDetails_Button
//                     WHERE PurchaseReceiveNo LIKE @prefix + '%'";
//                using (SqlCommand cmd = new SqlCommand(queryTemp, con))
//                {
//                    cmd.Parameters.AddWithValue("@prefix", prefix);
//                    var result = await cmd.ExecuteScalarAsync();
//                    lastCode = result?.ToString();
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
//        private async Task FebricBookingData(List<RmgCostingDetailsTemp> bookingData, string PoNO)
//        {
//            try
//            {
//                if (!bookingData.Any()) return;

//                var slno = febricRepo.All().Count() + 1;
//                var prn = await GenerateAutoFebrickId();

//                foreach (var b in bookingData)
//                {
//                    var existing = febricRepo.All()
//                        .Where(x => x.PoNo == b.PoNo && x.ItemId == b.ItemId)
//                        .ToList();

//                    if (existing.Any())
//                        await febricRepo.DeleteRangeAsync(existing);


//                    var entity = new RmgInvBookingReceivedDetailsFebric
//                    {
//                        PurchaseReceiveNo = prn ?? string.Empty,
//                        PoNo = PoNO ?? string.Empty,
//                        IntegraJobNo = costingInfoRepo.All()
//        .Where(x => x.CostingId == b.CostingId)
//        .Select(x => x.IntegraJobNo)
//        .FirstOrDefault() ?? string.Empty,

//                        Slno = slno++,

//                        ItemId = b.ItemId ?? string.Empty,
//                        FebricDetails = b.Description ?? string.Empty,
//                        ColorId = b.ColorId ?? string.Empty,

//                        OrderQty = b.Quantity ?? 0m,
//                        QtyUnit = b.TotalQuantityUnit ?? string.Empty,
//                        Consumption = b.Consumption ?? 0m,
//                        ConsumtionUnit = b.TotalQuantityUnit ?? string.Empty,
//                        TotalFebricQty = b.TotalQuantity ?? 0m,

//                        UnitPrice = b.UnitPrice ?? 0m,
//                        TotalPrice = b.TotalPrice ?? 0m,
//                        CurrencyId = b.TotalPriceCurrencyId ?? string.Empty,

//                        EmployeeId = string.Empty,

//                        // Optional defaults for other properties in the model
//                        Brdid = string.Empty,
//                        FabricItemId = string.Empty,
//                        Refcode = string.Empty,
//                        Percentage = 0m,
//                        TotalReceivedQty = 0m,
//                        CurrentReceiveQty = 0m,
//                        ReceivedUnitType = string.Empty,
//                        ReceivedUnitPrice = 0m,
//                        TotalReceivedQtyPre = 0m,
//                        PendingReceiveQty = 0m,
//                        PendingReceiveQtyPre = 0m
//                    };

//                    await febricRepo.AddAsync(entity);
//                }
//            }
//            catch (Exception)
//            {

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

//                // First check Temp table
//                string queryTemp = @"SELECT MAX(PurchaseReceiveNo) 
//                     FROM RMG_Inv_BookingReceivedDetails_Febric 
//                     WHERE PurchaseReceiveNo LIKE @prefix + '%'";
//                using (SqlCommand cmd = new SqlCommand(queryTemp, con))
//                {
//                    cmd.Parameters.AddWithValue("@prefix", prefix);
//                    var result = await cmd.ExecuteScalarAsync();
//                    lastCode = result?.ToString();
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

//        private async Task ExtraBookingData(
//    List<RmgCostingDetailsTemp> bookingData,
//    string poNo)
//        {
//            try
//            {
//                if (bookingData == null || !bookingData.Any())
//                    return;

//                string costingId = bookingData.First().CostingId;

//                string integraJobNo = costingInfoRepo.All()
//                    .Where(x => x.CostingId == costingId)
//                    .Select(x => x.IntegraJobNo)
//                    .FirstOrDefault();


//                int slno = 1;
//                string prn = await GenerateAutoExtraId();

//                foreach (var b in bookingData)
//                {
//                    var entity = new RmgInvBookingReceivedDetailsExtra
//                    {
//                        PurchaseReceiveNo = prn ?? string.Empty,
//                        PoNo = poNo ?? string.Empty,
//                        IntegraJobNo = integraJobNo ?? string.Empty,

//                        Slno = slno++,

//                        ItemId = b.ItemId ?? string.Empty,
//                        Description = b.Description ?? string.Empty,
//                        ColorId = b.ColorId ?? string.Empty,

//                        OrderQty = b.Quantity ?? 0m,
//                        OrderQtyIunitD = b.TotalQuantityUnit ?? string.Empty,

//                        Consumption = b.Consumption ?? 0m,
//                        ConsumptionUnitId = b.TotalQuantityUnit ?? string.Empty,

//                        TotalQty = b.TotalQuantity ?? 0m,
//                        TotalQtyUnitId = b.TotalQuantityUnit ?? string.Empty,

//                        ReqQty = 0m,
//                        ReqQtyUnitId = string.Empty,

//                        Percentage = string.Empty,

//                        TotalReceivedQty = 0m,
//                        CurrentReceiveQty = 0m,
//                        ReceivedUnitType = string.Empty,

//                        UnitPrice = b.UnitPrice ?? 0m,
//                        ReceivedUnitPrice = 0m,
//                        TotalPrice = b.TotalPrice ?? 0m,
//                        CurrencyId = b.TotalPriceCurrencyId ?? string.Empty,

//                        Remarks = string.Empty,
//                        EmployeeId = string.Empty,

//                        TotalReceivedQtyPre = 0m,
//                        PendingReceiveQty = 0m,
//                        PendingReceiveQtyPre = 0m,

//                        Brdid = string.Empty,
//                        FabricColorId = string.Empty
//                    };

//                    await extraRepo.AddAsync(entity);
//                }
//            }
//            catch (Exception ex)
//            {
//                throw;
//            }
//        }

//        public async Task<string> GenerateAutoExtraId()
//        {
//            var getYear = DateTime.Now.Year.ToString();
//            var prefix = "POR_" + getYear + "_";

//            string lastCode = null;

//            using (SqlConnection con = new SqlConnection(_connectionString))
//            {
//                await con.OpenAsync();

//                // First check Temp table
//                string queryTemp = @"SELECT MAX(PurchaseReceiveNo) 
//                     FROM RMG_Inv_BookingReceivedDetails_Extra
//                     WHERE PurchaseReceiveNo LIKE @prefix + '%'";
//                using (SqlCommand cmd = new SqlCommand(queryTemp, con))
//                {
//                    cmd.Parameters.AddWithValue("@prefix", prefix);
//                    var result = await cmd.ExecuteScalarAsync();
//                    lastCode = result?.ToString();
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


//        public async Task<(int total, List<RmgCostingInfoListDto> data)> GetAllForDataTableAsync(
//      int start,
//      int length,
//      string? search)
//        {
//            var query = costingInfoRepo.All();

//            // Search filter
//            if (!string.IsNullOrEmpty(search))
//            {
//                query = query.Where(x =>
//                    x.CostingId.Contains(search) ||
//                    x.PoNo.Contains(search) ||
//                    x.IntegraJobNo.Contains(search) ||
//                    x.StyleId.Contains(search) ||
//                    x.MasterPurchaseOrder.Contains(search));
//            }

//            var total = await query.CountAsync();

//            var data = await query
//                .OrderByDescending(x => x.AutoId)
//                .Skip(start)
//                .Take(length)
//                .Select(x => new RmgCostingInfoListDto
//                {
//                    AutoId = x.AutoId,
//                    CostingId = x.CostingId,
//                    EntryDate = x.EntryDate ?? DateTime.Now,
//                    IntegraJobNo = x.IntegraJobNo,
//                    StyleId = x.StyleId,
//                    StyleName = styleRepo.All().Where(c => c.StyleId == x.StyleId).Select(s => s.Style).FirstOrDefault() ?? "",
//                    MasterPurchaseOrder = x.MasterPurchaseOrder,
//                    PoNo = x.PoNo,
//                    ExportLcnoSc = x.ExportLcnoSc,
//                    IssuedBy = x.IssuedBy,
//                    CheckedBy = x.CheckedBy,
//                    CheckedName = empRepo.All().Where(c => c.EmployeeId == x.CheckedBy).Select(w => w.FirstName + " " + w.LastName).FirstOrDefault(),
//                    CreateDate = x.Ldate.HasValue ? x.Ldate.Value.ToString("dd/MM/yyyy") : "",
//                    ModifyDate = x.ModifyDate.HasValue ? x.Ldate.Value.ToString("dd/MM/yyyy") : ""
//                })
//                .ToListAsync();

//            return (total, data);
//        }

//        public async Task<(bool isSuccess, string message)> DeleteAsync(int autoId)
//        {
//            try
//            {
//                var entity = await costingInfoRepo.GetByIdAsync(autoId);
//                if (entity == null)
//                    return (false, "Record not found");

//                string costingId = entity.CostingId;
//                string integraJobNo = entity.IntegraJobNo;

//                var tempDetails = await rmgCostingDetailsTempRepo.All()
//                    .Where(x => x.CostingId == costingId)
//                    .ToListAsync();
//                if (tempDetails.Any())
//                    await rmgCostingDetailsTempRepo.DeleteRangeAsync(tempDetails);

//                var mainDetails = await rmgCostingDetailsRepo.All()
//                    .Where(x => x.CostingId == costingId)
//                    .ToListAsync();
//                if (mainDetails.Any())
//                    await rmgCostingDetailsRepo.DeleteRangeAsync(mainDetails);

//                var bookingOrders = await boRepo.All()
//                    .Where(x => x.IntegraJobNo == integraJobNo)
//                    .ToListAsync();
//                if (bookingOrders.Any())
//                    await boRepo.DeleteRangeAsync(bookingOrders);

//                var extraDetails = await extraRepo.All()
//                    .Where(x => x.IntegraJobNo == integraJobNo)
//                    .ToListAsync();
//                if (extraDetails.Any())
//                    await extraRepo.DeleteRangeAsync(extraDetails);

//                var cartonDetails = await cartonRepo.All()
//                    .Where(x => x.IntegraJobNo == integraJobNo)
//                    .ToListAsync();
//                if (cartonDetails.Any())
//                    await cartonRepo.DeleteRangeAsync(cartonDetails);

//                var threadDetails = await threadRepo.All()
//                    .Where(x => x.IntegraJobNo == integraJobNo)
//                    .ToListAsync();
//                if (threadDetails.Any())
//                    await threadRepo.DeleteRangeAsync(threadDetails);

//                var polyDetails = await polyRepo.All()
//                    .Where(x => x.IntegraJobNo == integraJobNo)
//                    .ToListAsync();
//                if (polyDetails.Any())
//                    await polyRepo.DeleteRangeAsync(polyDetails);

//                var buttonDetails = await buttonRepo.All()
//                    .Where(x => x.IntegraJobNo == integraJobNo)
//                    .ToListAsync();
//                if (buttonDetails.Any())
//                    await buttonRepo.DeleteRangeAsync(buttonDetails);

//                var fabricDetails = await febricRepo.All()
//                    .Where(x => x.IntegraJobNo == integraJobNo)
//                    .ToListAsync();
//                if (fabricDetails.Any())
//                    await febricRepo.DeleteRangeAsync(fabricDetails);

//                await costingInfoRepo.DeleteAsync(entity);

//                return (true, DeleteSuccess);
//            }
//            catch (Exception ex)
//            {
//                return (false, $"Delete failed: {ex.Message}");
//            }
//        }



//        public async Task<(bool isSuccess, string message, RmgCostingInfoDto data)> EditCostingAsync(int autoId)
//        {
//            try
//            {
//                var entity = await costingInfoRepo.GetByIdAsync(autoId);
//                if (entity == null)
//                    return (false, "Record not found", null);

//                var mainDetails = await rmgCostingDetailsRepo.All()
//                    .Where(x => x.CostingId == entity.CostingId)
//                    .ToListAsync();

//                var existingTemp = await rmgCostingDetailsTempRepo.All()
//                    .ToListAsync();

//                if (existingTemp.Any())
//                {
//                    await rmgCostingDetailsTempRepo.DeleteRangeAsync(existingTemp);
//                }

//                if (mainDetails.Any())
//                {
//                    var tempList = mainDetails.Select(d => new RmgCostingDetailsTemp
//                    {
//                        CostingDetailsId = d.CostingDetailsId,
//                        CostingId = d.CostingId,
//                        Slno = d.Slno,
//                        ItemId = d.ItemId ?? "",
//                        Description = d.Description ?? "",
//                        Width = d.Width ?? "",
//                        ColorId = d.ColorId ?? "",
//                        SupplierId = d.SupplierId ?? "",
//                        PoNo = d.PoNo ?? "",
//                        Quantity = d.Quantity ?? 0,
//                        Consumption = d.Consumption ?? 0,
//                        Extra = d.Extra ?? 0,
//                        TotalQuantityUnit = d.TotalQuantityUnit ?? "",
//                        UnitPrice = d.UnitPrice ?? 0,
//                        TotalPriceCurrencyId = d.TotalPriceCurrencyId ?? "",
//                        TotalAmountShhkg = d.TotalAmountShhkg ?? 0,
//                        TotalAmountBdt = d.TotalAmountBdt ?? 0,
//                        TotalAmountThb = d.TotalAmountThb ?? 0,
//                        ResponsibleBy = d.ResponsibleBy ?? "",
//                        TotalPrice = d.TotalPrice ?? 0,
//                        TotalQuantity = d.TotalQuantity ?? 0,
//                        BookingItemTypeId = d.BookingItemTypeId ?? "",
//                        Luser = d.Luser,
//                    }).ToList();

//                    await rmgCostingDetailsTempRepo.AddRangeAsync(tempList);
//                }

//                var model = new RmgCostingInfoDto
//                {
//                    AutoId = entity.AutoId,
//                    CostingId = entity.CostingId,
//                    EntryDate = entity.EntryDate ?? DateTime.Now,
//                    BuyerId = entity.BuyerId,
//                    StyleId = entity.StyleId,
//                    MasterPurchaseOrder = entity.MasterPurchaseOrder,
//                    PoNo = entity.PoNo,
//                    IntegraJobNo = entity.IntegraJobNo,
//                    ExportLcnoSc = entity.ExportLcnoSc,
//                    ShipmentDate = entity.ShipmentDate ?? DateTime.Now,
//                    FactorySuplier = entity.FactorySuplier,
//                    IssuedBy = entity.IssuedBy,
//                    CheckedBy = entity.CheckedBy,
//                    SubTotalAmountShhkg = entity.SubTotalAmountShhkg ?? 0,
//                    SubTotalAmountBdt = entity.SubTotalAmountBdt ?? 0,
//                    SubTotalAmountThb = entity.SubTotalAmountThb ?? 0,
//                    DamagePercentage = entity.DamagePercentage ?? 0,
//                    DamageAmountShhkg = entity.DamageAmountShhkg ?? 0,
//                    DamageAmountBdt = entity.DamageAmountBdt ?? 0,
//                    DamageAmountThb = entity.DamageAmountThb ?? 0,
//                    InterestOverheadPercentage = entity.InterestOverheadPercentage ?? 0,
//                    InterestOverheadShhkg = entity.InterestOverheadShhkg ?? 0,
//                    InterestOverheadBdt = entity.InterestOverheadBdt ?? 0,
//                    InterestOverheadThb = entity.InterestOverheadThb ?? 0,
//                    TotalAmountShhkg = entity.TotalAmountShhkg ?? 0,
//                    TotalAmountBdt = entity.TotalAmountBdt ?? 0,
//                    TotalAmountThb = entity.TotalAmountThb ?? 0,
//                    TotalMaterialCostOverseas = entity.TotalMaterialCostOverseas ?? 0,
//                    TotalMaterialCostBdt = entity.TotalMaterialCostBdt ?? 0,
//                    TotalMaterialCostBkk = entity.TotalMaterialCostBkk ?? 0,
//                    CmandProfit = entity.CmandProfit ?? 0,
//                    HandlingCharge = entity.HandlingCharge ?? 0,
//                    ProductionUpCharge = entity.ProductionUpCharge ?? 0,
//                    GrandTotal = entity.GrandTotal ?? 0,
//                    Ffprice = entity.Ffprice ?? 0,
//                    ShowCreateDate = entity.Ldate.HasValue ? entity.Ldate.Value.ToString("dd/MM/yyyy") : "",
//                    ShowModifyDate = entity.ModifyDate.HasValue ? entity.Ldate.Value.ToString("dd/MM/yyyy") : ""
//                };

//                return (true, "Data loaded successfully", model);
//            }
//            catch (Exception ex)
//            {
//                return (false, $"Failed: {ex.Message}", null);
//            }
//        }

//        public async Task<CostingReportDto> GetCostingReportByIdAsync(
//    string costingId,
//    string integraJobNo,
//    string purchaseOrder,
//    string productId)
//        {
//            using (var conn = new SqlConnection(_connectionString))
//            {
//                await conn.OpenAsync();
//                using (var multi = await conn.QueryMultipleAsync(
//                    "dbo.GetCostingReportByCostingId",
//                    new
//                    {
//                        CostingId = costingId,
//                        IntegraJOBNo = integraJobNo,
//                        PurchaseOrder = purchaseOrder,
//                        ProductId = productId
//                    },
//                    commandType: CommandType.StoredProcedure))
//                {
//                    var master = await multi.ReadFirstOrDefaultAsync<CostingReportDto>();
//                    if (master == null)
//                        return null;
//                    var breakup = (await multi.ReadAsync<ColorSizeBreakupReportDto>()).ToList();
//                    var details = (await multi.ReadAsync<CostingDetailReportDto>()).ToList();
//                    master.ColorSizeBreakups = breakup;
//                    master.Details = details;
//                    return master;
//                }
//            }
//        }
//    }
//}
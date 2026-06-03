
using Dapper;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.RMG_CostingInfoReport;
using GCTL.Data.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace GCTL.Service.RMG_CostingInfoReport
{


    public class RMG_CostingInfoReportService : AppService<CostingReportData>, IRMG_CostingInfoReportService
    {
        private readonly IRepository<CostingReportData> costingRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly IConfiguration _configuration;

        public RMG_CostingInfoReportService(
            IRepository<CostingReportData> costingRepo,
            IRepository<CoreAccessCode> accessCodeRepository,
            IConfiguration configuration
            ) : base(costingRepo)
        {
            this.costingRepo = costingRepo;
            this.accessCodeRepository = accessCodeRepository;
            _configuration = configuration;
        }
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "RMG Costing Info Report" && x.TitleCheck);
        }

        public async Task<CostingFilterResponse> GetFilterDataAsync(CostingFilterRequest request)
        {
            await using var conn = new SqlConnection(_configuration.GetConnectionString("ApplicationDbConnection"));
            var p = new DynamicParameters();
            p.Add("@CostingIds", string.Join(",", request.CostingIds));
            p.Add("@BuyerIds", string.Join(",", request.BuyerIds));

            var result = await conn.QueryMultipleAsync("GetCostingReportFilters", p, commandType: CommandType.StoredProcedure);
            return new CostingFilterResponse
            {
                Costings = (await result.ReadAsync<FilterItem>()).ToList(),
                Buyers = (await result.ReadAsync<FilterItem>()).ToList()
            };
        }

        public async Task<CostingReportData> GetCostingReportAsync(string costingId, string integraJobNo, string purchaseOrder, string productId)
        {
            await using var conn = new SqlConnection(_configuration.GetConnectionString("ApplicationDbConnection"));
            var p = new DynamicParameters();
            p.Add("@CostingId", costingId);
            p.Add("@IntegraJOBNo", integraJobNo);
            p.Add("@PurchaseOrder", purchaseOrder);
            p.Add("@ProductId", productId);

            var grid = await conn.QueryMultipleAsync("GetCostingReportByCostingId", p, commandType: CommandType.StoredProcedure);
            var master = await grid.ReadFirstOrDefaultAsync<CostingReportData>();
            if (master == null) return null;

            master.ColorSizeBreakups = (await grid.ReadAsync<ColorSizeBreakup>()).ToList();
            master.Details = (await grid.ReadAsync<CostingDetail>()).ToList();
            return master;
        }

        public async Task<List<CostingReportForExcel>> GetFilteredReportsAsync(CostingFilterRequest request)
        {
            await using var conn = new SqlConnection(_configuration.GetConnectionString("ApplicationDbConnection"));
            var p = new DynamicParameters();
            p.Add("@CostingIds", string.Join(",", request.CostingIds));
            p.Add("@BuyerIds", string.Join(",", request.BuyerIds));

            var multi = await conn.QueryMultipleAsync("GetFilteredCostingReports", p, commandType: CommandType.StoredProcedure);

            var masters = (await multi.ReadAsync<CostingReportForExcel>()).ToList();
            var colorSizes = (await multi.ReadAsync<ColorSizeWithCostingId>()).ToList();
            var details = (await multi.ReadAsync<CostingDetailWithCostingId>()).ToList();

            foreach (var master in masters)
            {
                master.ColorSizeBreakups = colorSizes
                    .Where(x => x.CostingId == master.CostingId)
                    .Select(x => new ColorSizeBreakup
                    {
                        ColorName = string.IsNullOrEmpty(x.ColorName) ? x.ColorId : x.ColorName,
                        SizeName = string.IsNullOrEmpty(x.SizeName) ? x.SizeId : x.SizeName,
                        Quantity = x.Quantity
                    })
                    .ToList();

                master.Details = details
                    .Where(x => x.CostingId == master.CostingId)
                    .OrderBy(x => x.Slno)
                    .Select(x => new CostingDetail
                    {
                        Slno = x.Slno,
                        ItemName = x.ItemName,
                        Description = x.Description,
                        Width = x.Width,
                        Quantity = x.Quantity,
                        Consumption = x.Consumption,
                        Extra = x.Extra,
                        Unit = x.Unit,
                        UnitPrice = x.UnitPrice,
                        TotalQuantityUnit = x.TotalQuantityUnit,
                        TotalAmountShhkg = x.TotalAmountShhkg,
                        TotalAmountBdt = x.TotalAmountBdt,
                        TotalAmountThb = x.TotalAmountThb
                    })
                    .ToList();
            }

            return masters;
        }
    }
}
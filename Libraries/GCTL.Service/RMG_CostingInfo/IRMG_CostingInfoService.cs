using GCTL.Core.ViewModels.RMG_CostingInfo;
using Microsoft.AspNetCore.Http;

namespace GCTL.Service.RMG_CostingInfo
{
    public interface IRMG_CostingInfoService
    {
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);

        Task<List<ProdOrderReportDto>> GetProdOrderReport(ProdOrderFilterDto filter);
        //Task<FilterOptionsDto> GetFilterOptions();
        Task<FilterOptionsDto> GetFilterOptions(ProdOrderFilterDto filter = null);





        //Task<List<RmgCostingDetailsTempDto>> GetAllByCostingIdAsync(string costingId);
        Task<List<RmgCostingDetailsTempDto>> GetAllByCostingIdAsync(string costingId, bool clearTemp = true, string username = null);
        Task<RmgCostingDetailsTempDto> GetByIdAsync(string id);
        Task<RmgCostingDetailsTempDto> AddAsync(RmgCostingDetailsTempDto dto);
        Task<RmgCostingDetailsTempDto> UpdateAsync(RmgCostingDetailsTempDto dto);
        Task<bool> DeleteAsync(string id);
        Task<bool> DeleteByCostingIdAsync(string costingId);
        //Task<RmgCostingSummaryDto> CalculateSummaryAsync(string costingId, decimal damagePercent, decimal interestPercent);
        Task<RmgCostingSummaryDto> CalculateSummaryAsync(string costingId, decimal damagePercent, decimal interestPercent, decimal cmAndProfit, decimal handlingCharge, decimal productionUpchargePercent);



        // Excel Import methods
        Task<List<RmgCostingDetailsTempDto>> PreviewExcelAsync(IFormFile file);
        Task<bool> ImportExcelAsync(IFormFile file, string costingId, string username);

        Task<(bool isSuccess, string message, object data)> CreateUpdateAsync(RmgCostingInfoDto model, string companyCode);
        Task<(int total, List<RmgCostingInfoListDto> data)> GetAllForDataTableAsync(int start, int length, string? search);
        Task<(bool isSuccess, string message)> DeleteAsync(int autoId);
        Task<(bool isSuccess, string message, RmgCostingInfoDto data)> EditCostingAsync(int autoId);

        Task<CostingReportDto> GetCostingReportByIdAsync(string costingId, string integraJobNo, string purchaseOrder, string productId);
    }
}

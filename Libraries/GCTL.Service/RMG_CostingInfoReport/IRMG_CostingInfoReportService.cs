using GCTL.Core.ViewModels.RMG_CostingInfoReport;

namespace GCTL.Service.RMG_CostingInfoReport
{
    public interface IRMG_CostingInfoReportService
    {
        Task<bool> PagePermissionAsync(string accessCode);
        Task<CostingFilterResponse> GetFilterDataAsync(CostingFilterRequest request);
        Task<CostingReportData> GetCostingReportAsync(string costingId, string integraJobNo, string purchaseOrder, string productId);
        Task<List<CostingReportForExcel>> GetFilteredReportsAsync(CostingFilterRequest request);
    }
}

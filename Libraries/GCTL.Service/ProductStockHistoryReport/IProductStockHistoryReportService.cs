using GCTL.Core.ViewModels.ProductStockHistoryReport;

namespace GCTL.Service.ProductStockHistoryReport
{
    public interface IProductStockHistoryReportService
    {
        Task<bool> PagePermissionAsync(string accessCode);
        Task<List<ProductStockHistoryReportSetupViewModel>> GetStockReportAsync(StockReportFilterViewModel filter);
        Task<ProductStockHistoryReportDropdownDto> GetFilteredDropdownAsync(StockReportFilterViewModel model);
    }
}

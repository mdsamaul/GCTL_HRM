using GCTL.Core.ViewModels.MonthWiseOrderBookingReport;

namespace GCTL.Service.MonthWiseOrderBookingReport
{
    public interface IOrderReportService
    {
        Task<bool> PagePermissionAsync(string accessCode);
        Task<OrderReportAllStyleResponse> GetOrderReportAllStyleAsync(OrderReportRequest request, string companyCode);
        Task<OrderReportStyleResponse> GetOrderReportStyleAsync(OrderReportRequest request, string companyCode);
        Task<OrderReportStylePoResponse> GetOrderReportStylePoAsync(OrderReportRequest request, string companyCode);
        Task<OrderReportStylePoCSResponse> GetOrderReportStylePoCSAsync(OrderReportRequest request, string companyCode);

        Task<List<StyleMaster>> GetStylesAsync();
        Task<List<ColorMaster>> GetColorsAsync();
        Task<List<SizeMaster>> GetSizesAsync();
    }
}

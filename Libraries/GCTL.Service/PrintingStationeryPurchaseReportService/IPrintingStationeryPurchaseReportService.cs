using GCTL.Core.ViewModels.PrintingStationeryPurchaseReport;

namespace GCTL.Service.PrintingStationeryPurchaseReportService
{
    public interface IPrintingStationeryPurchaseReportService
    {
        Task<bool> PagePermissionAsync(string accessCode);

        Task<List<PrintingStationeryPurchaseReportResultDto>> GetAllPROCPrintingAndStationeryReport(PrintingStationeryPurchaseReportFilterDto model);
        Task<PrintingStationeryDropdownDto> GetFilteredDropdownsAsync(PrintingStationeryPurchaseReportFilterDto model);
    }
}

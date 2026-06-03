using GCTL.Core.ViewModels.ProductIssueReport;

namespace GCTL.Service.ProductIssueReports
{
    public interface IProductIssueReportService
    {
        Task<bool> PagePermissionAsync(string accessCode);
        Task<List<ProductIssueReportSetupViewModel>> GetProductIssueReportAsync(ProductIssueReportFilterViewModel filter);
        Task<ProductIssueDropdownDto> GetProductIssueDropdownAsync(ProductIssueReportFilterViewModel model);
    }
}

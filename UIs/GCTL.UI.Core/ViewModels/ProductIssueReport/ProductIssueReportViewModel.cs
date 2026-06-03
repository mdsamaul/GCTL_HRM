using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.ProductIssueReport;

namespace GCTL.UI.Core.ViewModels.ProductIssueReport
{
    public class ProductIssueReportViewModel : BaseViewModel
    {
        public ProductIssueReportSetupViewModel Setup { get; set; } = new ProductIssueReportSetupViewModel();
    }
}

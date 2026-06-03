using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.ProductStockHistoryReport;

namespace GCTL.UI.Core.ViewModels.ProductStockHistoryReport
{
    public class ProductStockHistoryReportViewModel:BaseViewModel
    {
       public ProductStockHistoryReportSetupViewModel Setup { get; set; } = new ProductStockHistoryReportSetupViewModel();
    }
}

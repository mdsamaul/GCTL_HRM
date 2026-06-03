using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.MonthWiseOrderBookingReport;

namespace GCTL.UI.Core.ViewModels.MonthWiseOrderBookingReportViewModel
{
    public class OrderReportDataViewModel : BaseViewModel
    {
        public OrderReportData OrderReportData { get; set; } = new OrderReportData();
    }
}

using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.MonthlyTransportExpenseDetailsReport;

namespace GCTL.UI.Core.ViewModels.MonthlyTransportExpenseDetailsReport
{
    public class MonthlyTransportExpenseDetailsReportViewModel:BaseViewModel
    {
        public MonthlyTransportExpenseDetailsReportSetupDto Setup { get; set; } = new MonthlyTransportExpenseDetailsReportSetupDto();
    }
}

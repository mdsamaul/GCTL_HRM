using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.TransportExpenseStatementReport;

namespace GCTL.UI.Core.ViewModels.TransportExpenseStatementReport
{
    public class TransportExpenseStatementReportViewModel:BaseViewModel
    {
        public TransportExpenseStatementReportSetupViewModel Setup { get; set; } = new TransportExpenseStatementReportSetupViewModel();
    }
}

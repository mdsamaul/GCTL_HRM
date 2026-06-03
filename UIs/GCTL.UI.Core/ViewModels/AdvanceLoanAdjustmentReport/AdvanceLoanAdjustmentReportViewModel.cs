using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.AdvanceLoanAdjustmentReport;

namespace GCTL.UI.Core.ViewModels.AdvanceLoanAdjustmentReport
{
    public class AdvanceLoanAdjustmentReportViewModel :BaseViewModel
    {
        public AdvanceLoanAdjustmentReportSetupViewModel SetupViewModel { get; set; } = new AdvanceLoanAdjustmentReportSetupViewModel();
    }
}

using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.RMG_CostingInfoReport;

namespace GCTL.UI.Core.ViewModels.RMG_CostingInfoReport
{
    public class RMG_CostingInfoReportViewModel : BaseViewModel
    {
        public CostingReportData Setup { get; set; } = new CostingReportData();
    }
}

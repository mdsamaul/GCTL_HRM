using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.PrintingStationeryPurchaseEntry;

namespace GCTL.UI.Core.ViewModels.PrintingStationeryPurchaseReport
{
    public class PrintingStationeryPurchaseReportViewModel:BaseViewModel
    {
        public PrintingStationeryPurchaseEntrySetupViewModel Setup { get; set; } = new PrintingStationeryPurchaseEntrySetupViewModel();
    }
}

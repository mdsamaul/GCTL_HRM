using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.PrintingStationeryPurchaseEntry;

namespace GCTL.UI.Core.ViewModels.PrintingStationeryPurchaseEntry
{
    public class PrintingStationeryPurchaseEntryViewModel :BaseViewModel
    {
        public PrintingStationeryPurchaseEntrySetupViewModel Setup { get; set; } = new PrintingStationeryPurchaseEntrySetupViewModel();
    }
}

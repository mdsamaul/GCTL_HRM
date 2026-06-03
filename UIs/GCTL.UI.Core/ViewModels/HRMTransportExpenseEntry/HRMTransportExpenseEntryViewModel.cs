using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HRMTransportExpenseEntry;


namespace GCTL.UI.Core.ViewModels.HRMTransportExpenseEntry
{
    public class HRMTransportExpenseEntryViewModel :BaseViewModel
    {
        public HRMTransportExpenseEntrySetupViewModel Setup { get; set; } = new HRMTransportExpenseEntrySetupViewModel();
        public TransportExpenseDetailsTempDto Details { get; set; } = new TransportExpenseDetailsTempDto();
    }
}

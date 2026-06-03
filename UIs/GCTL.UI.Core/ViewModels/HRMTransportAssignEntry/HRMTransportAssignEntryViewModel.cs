using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HRMTransportAssignEntry;

namespace GCTL.UI.Core.ViewModels.HRMTransportAssignEntry
{
    public class HRMTransportAssignEntryViewModel : BaseViewModel
    {
        public HRMTransportAssignEntrySetupViewModel Setup { get; set; } = new HRMTransportAssignEntrySetupViewModel();
    }
}

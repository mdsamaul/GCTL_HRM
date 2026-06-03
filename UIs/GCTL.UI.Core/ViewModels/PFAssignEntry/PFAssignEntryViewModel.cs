using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.ManualEarnLeaveEntry;
using GCTL.Core.ViewModels.PFAssignEntry;

namespace GCTL.UI.Core.ViewModels.PFAssignEntry
{
    public class PFAssignEntryViewModel : BaseViewModel
    {
        public PFAssignEntrySetupViewModel Setup { get; set; } = new PFAssignEntrySetupViewModel();
    }
}

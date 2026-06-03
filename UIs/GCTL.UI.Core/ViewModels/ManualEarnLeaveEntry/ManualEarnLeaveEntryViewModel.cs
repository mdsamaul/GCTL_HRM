using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.ManualEarnLeaveEntry;

namespace GCTL.UI.Core.ViewModels.ManualEarnLeaveEntry
{
    public class ManualEarnLeaveEntryViewModel : BaseViewModel
    {
        public ManualEarnLeaveEntryEmployeeSetupViewModel Setup { get; set; } = new ManualEarnLeaveEntryEmployeeSetupViewModel();

    }
}

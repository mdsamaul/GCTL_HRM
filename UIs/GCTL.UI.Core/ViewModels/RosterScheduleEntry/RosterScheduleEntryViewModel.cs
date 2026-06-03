using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.RosterScheduleEntry;

namespace GCTL.UI.Core.ViewModels.RosterScheduleEntry
{
    public class RosterScheduleEntryViewModel :BaseViewModel
    {
        public RosterScheduleEntrySetupViewModel Setup { get; set; } = new RosterScheduleEntrySetupViewModel();
    }
}

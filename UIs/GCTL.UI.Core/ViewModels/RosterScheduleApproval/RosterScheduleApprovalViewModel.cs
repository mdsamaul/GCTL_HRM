using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.RosterScheduleApproval;

namespace GCTL.UI.Core.ViewModels.RosterScheduleApproval
{
    public class RosterScheduleApprovalViewModel : BaseViewModel
    {
        public RosterScheduleApprovalSetupViewModel Setup { get; set; } = new RosterScheduleApprovalSetupViewModel();
    }
}

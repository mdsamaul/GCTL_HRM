using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.ManualEntryApproval;
using GCTL.Core.ViewModels.RosterScheduleApproval;

namespace GCTL.UI.Core.ViewModels.ManualEntryApproval
{
    public class ManualEntryApprovalSetupViewModel:BaseViewModel
    {
        public ManualEntryApprovalSetupViewModelDto Setup { get; set; } = new ManualEntryApprovalSetupViewModelDto();
    }
}

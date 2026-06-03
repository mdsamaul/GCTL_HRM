using GCTL.Core.ViewModels;

namespace GCTL.UI.Core.Views.RosterScheduleApproval
{
    public class ApprovalRequest :BaseViewModel
    {
        public List<string> CheckedApprovalList { get; set; }
        public string Remark { get; set; }
    }
}

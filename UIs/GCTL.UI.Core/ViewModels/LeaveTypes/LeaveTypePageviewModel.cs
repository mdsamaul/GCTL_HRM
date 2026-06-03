
using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.LeaveTypes;
namespace GCTL.UI.Core.ViewModels.LeaveTypes
{
    public class LeaveTypePageviewModel:BaseViewModel
    {
      
        public LeaveTypeSetupViewModel Setup { get; set; }=new LeaveTypeSetupViewModel();
        public List<LeaveTypeSetupViewModel> LeaveTypeList { get; set; } = new List<LeaveTypeSetupViewModel>();
    }
}

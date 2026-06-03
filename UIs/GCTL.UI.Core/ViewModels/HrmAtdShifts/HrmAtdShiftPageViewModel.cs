using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HrmAtdShifts;
using GCTL.Core.ViewModels.LeaveTypes;

namespace GCTL.UI.Core.ViewModels.HrmAtdShifts
{
    public class HrmAtdShiftPageViewModel:BaseViewModel
    {
        public HrmAtdShiftSetupViewModel Setup { get; set; } = new HrmAtdShiftSetupViewModel();
        public List<HrmAtdShiftSetupViewModel> HrmAttendenceShiftList { get; set; } = new List<HrmAtdShiftSetupViewModel>();
    }
}

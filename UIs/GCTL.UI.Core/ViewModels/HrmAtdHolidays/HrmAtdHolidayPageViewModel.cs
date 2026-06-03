using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HrmAtdHolidays;
using GCTL.Core.ViewModels.HrmAtdShifts;

namespace GCTL.UI.Core.ViewModels.HrmAtdHolidays
{
    public class HrmAtdHolidayPageViewModel:BaseViewModel
    {
        public HrmAtdHolidaySetupViewModel Setup { get; set; } = new HrmAtdHolidaySetupViewModel();
        public List<HrmAtdHolidaySetupViewModel> HrmAdtHolidayList { get; set; } = new List<HrmAtdHolidaySetupViewModel>();
    }
}

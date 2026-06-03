using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HolidayTypes;
using GCTL.Core.ViewModels.HRMCompanyWeekEnds;

namespace GCTL.UI.Core.ViewModels.HolidayTypes
{
    public class HRMDefHolidayTypePageViewModel:BaseViewModel
    {
        public HRMDefHolidayTypeSetupViewModel Setup { get; set; } = new HRMDefHolidayTypeSetupViewModel();
        public List<HRMDefHolidayTypeSetupViewModel> HRMHolidayTypesList { get; set; } = new List<HRMDefHolidayTypeSetupViewModel>();
    }
}

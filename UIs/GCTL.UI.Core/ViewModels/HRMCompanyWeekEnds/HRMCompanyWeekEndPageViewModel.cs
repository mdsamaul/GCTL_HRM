using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HrmAtdShifts;
using GCTL.Core.ViewModels.HRMCompanyWeekEnds;

namespace GCTL.UI.Core.ViewModels.HRMCompanyWeekEnds
{
    public class HRMCompanyWeekEndPageViewModel:BaseViewModel
    {
        public HRMCompanyWeekEndSetupViewModel Setup { get; set; } = new HRMCompanyWeekEndSetupViewModel();
        public List<HRMCompanyWeekEndSetupViewModel> HRMCompanyWeekEndList { get; set; } = new List<HRMCompanyWeekEndSetupViewModel>();
    }
}

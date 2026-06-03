using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HrmDefInstitutes;

namespace GCTL.UI.Core.ViewModels.HrmDefInstitutes
{
    public class HrmDefInstitutesPageViewModel:BaseViewModel
    {
        public HrmDefInstitutesSetupViewModel Setup { get; set; } = new HrmDefInstitutesSetupViewModel();
        public List<HrmDefInstitutesSetupViewModel> TableListData { get; set; } = new List<HrmDefInstitutesSetupViewModel>();
    }
}

using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.Districts;

namespace GCTL.UI.Core.ViewModels.Districts
{
    public class DistrictsPageViewModel : BaseViewModel
    {
        public DistrictsSetupViewModel Setup { get; set; } = new DistrictsSetupViewModel();
        public List<DistrictsSetupViewModel> DistrictList { get; set; } = new List<DistrictsSetupViewModel>();

    }
}

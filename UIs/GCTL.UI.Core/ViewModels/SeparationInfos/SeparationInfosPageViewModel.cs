using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.SeparationInfos;

namespace GCTL.UI.Core.ViewModels.SeparationInfos
{
    public class SeparationInfosPageViewModel: BaseViewModel
    {
        public SeparationInfosSetupViewModel Setup { get; set; } = new SeparationInfosSetupViewModel();
        public List<SeparationInfosSetupViewModel> TableListData { get; set; } = new List<SeparationInfosSetupViewModel>();
    }
}

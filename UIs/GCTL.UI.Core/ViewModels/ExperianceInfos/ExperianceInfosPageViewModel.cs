using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.ExperianceInfos;

namespace GCTL.UI.Core.ViewModels.ExperianceInfos
{
    public class ExperianceInfosPageViewModel: BaseViewModel
    {
        public ExperianceInfosSetupViewModel Setup { get; set; } = new ExperianceInfosSetupViewModel();
        public List<ExperianceInfosSetupViewModel> TableListData { get; set; } = new List<ExperianceInfosSetupViewModel>();
    }
}

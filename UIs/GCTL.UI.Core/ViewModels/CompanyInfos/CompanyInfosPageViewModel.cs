using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.CompanyInfos;

namespace GCTL.UI.Core.ViewModels.CompanyInfos
{
    public class CompanyInfosPageViewModel: BaseViewModel
    {
        public CompanyInfosSetupViewModel Setup { get; set; } = new CompanyInfosSetupViewModel();
        public List<CompanyInfosSetupViewModel> TableListData { get; set; } = new List<CompanyInfosSetupViewModel>();
    }
}

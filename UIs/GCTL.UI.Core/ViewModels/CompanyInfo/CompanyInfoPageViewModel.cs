using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.CompanyInfo;
using GCTL.Core.ViewModels.PaymentTerms;

namespace GCTL.UI.Core.ViewModels.CompanyInfo
{
    public class CompanyInfoPageViewModel : BaseViewModel
    {
        public CompanyInfoSetupViewModel Setup { get; set; } = new CompanyInfoSetupViewModel();
        public List<CompanyInfoSetupViewModel> CompanyInfoList { get; set; } = new List<CompanyInfoSetupViewModel>();
    }
}


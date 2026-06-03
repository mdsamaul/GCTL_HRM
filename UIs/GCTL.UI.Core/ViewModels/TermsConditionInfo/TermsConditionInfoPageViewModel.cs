using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.TermsConditionInfo;

namespace GCTL.UI.Core.ViewModels.TermsConditionInfo
{
    public class TermsConditionInfoPageViewModel : BaseViewModel
    {
        public TermsConditionInfoSetupViewModel Setup { get; set; } = new TermsConditionInfoSetupViewModel();
        public List<TermsConditionInfoSetupViewModel> TermsConditionList { get; set; } = new List<TermsConditionInfoSetupViewModel>();
    }
}

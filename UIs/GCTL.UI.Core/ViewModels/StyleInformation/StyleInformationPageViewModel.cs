using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.StyleInformation;

namespace GCTL.UI.Core.ViewModels.StyleInformation
{
    public class StyleInformationPageViewModel : BaseViewModel
    {
        public StyleInformationSetupViewModel Setup { get; set; } = new StyleInformationSetupViewModel();
        public List<StyleInformationSetupViewModel> StyleList { get; set; } = new List<StyleInformationSetupViewModel>();
    }
}

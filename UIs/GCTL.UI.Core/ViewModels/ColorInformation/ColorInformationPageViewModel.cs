using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.ColorInformation;
using GCTL.Core.ViewModels.StyleInformation;

namespace GCTL.UI.Core.ViewModels.ColorInformation
{
    public class ColorInformationPageViewModel : BaseViewModel
    {
        public ColorInformationSetupViewModel Setup { get; set; } = new ColorInformationSetupViewModel();
        public List<ColorInformationSetupViewModel> ColorList { get; set; } = new List<ColorInformationSetupViewModel>();
    }
}

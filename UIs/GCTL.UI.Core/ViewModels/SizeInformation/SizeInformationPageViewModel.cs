using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.ColorInformation;
using GCTL.Core.ViewModels.SizeInformation;

namespace GCTL.UI.Core.ViewModels.SizeInformation
{
    public class SizeInformationPageViewModel : BaseViewModel
    {
        public SizeInformationSetupViewModel Setup { get; set; } = new SizeInformationSetupViewModel();
        public List<SizeInformationSetupViewModel> SizeList { get; set; } = new List<SizeInformationSetupViewModel>();
    }
}

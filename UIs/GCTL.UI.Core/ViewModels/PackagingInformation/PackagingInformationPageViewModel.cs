using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.Nationalitys;
using GCTL.Core.ViewModels.PackagingInformation;

namespace GCTL.UI.Core.ViewModels.PackagingInformation
{
    public class PackagingInformationPageViewModel : BaseViewModel
    {
        public PackagingInformationSetupViewModel Setup { get; set; } = new PackagingInformationSetupViewModel();
        public List<PackagingInformationSetupViewModel> PackagingList { get; set; } = new List<PackagingInformationSetupViewModel>();
    }
}

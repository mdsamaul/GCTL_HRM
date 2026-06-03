using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.PackageType;

namespace GCTL.UI.Core.ViewModels.PackageType
{
    public class PackageTypePageViewModel : BaseViewModel
    {
        public PackageTypeSetupViewModel Setup { get; set; } = new PackageTypeSetupViewModel();
        public List<PackageTypeSetupViewModel> PackageList { get; set; } = new List<PackageTypeSetupViewModel>();
    }
}

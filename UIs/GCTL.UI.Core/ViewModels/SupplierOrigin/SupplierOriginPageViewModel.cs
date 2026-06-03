using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.SupplierOrigin;

namespace GCTL.UI.Core.ViewModels.SupplierOrigin
{
    public class SupplierOriginPageViewModel : BaseViewModel
    {
        public SupplierOriginSetupViewModel Setup { get; set; } = new SupplierOriginSetupViewModel();
        public List<SupplierOriginSetupViewModel> SupplierOriginList { get; set; } = new List<SupplierOriginSetupViewModel>();
    }
}

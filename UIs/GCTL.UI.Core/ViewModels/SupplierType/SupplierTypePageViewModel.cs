using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.SupplierType;

namespace GCTL.UI.Core.ViewModels.SupplierType
{
    public class SupplierTypePageViewModel : BaseViewModel
    {
        public SupplierTypeSetupViewModel Setup { get; set; } = new SupplierTypeSetupViewModel();
        public List<SupplierTypeSetupViewModel> SupplierTypeList { get; set; } = new List<SupplierTypeSetupViewModel>();
    }
}

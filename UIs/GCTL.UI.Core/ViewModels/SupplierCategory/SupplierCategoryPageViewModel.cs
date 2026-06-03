using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.SupplierCategory;

namespace GCTL.UI.Core.ViewModels.SupplierCategory
{
    public class SupplierCategoryPageViewModel : BaseViewModel
    {
        public SupplierCategorySetupViewModel Setup { get; set; } = new SupplierCategorySetupViewModel();
        public List<SupplierCategorySetupViewModel> SupplierCategoryList { get; set; } = new List<SupplierCategorySetupViewModel>();
    }
}


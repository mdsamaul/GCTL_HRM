using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.SupplierInformation;

namespace GCTL.UI.Core.ViewModels.SupplierInformation
{
    public class SupplierInformationPageViewModel : BaseViewModel
    {
        public SupplierInformationSetupViewModel Setup { get; set; } = new SupplierInformationSetupViewModel();
        public List<SupplierInformationSetupViewModel> SupplierInformationList { get; set; } = new List<SupplierInformationSetupViewModel>();
      
    }
}

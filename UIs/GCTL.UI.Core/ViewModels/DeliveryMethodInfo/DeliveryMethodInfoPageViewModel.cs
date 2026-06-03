using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.CoreBankAccountInformations;
using GCTL.Core.ViewModels.DeliveryMethodInfo;

namespace GCTL.UI.Core.ViewModels.DeliveryMethodInfo
{
    public class DeliveryMethodInfoPageViewModel : BaseViewModel
    {
        public DeliveryMethodInfoSetupViewModel Setup = new DeliveryMethodInfoSetupViewModel();
        public List<DeliveryMethodInfoSetupViewModel> DeliveryMethodList = new List<DeliveryMethodInfoSetupViewModel>();
    }
}

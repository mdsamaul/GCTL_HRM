using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.PaymentModes;

namespace GCTL.UI.Core.ViewModels.PaymentModes
{
    public class PaymentModePageViewModel : BaseViewModel
    {
        public PaymentModeSetupViewModel Setup { get; set; } = new PaymentModeSetupViewModel();
    }
}

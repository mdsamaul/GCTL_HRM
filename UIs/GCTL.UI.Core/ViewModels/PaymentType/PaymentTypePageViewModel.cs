using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.PaymentType;

namespace GCTL.UI.Core.ViewModels.PaymentType
{
    public class PaymentTypePageViewModel : BaseViewModel
    {
        public PaymentTypeSetupViewModel Setup { get; set; } = new PaymentTypeSetupViewModel();
        public List<PaymentTypeSetupViewModel> PaymentList { get; set; } = new List<PaymentTypeSetupViewModel>();
    }
}

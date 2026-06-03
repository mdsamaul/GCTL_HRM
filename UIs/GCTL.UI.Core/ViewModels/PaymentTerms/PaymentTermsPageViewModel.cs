using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.PaymentTerms;
using GCTL.Core.ViewModels.PaymentType;

namespace GCTL.UI.Core.ViewModels.PaymentTerms
{
    public class PaymentTermsPageViewModel : BaseViewModel
    {
        public PaymentTermsSetupViewModel Setup { get; set; } = new PaymentTermsSetupViewModel();
        public List<PaymentTermsSetupViewModel> PaymentTermsList { get; set; } = new List<PaymentTermsSetupViewModel>();
    }
}
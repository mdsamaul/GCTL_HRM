using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.PaymentModes
{
    public class PaymentModeSetupViewModel : BaseViewModel
    {
        public string PaymentModeId { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        public string PaymentModeName { get; set; }
        public string PaymentModeShortName { get; set; }
    }
}

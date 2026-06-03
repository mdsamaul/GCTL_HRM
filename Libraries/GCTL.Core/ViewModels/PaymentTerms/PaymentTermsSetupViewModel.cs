using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.PaymentTerms
{
    public class PaymentTermsSetupViewModel : BaseViewModel
    {
        public int Tc { get; set; }
        public string PaymentTermsId { get; set; }

        public string PaymentTermsName { get; set; }
        [Required(ErrorMessage = "Style is required")]
        public string Percentise { get; set; }
        [Required(ErrorMessage = "Style is required")]
        public string Type { get; set; }
        public int? CreditDays { get; set; }
        public string PaymentType { get; set; }
    }
}

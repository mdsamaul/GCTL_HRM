using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.PaymentType
{
    public class PaymentTypeSetupViewModel : BaseViewModel
    {
        public int Tc { get; set; }
        public string PaymentTypeId { get; set; }
        [Required(ErrorMessage = "Payment Type is required")]
        public string PaymentType { get; set; }
        public string ShortName { get; set; }
    }
}

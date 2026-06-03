using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.DeliveryMethodInfo
{
    public class DeliveryMethodInfoSetupViewModel : BaseViewModel
    {
        public decimal Tc { get; set; }
        public string DeliveryMethodId { get; set; }
        [Required(ErrorMessage = "Delivery Method is required")]
        public string DeliveryMethod { get; set; }
        public string Detail { get; set; }
        public string EmployeeId { get; set; }
        public string CompanyId { get; set; }
    }
}

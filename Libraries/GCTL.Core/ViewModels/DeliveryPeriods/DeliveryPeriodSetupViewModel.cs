using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.DeliveryPeriods
{
    public class DeliveryPeriodSetupViewModel : BaseViewModel
    {
        public int AutoId { get; set; }
        public string DeliveryPeriodId { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Delivery Period")]
        public string DeliveryPeriod { get; set; }

        [Display(Name = "Short Name")]
        public string ShortName { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.Currencies
{
    public class CurrencySetupViewModel : BaseViewModel
    {
        public decimal Tc { get; set; }
        public string CurrencyId { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Currency Name")]
        public string CurrencyName { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Short Name")]
        public string ShortName { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        public string Symbol { get; set; }

        [Display(Name = "Decimal Places")]
        public int? DecimalPlaces { get; set; }

        [Display(Name = "Negative Format")]
        public string NegativeFormat { get; set; }
    }
}

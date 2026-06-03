using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.UnitTypes
{
    public class UnitTypeSetupViewModel : BaseViewModel
    {
        public int Tc { get; set; }
        public string UnitTypId { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Unit Type Name")]
        public string UnitTypeName { get; set; }

        [Display(Name = "Unit Type Short Name")]
        public string ShortName { get; set; }

        [Display(Name = "Decimal Places")]
        public int? DecimalPlaces { get; set; }
    }
}

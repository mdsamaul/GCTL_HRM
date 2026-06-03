using GCTL.Core.Enums;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace GCTL.Core.ViewModels.Units
{
    public class MeasurementUnitSetupViewModel : BaseViewModel
    {
        public int AutoId { get; set; }
        public UnitType UnitType { get; set; }
        public string UnitId { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Name")]
        public string UnitName { get; set; }
        public string ShortName { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.Units
{
    public class UnitSetupViewModel : BaseViewModel
    {
        public int AutoId { get; set; }
        public string UnitId { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Name")]
        public string UnitName { get; set; }
        public string ShortName { get; set; }
    }
}

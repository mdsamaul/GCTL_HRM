using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.Religions
{
    public class ReligionSetupViewModel : BaseViewModel
    {
        public string ReligionCode { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Religion Name")]
        public string Religion { get; set; }

        [Display(Name = "Religion Short Name")]
        public string ShortName { get; set; }
    }
}

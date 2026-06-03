using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.DoctorWorkingPlace
{
    public class DoctorWorkingPlaceSetupViewModel : BaseViewModel
    {
        public string WorkingPlaceCode { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Working Place Name")]
        public string WorkingPlaceName { get; set; }
        [Display(Name = "Short Name")]
        public string WorkingPlaceShortName { get; set; }
        [Display(Name = "Bangla Working Palace ")]
        public string BanglaWorkingPlace { get; set; }
    }
}

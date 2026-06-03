using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.Specialities
{
    public class SpecialitySetupViewModel : BaseViewModel
    {
        public string AutoId { get; set; }
        public string SpecialityCode { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Speciality Name")]
        public string SpecialityName { get; set; }

        [Display(Name = "Speciality Short Name")]
        public string SpecialityShortName { get; set; }
        public string BanglaSpeciality { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.Qualifications
{
    public class QualificationSetupViewModel : BaseViewModel
    {
        public string AutoId { get; set; }
        public string QualificationCode { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Qualifcation Name")]
        public string QualificationName { get; set; }

        [Display(Name = "Qualifcation Short Name")]
        public string QualificationShortName { get; set; }
        public string BanglaQualification { get; set; }
    }
}

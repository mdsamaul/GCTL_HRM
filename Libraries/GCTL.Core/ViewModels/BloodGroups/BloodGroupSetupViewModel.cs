using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.BloodGroups
{
    public class BloodGroupSetupViewModel : BaseViewModel
    {
        public int Tc { get; set; }
        public string BloodGroupCode { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Blood Group")]
        public string BloodGroup { get; set; }

        [Display(Name = "Blood Group (বাংলা)")]
        public string BanglaBloodGroup { get; set; }
    }
}

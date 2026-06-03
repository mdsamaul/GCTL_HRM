using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.AdmissionTypes
{
    public class AdmissionTypeSetupViewModel : BaseViewModel
    {
        public int AutoId { get; set; }


        [Display(Name = "Admission Type Id")]
        public string AdmissionTypeId { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Admission Type")]
        public string AdmissionTypeName { get; set; }

        [Display(Name = "Short Name")]
        public string ShortName { get; set; }

        [Display(Name = "Short Name (বাংলা)")]
        public string BanglaShortName { get; set; }     
    }
}

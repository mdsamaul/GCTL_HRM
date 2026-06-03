using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.DoctorTypes
{
    public class DoctorTypeSetupViewModel : BaseViewModel
    {
        public string DoctorTypeId { get; set; }
        public string DoctorTypeCode { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Doctor Type Name")]
        public string DoctorTypeName { get; set; }

        [Display(Name = "Doctor Type Short Name")]
        public string DoctorTypeShortName { get; set; }
    }
}

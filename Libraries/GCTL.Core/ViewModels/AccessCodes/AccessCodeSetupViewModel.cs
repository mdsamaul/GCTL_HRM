using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.AccessCodes
{
    public class AccessCodeSetupViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Access Code Id")]
        public string AccessCodeId { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Access Code Name")]
        public string AccessCodeName { get; set; }

        public List<AccessCodeModel> Accesses { get; set; } = new List<AccessCodeModel>();
    }
}

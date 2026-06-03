using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.Surnames
{
    public class SurnameSetupViewModel : BaseViewModel
    {
        public decimal AutoId { get; set; }               

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Surname ID")]
        public string SurnameId { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Surname")]
        public string Surname { get; set; }

        [Display(Name = "Surname (বাংলা)")]
        public string BanglaSurname { get; set; }     
    }
}

using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.BranchesTypeInfo
{
    public class BranchTypeSetupViewModel : BaseViewModel
    {

        public string BranchCode { get; set; }

        public string CompanyCode { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        //[Display(Name = "Company Name")]
        public string Company { get; set; } = string.Empty;

        public string CompanyName { get; set; } = string.Empty;

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Branch Name")]
        public string BranchName { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Branch Name (বাংলা)")]
        public string BanglaBranch { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [Display(Name = "Address ")]
        public string Address { get; set; }

        //[Required(ErrorMessage = "Address is required.")]
        [Display(Name = "Address (বাংলা)")]
        public string AddressBangla { get; set; }

        //[Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Phone ")]
        [RegularExpression(@"^(?:(?:\+|00)88|01)?\d{11}$", ErrorMessage = "Invalid Phone")]
        [MaxLength(14)]
        public string Phone { get; set; }

        [EmailAddress(ErrorMessage = "Please enter valid email")]
        public string Email { get; set; }

        [Display(Name = "Fax")]
        public string Fax { get; set; }

        //[RegularExpression(@"^[\u0980-\u09FF\s]+$", ErrorMessage = "Please enter in Bangla.")]
        //public string BanglaBranch { get; set; }
        //[RegularExpression(@"^[\u0980-\u09FF\s]+$", ErrorMessage = "Please enter in Bangla.")]
        //public string AddressBangla { get; set; }
    }
}

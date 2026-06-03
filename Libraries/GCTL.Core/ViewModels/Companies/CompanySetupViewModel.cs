using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.Companies
{
    public class CompanySetupViewModel : BaseViewModel
    {
        public string CompanyCode { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        public string CompanyShortName { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [Display(Name = "Address 1")]
        public string Address1 { get; set; }

        public string Address2 { get; set; }


        //[Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Phone 1")]
        [RegularExpression(@"^(?:(?:\+|00)88|01)?\d{11}$", ErrorMessage = "Invalid Phone")]
        [MaxLength(14)]
        public string Phone1 { get; set; }

        [Display(Name = "Phone 2")]
        [RegularExpression(@"^(?:(?:\+|00)88|01)?\d{11}$", ErrorMessage = "Invalid Phone")]

        public string Phone2 { get; set; }
        public string Fax { get; set; }

        [RegularExpression(@"^(?:(?:\+|00)88|01)?\d{11}$", ErrorMessage = "Hotline/Customer Care No")]
        public string HotLine { get; set; }

        [Display(Name = "Website")]
        [Url()]
        public string Url { get; set; }

        [EmailAddress(ErrorMessage = "Please enter valid email")]
        public string Email { get; set; }

        public string ImgTitle { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string RegNo { get; set; }
        public string Tin { get; set; }
        public string BackImage { get; set; }
        public string BaseCurrency { get; set; }
        public string CompanyBanglaName { get; set; }
        public string AddressBangla { get; set; }
    }
}

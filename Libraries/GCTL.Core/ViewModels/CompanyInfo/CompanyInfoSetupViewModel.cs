using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.CompanyInfo
{
    public class CompanyInfoSetupViewModel : BaseViewModel
    {
        public int Tc { get; set; }
        public string CompanyId { get; set; }
        [Required(ErrorMessage = "Company Name is required")]
        public string CompanyName { get; set; }
        public string ShortName { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string CompanyAddress { get; set; }
        public string ZipCode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string CountryId { get; set; }
        [RegularExpression(@"^(?:(?:\+|00)88|01)?\d{11}$", ErrorMessage = "Invalid Phone")]
        [MaxLength(14)]
        public string Phone { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9]+$", ErrorMessage = "Only letters and numbers are allowed")]
        public string Fax { get; set; }
        [EmailAddress(ErrorMessage = "Please enter valid email")]
        public string Email { get; set; }
        public string Url { get; set; }
        public string Remarks { get; set; }
        public string CompanyCode { get; set; }
        public string EmployeeId { get; set; }
        public string LocalOfficeAddress { get; set; }
        [Required(ErrorMessage = "Company For is required")]
        public string CompanyForId { get; set; }
        public string CompanyForName { get; set; }
    }
}

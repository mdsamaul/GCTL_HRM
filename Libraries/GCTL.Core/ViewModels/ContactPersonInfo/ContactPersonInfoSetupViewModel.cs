using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.ContactPersonInfo
{
    public class ContactPersonInfoSetupViewModel : BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string Cpid { get; set; }
        [Required(ErrorMessage = "Contact Person Name is required")]
        public string ContactPersonName { get; set; }
        public string DesignationCode { get; set; }
        public string DesignationName { get; set; }
        [RegularExpression(@"^(?:\+8801\d{9}|01\d{9})$", ErrorMessage = "Invalid Phone")]
        [MaxLength(14)]
        public string ContactPersonMobile { get; set; }
        [EmailAddress(ErrorMessage = "Please enter valid email")]
        public string ContactPersonEmail { get; set; }
        public string CompanyCode { get; set; }
        public string EmployeeId { get; set; }
    }
}

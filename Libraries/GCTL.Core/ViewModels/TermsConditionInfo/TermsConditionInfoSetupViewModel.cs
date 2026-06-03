using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.TermsConditionInfo
{
    public class TermsConditionInfoSetupViewModel : BaseViewModel
    {
        public decimal Tc { get; set; }
        public string TermsConditionId { get; set; }
        [Required(ErrorMessage = "Terms Condition Name is required")]
        public string TermsConditionName { get; set; }
        public string EmployeeId { get; set; }
        public string CompanyId { get; set; }
    }
}

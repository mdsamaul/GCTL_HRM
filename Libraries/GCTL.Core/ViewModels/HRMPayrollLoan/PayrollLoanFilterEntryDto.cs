using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRMPayrollLoan
{
    public class PayrollLoanFilterEntryDto :BaseViewModel
    {
        public List<string> CompanyCodes { get; set; }
        public List<string> EmployeeIds { get; set; }
    }
}

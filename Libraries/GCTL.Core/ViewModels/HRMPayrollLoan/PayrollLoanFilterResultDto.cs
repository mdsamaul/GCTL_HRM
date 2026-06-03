using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRMPayrollLoan
{
    public class PayrollLoanFilterResultDto :BaseViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string EmpId { get; set; }
        public string EmpName { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public string joinDate { get; set; }
        
    }
}

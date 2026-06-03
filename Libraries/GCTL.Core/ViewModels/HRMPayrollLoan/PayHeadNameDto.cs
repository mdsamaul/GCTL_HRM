using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRMPayrollLoan
{
    public class PayHeadNameDto
    {
        public decimal PayHeadNameCode { get; set; }
        public string PayHeadNameId { get; set; }
        public string Name { get; set; }
        public DateTime Wef { get; set; }
        public string LoanTypeId { get; set; }
    }
}

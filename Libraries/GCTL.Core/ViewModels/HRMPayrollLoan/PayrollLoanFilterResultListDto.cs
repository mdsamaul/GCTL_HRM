using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRMPayrollLoan
{
    public class PayrollLoanFilterResultListDto
    {
        public List<PayrollLoanFilterResultDto> Company { get; set; }
        public List<PayrollLoanFilterResultDto> Employees { get; set; }
        public List<HrmPayrollPaymentReceiveDto> PaymentReceiveEmployee { get; set; } = new List<HrmPayrollPaymentReceiveDto>();
    }
}

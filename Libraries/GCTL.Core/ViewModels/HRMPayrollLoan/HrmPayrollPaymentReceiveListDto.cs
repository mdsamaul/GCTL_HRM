using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRMPayrollLoan
{
    public class HrmPayrollPaymentReceiveListDto
    {
        public List<HrmPayrollPaymentReceiveDto> hrmPayrollPaymentReceiveDtos { get; set; } = new List<HrmPayrollPaymentReceiveDto>();
        public List<PayrollLoanFilterResultDto> PayrollLoanFilterResultDto { get; set; } = new List<PayrollLoanFilterResultDto>();
    }
}

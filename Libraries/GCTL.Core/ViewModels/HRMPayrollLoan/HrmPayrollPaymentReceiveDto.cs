using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRMPayrollLoan
{
    public class HrmPayrollPaymentReceiveDto :BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string LoanId { get; set; }
        public string PaymentId { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string ShowPaymentDate { get; set; }
        public decimal? PaymentAmount { get; set; }
        public string PaymentMode { get; set; }
        public string ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }
        public string? ShowChequeDate { get; set; }
        public string BankName { get; set; }      
        public string BankAccount { get; set; }
        public string Remarks { get; set; }
        public string EmpId { get; set; }
        public string EmpName { get; set; }
        public string Designation{ get; set; }
        public string BankId{ get; set; }
        public string ShowCreateDate { get; set; }
        public string ShowModifyDate { get; set; }
    }
}

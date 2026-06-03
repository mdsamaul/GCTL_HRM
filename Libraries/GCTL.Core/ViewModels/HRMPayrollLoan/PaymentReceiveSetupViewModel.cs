using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRMPayrollLoan
{
    public class PaymentReceiveSetupViewModel :BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string LoanId { get; set; }
        public string EmployeeId { get; set; }
        public string LoanTypeId { get; set; }
        public decimal LoanAmount { get; set; }
        public string NoOfInstallment { get; set; }
        public decimal MonthlyDeduction { get; set; }
        public string PayHeadNameId { get; set; }
        public string PaymentModeId { get; set; }
        public string ChequeNo { get; set; }
        public string BankId { get; set; }
        public string BankAccount { get; set; }
        public string Remarks { get; set; }
        public string CompanyCode { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ChequeDate { get; set; } 
        public string PaymentId { get; set; }
    }
}

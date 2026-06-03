using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRMPayrollLoan
{
    public class HRMPayrollLoanSetupViewModel:BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string LoanId { get; set; }
        public string EmployeeId { get; set; }
        public DateTime? LoanDate { get; set; }
        public string LoanTypeId { get; set; }
        public string LoanTypeName{ get; set; }
        public DateTime? StartDate { get; set; }
        public string StartShowDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? EndShowDate { get; set; }
        public decimal? LoanAmount { get; set; }
        public decimal? paymentLoanAmount { get; set; }
        public DateTime? paymentLoanDate { get; set; }
        public string NoOfInstallment { get; set; }
        public decimal? MonthlyDeduction { get; set; }
        public string PayHeadNameId { get; set; }
        public string PaymentModeId { get; set; }
        public string PaymentModeName { get; set; }
        public string ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }
        public string BankId { get; set; }
        public string BankAccount { get; set; }
        public string Remarks { get; set; }       
        public string ReasonOfLoanTaken { get; set; }       
        public string CompanyCode { get; set; }
        public string ShowLoanDate { get; set; }
        public string EmpName { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public string ShowJoiningDate { get; set; }
        public string showCreateDate { get; set; }
        public string showModifyDate { get; set; }
       
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.AdvanceLoanAdjustment
{
    public class AdvanceLoanAdjustmentSetupViewModel:BaseViewModel
    {
        public decimal AdvancePayCode { get; set; }
        public string AdvancePayId { get; set; }
        public string EmployeeID { get; set; }
        public string AdvanceAdjustStatus { get; set; }
        public decimal AdvanceAmount { get; set; }
        public decimal MonthlyDeduction { get; set; }
        public string SalaryMonth { get; set; }
        public string SalaryYear { get; set; }
        public string NoOfPaymentInstallment { get; set; }
        public string PayHeadNameId { get; set; }
        public string Remarks { get; set; }        
        public DateTime? ModifyDate { get; set; }
        public string CompanyCode { get; set; }
        public string? AdjustmentType { get; set; }
        public string? LoanID { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? Todate { get; set; }
    }
}

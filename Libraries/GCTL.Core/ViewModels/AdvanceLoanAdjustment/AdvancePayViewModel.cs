using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.AdvanceLoanAdjustment
{
    public class AdvancePayViewModel
    {
             
        public string AdvancePayId { get; set; } 
        public string EmployeeID { get; set; } 
        public string FullName { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationName { get; set; }
        public decimal AdvanceAmount { get; set; }
        public decimal? MonthlyDeduction { get; set; }
        public string SalaryMonth { get; set; }
        public string SalaryYear { get; set; }
        public string NoOfPaymentInstallment { get; set; } 
        public string PayHeadNameId { get; set; }           
        public string Remarks { get; set; }
        public string LoanID { get; set; }                 
        public string AdjustmentType { get; set; }                
        public string AdvanceAdjustStatus { get; set; }                
        public decimal AdvancePayCode { get; set; }
        public string JoiningDate { get; set; }
        public string LoanDate { get; set; }
        public string LoanTypeId { get; set; }
        public string LoanTypeName { get; set; }
        public string LoanStartDate { get; set; }
        public string LoanEndDate { get; set; }
        public string CreateDate { get; set; }
        public string ModifyDate { get; set; }
    }

}

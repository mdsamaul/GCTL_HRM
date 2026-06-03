using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.EmployeeLoanInformationReport
{
    public class EmployeeLoanInformationReportVM : BaseViewModel
    {
       

        public string LoanID { get; set; }
        public string EmployeeID { get; set; }
        public string FullName { get; set; }
        public string Remarks { get; set; }
        public string DepartmentName { get; set; }
        public string CompanyName { get; set; }
        public string DesignationName { get; set; }
        public string Reason { get; set; }
        public decimal TotalLoans { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal MonthlyDeduction { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string InstallmentDetails { get; set; }
        public string PaymentMode { get; set; }
        public string LoanRepaymentMethod { get; set; }

        public List<InstallmentVM> Installments { get; set; } = new();
    }
}

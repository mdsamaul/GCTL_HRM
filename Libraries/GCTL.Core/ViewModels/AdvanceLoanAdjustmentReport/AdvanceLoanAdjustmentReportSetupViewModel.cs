using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.AdvanceLoanAdjustmentReport
{
    public class AdvanceLoanAdjustmentReportSetupViewModel :BaseViewModel
    {
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationCode { get; set; }
        public string DesignationName { get; set; }
        public string EmployeeID { get; set; }
        public string FullName { get; set; }
        public string PayHeadNameId { get; set; }
        public string PayHeadName { get; set; }
        public string MonthId { get; set; }
        public string MonthName { get; set; }
        public int SalaryYear { get; set; }
        public int TotalInThisDepartment { get; set; }
        public decimal AdvanceAmount { get; set; }
        public decimal MonthlyDeduction { get; set; }
        public string Remarks { get; set; }
    }
}

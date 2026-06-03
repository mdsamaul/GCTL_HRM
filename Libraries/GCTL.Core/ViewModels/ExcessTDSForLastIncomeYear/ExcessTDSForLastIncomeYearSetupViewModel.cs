using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.ExcessTDSForLastIncomeYear
{
    public class ExcessTDSForLastIncomeYearSetupViewModel : BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string Etdsliyid { get; set; }
        public string EmployeeId { get; set; }
        public decimal Tdsamount { get; set; }
        public string SalaryMonth { get; set; }
        public string SalaryYear { get; set; }
        public string ApprovedStatus { get; set; }
        public string Remark { get; set; }
        public string Name { get; set; }
        public string CompanyCode { get; set; }
        public string FinancialCodeNo { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string IsfullAmountAdjust { get; set; }
        public string CompanyName { get; set; }
        public string EmployeeName { get; set; }
        public string DesignationCode { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public List<string> SelectedEmployeeIds { get; set; } = new List<string>();
    }
}

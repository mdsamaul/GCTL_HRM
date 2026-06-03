using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.TaxChallanEntry
{
    public class HrmPayMonthlyTaxDepositEntryDto: BaseViewModel
    {
        public decimal TaxDepositCode { get; set; } = 0m;
        public string? TaxDepositId { get; set; }
        public string? EmployeeId { get; set; }
        public string? EmpName { get; set; }
        public string? FinancialCodeNo { get; set; }
        public string? ShowFinancialCodeNo { get; set; }
        public decimal? TaxDepositAmount { get; set; }
        public string? SalaryMonth { get; set; }
        public string? SalaryMonthName { get; set; }
        public string? SalaryYear { get; set; }
        public string? TaxChallanNoPrefix { get; set; }
        public string? TaxChallanNo { get; set; }
        public DateTime? TaxChallanDate { get; set; }
        public string? ShowTaxChallanDate { get; set; }
        public string? BankId { get; set; }
        public string? BankBranchId { get; set; }
        public string? ApprovedStatus { get; set; }
        public string? Remark { get; set; }
        public string? CompanyCode { get; set; }
        public decimal? ChallanAmount { get; set; }
        public List<string>? EmployeeIds{ get; set; }
        public string? DesignationCode { get; set; }
        public string? DesignationName { get; set; }
        public string? ShowCreateDate { get; set; }
        public string? ShowModifyDate { get; set; }
    }
}

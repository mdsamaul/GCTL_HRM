using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.ExcessTDSForLastIncomeYear
{
    public class ExcessTDSForLastIncomeYearFilterResultDto
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string CompanyName { get; set; }
        public string EmpId { get; set; }
        public string DesignationName { get; set; }
        public string BranchName { get; set; }
        public string DepartmentName { get; set; }
        public string EmployeeStatus { get; set; }
        public string? EmployeeType { get; set; }
        public string EmploymentNatureId { get; set; }
        public string EmploymentNature { get; set; }
        public DateTime? ConfirmeDate { get; set; }
        public DateTime Date { get; set; }
        public DateTime? JoiningDate { get; set; }
        public string EmployeeId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRM_EmployeeWeekendDeclaration
{
    public class EmployeeFilterDataDto
    {
        public string? EmpId { get; set; }
        public string EmpFName { get; set; }
        public string EmpLName { get; set; }
        public DateTime? JoiningDate { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string DesignationCode { get; set; }
        public string DesignationName { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public string EmployeeTypeName { get; set; }
        public string? EmployeeStatusId { get; set; }
        public string EmployeeStatus { get; set; }
    }
}

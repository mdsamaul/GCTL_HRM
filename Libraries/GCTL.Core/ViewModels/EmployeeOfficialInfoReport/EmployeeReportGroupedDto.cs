using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.EmployeeOfficialInfoReport
{
    public class EmployeeOfficialInfoDto
    {
        public string EmployeeID { get; set; }
        public string EmpName { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public string BranchName { get; set; }
        public string EmploymentNature { get; set; }
        public string EmpTypeName { get; set; }
        public string JoiningDate { get; set; }
        public string SeparationDate { get; set; }
        public string ServiceLength { get; set; }
        public string ShiftName { get; set; }
        public string ImmediateSupervisorName { get; set; }
        public string HeadOfDepartmentName { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string EmployeeStatus { get; set; }
    }

    public class DepartmentEmployeeGroupDto
    {
        public string DepartmentName { get; set; }
        public List<EmployeeOfficialInfoDto> Employees { get; set; } = new();
        public int TotalCount => Employees.Count;
    }

    public class EmployeeReportGroupedDto
    {
        public List<DepartmentEmployeeGroupDto> DepartmentGroups { get; set; } = new();
        public int GrandTotal => DepartmentGroups.Sum(d => d.TotalCount);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.EmployeeOfficialInfoReport
{
    public class OfficialInfoReportDto
    {
        public string Id { get; set; }
        public string EmpName { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentName { get; set; }
        public string BranchName { get; set; }
        public string EmpTypeName { get; set; }
        public string EmploymentNature { get; set; }
        public string EmployeeStatus { get; set; }
        public decimal? GrossSalary { get; set; }
        public DateTime? JoiningDate { get; set; }
    }

}

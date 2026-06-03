using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.ManualEntryApproval
{
    public class ManualEntryApprovalFilterResultDto
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string CompanyName { get; set; }
        public string EmpId { get; set; }
        public string ManualId { get; set; }
        public string DesignationName { get; set; }
        public string BranchName { get; set; }
        public string DivisionName { get; set; }
        public string DepartmentName { get; set; }
        public string EmployeeStatus { get; set; }
        public string AttandanceType { get; set; }
        public DateTime Date { get; set; }
        public string ShowDate { get; set; }
        public string Time { get; set; }
        public string ShowTime { get; set; }
        public string Remark { get; set; }
    }
}

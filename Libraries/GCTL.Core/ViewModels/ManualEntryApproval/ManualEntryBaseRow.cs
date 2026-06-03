using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.ManualEntryApproval
{
    public class ManualEntryBaseRow
    {
        public string ManualCode { get; set; }

        public string? EmpId { get; set; }
        public string EmpName { get; set; }

        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }

        public string BranchCode { get; set; }
        public string BranchName { get; set; }

        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }

        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }

        public string DesignationCode { get; set; }
        public string DesignationName { get; set; }

        public string? EmployeeStatusCode { get; set; }
        public string EmployeeStatusName { get; set; }

        public string AttendanceType { get; set; }

        public DateTime Date { get; set; }
        public DateTime? Time { get; set; }

        public string Remark { get; set; }
    }

}

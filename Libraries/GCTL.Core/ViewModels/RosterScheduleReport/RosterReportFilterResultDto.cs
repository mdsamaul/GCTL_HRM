using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.RosterScheduleReport
{
    public class RosterReportFilterResultDto
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string CompanyName { get; set; }
        public string EmpId { get; set; }
        public string DesignationName { get; set; }
        public string BranchName { get; set; }
        public string DivisionName { get; set; }
        public string DepartmentName { get; set; }
        public string EmployeeStatus { get; set; }
        public string RosterScheduleId { get; set; }
        public DateTime Date { get; set; }
        public string ShowDate { get; set; }
        public string ShiftName { get; set; }
        public string DayName { get; set; }
        public string EmployeeID { get; set; }
        public string Remark { get; set; }
        public string? EmployeeType { get; set; }
        public string ApprovalStatus { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovalDatetime { get; set; }
        public string? ShowApprovalDatetime { get; set; }
        public string? Luser { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
    }
}

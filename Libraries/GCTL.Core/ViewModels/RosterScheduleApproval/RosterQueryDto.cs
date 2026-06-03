using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.RosterScheduleApproval
{
    public class RosterQueryDto
    {
        public string EmpId { get; set; }
        public string EmpName { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }
        public string DesignationCode { get; set; }
        public string DesignationName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string BranchName { get; set; }
        public string CompanyName { get; set; }
        public string EmployeeStatusCode { get; set; }
        public DateTime Date { get; set; }
        public string RosterId { get; set; }
        public string ShiftCode { get; set; }
        public string ShiftName { get; set; }
        public string Remark { get; set; }
        public string EmpStatus { get; set; }
    }
}

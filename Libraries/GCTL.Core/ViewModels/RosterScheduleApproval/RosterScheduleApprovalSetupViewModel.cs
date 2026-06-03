using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.RosterScheduleApproval
{
    public class RosterScheduleApprovalSetupViewModel :BaseViewModel
    {
        public decimal Tc { get; set; }
        public string RosterScheduleApprovalId { get; set; }
        public string EmployeeId { get; set; }
        public List<string> checkedApprovalList { get; set; } = new List<string>();
        public DateTime Date { get; set; }
        public string ShiftCode { get; set; }
        public string Weekend { get; set; }
        public string Remark { get; set; }       
        public string CompanyCode { get; set; }
        public string EmployeeIdSao { get; set; }
        public string ModifyBy { get; set; }
        public string ApprovalStatus { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovalDatetime { get; set; }
        public string RosterScheduleId { get; set; }
        public string empName { get; set; }
        public string DegiName { get; set; }
        public string ShiftName { get; set; }
    }
}

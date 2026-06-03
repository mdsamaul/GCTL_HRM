using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.RosterScheduleEntry
{
    public class RosterScheduleEntrySetupViewModel : BaseViewModel
    {
        public decimal TC { get; set; }
        public string  RosterScheduleId { get; set; }
        public string  EmployeeID { get; set; }
        public List<string>  EmployeeListID { get; set; }
        public DateTime  Date { get; set; }
        public List<string> DateList { get; set; } = new List<string>();
        public List<string> RemarkList { get; set; } = new List<string>();
        public string  DateShow { get; set; }
        public string  FromDate { get; set; }
        public string  ToDate { get; set; }
        public string ShiftCode { get; set; }
        public List<string> ShiftCodeList { get; set; } = new List<string>();
        public string Month { get; set; }
        public string year { get; set; }
        public string Weekend { get; set; }
        public string Remark { get; set; }
        //public string? LUser { get; set; }
        //public DateTime? LDate { get; set; }
        //public string? LIP { get; set; }
        //public string? LMAC { get; set; }
        //public DateTime? ModifyDate { get; set; }
        public string CompanyCode { get; set; }
        //public string EmployeeID_SAO { get; set; }
        public bool isUpdate { get; set; }
        public string Name { get; set; }
        public string DesignationName { get; set; }
        public string ShiftName { get; set; }
        public string DayName { get; set; }       
        public string ApprovalDatetimeShow { get; set; }
        public string ApprovalStatus { get; set; } = "";
        public string ApprovedBy { get; set; } = "";
        public DateTime? ApprovalDatetime { get; set; } = null;
        public string ModifyBy { get; set; } = "";

    }
}

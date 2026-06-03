using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.ManualEarnLeaveEntry
{
    public class ManualEarnLeaveEntryEmployeeCreateDto : BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string? EarnLeaveID { get; set; }
        public List<string>? EmployeeID { get; set; } = new List<string>();
        public string? EmpId { get; set; }
        public string? EmployeeName { get; set; }
        public string? Designation { get; set; }
        public string? Year { get; set; }
        public decimal? GrantedLeaveDays { get; set; }
        public decimal? AvailedLeaveDays { get; set; }
        public decimal? BalancedLeaveDays { get; set; }
        public string? Remarks { get; set; }
        public string? EntryUser { get; set; }
        public bool isUpdate { get; set; }
        //public string LUser { get; set; }
        //public DateTime? LDate { get; set; }
        //public string LIP { get; set; }
        //public string LMAC { get; set; }
        //public DateTime? ModifyDate { get; set; }
        public string? CompanyCode { get; set; }

        public List<string>? YearList { get; set; } = new List<string>();
        public List<decimal>? GrantedLeaveDaysList { get; set; } = new List<decimal>();
        public List<decimal>? AvailedLeaveDaysList { get; set; } = new List<decimal>();
        public List<decimal>? BalancedLeaveDaysList { get; set; } = new List<decimal>();
        public List<string>? RemarksList { get; set; } = new List<string>();
    }
}

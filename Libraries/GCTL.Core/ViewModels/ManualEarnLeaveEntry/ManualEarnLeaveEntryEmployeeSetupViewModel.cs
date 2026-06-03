using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.ManualEarnLeaveEntry
{
    public class ManualEarnLeaveEntryEmployeeSetupViewModel : BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string EarnLeaveID { get; set; }
        public string EmployeeID { get; set; }
        public string Year { get; set; }
        public decimal? GrantedLeaveDays { get; set; }
        public decimal? AvailedLeaveDays { get; set; }
        public decimal? BalancedLeaveDays { get; set; }
        public string Remarks { get; set; }
        public string LUser { get; set; }
        public DateTime? LDate { get; set; }
        public string LIP { get; set; }
        public string LMAC { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string CompanyCode { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.PFAssignEntry
{
    public class PFAssignEntrySetupViewModel : BaseViewModel
    {
        public decimal AutoId { get; set; }  
        public string PFAssignID { get; set; }
        public string EmployeeId { get; set; }
        public List<string> EmployeeIds { get; set; } = new List<string>();
        public List<string> ApprovalRemarkList { get; set; } = new List<string>(); //added excel
        public List<string> PFApprovedStatusList { get; set; } = new List<string>(); //added excel
        public List<string> EFDateList { get; set; } = new List<string>(); //added excel
        public string PFApprovedStatus { get; set; }
        public string ApprovalRemark { get; set; }
        public string EmployeeName { get; set; }
        public string EntryUser { get; set; }
        public string Designation { get; set; }
        public DateTime EFDate { get; set; }
        public string EFDateShow { get; set; }
        public bool isUpdate { get; set; }
        //public string LUser { get; set; }
        //public DateTime? LDate { get; set; }  
        //public string LIP { get; set; }
        //public string LMAC { get; set; }
        //public DateTime? ModifyDate { get; set; }  
        public string CompanyCode { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRMTransportAssignEntry
{
    public class HRMTransportAssignEntrySetupViewModel : BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string TAID { get; set; }
        public string? EmployeeID { get; set; }
        public string? ShowEmployeeID { get; set; }
        public string? TransportNoId { get; set; }
        public string? ShowTransportNoId { get; set; }
        public string? TransportTypeId { get; set; }
        public string HelperId { get; set; }
        public string? ShowTransportTypeId { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string? ShowEffectiveDate { get; set; }
        public string? ShowModifyDate { get; set; }
        public string? ShowCreateDate { get; set; }
        public string? Active { get; set; }
        
        public string? CompanyCode { get; set; }
        public string? EntryUserEmployeeID { get; set; }
        public List<string>? TransportUser { get; set; }
        public string? ShowTransportUser { get; set; }
    }
}

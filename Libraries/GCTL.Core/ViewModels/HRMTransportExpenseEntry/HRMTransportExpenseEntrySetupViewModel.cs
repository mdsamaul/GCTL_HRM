using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRMTransportExpenseEntry
{
    public class HRMTransportExpenseEntrySetupViewModel :BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string TEDID { get; set; }
        public string TEID { get; set; }
        public DateTime? TEDate { get; set; }
        public string? ShowTEDate { get; set; }         
        public string? TransportNo { get; set; }         
        public string? Driver { get; set; }         
        public string? Phone { get; set; }         
        public string? CompanyCode { get; set; }
        public string? EntryUserEmployeeID { get; set; }
        public string? ShowCreateDate { get; set; }
        public string? ShowModifyDate { get; set; }
        public List<TransportExpenseDetailsTempDto>? transportExpenseDetailsTempDtos { get; set; }

    }
}

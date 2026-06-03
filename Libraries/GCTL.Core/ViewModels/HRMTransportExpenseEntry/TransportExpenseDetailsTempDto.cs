using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRMTransportExpenseEntry
{
    public class TransportExpenseDetailsTempDto : BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string Tedid { get; set; }
        public string Teid { get; set; }
        public string ExpenseHeadId { get; set; }
        public string ExpenseHead { get; set; }
        public string Amount { get; set; }
        //public string Luser { get; set; }
        public string Remarks { get; set; }
    }
}

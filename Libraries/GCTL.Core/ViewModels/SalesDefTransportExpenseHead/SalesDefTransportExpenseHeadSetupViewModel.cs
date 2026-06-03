using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.SalesDefTransportExpenseHead
{
    public class SalesDefTransportExpenseHeadSetupViewModel : BaseViewModel
    {
        public int TC { get; set; }
        public string ExpenseHeadID { get; set; }
        public string? ExpenseHead { get; set; }
        public string? ShortName { get; set; }
        public string? ShowModifyDate { get; set; }
        public string? ShowCreateDate { get; set; }        
        public string? CompanyCode { get; set; }
        public string? EntryUserEmployeeID { get; set; }
    }
}

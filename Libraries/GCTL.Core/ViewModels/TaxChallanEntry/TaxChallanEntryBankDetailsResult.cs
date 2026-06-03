using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.TaxChallanEntry
{
    public class TaxChallanEntryBankDetailsResult
    {
        //public string BankId { get; set; }
        //public string BankName { get; set; }
        public string? BankBranchId { get; set; }
        public string? BankBranchName { get; set; }
        public string? BankBranchAddress { get; set; }
    }
}

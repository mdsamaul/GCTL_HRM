using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.SupplierBankAccountTemp
{
    public class SupplierBankAccountTempSetupViewModel : BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string Sbaid { get; set; }
        public string SupplierId { get; set; }
        public string BankId { get; set; }
        public string BankName { get; set; }
        public string BankBranchId { get; set; }
        public string BankBranchName { get; set; }
        public string AccountName { get; set; }
    }
}


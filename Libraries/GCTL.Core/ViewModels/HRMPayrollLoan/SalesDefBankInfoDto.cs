using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRMPayrollLoan
{
    public class SalesDefBankInfoDto
    {
        public decimal AutoId { get; set; }
        public string BankId { get; set; }
        public string BankName { get; set; }
        public string ShortName { get; set; }
        public DateTime? Ldate { get; set; }
        public string Luser { get; set; }
        public string Lip { get; set; }
        public string Lmac { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}

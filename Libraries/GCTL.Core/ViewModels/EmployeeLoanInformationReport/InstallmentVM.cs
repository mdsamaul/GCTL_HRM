using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.EmployeeLoanInformationReport
{
    public class InstallmentVM
    {
        public int InstallmentNo { get; set; }
        public string InstallmentDate { get; set; }
        public string PaymentMode { get; set; }
        public decimal Deposit { get; set; }
        public decimal OutstandingBalance { get; set; }
    }
}

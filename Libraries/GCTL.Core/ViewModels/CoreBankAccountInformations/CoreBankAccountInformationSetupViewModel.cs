using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.CoreBankAccountInformations
{
    public class CoreBankAccountInformationSetupViewModel:BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string AccInfoId { get; set; }
        [Required]
        public string AccountName { get; set; }
        [Required]
        public string AccountNo { get; set; }
        public string BankId { get; set; }

        public string BankName { get; set; }
        public string BranchId { get; set; }
        public string BranchName { get; set; } 
        public string UserInfoEmployeeID { get; set; }
        public string CompanyCode { get; set; }
    }
}

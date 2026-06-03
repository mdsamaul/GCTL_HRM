using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.SalesDefBankBranchInfos
{
    public class SalesDefBankBranchInfoSetupViewModel:BaseViewModel
    {
        public int AutoId { get; set; }
        public string BankBranchId { get; set; }
        [Required(ErrorMessage ="Enter Bank Name")]
        public string BankId { get; set; }

        public string BankName { get; set; }
        public string BankBranchName { get; set; }
        public string ShortName { get; set; }
        public string Swiftcode { get; set; }
        public string Address { get; set; }
        //[DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^(\+8801|01)[3-9]\d{8}$", ErrorMessage = "Please enter a valid phone number.")]
        public string Phone { get; set; }

    }
}

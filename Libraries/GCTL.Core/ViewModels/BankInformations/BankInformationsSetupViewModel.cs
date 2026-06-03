using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.BankInformations
{
    public class BankInformationsSetupViewModel:BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string BankId { get; set; }
       
        public string BankName { get; set; }
        public string ShortName { get; set; }
      
    }
}

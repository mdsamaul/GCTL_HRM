using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRMDefBoardCountryNames
{
    public class HRMDefBoardCountryNamesSetupViewModel:BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string BoardCode { get; set; }
        [Required]
        public string BoardName { get; set; }
        public string ShortName { get; set; }
        public string CompanyCode { get; set; }
    
    }
}

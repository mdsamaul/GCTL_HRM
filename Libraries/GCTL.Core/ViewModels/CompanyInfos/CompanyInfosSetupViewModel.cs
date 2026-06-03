using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.CompanyInfos
{
    public class CompanyInfosSetupViewModel: BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string CompanyNameId { get; set; }
        [Required(ErrorMessage = "CompanyName is required")]
        public string CompanyName { get; set; }
        public string ShortName { get; set; }
    }
}

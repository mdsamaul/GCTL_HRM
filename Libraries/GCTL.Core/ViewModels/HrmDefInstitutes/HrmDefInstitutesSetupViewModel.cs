using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HrmDefInstitutes
{
    public class HrmDefInstitutesSetupViewModel:BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string InstituteCode { get; set; }
        [Required]
        public string InstituteName { get; set; }
        public string ShortName { get; set; }
        public string CompanyCode { get; set; }
    
    }
}

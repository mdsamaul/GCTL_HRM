using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRMCompanyWeekEnds
{
    public class HRMCompanyWeekEndSetupViewModel:BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string CompanyWeekEndCode { get; set; }
        [Required]
        public List<string> Weekend { get; set; } = new List<string>();
      
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EffectiveDate { get; set; }
      
    }
}

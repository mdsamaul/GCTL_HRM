using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HrmDefDegrees
{
    public class HrmDefDegreesSetupViewModel:BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string DegreeCode { get; set; }
        [Required]
        public string DegreeName { get; set; }
        public string ShortName { get; set; }
  
        public string CompanyCode { get; set; }
       
    }
}

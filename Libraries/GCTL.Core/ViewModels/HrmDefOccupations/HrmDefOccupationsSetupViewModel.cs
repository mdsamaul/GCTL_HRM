using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HrmDefOccupations
{
    public class HrmDefOccupationsSetupViewModel:BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string OccupationCode { get; set; }
        [Required,StringLength(50)]
        public string Occupation { get; set; }
        [StringLength(50)]
        public string ShortName { get; set; }
   
    }
}

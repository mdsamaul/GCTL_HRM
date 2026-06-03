using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.Districts
{
    public class DistrictsSetupViewModel : BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string DistrictId { get; set; }
        [Required(ErrorMessage = "District is required")]
        public string District { get; set; }
        public string CompanyCode { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HolidayTypes
{
    public class HRMDefHolidayTypeSetupViewModel:BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string HolidayTypeCode { get; set; }
        public string ShortName { get; set; }
        [Required(ErrorMessage ="Hoiday Type is required")]
        public string HolidayType { get; set; }
        public string CompanyCode { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HrmAtdHolidays
{
    public class HrmAtdHolidaySetupViewModel:BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string HolidayCode { get; set; }
        [Required]
        public string HolidayTypeCode { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime FromDate { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ToDate { get; set; }
        public byte NoOfDays { get; set; }
        [Required]
        public string HolidayName { get; set; }
        public string HolidayTypeName { get; set; }
       

       /// public virtual HrmDefHolidayType HolidayTypeCodeNavigation { get; set; }
    }
}

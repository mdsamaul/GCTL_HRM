using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HrmEmployeeAdditionalInfos
{
    public class HolidayWeekenderAddDto
    {
        public string Date { get; set; }     // yyyy-MM-dd
        public string Title { get; set; }    // Hover text
        public string Type { get; set; }     // "holiday" | "weekend"
    }
}

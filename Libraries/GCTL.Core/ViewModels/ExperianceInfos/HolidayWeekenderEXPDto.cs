using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.ExperianceInfos
{
    public class HolidayWeekenderEXPDto
    {
        public string Date { get; set; }     // yyyy-MM-dd
        public string Title { get; set; }    // Hover text
        public string Type { get; set; }     // "holiday" | "weekend"
    }
}

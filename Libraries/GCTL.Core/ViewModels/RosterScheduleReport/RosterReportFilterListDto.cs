using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.RosterScheduleReport
{
    public class RosterReportFilterListDto
    {
        public List<RosterReportFilterResultDto> Companies { get; set; }
        public List<RosterReportFilterResultDto> Branches { get; set; }
        public List<RosterReportFilterResultDto> Divisions { get; set; }
        public List<RosterReportFilterResultDto> Departments { get; set; }
        public List<RosterReportFilterResultDto> Designations { get; set; }
        public List<RosterReportFilterResultDto> Employees { get; set; }
        public List<RosterReportFilterResultDto> ActivityStatuses { get; set; }
        public DateTime Date { get; set; }
        public string ShiftName { get; set; }
        public int TotalCount { get; set; } // total data count
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.ViewModels.PFAssignEntry;

namespace GCTL.Core.ViewModels.RosterScheduleEntry
{
    public class RosterScheduleEntryFilterListDto
    {
        public List<RosterScheduleEntryFilterResultDto> Companies { get; set; }
        public List<RosterScheduleEntryFilterResultDto> Branches { get; set; }
        public List<RosterScheduleEntryFilterResultDto> Divisions { get; set; }
        public List<RosterScheduleEntryFilterResultDto> Departments { get; set; }
        public List<RosterScheduleEntryFilterResultDto> Designations { get; set; }
        public List<RosterScheduleEntryFilterResultDto> Employees { get; set; }
        public List<RosterScheduleEntryFilterResultDto> ActivityStatuses { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.RosterScheduleEntry
{
    public class MasterBaseFilterResultDto
    {
        public List<MasterFilterItemDto> Companies { get; set; } = new();
        public List<MasterFilterItemDto> Branches { get; set; } = new();
        public List<MasterFilterItemDto> Divisions { get; set; } = new();
        public List<MasterFilterItemDto> Departments { get; set; } = new();
        public List<MasterFilterItemDto> Designations { get; set; } = new();
        public List<RosterScheduleEntryFilterResultDto> Employees { get; set; } = new();
        public List<MasterFilterItemDto> ActivityStatuses { get; set; } = new();
    }

}

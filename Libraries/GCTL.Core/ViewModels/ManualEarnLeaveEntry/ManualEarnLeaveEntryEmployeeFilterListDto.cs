using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.ManualEarnLeaveEntry
{
    public class ManualEarnLeaveEntryEmployeeFilterListDto
    {
        public List<ManualEarnLeaveEntryEmployeeFilterResultDto> Companies { get; set; }
        public List<ManualEarnLeaveEntryEmployeeFilterResultDto> Branches { get; set; }
        public List<ManualEarnLeaveEntryEmployeeFilterResultDto> Divisions { get; set; }
        public List<ManualEarnLeaveEntryEmployeeFilterResultDto> Departments { get; set; }
        public List<ManualEarnLeaveEntryEmployeeFilterResultDto> Designations { get; set; }
        public List<ManualEarnLeaveEntryEmployeeFilterResultDto> Employees { get; set; }
        public List<ManualEarnLeaveEntryEmployeeFilterResultDto> ActivityStatuses { get; set; }
    }
}

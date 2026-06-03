using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.ViewModels.ManualEarnLeaveEntry;

namespace GCTL.Core.ViewModels.PFAssignEntry
{
    public class PFAssignEntryFilterListDto
    {
        public List<PFAssignEntryFilterResultDto> Companies { get; set; }
        public List<PFAssignEntryFilterResultDto> Branches { get; set; }
        public List<PFAssignEntryFilterResultDto> Divisions { get; set; }
        public List<PFAssignEntryFilterResultDto> Departments { get; set; }
        public List<PFAssignEntryFilterResultDto> Designations { get; set; }
        public List<PFAssignEntryFilterResultDto> Employees { get; set; }
        public List<PFAssignEntryFilterResultDto> ActivityStatuses { get; set; }
       
    }
}

using GCTL.Core.ViewModels.RosterScheduleApproval;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.ManualEntryApproval
{
    public class ManualEntryApprovalFilterListDto
    {
        public List<ManualEntryApprovalFilterResultDto> Companies { get; set; }
        public List<ManualEntryApprovalFilterResultDto> Branches { get; set; }
        public List<ManualEntryApprovalFilterResultDto> Divisions { get; set; }
        public List<ManualEntryApprovalFilterResultDto> Departments { get; set; }
        public List<ManualEntryApprovalFilterResultDto> Designations { get; set; }
        public List<ManualEntryApprovalFilterResultDto> Employees { get; set; }
        public List<ManualEntryApprovalFilterResultDto> ActivityStatuses { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}

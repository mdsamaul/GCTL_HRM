using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.RosterScheduleEntry
{
    public class MasterBaseFilterDto
    {
        public List<string>? CompanyCodes { get; set; }
        public List<string>? BranchCodes { get; set; }
        public List<string>? DivisionCodes { get; set; }
        public List<string>? DepartmentCodes { get; set; }
        public List<string>? DesignationCodes { get; set; }
        public List<string>? EmployeeIds { get; set; }
        public List<string>? EmployeeStatuses { get; set; }
        public int Page { get; set; } = 1;          // 1-based
        public int PageSize { get; set; } = 30;     // default page size
        public string? Search { get; set; }         // search text (emp name/id)
    }
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public bool More { get; set; }
    }

}

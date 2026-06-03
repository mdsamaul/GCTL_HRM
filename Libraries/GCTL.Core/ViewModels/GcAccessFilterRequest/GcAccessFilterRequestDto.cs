using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.GcAccessFilterRequest
{
    public class GcAccessFilterRequestDto
    {
        public string? AccessCode { get; set; }
        public string? EmployeeId { get; set; }

        public List<string>? CompanyCodes { get; set; }
        public List<string>? BranchCodes { get; set; }
        public List<string>? DivisionCodes { get; set; }
        public List<string>? DepartmentCodes { get; set; }
        public List<string>? DesignationCodes { get; set; }
        public List<string>? EmployeeStatuses { get; set; }
        public List<string>? EmployeeNatureCodes { get; set; }
        public List<string>? EmployeeTypes { get; set; }

        public DateTime? JoiningDateFrom { get; set; }
        public DateTime? JoiningDateTo { get; set; }

        public string? Search { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 100;
    }

    public class GcAccessItemDto
    {
        public string? Code { get; set; }
        public string? Name { get; set; }
    }

    public class PagedAccessResultDto<T>
    {
        public List<T> Items { get; set; } = new();
        public bool More { get; set; }
    }
}

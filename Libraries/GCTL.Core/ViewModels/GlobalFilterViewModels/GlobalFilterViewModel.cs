//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace GCTL.Core.ViewModels.GlobalFilterViewModel
//{
//    public class PagedLookupDto
//    {
//        public List<IdNamePair> Items { get; set; } = new();
//        public int Total { get; set; }
//    }

//    public class LookupRow
//    {
//        public string Id { get; set; } = "";
//        public string Name { get; set; } = "";
//        public int Total { get; set; }
//    }

//    public class IdNamePair
//    {
//        public string Id { get; set; } = "";
//        public string Name { get; set; } = "";
//    }

//    // ✅ Request DTO - Controller [FromQuery] এ bind হবে
//    public class PagedLookupRequest
//    {
//        public string Sp { get; set; } = "";
//        public string Type { get; set; } = "";
//        public string? Q { get; set; }
//        public int Page { get; set; } = 1;
//        public int PageSize { get; set; } = 100;
//        public List<string>? CompanyCodes { get; set; }
//        public List<string>? BranchCodes { get; set; }
//        public List<string>? DepartmentCodes { get; set; }
//        public List<string>? DesignationCodes { get; set; }
//        public List<string>? EmployeeIds { get; set; }
//    }
//}


namespace GCTL.Core.ViewModels.GlobalFilterViewModel
{
    public class PagedLookupDto
    {
        public List<IdNamePair> Items { get; set; } = new();
        public int Total { get; set; }
    }

    public class LookupRow
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public int Total { get; set; }
    }

    public class IdNamePair
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
    }

    /// <summary>
    /// Request DTO — Controller [FromQuery] এ bind হবে
    /// EmployeeStatus: "01" = Active, "02" = Inactive, null/empty = All
    /// </summary>
    public class PagedLookupRequest
    {
        public string Sp { get; set; } = "";
        public string Type { get; set; } = "";
        public string? Q { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 100;

        public List<string>? CompanyCodes { get; set; }
        public List<string>? BranchCodes { get; set; }
        public List<string>? DepartmentCodes { get; set; }
        public List<string>? DesignationCodes { get; set; }
        public List<string>? EmployeeIds { get; set; }

        // "01" = Active | "02" = Inactive | null/empty = All
        public List<string>? EmployeeStatuses { get; set; }
    }
}

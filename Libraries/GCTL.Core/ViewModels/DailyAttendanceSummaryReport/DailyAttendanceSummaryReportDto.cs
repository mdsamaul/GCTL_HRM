using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.DailyAttendanceSummaryReport
{

    public class DailyAttendanceSummaryFilterDto : BaseViewModel
    {
        public string? CompanyCode { get; set; }
        public List<string>? DepartmentCodes { get; set; }
        public DateTime? FromDate { get; set; }
        public string? LoginEmployeeId { get; set; }  
        public string? AccessCodeId { get; set; }     
    }
    public class DailyAttendanceSummaryDto
    {
        public string DepartmentName { get; set; } = "";
        public int NoOfEmps { get; set; }
        public int PresentCount { get; set; }
        public int LateCount { get; set; }
        public int LeaveCount { get; set; }
        public int AbsentCount { get; set; }
        public string? DataDate { get; set; }
    }

    public class DailyAttendanceSummaryResponseDto
    {
        public List<DailyAttendanceSummaryDto> Departments { get; set; } = new();
        public int TotalNoOfEmps { get; set; }
        public int TotalPresent { get; set; }
        public int TotalLate { get; set; }
        public int TotalLeave { get; set; }
        public int TotalAbsent { get; set; }
        public string? DataDate { get; set; }
        public string? CompanyName { get; set; }
    }

}

// GCTL.Core/ViewModels/Dashboard/LeaveDashboardDtos.cs — replace করো

namespace GCTL.Core.ViewModels.Dashboard
{
    public class LeaveSummaryCardDto
    {
        public int TotalApplied { get; set; }
        public int Approved { get; set; }
        public int Canceled { get; set; }
        public int Pending { get; set; }
    }

    public class LeaveTypeDto
    {
        public string LeaveTypeCode { get; set; }
        public string ShortName { get; set; }
        public decimal NoOfDay { get; set; }
    }

    public class EmployeeLeaveRowDto
    {
        public string EmployeeId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string JoiningDate { get; set; }
        public string LeaveTypeCode { get; set; }
        public string LeaveShortName { get; set; }
        public decimal GrantedDays { get; set; }
        public decimal AvailedDays { get; set; }
        public decimal BalancedDays { get; set; }
        public int RowNum { get; set; }
        public int TotalCount { get; set; }  // ← pagination এর জন্য
    }

    public class LeaveDashboardResponseDto
    {
        public LeaveSummaryCardDto Summary { get; set; }
        public List<LeaveTypeDto> LeaveTypes { get; set; }
        public List<EmployeeLeaveRowDto> Employees { get; set; }
        public int TotalCount { get; set; }  // ← total unique employees
    }
}
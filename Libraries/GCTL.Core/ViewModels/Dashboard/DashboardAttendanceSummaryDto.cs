namespace GCTL.Core.ViewModels.Dashboard
{
    public class DashboardAttendanceSummaryDto
    {
        public int TotalEmployees { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateCount { get; set; }
        public int OnLeaveCount { get; set; }
        public decimal PresentPct { get; set; }
        public decimal AbsentPct { get; set; }
        public decimal LatePct { get; set; }
        public decimal OnLeavePct { get; set; }
        public DateTime DataDate { get; set; }
    }
}
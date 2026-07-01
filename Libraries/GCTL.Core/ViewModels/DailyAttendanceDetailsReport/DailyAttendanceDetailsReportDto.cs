// ═══════════════════════════════════════════════════════════════════
// File: GCTL.Core/ViewModels/DailyAttendanceDetailsReport/DailyAttendanceDetailsReportDto.cs
// ═══════════════════════════════════════════════════════════════════
using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.DailyAttendanceDetailsReport;

namespace GCTL.Core.ViewModels.DailyAttendanceDetailsReport
{
    // ── Filter sent from UI ──────────────────────────────────────────
    public class DailyAttendanceDetailsFilterDto
    {
        public string? CompanyCode { get; set; }
        public List<string>? BranchCodes { get; set; }
        public List<string>? DepartmentCodes { get; set; }
        public List<string>? EmployeeIds { get; set; }
        public string? FromDate { get; set; }
        // Present | Absent | Late | InOut | MissingCheckOut | EarlyLeave
        public string ReportType { get; set; } = "Present";
        public string? LoginEmployeeId { get; set; }
        public string? AccessCodeId { get; set; }
    }

    // ── Common header info returned by SP (2nd result set) ──────────
    public class DailyAttendanceDetailsHeaderDto
    {
        public string DataDate { get; set; } = "";
        public string CompanyName { get; set; } = "";
        public string ReportType { get; set; } = "";
    }

    // ── Present Report row ───────────────────────────────────────────
    public class DailyAttendancePresentRowDto
    {
        public int SN { get; set; }
        public string EmployeeId { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string Designation { get; set; } = "";
        public string Division { get; set; } = "";
        public string DepartmentName { get; set; } = "";
        public string ShiftName { get; set; } = "";
        public string InTime { get; set; } = "";
        public string LateDisplay { get; set; } = "";
        public string Status { get; set; } = "";
        public string Remarks { get; set; } = "";
    }

    // ── Absent Report row ────────────────────────────────────────────
    public class DailyAttendanceAbsentRowDto
    {
        public int SN { get; set; }
        public string EmployeeId { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string Designation { get; set; } = "";
        public string DepartmentName { get; set; } = "";
        public string Status { get; set; } = "";
    }

    // ── Late Report row ──────────────────────────────────────────────
    public class DailyAttendanceLateRowDto
    {
        public int SN { get; set; }
        public string EmployeeId { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string Designation { get; set; } = "";
        public string DepartmentName { get; set; } = "";
        public string ShiftName { get; set; } = "";
        public string InTime { get; set; } = "";
        public string LateDisplay { get; set; } = "";
        public string Status { get; set; } = "";
        public string Remarks { get; set; } = "";
    }

    // ── In-Out Report row ────────────────────────────────────────────
    public class DailyAttendanceInOutRowDto
    {
        public int SN { get; set; }
        public string EmployeeId { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string Designation { get; set; } = "";
        public string DepartmentName { get; set; } = "";
        public string ShiftName { get; set; } = "";
        public string InTime { get; set; } = "";
        public string LateDisplay { get; set; } = "";
        public string OutTime { get; set; } = "";
        public string EarlyOut { get; set; } = "";
        public string WorkHours { get; set; } = "";
        public decimal OTHours { get; set; }
        public string Status { get; set; } = "";
        public string Remarks { get; set; } = "";
    }

    // ── Missing Check-Out Report row (punched in, never punched out) ──
    public class DailyAttendanceMissingCheckOutRowDto
    {
        public int SN { get; set; }
        public string EmployeeId { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string Designation { get; set; } = "";
        public string DepartmentName { get; set; } = "";
        public string ShiftName { get; set; } = "";
        public string InTime { get; set; } = "";
        public string WorkHours { get; set; } = "";
        public string Status { get; set; } = "";
        public string Remarks { get; set; } = "";
    }

    // ── Early Office Leave Report row (left before shift end) ─────────
    public class DailyAttendanceEarlyLeaveRowDto
    {
        public int SN { get; set; }
        public string EmployeeId { get; set; } = "";
        public string EmployeeName { get; set; } = "";
        public string Designation { get; set; } = "";
        public string DepartmentName { get; set; } = "";
        public string ShiftName { get; set; } = "";
        public string InTime { get; set; } = "";
        public string OutTime { get; set; } = "";
        public string EarlyOut { get; set; } = "";
        public string Status { get; set; } = "";
        public string Remarks { get; set; } = "";
    }

    // ── Unified response wrapper ─────────────────────────────────────
    public class DailyAttendanceDetailsResultDto
    {
        public DailyAttendanceDetailsHeaderDto Header { get; set; } = new();
        public List<DailyAttendancePresentRowDto> PresentRows { get; set; } = new();
        public List<DailyAttendanceAbsentRowDto> AbsentRows { get; set; } = new();
        public List<DailyAttendanceLateRowDto> LateRows { get; set; } = new();
        public List<DailyAttendanceInOutRowDto> InOutRows { get; set; } = new();
        public List<DailyAttendanceInOutRowDto> MissingCheckOutRows { get; set; } = new();
        public List<DailyAttendanceInOutRowDto> EarlyLeaveRows { get; set; } = new();
    }
}
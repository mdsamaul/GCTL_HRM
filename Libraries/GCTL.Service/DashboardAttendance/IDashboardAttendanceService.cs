using GCTL.Core.ViewModels.Dashboard;

namespace GCTL.Service.DashboardAttendance
{
    public interface IDashboardAttendanceService
    {
        Task<(DashboardAttendanceSummaryDto Summary,
              IEnumerable<DashboardAttendanceMovementDto> Items,
              int TotalCount)>
            GetAttendanceMovementAsync(
                string companyCode, string branchCode, string departmentCode,
                DateTime forDate, int page, int pageSize, string search = null);
        Task<LeaveDashboardResponseDto> GetLeaveDashboardAsync(
            string companyCode,
            string branchCode,
            string departmentCode,
            int year,
            int page,
            int pageSize,
            string search,
            string employeeId = null);
    }
}
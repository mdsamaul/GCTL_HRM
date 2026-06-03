using GCTL.Core.ViewModels.AttendanceMovementRegisterReportDto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GCTL.Service.AttendanceMovementRegisterReportService
{
    public interface IAttendanceMovementRegisterReportService
    {
        Task<bool> PagePermissionAsync(string accessCode);
        Task<List<DepartmentGroupedData>> GetAttendanceMachineDataAsync(AttendanceMovementRegisterReportFilterData filter);
        Task<AttendanceMovementRegisterReportDropdownListDto> GetAttendanceMachineDataFiltersAsync(AttendanceMovementRegisterReportFilterData filter);
    }
}
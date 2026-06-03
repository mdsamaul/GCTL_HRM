using GCTL.Core.ViewModels.Accounts;
using GCTL.Core.ViewModels.AttendanceMovementRegisterReportCount;
using GCTL.Core.ViewModels.AttendanceMovementRegisterReportDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.AttendanceMovementRegisterReportCountService
{
    public interface IAttendanceMovementRegisterReportCountService
    {
        Task<AttendanceMovementRegisterReportCountDropdownListDto> GetAttendanceMachineDataFiltersAsync(AttendanceMovementRegisterReportCountFilterData filter);
        Task<byte[]> GetEmployeeMovementPdfAsync(EmployeeMovementRequestDto requestDto);
        Task<List<DepartmentAndDateGroupedData>> GetAttendanceMachineDataAsync(AttendanceMovementRegisterReportCountFilterData filter, string BaseUrl, UserInfoViewModel loginInfo);
        Task<bool> PagePermissionAsync(string accessCode);
    }
}


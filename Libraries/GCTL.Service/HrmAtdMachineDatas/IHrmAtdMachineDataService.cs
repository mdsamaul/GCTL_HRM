using GCTL.Core.ViewModels.HrmAtdMachineData;
using GCTL.Data.Models;

namespace GCTL.Service.HrmAtdMachineDatas
{
    public interface IHrmAtdMachineDataService
    {
        Task<(List<HrmAtdMachineData> Data, int TotalRecords)> GetPaginatedDataAsync(string searchValue, int page, int pageSize, string sortColumn, string sortDirection);
        Task<List<EmployeeAttendanceGroupViewModel>> GetFilteredAttendanceAsync(
              string employeeIds,
              DateTime? fromDate,
              DateTime? toDate,
              int? fromMonth,
              int? fromYear,
              int? toMonth,
              int? toYear);

        Task<byte[]> ExportAttendanceToExcelAsync(
            string employeeIds,
            DateTime? fromDate,
            DateTime? toDate,
            int? fromMonth,
            int? fromYear,
            int? toMonth,
            int? toYear);
    }
}


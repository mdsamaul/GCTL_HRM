using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.ManualAttendance;
using GCTL.Data.Models;

namespace GCTL.Service.ManualAttendances
{
    public interface IManualAttendanceService
    {
        #region Permissions
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
        #endregion


        #region CRUD

        Task<(int totalRecords, List<ManualAttendanceSetupViewModel> data)> GetAllAsync(
        int skip,
        int pageSize,
        string sortColumn,
        string sortDirection,
        string searchValue);
        Task<ManualAttendanceSetupViewModel> GetByIdAsync(string code);
        Task<(bool IsSuccess, HrmAtdManual? SavedRecord)> SaveAsync(ManualAttendanceSetupViewModel model);
        //Task<(bool IsSuccess, HrmAtdManual? DeletedRecord)> DeleteAsync(
        //    List<string> ids,
        //    List<string> selectedEmployeeIds,
        //    string attendanceTypeCode,
        //    string fromDate,
        //    string toDate);
        Task<(bool IsSuccess,  HrmAtdManual? DeletedRecord)> DeleteAsync(
    List<string> ids,
    List<string> selectedEmployeeIds,
    string attendanceTypeCode,
    string fromDate,
    string toDate,
    bool isBothInOutEntry,
    DeleteHistoryViewModel deleteModel);
        #endregion


        #region Others
        Task<List<ManualAttendanceSetupViewModel>> GetEmployeeByCompany(string companyId);
        Task<List<ManualAttendanceSetupViewModel>> GetCompanyDataById(string companyId);
        Task<ManualAttendanceSetupViewModel> GetEmployeeDetailsById(string id);
        //Task<List<ManualAttendanceSetupViewModel>> GetEmployeeDataById(string employeeId);
        Task<string> GenerateNextCode();
        IEnumerable<CommonSelectModel> EmployeeSelection();

        Task<(int totalRecords, List<ManualAttendanceSetupViewModel> data)> GetEmployeeDataByIdAsync(
        string employeeId,
        int skip,
        int pageSize,
        string searchValue,
        string sortColumn,
        string sortDirection);
        Task<ShiftTimeDto> SandRTimeByEmployeeAsync(string employeeId, DateTime formDate);
        #endregion
    }
}

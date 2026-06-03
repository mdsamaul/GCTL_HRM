using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.ManualAttendanceBulk;

namespace GCTL.Service.ManualAttendanceBulks
{
    public interface IManualAttendanceBulkService
    {
        #region Permissions
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
        #endregion


        #region CRUD
        //Task<List<ManualAttendanceBulkSetupViewModel>> GetAllAsync(string companyId);
        Task<(int totalRecords, List<ManualAttendanceBulkSetupViewModel> data)> GetAllPagedAsync(
        string companyId,
        int skip,
        int pageSize,
        string sortColumn,
        string sortDirection,
        string searchValue);

        Task<ManualAttendanceBulkSetupViewModel> GetByIdAsync(string code);
        Task<bool> SaveAsync(ManualAttendanceBulkSetupViewModel model, List<string> selectedEmployeeIds);
        //Task<bool> DeleteAsync(List<string> ids, List<string> selectedEmployeeIds, string attendanceTypeCode, string fromDate, string toDate);
        #endregion


        #region Filter by Company, Department, Designation
        Task<List<ManualAttendanceBulkSetupViewModel>> GetBranchByCompanyId(string companyId);

        Task<List<ManualAttendanceBulkSetupViewModel>> GetDepartmentByCompanyId(string companyId);
        Task<List<ManualAttendanceBulkSetupViewModel>> GetDesignationByCompanyId(string companyId);
        Task<List<ManualAttendanceBulkSetupViewModel>> GetEmployeeByCompanyId(string companyId);

        Task<List<ManualAttendanceBulkSetupViewModel>> GetDepartmentByBranchId(string branchId);
        Task<List<ManualAttendanceBulkSetupViewModel>> GetDesignationByBranchId(string branchId);
        Task<List<ManualAttendanceBulkSetupViewModel>> GetEmployeeByBranchId(string companyId, string branchId);
        Task<bool> DeleteAsync(List<string> selectedEmployeeIds, string attendanceTypeCode, string fromDate, string toDate, bool isBothInOutEntry, DeleteHistoryViewModel deleteModel);
        Task<List<ManualAttendanceBulkSetupViewModel>> GetDesignationByDepartmentId(List<string> departmentId);
        //Task<List<ManualAttendanceBulkSetupViewModel>> GetDesignationByDepartmentId(string departmentId);
        //Task<List<ManualAttendanceBulkSetupViewModel>> GetEmployeeByDepartmentId(string companyId, string branchId, string departmentId);

        //Task<List<ManualAttendanceBulkSetupViewModel>> GetEmployeeByDesignationId(string companyId, string branchId, string departmentId, string designationId);

        Task<List<ManualAttendanceBulkSetupViewModel>> GetEmployeeByDepartmentId(string companyId, string branchId, List<string> departmentId, string selectedListType, string selectedActiveStatus);

        Task<List<ManualAttendanceBulkSetupViewModel>> GetEmployeeByDesignationId(string companyId, string branchId, List<string> departmentId, List<string> designationId, string selectedListType, string selectedActiveStatus);
        #endregion


        #region Others
        Task<ManualAttendanceBulkSetupViewModel> GetEmployeeDetailsById(string id);
        Task<List<ManualAttendanceBulkSetupViewModel>> GetEmployeeDataById(string employeeId);
        Task<List<ManualAttendanceBulkSetupViewModel>> GetCompanyDataById(string companyId);
        Task<string> GenerateNextCode();
        string GenerateNextBulkEntryCode(ManualAttendanceBulkSetupViewModel model);
        #endregion
    }
}

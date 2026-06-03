using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HrmEmployeeOfficialInfo;

namespace GCTL.Service.EmployeeOfficialInfo
{
    public interface IEmployeeOfficialInfoService
    {
        #region Permissions
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
        #endregion

        #region CRUD
        Task<List<HrmEmployeeOfficialInfoSetupViewModel>> GetAllAsync();
        Task<HrmEmployeeOfficialInfoSetupViewModel> GetByIdAsync(string code);
        Task<bool> SaveAsync(HrmEmployeeOfficialInfoSetupViewModel model);
        Task<bool> UpdateAsync(HrmEmployeeOfficialInfoSetupViewModel model);
        // bool DeleteTab(List<string> ids);
        Task<bool> DeleteTab(List<string> ids, DeleteHistoryViewModel model);
        #endregion

        Task<List<HolidayWeekenderOFFDto>> GetHolidayAndWeekendAsync(int year);

        #region Others
        Task<HrmEmployeeOfficialInfoSetupViewModel> GetEmployeeDetailsByCode(string code);
        Task<bool> IsExistsByCode(string code);
        IEnumerable<CommonSelectModel> EmployeeSelection();
        #endregion
    }
}

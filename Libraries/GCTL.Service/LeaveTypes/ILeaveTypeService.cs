using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.LeaveTypes;
using GCTL.Data.Models;


namespace GCTL.Service.LeaveTypes
{
    public interface ILeaveTypeService
    {


        HrmAtdLeaveType GetLeaveType(string code);
        Task<(bool success, string message, bool refError)> DeleteLeaveType(List<string> ids, DeleteHistoryViewModel model);


        Task<List<LeaveTypeSetupViewModel>> GetLeaveTypesAsync();
        Task<LeaveTypeSetupViewModel> GetLeaveTypeAsync(string code);
        Task<bool> SaveLeaveTypeAsync(LeaveTypeSetupViewModel entityVM);
        Task<bool> UpdateLeaveTypeAsync(LeaveTypeSetupViewModel entityVM);
        Task<bool> DeleteLeaveTypeAsync(string id);
        Task<bool> IsLeaveTypeExistByCodeAsync(string code);
        Task<bool> IsLeaveTypeExistAsync(string name);
        Task<bool> IsLeaveTypeExistAsync(string name, string typeCode);
        Task<string> GenerateNextLeaveTypeCode();
        Task<IEnumerable<CommonSelectModel>> LeaveTypeSelectionAsync();
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}




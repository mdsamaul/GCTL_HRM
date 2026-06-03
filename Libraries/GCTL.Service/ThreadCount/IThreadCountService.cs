using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.ThreadCount;

namespace GCTL.Service.ThreadCount
{
    public interface IThreadCountService
    {
        Task<List<ThreadCountSetupViewModel>> GetAllAsync();
        Task<ThreadCountSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(ThreadCountSetupViewModel entityVM);
        Task<bool> UpdateAsync(ThreadCountSetupViewModel entityVM);

        Task<IEnumerable<CommonSelectModel>> SelectionThreadCountAsync();

        Task<(bool succses, string messege)> DeleteTab(List<string> ids, DeleteHistoryViewModel model);

        //  Task<bool> DeleteTab(List<string> ids);

        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string name, string typeCode);

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

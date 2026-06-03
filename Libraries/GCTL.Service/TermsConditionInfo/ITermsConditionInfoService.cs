using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.TermsConditionInfo;

namespace GCTL.Service.TermsConditionInfo
{
    public interface ITermsConditionInfoService
    {
        Task<List<TermsConditionInfoSetupViewModel>> GetAllAsync();
        Task<TermsConditionInfoSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(TermsConditionInfoSetupViewModel entityVM);
        Task<bool> UpdateAsync(TermsConditionInfoSetupViewModel entityVM);

        Task<IEnumerable<CommonSelectModel>> SelectionTermsConditionAsync();

        Task<(bool succses, string messege)> DeleteTab(List<string> ids, DeleteHistoryViewModel model);

        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string name, string typeCode);

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

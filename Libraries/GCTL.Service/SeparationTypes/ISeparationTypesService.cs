using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.SeparationTypes;

namespace GCTL.Service.SeparationTypes
{
    public interface ISeparationTypesService
    {
        Task<List<SeparationTypesSetupViewModel>> GetAllAsync();
        Task<SeparationTypesSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(SeparationTypesSetupViewModel entityVM);
        Task<bool> UpdateAsync(SeparationTypesSetupViewModel entityVM);
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string name, string typeCode);
        Task<IEnumerable<CommonSelectModel>> SelectionSeparationTypeAsync();
        Task<(bool succses, string messege, bool refSuccess)> DeleteTab(List<string> ids, DeleteHistoryViewModel model);
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

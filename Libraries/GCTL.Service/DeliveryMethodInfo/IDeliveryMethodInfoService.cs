using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.DeliveryMethodInfo;

namespace GCTL.Service.DeliveryMethodInfo
{
    public interface IDeliveryMethodInfoService
    {
        Task<List<DeliveryMethodInfoSetupViewModel>> GetAllAsync();
        Task<DeliveryMethodInfoSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(DeliveryMethodInfoSetupViewModel entityVM);
        Task<bool> UpdateAsync(DeliveryMethodInfoSetupViewModel entityVM);

        Task<IEnumerable<CommonSelectModel>> SelectionDeliveryMethodAsync();

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

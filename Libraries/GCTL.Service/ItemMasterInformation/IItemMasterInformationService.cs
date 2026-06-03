using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.INV_Catagory;
using GCTL.Core.ViewModels.ItemMasterInformation;
using GCTL.Core.ViewModels.ItemModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.ItemMasterInformation
{
    public interface IItemMasterInformationService
    {
        Task<List<ItemMasterInformationSetupViewModel>> GetAllAsync();
        Task<ItemMasterInformationSetupViewModel> GetByIdAsync(string id);
        Task<(bool isSuccess, string message, object data)> CreateUpdateAsync(ItemMasterInformationSetupViewModel model);

        Task<(bool success, bool refSuccess, string message)> DeleteAsync(List<string> ids, DeleteHistoryViewModel model);
        Task<string> AutoProductIdAsync();
        Task<(bool isSuccess, string message, object data)> AlreadyExistAsync(string catagoryValue);

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

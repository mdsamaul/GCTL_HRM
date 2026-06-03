using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.ItemType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.ItemType
{
    public interface IItemTypeService
    {
        Task<List<ItemTypeSetupViewModel>> GetAllAsync();
        Task<ItemTypeSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(ItemTypeSetupViewModel entityVM);
        Task<bool> UpdateAsync(ItemTypeSetupViewModel entityVM);

        Task<IEnumerable<CommonSelectModel>> SelectionItemTypeAsync();

        Task<bool> DeleteTab(List<string> ids);

        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string name, string typeCode);

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

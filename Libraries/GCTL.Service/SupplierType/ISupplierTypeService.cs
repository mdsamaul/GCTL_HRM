using GCTL.Core.ViewModels.ColorInformation;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.SupplierType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.SupplierType
{
    public interface ISupplierTypeService
    {
        Task<List<SupplierTypeSetupViewModel>> GetAllAsync();
        Task<SupplierTypeSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(SupplierTypeSetupViewModel entityVM);
        Task<bool> UpdateAsync(SupplierTypeSetupViewModel entityVM);

        Task<IEnumerable<CommonSelectModel>> SelectionSupplierTypeAsync();

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

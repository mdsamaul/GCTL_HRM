using GCTL.Core.ViewModels.ColorInformation;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.SupplierOrigin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.SupplierOrigin
{
    public interface ISupplierOriginService
    {
        Task<List<SupplierOriginSetupViewModel>> GetAllAsync();
        Task<SupplierOriginSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(SupplierOriginSetupViewModel entityVM);
        Task<bool> UpdateAsync(SupplierOriginSetupViewModel entityVM);

        Task<IEnumerable<CommonSelectModel>> SelectionSupplierOriginAsync();

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

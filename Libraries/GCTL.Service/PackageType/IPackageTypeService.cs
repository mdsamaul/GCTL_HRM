using GCTL.Core.ViewModels.ColorInformation;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.PackageType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.PackageType
{
    public interface IPackageTypeService
    {
        Task<List<PackageTypeSetupViewModel>> GetAllAsync();
        Task<PackageTypeSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(PackageTypeSetupViewModel entityVM);
        Task<bool> UpdateAsync(PackageTypeSetupViewModel entityVM);

        Task<IEnumerable<CommonSelectModel>> SelectionPackageTypeAsync();

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

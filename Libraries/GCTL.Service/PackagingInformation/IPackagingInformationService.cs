using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.PackagingInformation;
using GCTL.Core.ViewModels.SupplierCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.PackagingInformation
{
    public interface IPackagingInformationService
    {
        Task<List<PackagingInformationSetupViewModel>> GetAllAsync();
        Task<PackagingInformationSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(PackagingInformationSetupViewModel entityVM);
        Task<bool> UpdateAsync(PackagingInformationSetupViewModel entityVM);

        Task<IEnumerable<CommonSelectModel>> SelectionPackagingAsync();

        Task<bool> DeleteTab(List<string> ids);

        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string name, string typeCode, string type);

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

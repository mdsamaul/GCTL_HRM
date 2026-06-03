using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.ContactPersonInfo;
using GCTL.Core.ViewModels.SupplierCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.ContactPersonInfo
{
    public interface IContactPersonInfoService
    {
        Task<List<ContactPersonInfoSetupViewModel>> GetAllAsync();
        Task<ContactPersonInfoSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(ContactPersonInfoSetupViewModel entityVM);
        Task<bool> UpdateAsync(ContactPersonInfoSetupViewModel entityVM);

        Task<IEnumerable<CommonSelectModel>> SelectionContactPersonAsync();

        Task<bool> DeleteTab(List<string> ids);

        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string name, string typeCode);

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);

        Task<string> Autoid();
    }
}

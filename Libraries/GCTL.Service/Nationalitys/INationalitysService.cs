using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.Nationalitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.Nationalitys
{
    public interface INationalitysService
    {
        Task<List<NationalitysSetupViewModel>> GetAllAsync();
        Task<NationalitysSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(NationalitysSetupViewModel entityVM);
        Task<bool> UpdateAsync(NationalitysSetupViewModel entityVM);
        Task<bool> DeleteTab(List<string> ids);
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string name, string typeCode);
        Task<IEnumerable<CommonSelectModel>> SelectionNationalityAsync();

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

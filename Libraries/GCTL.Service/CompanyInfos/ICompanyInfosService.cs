using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.CompanyInfos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.CompanyInfos
{
    public interface ICompanyInfosService
    {
        Task<List<CompanyInfosSetupViewModel>> GetAllAsync();
        Task<CompanyInfosSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(CompanyInfosSetupViewModel entityVM);
        Task<bool> UpdateAsync(CompanyInfosSetupViewModel entityVM);
        Task<bool> DeleteTab(List<string> ids);
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string name, string typeCode);

        Task<IEnumerable<CommonSelectModel>> SelectionCompanyTypeAsync();

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

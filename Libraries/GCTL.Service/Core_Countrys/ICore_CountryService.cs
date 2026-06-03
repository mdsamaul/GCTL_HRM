using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.ViewModels.Core_Country;

namespace GCTL.Service.Core_Countrys
{
    public interface ICore_CountryService
    {
        Task<List<Core_CountrySetupViewModel>> GetAllAsync();
        Task<Core_CountrySetupViewModel> GetByIdAsync(string id);
        Task<(bool isSuccess, string message, object data)> CreateUpdateAsync(Core_CountrySetupViewModel model);
        Task<(bool isSuccess, string message, object data)> DeleteAsync(List<int> ids);
        Task<string> AutoCountryIdAsync();

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
        Task<(bool isSuccess, string message, object data)> AlreadyExistAsync(string countryValue);
    }
}

using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.Country;
using GCTL.Core.ViewModels.SupplierCategory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.Country
{
    public interface ICountryService
    {
        Task<List<CountrySetuoViewModel>> GetAllAsync();
        Task<CountrySetuoViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(CountrySetuoViewModel entityVM);
        Task<bool> UpdateAsync(CountrySetuoViewModel entityVM);

        Task<IEnumerable<CommonSelectModel>> SelectionCountryAsync();

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

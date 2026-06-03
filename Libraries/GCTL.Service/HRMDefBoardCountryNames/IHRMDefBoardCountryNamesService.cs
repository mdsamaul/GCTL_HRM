using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HRMDefBoardCountryNames;
using GCTL.Core.ViewModels.HrmDefDegrees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HRMDefBoardCountryNames
{
    public interface IHRMDefBoardCountryNamesService
    {
        Task<List<HRMDefBoardCountryNamesSetupViewModel>> GetAllAsync();
        Task<HRMDefBoardCountryNamesSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(HRMDefBoardCountryNamesSetupViewModel entityVM);
        Task<bool> UpdateAsync(HRMDefBoardCountryNamesSetupViewModel entityVM);
        Task<bool> DeleteTab(List<string> ids);
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string name, string typeCode);
      Task<IEnumerable<CommonSelectModel>> SelectionHrmDefBoardCountryTypeAsync();
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

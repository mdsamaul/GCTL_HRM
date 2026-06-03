using GCTL.Core.ViewModels.ColorInformation;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.SeasonInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.SeasonInformation
{
    public interface ISeasonInformationService
    {
        Task<List<SeasonInformationSetupViewModel>> GetAllAsync();
        Task<SeasonInformationSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(SeasonInformationSetupViewModel entityVM);
        Task<bool> UpdateAsync(SeasonInformationSetupViewModel entityVM);

        Task<IEnumerable<CommonSelectModel>> SelectionSeasonInformationAsync();

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

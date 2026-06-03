using GCTL.Core.ViewModels.ColorInformation;
using GCTL.Core.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.ColorInformation
{
    public interface IColorInformationService
    {
        Task<List<ColorInformationSetupViewModel>> GetAllAsync();
        Task<ColorInformationSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(ColorInformationSetupViewModel entityVM);
        Task<bool> UpdateAsync(ColorInformationSetupViewModel entityVM);

        Task<IEnumerable<CommonSelectModel>> SelectionColorInformationAsync();

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

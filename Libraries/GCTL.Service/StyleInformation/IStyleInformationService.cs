using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.StyleInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.StyleInformation
{
    public interface IStyleInformationService
    {
        Task<List<StyleInformationSetupViewModel>> GetAllAsync(string id);
        Task<List<StyleInformationSetupViewModel>> GetAllAsync();
        Task<StyleInformationSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(StyleInformationSetupViewModel entityVM);
        Task<bool> UpdateAsync(StyleInformationSetupViewModel entityVM);

        Task<IEnumerable<CommonSelectModel>> SelectionStyleInformationAsync();

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

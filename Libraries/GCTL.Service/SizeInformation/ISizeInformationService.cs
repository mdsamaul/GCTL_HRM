using GCTL.Core.ViewModels.ColorInformation;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.SizeInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.SizeInformation
{
    public interface ISizeInformationService
    {
        Task<List<SizeInformationSetupViewModel>> GetAllAsync();
        Task<SizeInformationSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(SizeInformationSetupViewModel entityVM);
        Task<bool> UpdateAsync(SizeInformationSetupViewModel entityVM);

        Task<IEnumerable<CommonSelectModel>> SelectionSizeInformationAsync();

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

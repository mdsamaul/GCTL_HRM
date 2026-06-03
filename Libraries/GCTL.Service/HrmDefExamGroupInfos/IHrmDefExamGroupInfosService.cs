using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HrmDefExamGroupInfos;
using GCTL.Core.ViewModels.HRMDefExamTitles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HrmDefExamGroupInfos
{
    public interface IHrmDefExamGroupInfosService
    {
        Task<List<HrmDefExamGroupInfosSetupViewModel>> GetAllAsync();
        Task<HrmDefExamGroupInfosSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(HrmDefExamGroupInfosSetupViewModel entityVM);
        Task<bool> UpdateAsync(HrmDefExamGroupInfosSetupViewModel entityVM);
        Task<bool> DeleteTab(List<string> ids);
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string name, string typeCode);
        Task<IEnumerable<CommonSelectModel>> SelectionHrmDefExamGroupInfoTypeAsync();
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

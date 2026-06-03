using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HRMDefBoardCountryNames;
using GCTL.Core.ViewModels.HRMDefExamTitles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HRMDefExamTitles
{
    public interface IHRMDefExamTitlesService
    {
        Task<List<HRMDefExamTitlesSetupViewModel>> GetAllAsync();
        Task<HRMDefExamTitlesSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(HRMDefExamTitlesSetupViewModel entityVM);
        Task<bool> UpdateAsync(HRMDefExamTitlesSetupViewModel entityVM);
        Task<bool> DeleteTab(List<string> ids);
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string name, string typeCode);
       Task<IEnumerable<CommonSelectModel>> SelectionHrmDefExamTitleTypeAsync();
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

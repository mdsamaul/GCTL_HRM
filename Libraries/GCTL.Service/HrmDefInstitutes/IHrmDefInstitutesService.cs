using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HRMDefExamTitles;
using GCTL.Core.ViewModels.HrmDefInstitutes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HrmDefInstitutes
{
    public interface IHrmDefInstitutesService
    {
        Task<List<HrmDefInstitutesSetupViewModel>> GetAllAsync();
        Task<HrmDefInstitutesSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(HrmDefInstitutesSetupViewModel entityVM);
        Task<bool> UpdateAsync(HrmDefInstitutesSetupViewModel entityVM);
        Task<bool> DeleteTab(List<string> ids);
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string name, string typeCode);
        Task<IEnumerable<CommonSelectModel>> SelectionHrmDefInstituteTypeAsync();
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

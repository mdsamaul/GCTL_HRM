using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.Grades;
using GCTL.Core.ViewModels.HrmDefExamGroupInfos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.Grades
{
    public interface IGradesService
    {
        Task<List<GradesSetupViewModel>> GetAllAsync();
        Task<GradesSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(GradesSetupViewModel entityVM);
        Task<bool> UpdateAsync(GradesSetupViewModel entityVM);
        Task<bool> DeleteTab(List<string> ids);
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string name, string typeCode);
        Task<IEnumerable<CommonSelectModel>> SelectionGradesTypeAsync();

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

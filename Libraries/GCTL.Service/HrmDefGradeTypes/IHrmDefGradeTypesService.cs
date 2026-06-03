using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HrmDefExamGroupInfos;
using GCTL.Core.ViewModels.HrmDefGradeTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HrmDefGradeTypes
{
    public interface IHrmDefGradeTypesService
    {
        Task<List<HrmDefGradeTypesSetupViewModel>> GetAllAsync();
        Task<HrmDefGradeTypesSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(HrmDefGradeTypesSetupViewModel entityVM);
        Task<bool> UpdateAsync(HrmDefGradeTypesSetupViewModel entityVM);
        Task<bool> DeleteTab(List<string> ids);
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string name, string typeCode);
        Task<IEnumerable<CommonSelectModel>> SelectionHrmDefGradeTypeAsync();
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);

    }
}


using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HolidayTypes;
using GCTL.Core.ViewModels.HrmDefDegrees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HrmDefDegrees
{
    public interface IHrmDefDegreesService
    {
        Task<List<HrmDefDegreesSetupViewModel>> GetAllAsync();
        Task<HrmDefDegreesSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(HrmDefDegreesSetupViewModel entityVM);
        Task<bool> UpdateAsync(HrmDefDegreesSetupViewModel entityVM);
        Task<bool> DeleteTab(List<string> ids);
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string name, string typeCode);
         Task<IEnumerable<CommonSelectModel>>SelectionHrmDefDegreeTypeAsync();
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

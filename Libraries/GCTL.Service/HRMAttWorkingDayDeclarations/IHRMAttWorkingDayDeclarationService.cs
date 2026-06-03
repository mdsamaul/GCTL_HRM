using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HrmAttWorkingDayDeclarations;
using GCTL.Core.ViewModels.HRMCompanyWeekEnds;
using GCTL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HRMAttWorkingDayDeclarations
{
    public interface IHRMAttWorkingDayDeclarationService
    {
        HrmAttWorkingDayDeclaration GetLeaveType(string code);
        bool DeleteLeaveType(string id);

        Task<List<HrmAttWorkingDayDeclarationSetupViewModel>> GetAllAsync();
        Task<HrmAttWorkingDayDeclarationSetupViewModel> GetByIdAsync(string code);
        Task<bool> SaveAsync(HrmAttWorkingDayDeclarationSetupViewModel entityVM);
        Task<bool> UpdateAsync(HrmAttWorkingDayDeclarationSetupViewModel entityVM);
        Task<bool> DeleteAsync(string id);
        Task<string> GenerateNextCode();
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
       
        Task<bool> IsExistAsync(string typeCode, string workingDayDate);
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

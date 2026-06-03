using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HrmDefSeparationTypes;
using GCTL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HrmDefSeparationTypes
{
    public interface IHrmDefSeparationTypeService
    {
        HrmDefSeparationType GetLeaveType(string code);
        bool DeleteLeaveType(string id);
        Task<List<HrmDefSeparationTypeSetupViewModel>> GetAllAsync();
        Task<HrmDefSeparationTypeSetupViewModel> GetByIdAsync(string code);
        Task<bool> SaveAsync(HrmDefSeparationTypeSetupViewModel entityVM);
        Task<bool> UpdateAsync(HrmDefSeparationTypeSetupViewModel entityVM);
        
        Task<string> GenerateNextCode();
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string name, string typeCode);
        Task<IEnumerable<CommonSelectModel>> SelectionHrmDefSeparationTypeAsync(); 

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

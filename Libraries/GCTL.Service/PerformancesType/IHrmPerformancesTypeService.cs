using GCTL.Core.ViewModels.BranchesTypeInfo;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HrmDefPerformance2;
using GCTL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.PerformancesType
{
    public interface IHrmPerformancesTypeService
    {
        HrmDefPerformance GetLeaveType(string code);
        bool DeleteLeaveType(string id);
        Task<List<HrmDefPerformance2SetupViewModel>> GetAllAsync();
        Task<HrmDefPerformance2SetupViewModel> GetByIdAsync(string code);
        Task<bool> SaveAsync(HrmDefPerformance2SetupViewModel entityVM);
        Task<bool> UpdateAsync(HrmDefPerformance2SetupViewModel entityVM);

        //
        Task<List<HrmDefPerformance2SetupViewModel>> GetByIdJobTitleAsync(string jobTitleCode);
        //

        Task<string> GenerateNextCode();
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string name, string typeCode);
        Task<IEnumerable<CommonSelectModel>> SelectionPerformancesTypeAsync();

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HrmDefPerformance;
using GCTL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HrmDefPerformances
{
    public interface IHrmDefPerformanceService
    {
        HrmDefPerformance GetLeaveType(string code);
        bool DeleteLeaveType(string id);
        Task<List<HrmDefPerformanceSetupViewModel>> GetAllAsync();
        Task<HrmDefPerformanceSetupViewModel> GetByIdAsync(string code);
        Task<bool> SaveAsync(HrmDefPerformanceSetupViewModel entityVM);
        Task<bool> UpdateAsync(HrmDefPerformanceSetupViewModel entityVM);

        Task<string> GenerateNextCode();
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string name, string typeCode);
        Task<IEnumerable<CommonSelectModel>> SelectionHrmDefPerformanceAsync();

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

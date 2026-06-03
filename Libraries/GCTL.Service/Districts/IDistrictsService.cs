using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.Districts;
using GCTL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.Districts
{
    public interface IDistrictsService
    {

        HrmDefDistrict GetLeaveType(string code);
        bool DeleteLeaveType(string id);
        Task<List<DistrictsSetupViewModel>> GetAllAsync();
        Task<DistrictsSetupViewModel> GetByIdAsync(string code);
        Task<bool> SaveAsync(DistrictsSetupViewModel entityVM);
        Task<bool> UpdateAsync(DistrictsSetupViewModel entityVM);

        Task<string> GenerateNextCode();
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string name, string typeCode);
        Task<IEnumerable<CommonSelectModel>> GetDistrictsSelectionsAsync();

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

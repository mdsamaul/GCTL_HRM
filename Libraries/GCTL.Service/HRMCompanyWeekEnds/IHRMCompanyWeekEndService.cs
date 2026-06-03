using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HrmAtdShifts;
using GCTL.Core.ViewModels.HRMCompanyWeekEnds;
using GCTL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HRMCompanyWeekEnds
{
    public interface IHRMCompanyWeekEndService
    {
        HrmAtdCompanyWeekEnd GetLeaveType(string code);
        bool DeleteLeaveType(string id);

        Task<List<HRMCompanyWeekEndSetupViewModel>> GetAllAsync();
        Task<HRMCompanyWeekEndSetupViewModel> GetByIdAsync(string code);
        Task<bool> SaveAsync(HRMCompanyWeekEndSetupViewModel entityVM);
        Task<bool> UpdateAsync(HRMCompanyWeekEndSetupViewModel entityVM);
        Task<bool> DeleteAsync(string id);
        Task<string> GenerateNextCode();
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string name, string typeCode);
        Task<IEnumerable<CommonSelectModel>> SelectionHrmAtdShiftAsync();
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);

    }
}

using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HolidayTypes;
using GCTL.Core.ViewModels.HrmAtdHolidays;
using GCTL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HrmAtdHolidays
{
    public interface IHrmAtdHolidayService
    {
        Task<List<HrmAtdHolidaySetupViewModel>> GetAllAsync();
        Task<HrmAtdHolidaySetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(HrmAtdHolidaySetupViewModel entityVM);
        Task<bool> UpdateAsync(HrmAtdHolidaySetupViewModel entityVM);
        HrmAtdHoliday GetLeaveType(string code);
        bool DeleteLeaveType(string id);
        Task<string> GenerateNextCode();
        IEnumerable<CommonSelectModel> DropSelection();
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string name, string typeCode);
        Task<bool> IsExistAsync(string name, string typeCode,string holidayTypeCode, string fromDate, string toDate);
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

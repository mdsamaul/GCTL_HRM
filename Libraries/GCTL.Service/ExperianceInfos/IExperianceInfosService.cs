using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.ExcessTDSForLastIncomeYear;
using GCTL.Core.ViewModels.ExperianceInfos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.ExperianceInfos
{
    public interface IExperianceInfosService
    {
        Task<List<ExperianceInfosSetupViewModel>> GetAllAsync(string employeeId);
        Task<ExperianceInfosSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(ExperianceInfosSetupViewModel entityVM, string CompanyCode);
        Task<bool> UpdateAsync(ExperianceInfosSetupViewModel entityVM);
        Task<bool> DeleteTab(List<string> ids);
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string employeeCode, string typeCode, string name);

        Task<List<ExperianceInfosSetupViewModel>> GetEmployeeByCompanyCode(string companyCode);
        Task<ExperianceInfosSetupViewModel> GetEmployeeNameDesDeptByCode(string employeeId);

        IEnumerable<CommonSelectModel> SelectionExperianceInfosTypeAsync();
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);

        Task<List<HolidayWeekenderEXPDto>> GetHolidayAndWeekendAsync(int year);

    }
}

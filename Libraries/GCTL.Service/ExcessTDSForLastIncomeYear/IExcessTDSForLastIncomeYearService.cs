using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.ExcessTDSForLastIncomeYear;
using GCTL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.ExcessTDSForLastIncomeYear
{
    public interface IExcessTDSForLastIncomeYearService 
    {
        Task<ExcessTDSForLastIncomeYearFilterListDto> GetFilterDataAsync(ExcessTDSForLastIncomeYearFilterDto filter);
        Task<object> GetPopulateEmployee(string employeeId);

        Task<bool> SaveAsync(ExcessTDSForLastIncomeYearSetupViewModel entityVM, string CompanyCode);
        Task<bool> UpdateAsync(ExcessTDSForLastIncomeYearSetupViewModel entityVM);

        Task<List<ExcessTDSForLastIncomeYearSetupViewModel>> GetAllAsync();
        Task<ExcessTDSForLastIncomeYearSetupViewModel> GetByIdAsync(string code);

        IEnumerable<CommonSelectModel> SelectionExcessTDSForLastIncomeYearAsync();

        Task<bool> DeleteTab(List<string> ids);


        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string employeeCode, string fyear, DateTime efctive, decimal amount);

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Spreadsheet;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.Employees;
using GCTL.Core.ViewModels.HrmAtdHolidays;
using GCTL.Core.ViewModels.HrmDefEmpTypes;
using GCTL.Core.ViewModels.HrmEmployees2;
using GCTL.Data.Models;

namespace GCTL.Service.HrmEmployees2
{
    public interface IHrmEmployee2Service
    {
        Task<List<HrmEmployee2SetUpViewModel>> GetAllAsync();
        Task<HrmEmployee2SetUpViewModel> GetByIdAsync(string id);
        Task<bool> SaveAsync(HrmEmployee2SetUpViewModel entityVM, string CompanyCode);
        Task<bool> UpdateAsync(HrmEmployee2SetUpViewModel entityVM);
        Task<HrmEmployee> GetLeaveType(string code);
        Task<(bool isSuccess, string message, bool refError)> DeleteLeaveType(List<string> ids, DeleteHistoryViewModel model);

        Task<IEnumerable<CommonSelectModel>> GetEmployeeDropSelections();

        Task<bool> IsExistByAsync(string code, string firstName, string dateOfBirthOriginal, string fathersName);
        Task<string> GenerateNextCode();
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

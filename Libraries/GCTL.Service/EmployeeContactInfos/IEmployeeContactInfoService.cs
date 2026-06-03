using GCTL.Core.ViewModels.EmployeeContactInfos;
using GCTL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.EmployeeContactInfos
{
    public interface IEmployeeContactInfoService
    {
        Task<List<EmployeeContactInfosSetupViewModel>> GetAllAsync();
        HrmEmployeeContactInfo GetLeaveType(string code);
        Task<EmployeeContactInfosSetupViewModel> GetByIdAsync(string id);
        Task<bool> SaveAsync(EmployeeContactInfosSetupViewModel entityVM, string CompanyCode);
        Task<bool> UpdateAsync(EmployeeContactInfosSetupViewModel entityVM);
        bool DeleteLeaveType(string id);

        Task<EmployeeContactInfosSetupViewModel> GetEmployeeByCode(string employeeId);
        Task<List<EmployeeContactInfosSetupViewModel>> GetComapnyByBranchCode(string companyCode);
        Task<List<EmployeeContactInfosSetupViewModel>> GetComapnyByCode(string companyCode);

        Task<bool> IsExistByAsync(string code, string EmpContactId);
        Task<string> GenerateNextCode();
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

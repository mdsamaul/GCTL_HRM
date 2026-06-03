using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.EmployeeReferenceInfos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.EmployeeReferenceInfos
{
    public interface IEmployeeReferenceInfosService
    {
        Task<List<EmployeeReferenceInfosSetupViewModel>> GetAllAsync(string employeeId);
        Task<EmployeeReferenceInfosSetupViewModel> GetByIdAsync(string id);

        Task<bool> SaveAsync(EmployeeReferenceInfosSetupViewModel entityVM, string CompanyCode);
        Task<bool> UpdateAsync(EmployeeReferenceInfosSetupViewModel entityVM);
        Task<bool> DeleteTab(List<string> ids);
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string employeeCode, string typeCode, string name, string empReferenceId);

        Task<List<EmployeeReferenceInfosSetupViewModel>> GetEmployeeByCompanyCode(string companyCode);
        Task<List<EmployeeReferenceInfosSetupViewModel>> GetComapnyByBranchCode(string companyCode);
        Task<EmployeeReferenceInfosSetupViewModel> GetEmployeeNameDesDeptByCode(string employeeId);

        IEnumerable<CommonSelectModel> SelectionReferenceTypeAsync();
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

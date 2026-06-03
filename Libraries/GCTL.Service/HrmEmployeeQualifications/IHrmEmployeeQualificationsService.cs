using GCTL.Core.ViewModels.Common;

using GCTL.Core.ViewModels.HrmEmployeeQualifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HrmEmployeeQualifications
{
    public interface IHrmEmployeeQualificationsService
    {

        Task<List<HrmEmployeeQualificationsSetupViewModel>> GetAllAsync(string employeeId);
        Task<HrmEmployeeQualificationsSetupViewModel> GetByIdAsync(string code);

        Task<List<HrmEmployeeQualificationsSetupViewModel>> GetEmployeeByCompanyCode(string companyCode);

        Task<HrmEmployeeQualificationsSetupViewModel> GetEmployeeNameDesDeptByCode(string employeeId);
        Task<bool> SaveAsync(HrmEmployeeQualificationsSetupViewModel entityVM, string CompanyCode);
        Task<bool> UpdateAsync(HrmEmployeeQualificationsSetupViewModel entityVM);
        Task<bool> DeleteTab(List<string> ids);
        Task<bool> IsExistByAsync(string code);
        Task<bool> IsExistAsync(string code, string employeeCode, string courseTypeId, string couresetitleID);

        Task<IEnumerable<CommonSelectModel>> DropSelection();
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissonAsync(string accessCode);
        Task<bool> UpdateParmissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);

    }
}
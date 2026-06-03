using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HRMDefExamTitles;
using GCTL.Core.ViewModels.HrmDefInstitutes;
using GCTL.Core.ViewModels.HrmEmployeeAdditionalInfos;
using GCTL.Core.ViewModels.HrmEmployeeEducations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HrmEmployeeEducations
{
    public interface IHrmEmployeeEducationsService
    {
        Task<List<HrmEmployeeEducationsSetupViewModel>> GetAllAsync(string employeeId);

        Task<HrmEmployeeEducationsSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(HrmEmployeeEducationsSetupViewModel entityVM);
        Task<bool> UpdateAsync(HrmEmployeeEducationsSetupViewModel entityVM);

        Task<List<HrmEmployeeEducationsSetupViewModel>> GetEmployeeByCompanyCode(string companyCode);
        Task<HrmEmployeeEducationsSetupViewModel> GetEmployeeNameDesDeptByCode(string employeeId);

        Task<bool> DeleteTab(List<string> ids);
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string employeeCode, string typeCode, string degreeCode , string eduCode);

        IEnumerable<CommonSelectModel> SelectionHrmDefEmpEduTypeAsync();
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}
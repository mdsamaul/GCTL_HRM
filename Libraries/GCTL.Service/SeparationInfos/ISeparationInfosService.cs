using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.SeparationInfos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.SeparationInfos
{
    public interface ISeparationInfosService
    {
        Task<List<SeparationInfosSetupViewModel>> GetAllAsync();
        Task<SeparationInfosSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(SeparationInfosSetupViewModel entityVM);
        Task<bool> UpdateAsync(SeparationInfosSetupViewModel entityVM);
        Task<bool> DeleteTab(List<string> ids);
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string employeeCode, string typeCode, string name);

        Task<SeparationInfosSetupViewModel> GetEmployeeByCode(string employeeId);
        Task<List<SeparationInfosSetupViewModel>> GetComapnyByCode(string companyCode);

        IEnumerable<CommonSelectModel> SelectionSeparationInfosTypeAsync();
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

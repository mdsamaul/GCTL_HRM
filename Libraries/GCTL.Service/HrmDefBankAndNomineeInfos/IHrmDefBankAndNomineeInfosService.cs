using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HrmDefBankAndNomineeInfos;
using GCTL.Core.ViewModels.HrmEmployeeDocumentInfos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HrmDefBankAndNomineeInfos
{
    public interface IHrmDefBankAndNomineeInfosService
    {
        Task<List<HrmDefBankAndNomineeInfosSetupViewModel>> GetAllAsync(string employeeId);
        Task<HrmDefBankAndNomineeInfosSetupViewModel> GetByIdAsync(string code);
        Task<List<HrmDefBankAndNomineeInfosSetupViewModel>> GetEmployeeByCompanyCode(string companyCode);
        Task<List<HrmDefBankAndNomineeInfosSetupViewModel>> GetComapnyByBranchCode(string companyCode);
        Task<HrmDefBankAndNomineeInfosSetupViewModel> GetEmployeeNameDesDeptByCode(string employeeId);
        Task<bool> SaveAsync(HrmDefBankAndNomineeInfosSetupViewModel entityVM, string CompanyCode);
        Task<bool> UpdateAsync(HrmDefBankAndNomineeInfosSetupViewModel entityVM);
        Task<bool> DeleteTab(List<string> ids);
        Task<bool> IsExistAsync(string bankCode, string branchBankCode, string acName, string acNO, string nomineeName, string code);
        IEnumerable<CommonSelectModel> SelectionHrmDefEmpDocumentTypeAsync();
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

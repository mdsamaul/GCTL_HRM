using GCTL.Core.ViewModels.CoreBankAccountInformations;
using GCTL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.CoreBankAccountInformations
{
    public interface ICoreBankAccountInformationService
    {
        public Task<List<CoreBankAccountInformationSetupViewModel>> GetAllAsync();
        public Task<CoreBankAccountInformationSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(CoreBankAccountInformationSetupViewModel entityVM, string CompanyCode);
        Task<bool> UpdateAsync(CoreBankAccountInformationSetupViewModel entityVM);
        CoreBankAccountInformation GetLeaveType(string code);
        bool DeleteLeaveType(string id);
        Task<bool> DeleteAsync(string id);

        Task<string> GenerateNexCode();
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string accountName, string typeCode, string accountNo,string bankId, string branchId);
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);

    }
}

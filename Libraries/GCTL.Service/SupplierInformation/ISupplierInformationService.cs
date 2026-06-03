using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.SupplierInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.SupplierInformation
{
    public interface ISupplierInformationService
    {
        Task<List<SupplierInformationSetupViewModel>> GetAllAsync();
        Task<List<SalesSupplierBankAccountTempDto>> GetTableBankAccountInfoDataAsync();
        Task<SupplierInformationSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(SupplierInformationSetupViewModel entityVM);
        Task<bool> UpdateAsync(SupplierInformationSetupViewModel entityVM);

        IEnumerable<CommonSelectModel> SelectionSupplierInformationTypeAsync();

        Task<bool> DeleteTab(List<string> ids);

        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string employeeCode, string phone, string email);

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
        Task<string> Autoid();
        Task<bool> BankAccountInfoSaveEditAsync(SalesSupplierBankAccountTempDto bankAccountInfoSaveEdit);
        Task<bool> BankAccountInfoDeleteAsync(string sbaid);
        Task<bool> BankAccountInfoClearTableTempAsync();
    }
}

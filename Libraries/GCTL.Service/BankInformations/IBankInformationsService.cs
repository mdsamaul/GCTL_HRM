using DocumentFormat.OpenXml.Office2013.Word;
using GCTL.Core.ViewModels.BankInformations;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HrmDefEmpTypes;
using GCTL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.BankInformations
{
    public interface IBankInformationsService
    {


        Task<List<BankInformationsSetupViewModel>> GetAllAsync();
        Task<BankInformationsSetupViewModel> GetByIdAsync(string code);
        Task<bool> SaveAsync(BankInformationsSetupViewModel entityVM);
        Task<bool> UpdateAsync(BankInformationsSetupViewModel entityVM);

        //SalesDefBankInfo GetLeaveType(string code);
        //bool DeleteLeaveType(string id);

        SalesDefBankInfo GetBankById(string code);
        bool DeleteBank(string id);
        Task<bool> DeleteAsync(string id);
        Task<string> GenerateNextCode();

        IEnumerable<CommonSelectModel> BankDropSelectionAsync();
        
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string name, string typeCode);
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

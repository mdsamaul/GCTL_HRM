using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.ViewModels.INV_Catagory;
using GCTL.Core.ViewModels.InvDefSupplierType;

namespace GCTL.Service.InvDefSupplierTypes
{
    public interface IInvDefSupplierTypeService
    {
        Task<List<InvDefSupplierTypeSetupViewModel>> GetAllAsync();
        Task<InvDefSupplierTypeSetupViewModel> GetByIdAsync(string id);
        Task<(bool isSuccess, string message, object data)> CreateUpdateAsync(InvDefSupplierTypeSetupViewModel model);
        Task<(bool isSuccess, string message, object data)> DeleteAsync(List<string> ids);
        Task<string> AutoSuplierTypeIdAsync();

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
        Task<(bool isSuccess, string message, object data)> AlreadyExistAsync(string catagoryValue);
    }
}

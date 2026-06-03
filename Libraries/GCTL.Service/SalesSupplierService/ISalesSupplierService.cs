using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.INV_Catagory;
using GCTL.Core.ViewModels.SalesSupplier;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.SalesSupplierService
{
    public interface ISalesSupplierService
    {
        Task<List<SalesSupplierSetupViewModel>> GetAllAsync();
        Task<SalesSupplierSetupViewModel> GetByIdAsync(string id);
        Task<(bool isSuccess, string message, object data)> CreateUpdateAsync(SalesSupplierSetupViewModel model);
        Task<(bool success, bool refSuccess, string message)> DeleteAsync(List<string> ids, DeleteHistoryViewModel model);
        Task<string> AutoSalesSupplierIdAsync();

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
        Task<(bool isSuccess, string message, object data)> AlreadyExistAsync(string catagoryValue);
    }
}

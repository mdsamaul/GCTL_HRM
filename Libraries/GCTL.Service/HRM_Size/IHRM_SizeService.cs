using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HRM_Size;
using GCTL.Core.ViewModels.INV_Catagory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HRM_Size
{
    public interface IHRM_SizeService
    {
        Task<List<HRM_SizeSetupViewModel>> GetAllAsync();
        Task<HRM_SizeSetupViewModel> GetByIdAsync(string id);
        Task<(bool isSuccess, string message, object data)> CreateUpdateAsync(HRM_SizeSetupViewModel model);
        Task<(bool success, bool refSuccess, string message)> DeleteAsync(List<string> ids, DeleteHistoryViewModel model);
        Task<string> AutoSizeyIdAsync();

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
        Task<(bool isSuccess, string message, object data)> AlreadyExistAsync(string catagoryValue);
    }
}

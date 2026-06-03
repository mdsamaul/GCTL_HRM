
using GCTL.Core.ViewModels.BuyerDepartment;
using GCTL.Core.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.BuyerDepartmentEntry
{
    public interface IBuyerDepEntryService
    {
        Task<bool> IsDuplicate(string name, string id);
        Task<(bool isSuccess, string message)> BulkDeleteAsync(List<int> tcs);
        Task<(bool isSuccess, string message, object data)> SaveAsync(InvBuyerDepartmentViewModel model);
        Task<InvBuyerDepartmentViewModel> GetByIdAsync(int id);
        Task<(List<InvBuyerDepartmentViewModel> Data, int totalRecord, int curentRecord)> GetPaginatedDataAsync(string searchValue, int page, int pageSize, string sortColumn, string sortDirection);
        Task<IEnumerable<CommonSelectModel>> SelectionBuyerDepAsync();



        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }

}

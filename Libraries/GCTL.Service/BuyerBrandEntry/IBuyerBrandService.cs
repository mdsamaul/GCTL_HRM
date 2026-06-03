using GCTL.Core.ViewModels.BuyerBrands;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.BuyerBrandEntry
{
    public interface IBuyerBrandService
    {
        Task<(bool isSuccess, string message)> DeleteImageAsync(decimal tc);
        Task<bool> IsDuplicate(string brandName, string buyerId, string id);
        Task<(bool isSuccess, string message)> BulkDeleteAsync(List<decimal> tcs, bool useTransaction = true);
        Task<(bool isSuccess, string message, object data)> SaveAsync(RMGProdBrandViewModel model);
        Task<RMGProdBrandViewModel> GetByIdAsync(decimal id);
        Task<(List<RMGProdBrandViewModel> Data, int totalRecord, int curentRecord)> GetPaginatedDataAsync(string searchValue, int page, int pageSize, string sortColumn, string sortDirection, string id, string buyerId);


        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

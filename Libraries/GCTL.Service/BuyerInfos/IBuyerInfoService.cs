
using DocumentFormat.OpenXml.Drawing;
using GCTL.Core.ViewModels.BuyerInfos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.BuyerInfos
{
    public interface IBuyerInfoService
    {
        Task<(bool isSuccess, string message)> BulkDeleteAsync(List<decimal> tcs);
        Task<(bool isSuccess, string message)> DeleteImageAsync(decimal tc);
        Task<(bool isSuccess, string message, object data)> SaveAsync(BuyerInfoSetupViewModel model);
        Task<BuyerInfoSetupViewModel> GetByIdAsync(decimal id);
        Task<(List<BuyerInfoGridViewModel> Data, int totalRecord, int curentRecord)> GetPaginatedDataAsync(string searchValue, int page, int pageSize, string sortColumn, string sortDirection, string id);


        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

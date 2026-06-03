using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.ViewModels.ItemMasterInformation;
using GCTL.Core.ViewModels.ItemModel;
using GCTL.Core.ViewModels.PrintingStationeryPurchaseEntry;
using GCTL.Core.ViewModels.ProductIssueEntry;
using GCTL.Core.ViewModels.SalesSupplier;

namespace GCTL.Service.ProductIssueEntrys
{
    public interface IProductIssueEntryService
    {
        Task<List<ProductIssueEntrySetupViewModel>> GetAllAsync();
        Task<ProductIssueEntrySetupViewModel> GetByIdAsync(string id);
        Task<(bool isSuccess, string message, object data)> CreateUpdateAsync(ProductIssueEntrySetupViewModel model, string companyCode);
        Task<(bool isSuccess, string message, object data)> DeleteAsync(List<string> ids);
        Task<string> AutoProdutIssueIdAsync();

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
        Task<(bool isSuccess, string message, object data)> AlreadyExistAsync(string catagoryValue);
        Task<(bool isSuccess, string message, object data)> PurchaseIssueAddmoreCreateEditDetailsAsync(ProductIssueInformationDetailViewModel model);
        Task<List<ProductIssueInformationDetailViewModel>> LoadTempDataAsync();
        Task<(bool isSuccess, object data)> detailsDeleteByIdAsync(decimal id);
        Task<(bool isSuccess, object data)> detailsEditByIdAsync(decimal id);
        Task<ProductIssueEntrySetupViewModel> EditPopulateIssueidAsync(decimal issueId);
    }
}

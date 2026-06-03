using GCTL.Core.ViewModels.RMGProdOrderInformationEntry;
using GCTL.UI.Core.Views.RMGProdOrderInformationEntry;

namespace GCTL.Service.RMGProdOrderInformationEntry
{
    public interface IRMGProdOrderInformationEntryService
    {
        Task<string> EntryAutoIdAsync();
        Task<string> IntegraJOBNoAutoAsync();
        Task<(bool isSuccess, string message, object data)> OrderSaveEditAsync(RMG_Prod_OrderDto fromData, string companyCode);
        Task<(bool isSuccess, string message, object data)> DetailsSaveEditAsync(RMG_Prod_OrderDetailsDto fromData, string companyCode);
        Task<(bool isSuccess, string message, object data)> SaveEditColorSizeBreakupAsync(RMG_Prod_Temp_ColorSizeBreakupDto fromData, string companyCode);
        Task<(bool isSuccess, string message, object data)> SaveEditColorSizeBreakupListAsync(RMG_Prod_Temp_ColorSizeBreakupDto fromData, string companyCode);
        Task<bool> UpdateColorSizeBreakupsAsync(List<RMG_Prod_Temp_ColorSizeBreakupDto> dtos);
        Task<(bool isSuccess, string message, object data)> DeleteOrderInfoAsync(List<decimal> selectedIds);
        Task<(bool isSuccess, string message, object data)> DeleteOrderDetailsAsync(List<decimal> selectedIds);
        Task<PagedResult<RMG_Prod_OrderDto>> GetPagedOrdersAsync(DataTableFilter filter);
        Task<PagedResult<RMG_Prod_OrderDetailsDto>> GetPagedOrderDetailsAsync(DataTableFilter filter);
        Task<PagedResult<RMG_Prod_Temp_ColorSizeBreakupDto>> GetPagedColorSizeBreakupsAsync(DataTableFilter filter);
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
        Task SaveFromTempToMainAsync(RMG_Prod_Temp_ColorSizeBreakupDto fromData);
        Task ClearTempDataAsync(string integraJobNo);
        //Task PoIjobNoGetTempAsync(OrderJobDto orderJobDto);
        Task<(List<string> colorIds, List<string> sizeIds)> PoIjobNoGetTempAsync(OrderJobDto orderJobDto);
        Task<List<MerchandiserContactPersonDto>> GetMerchandiserContactPersonListAsync();

    }
}

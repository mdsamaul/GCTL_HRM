using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.SalesDefInvMainItem;

namespace GCTL.Service.SALES_Def_Inv_MainItemGroupService
{
    public interface ISALES_Def_Inv_MainItemGroup
    {
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
        Task<(bool isSuccess, string message)> AdddEditMainSetupAsync(SalesDefInvMainItemSetupDto modelData, string companyCode);
        Task<(bool isSuccess, string message)> AdddEditSubSetupAsync(SalesDefInvSubItemDto modelData, string companyCode);
        Task<(bool isSuccess, string message)> AdddEditSubTwoSetupAsync(RmgProdDefInvSubItem2Dto modelData, string companyCode);
        Task<(bool isSuccess, string message)> AdddEditItemInfoSetupAsync(InvDefItemDto modelData, string companyCode);
        Task<(bool isSuccess, string message)> StockLevelManagementSetupAsync(DefInvStockLevelManagementDto modelData, string companyCode);
        Task<string> GetAutoAllIdAsync(string tabName);
        Task<(List<SalesDefInvMainItemSetupDto> data, int totalRecords)> GetMainGroup(string sortColumn, string sortColumnDir, string searchValue, int skip, int pageSize);
        //Task<(List<DefInvStockLevelManagementDto> data, int totalRecords)> LoadStockLevelManagementDataAsync(string sortColumn, string sortColumnDir, string searchValue, int skip, int pageSize);
        Task<(bool isSuccess, string message)> DeleteAsync(DeleteItemRequest modelData);
        Task<SelectDropdownResultDto> ChangeAbleDropdownAsync(SelectDropdownFilterDto filterData);

        Task<bool> UploadPhotoAsync(ItemPhotoDto dto);
        Task<bool> DeletePhotoAsync(string itemId);
        Task<ItemPhotoDto> GetPhotoByItemIdAsync(string itemId);
        Task<IEnumerable<CommonSelectModel>> SelectionGetItemTypeTitleAsync();

        Task<ItemHierarchyResponseDto> GetItemHierarchyAsync(ItemHierarchyFilterDto filter);
    }
}

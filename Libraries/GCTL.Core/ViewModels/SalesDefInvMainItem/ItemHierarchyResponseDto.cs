namespace GCTL.Core.ViewModels.SalesDefInvMainItem
{
    public class ItemHierarchyResponseDto
    {
        public List<SalesDefInvMainItemSetupDto> MainGroupList { get; set; } = new();
        public List<SalesDefInvSubItemDto> SubGroupList { get; set; } = new();
        public List<RmgProdDefInvSubItem2Dto> Sub2GroupList { get; set; } = new();
        public List<InvDefItemDto> ItemList { get; set; } = new();
        public List<DefInvStockLevelManagementDto> StockItemList { get; set; } = new();
    }
}

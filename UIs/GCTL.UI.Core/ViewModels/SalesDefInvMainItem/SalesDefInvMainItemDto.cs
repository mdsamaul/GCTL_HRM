using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.SalesDefInvMainItem;

namespace GCTL.UI.Core.ViewModels.SalesDefInvMainItem
{
    public class SalesDefInvMainItemDto : BaseViewModel
    {
        public SalesDefInvMainItemSetupDto MainSetup { get; set; } = new SalesDefInvMainItemSetupDto();
        public SalesDefInvSubItemDto SubItem { get; set; } = new SalesDefInvSubItemDto();
        public RmgProdDefInvSubItem2Dto SubItemTwo { get; set; } = new RmgProdDefInvSubItem2Dto();
        public InvDefItemDto MainItem { get; set; } = new InvDefItemDto();
        public DefInvStockLevelManagementDto StockLevelManagement { get; set; } = new DefInvStockLevelManagementDto();
    }
}

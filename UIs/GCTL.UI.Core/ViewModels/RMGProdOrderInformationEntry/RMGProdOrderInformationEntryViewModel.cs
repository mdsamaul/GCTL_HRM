using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.RMGProdOrderInformationEntry;
using GCTL.UI.Core.Views.RMGProdOrderInformationEntry;

namespace GCTL.UI.Core.ViewModels.RMGProdOrderInformationEntry
{
    public class RMGProdOrderInformationEntryViewModel : BaseViewModel
    {
        public RMG_Prod_OrderDto OrderDto { get; set; } = new RMG_Prod_OrderDto();
        public RMG_BookingOrderDto BookingOrderDto { get; set; } = new RMG_BookingOrderDto();
        public RMG_Prod_OrderDetailsDto OrderDetailsDto { get; set; } = new RMG_Prod_OrderDetailsDto();
        public RMG_Prod_Temp_ColorSizeBreakupDto TempColorSizeBreakupDto { get; set; } = new RMG_Prod_Temp_ColorSizeBreakupDto();
        public RMG_Prod_Temp_List_ColorSizeBreakupDto TempList_ColorSizeBreakupDto { get; set; } = new RMG_Prod_Temp_List_ColorSizeBreakupDto();
    }
}

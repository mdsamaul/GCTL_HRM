using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.RMGBookingOrderEntryBukl;

namespace GCTL.UI.Core.ViewModels.RMGBookingOrderEntryBukl
{
    public class RMGBookingOrderEntryBuklViewModel : BaseViewModel
    {
        public RMGBookingOrderEntryBuklDto BookingOrderEntryBuklSetup { get; set; } = new RMGBookingOrderEntryBuklDto();
        public BookingReceivedDetailsThreadDto bookingReceivedDetailsThreadSetup { get; set; } = new BookingReceivedDetailsThreadDto();
    }
}

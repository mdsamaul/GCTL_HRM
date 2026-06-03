using GCTL.Core.ViewModels;
using GCTL.Service.HRMTransportExpenseEntryService;

namespace GCTL.Core.ViewModels.HRMTransportExpenseEntry
{
    public class TransportDetailsDto:BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string TransportID { get; set; }
        public string TransportNo { get; set; }
        public string TransportTypeID { get; set; }
        public string TransportType { get; set; }
        public string DriverId { get; set; }
        public string DriverName { get; set; }
        public string DriverPhone { get; set; }
        public List<TransportUserDto> TransportUsers { get; set; }
    }
}
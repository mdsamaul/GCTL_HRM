using GCTL.Data.Models;

namespace GCTL.Service.ViewModels.Commissions
{
    public class CommissionReceiverModel
    {
        public HmsCommissionPaymentHistory History { get; set; }
        public string ReceiverName { get; set; }
    }
}

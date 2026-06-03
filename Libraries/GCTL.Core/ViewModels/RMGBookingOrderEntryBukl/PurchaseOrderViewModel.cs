namespace GCTL.Core.ViewModels.RMGBookingOrderEntryBukl
{
    public class PurchaseOrderViewModel
    {
        public string CostingId { get; set; }
        public string Style { get; set; }
        public string StyleName { get; set; }
        public string FunJobNo { get; set; }
        public string Buyer { get; set; }
        public string BuyerName { get; set; }
        public string PoNo { get; set; }
        public int? OrderQty { get; set; }
        public string MasterPo { get; set; }
        public bool Selected { get; set; }
    }
}

namespace GCTL.Core.ViewModels.MonthWiseOrderBookingReport
{
    public class OrderReportData
    {
        public string OrderId { get; set; }
        public string BuyerId { get; set; }
        public string BuyerOrderNo { get; set; }
        public string MasterPurchaseOrder { get; set; }
        public string StyleId { get; set; }
        public string PODate { get; set; }
        public string DetailOrderId { get; set; }
        public string ProductId { get; set; }
        public string PurchaseOrder { get; set; }
        public string Style { get; set; }
        public string RefNo { get; set; }
        public string ColorId { get; set; }
        public string SizeId { get; set; }
        public Dictionary<string, string> MonthlyQuantities { get; set; } = new Dictionary<string, string>();
        public string TotalQuantity { get; set; }
    }
}

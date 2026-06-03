namespace GCTL.Core.ViewModels.MonthWiseOrderBookingReport
{

    public class OrderReportAllStyleResponse
    {
        public string CompanyName { get; set; }
        public string ReportTitle { get; set; }
        public string ReportYear { get; set; }
        public List<OrderReportDataAllStyle> Data { get; set; }
    }


    public class OrderReportDataAllStyle
    {
        public string SlNo { get; set; }
        public string BuyerName { get; set; }
        public string Style { get; set; }
        public string Item { get; set; }
        public string TotalOrderQuantity { get; set; }
        public Dictionary<string, string> MonthlyQuantities { get; set; } = new Dictionary<string, string>();
    }

    public class OrderReportRequestAllStyle
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? FromYear { get; set; }
        public int? ToYear { get; set; }
        public List<string> BuyerIds { get; set; }
        public List<string> StyleIds { get; set; }
        public List<string> PurchaseOrders { get; set; }
        public List<string> ColorIds { get; set; }
        public List<string> SizeIds { get; set; }
    }


    public class OrderReportDataStyle
    {
        public string SlNo { get; set; }
        public string BuyerName { get; set; }
        public string Style { get; set; }
        public string Item { get; set; }
        public string TotalOrderQuantity { get; set; }
        public Dictionary<string, string> MonthlyQuantities { get; set; } = new Dictionary<string, string>();
    }
    public class OrderReportStyleResponse
    {
        public string CompanyName { get; set; }
        public string ReportTitle { get; set; }
        public string ReportYear { get; set; }
        public List<OrderReportDataStyle> Data { get; set; }
        public List<string> MonthColumns { get; set; }
    }


}

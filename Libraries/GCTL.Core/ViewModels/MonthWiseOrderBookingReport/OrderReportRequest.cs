namespace GCTL.Core.ViewModels.MonthWiseOrderBookingReport
{
    //public class OrderReportRequest
    //{
    //    public DateTime? FromDate { get; set; }
    //    public DateTime? ToDate { get; set; }
    //    public int? FromYear { get; set; }
    //    public int? ToYear { get; set; }
    //    public List<string> BuyerIds { get; set; }
    //    public List<string> StyleIds { get; set; }
    //    public List<string> PurchaseOrders { get; set; }
    //    public List<string> ColorIds { get; set; }
    //    public List<string> SizeIds { get; set; }
    //    public int PageNumber { get; set; } = 1;
    //    public int PageSize { get; set; } = 10;
    //}
    public class OrderReportRequest : BaseViewModel
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? FromYear { get; set; }
        public int? ToYear { get; set; }
        public List<string> BuyerIds { get; set; }
        //public List<string> StyleIds { get; set; }
        //public List<string> PurchaseOrders { get; set; }
        //public List<string> ColorIds { get; set; }
        public string SizeIds { get; set; }
    }


    public class OrderReportDataStylePo
    {
        public string SlNo { get; set; }
        public string BuyerName { get; set; }
        public string Style { get; set; }
        public string Item { get; set; }
        public string PurchaseOrder { get; set; }
        public string OrderQuantity { get; set; }
        public Dictionary<string, string> MonthlyQuantities { get; set; } = new Dictionary<string, string>();
    }

    public class OrderReportStylePoResponse
    {
        public string CompanyName { get; set; }
        public string ReportTitle { get; set; }
        public string ReportYear { get; set; }
        public List<OrderReportDataStylePo> Data { get; set; }
        public List<string> MonthColumns { get; set; }
    }

    public class OrderReportDataStylePoCS
    {
        public string SlNo { get; set; }
        public string BuyerName { get; set; }
        public string Style { get; set; }
        public string Item { get; set; }
        public string PurchaseOrder { get; set; }
        public string OrderQuantity { get; set; }
        public Dictionary<string, List<ColorSizeDetail>> MonthlyData { get; set; } = new Dictionary<string, List<ColorSizeDetail>>();


    }

    public class ColorSizeDetail
    {
        public string Color { get; set; }
        public string Size { get; set; }
        public string Quantity { get; set; }
    }

    public class OrderReportStylePoCSResponse
    {
        public string CompanyName { get; set; }
        public string ReportTitle { get; set; }
        public string ReportYear { get; set; }
        public List<OrderReportDataStylePoCS> Data { get; set; }
        public List<string> MonthColumns { get; set; }
    }

}

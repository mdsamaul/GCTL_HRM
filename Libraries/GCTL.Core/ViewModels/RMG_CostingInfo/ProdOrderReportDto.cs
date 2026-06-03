namespace GCTL.Core.ViewModels.RMG_CostingInfo
{
    // DTOs/ColorSizeBreakupDto.cs
    public class ColorSizeBreakupDto
    {
        public string StyleId { get; set; }
        public string StyleName { get; set; }
        public string ColorId { get; set; }
        public string ColorName { get; set; }
        public string SizeId { get; set; }
        public string SizeName { get; set; }
        public int? Quantity { get; set; }
    }

    // DTOs/ProdOrderReportDto.cs
    public class ProdOrderReportDto
    {
        public string BuyerId { get; set; }
        public string BuyerName { get; set; }
        public string IntegraJOBNo { get; set; }
        public string StylePOWise { get; set; }
        public string StyleId { get; set; }
        public string StyleName { get; set; }
        public string MasterPurchaseOrder { get; set; }
        public string PurchaseOrder { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string PDescription { get; set; }
        public string SupplierId { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string LUser { get; set; }
        public List<ColorSizeBreakupDto> ColorSizeBreakups { get; set; } = new List<ColorSizeBreakupDto>();
    }

}

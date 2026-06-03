namespace GCTL.Core.ViewModels.RMG_CostingInfo
{
    public class CostingReportDto
    {
        public string CostingId { get; set; }
        public DateTime? EntryDate { get; set; }
        public DateTime? EntryDateTime { get; set; }
        public string IssuedBy { get; set; }
        public string CheckedBy { get; set; }
        public string CheckedName { get; set; }
        public string Remarks { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public string BuyerId { get; set; }
        public string BuyerName { get; set; }
        public string StyleId { get; set; }
        public string StyleName { get; set; }
        public string ProductId { get; set; }
        public string ItemName { get; set; }
        public string ProductDescription { get; set; }
        public string PoNo { get; set; }
        public string FunJobNo { get; set; }
        public string RefNo { get; set; }
        public string LUser { get; set; }
        public decimal SubTotalAmountShhkg { get; set; }
        public decimal SubTotalAmountBdt { get; set; }
        public decimal SubTotalAmountThb { get; set; }
        public decimal TotalAmountShhkg { get; set; }
        public decimal TotalAmountBdt { get; set; }
        public decimal TotalAmountThb { get; set; }
        public decimal TotalMaterialCostOverseas { get; set; }
        public decimal TotalMaterialCostBdt { get; set; }
        public decimal CmandProfit { get; set; }
        public decimal HandlingCharge { get; set; }
        public decimal Ffprice { get; set; }
        public decimal ProductionUpCharge { get; set; }
        public decimal TotalMaterialCostBkk { get; set; }
        public decimal DamagePercentage { get; set; }
        public decimal InterestOverheadPercentage { get; set; }
        public List<ColorSizeBreakupReportDto> ColorSizeBreakups { get; set; } = new();
        public List<CostingDetailReportDto> Details { get; set; } = new();
    }

}

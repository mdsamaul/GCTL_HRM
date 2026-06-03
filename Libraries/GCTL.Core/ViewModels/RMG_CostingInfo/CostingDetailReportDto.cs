namespace GCTL.Core.ViewModels.RMG_CostingInfo
{
    public class CostingDetailReportDto
    {
        public string CostingDetailsId { get; set; }
        public string CostingId { get; set; }
        public int Slno { get; set; }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public string Width { get; set; }
        public decimal Quantity { get; set; }
        public decimal Consumption { get; set; }
        public decimal Extra { get; set; }
        public string TotalPriceCurrencyId { get; set; }
        public decimal TotalAmountShhkg { get; set; }
        public decimal TotalAmountBdt { get; set; }
        public decimal TotalAmountThb { get; set; }
        public string TotalQuantityUnit { get; set; }
        public string Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
    }

}

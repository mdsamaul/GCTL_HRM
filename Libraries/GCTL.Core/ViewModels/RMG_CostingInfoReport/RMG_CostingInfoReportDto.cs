// File: Core/ViewModels/RMG_CostingInfoReport/CostingReportViewModels.cs
namespace GCTL.Core.ViewModels.RMG_CostingInfoReport
{
    public class CostingFilterRequest
    {
        public List<string> CostingIds { get; set; } = new();
        public List<string> BuyerIds { get; set; } = new();
    }

    public class CostingFilterResponse
    {
        public List<FilterItem> Costings { get; set; } = new();
        public List<FilterItem> Buyers { get; set; } = new();
    }

    public class FilterItem
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }

    public class CostingReportData
    {
        public string CostingId { get; set; } = string.Empty;
        public DateTime? EntryDateTime { get; set; }
        public string IssuedBy { get; set; } = string.Empty;
        public string CheckedBy { get; set; } = string.Empty;
        public string CheckedName { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public string BuyerName { get; set; } = string.Empty;
        public string FunJobNo { get; set; } = string.Empty;
        public string StyleName { get; set; } = string.Empty;
        public string PoNo { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public string RefNo { get; set; } = string.Empty;
        public string PurchaseOrder { get; set; } = string.Empty;
        public DateTime? ShipmentDate { get; set; }

        public decimal SubTotalAmountShhkg { get; set; }
        public decimal SubTotalAmountBdt { get; set; }
        public decimal SubTotalAmountThb { get; set; }
        public decimal TotalAmountThb { get; set; }
        public decimal DamagePercentage { get; set; }
        public decimal InterestOverheadPercentage { get; set; }
        public decimal TotalMaterialCostOverseas { get; set; }
        public decimal TotalMaterialCostBdt { get; set; }
        public decimal CmandProfit { get; set; }
        public decimal HandlingCharge { get; set; }
        public decimal ProductionUpCharge { get; set; }
        public decimal Ffprice { get; set; }

        public List<CostingDetail> Details { get; set; } = new();
        public List<ColorSizeBreakup> ColorSizeBreakups { get; set; } = new();
    }

    public class CostingReportForExcel : CostingReportData { }

    public class CostingDetail
    {
        public int Slno { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Width { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Consumption { get; set; }
        public decimal Extra { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public string TotalQuantityUnit { get; set; } = string.Empty;
        public decimal TotalAmountShhkg { get; set; }
        public decimal TotalAmountBdt { get; set; }
        public decimal TotalAmountThb { get; set; }
    }

    public class ColorSizeBreakup
    {
        public string ColorName { get; set; } = string.Empty;
        public string SizeName { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }

    // Dapper এর জন্য প্রাইভেট হেল্পার ক্লাস
    public class ColorSizeWithCostingId
    {
        public string CostingId { get; set; } = string.Empty;
        public string ColorName { get; set; } = string.Empty;
        public string ColorId { get; set; } = string.Empty;
        public string SizeName { get; set; } = string.Empty;
        public string SizeId { get; set; } = string.Empty;
        public int Quantity { get; set; }
    }

    public class CostingDetailWithCostingId
    {
        public string CostingId { get; set; } = string.Empty;
        public int Slno { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Width { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Consumption { get; set; }
        public decimal Extra { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public string TotalQuantityUnit { get; set; } = string.Empty;
        public decimal TotalAmountShhkg { get; set; }
        public decimal TotalAmountBdt { get; set; }
        public decimal TotalAmountThb { get; set; }
    }
}
namespace GCTL.Core.ViewModels.RMG_CostingInfo
{
    public class RmgCostingDetailsTempDto
    {
        public decimal Id { get; set; }
        public string CostingDetailsId { get; set; }
        public string CostingId { get; set; }
        public string Slno { get; set; }
        public string BookingItemTypeId { get; set; }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public string Width { get; set; }
        public string ColorId { get; set; }
        public string ColorName { get; set; }
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string PoNo { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Consumption { get; set; }
        public decimal? Extra { get; set; }
        public decimal? TotalQuantity { get; set; }
        public string TotalQuantityUnit { get; set; }
        public string UnitName { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public string TotalPriceCurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public decimal? TotalAmountShhkg { get; set; }
        public decimal? TotalAmountBdt { get; set; }
        public decimal? TotalAmountThb { get; set; }
        public string ResponsibleBy { get; set; }
        public string ResponsibleByName { get; set; }
        public string Luser { get; set; }
    }

    public class CalculateSummaryRequest
    {
        public string CostingId { get; set; }
        public decimal DamagePercent { get; set; }
        public decimal InterestPercent { get; set; }
        public decimal CmAndProfit { get; set; }
        public decimal HandlingCharge { get; set; }
        public decimal ProductionUpchargePercent { get; set; }
    }

    public class RmgCostingSummaryDto
    {
        // Sub Total
        public decimal SubTotalShhkg { get; set; }
        public decimal SubTotalBdt { get; set; }
        public decimal SubTotalThb { get; set; }
        public decimal SubTotal { get; set; }

        // Sub Total (per Gar. Qty)
        public decimal SubTotalPerGarQtyShhkg { get; set; }
        public decimal SubTotalPerGarQtyBdt { get; set; }
        public decimal SubTotalPerGarQtyThb { get; set; }

        // Damage
        public decimal DamagePercent { get; set; }
        public decimal DamageAmountShhkg { get; set; }
        public decimal DamageAmountBdt { get; set; }
        public decimal DamageAmountThb { get; set; }

        // Interest/Overhead
        public decimal InterestOverheadPercent { get; set; }
        public decimal InterestOverheadAmountShhkg { get; set; }
        public decimal InterestOverheadAmountBdt { get; set; }
        public decimal InterestOverheadAmountThb { get; set; }

        // Total
        public decimal TotalShhkg { get; set; }
        public decimal TotalBdt { get; set; }
        public decimal TotalThb { get; set; }

        // Material Cost
        public decimal TotalMaterialCostOverseas { get; set; }
        public decimal TotalMaterialCostBangladesh { get; set; }
        public decimal TotalMaterialCostBkk { get; set; }

        // User Inputs
        public decimal CmAndProfit { get; set; }
        public decimal HandlingCharge { get; set; }
        public decimal ProductionUpchargePercent { get; set; }
        public decimal ProductionUpcharge { get; set; }

        // Final
        public decimal FfPrice { get; set; }
        public decimal GrandTotal { get; set; }
    }


    //public class RmgCostingSummaryDto
    //{
    //    public decimal SubTotal { get; set; }
    //    public decimal SubTotalPerGarQty { get; set; }
    //    public decimal DamagePercent { get; set; }
    //    public decimal DamageAmount { get; set; }
    //    public decimal InterestOverheadPercent { get; set; }
    //    public decimal InterestOverheadAmount { get; set; }
    //    public decimal Total { get; set; }
    //    public decimal TotalMaterialCostOverseas { get; set; }
    //    public decimal TotalMaterialCostBangladesh { get; set; }
    //    public decimal TotalMaterialCostBkk { get; set; }
    //    public decimal CmAndProfit { get; set; }
    //    public decimal HandlingCharge { get; set; }
    //    public decimal ProductionUpcharge { get; set; }
    //    public decimal FfPrice { get; set; }
    //    public decimal GrandTotal { get; set; }
    //}

    //// ========== INTERFACE ==========
    //public interface IRmgCostingDetailsTempService
    //{
    //    Task<List<RmgCostingDetailsTempDto>> GetAllByCostingIdAsync(string costingId);
    //    Task<RmgCostingDetailsTempDto> GetByIdAsync(decimal id);
    //    Task<RmgCostingDetailsTempDto> AddAsync(RmgCostingDetailsTempDto dto);
    //    Task<RmgCostingDetailsTempDto> UpdateAsync(RmgCostingDetailsTempDto dto);
    //    Task<bool> DeleteAsync(decimal id);
    //    Task<bool> DeleteByCostingIdAsync(string costingId);
    //    Task<RmgCostingSummaryDto> CalculateSummaryAsync(string costingId, decimal damagePercent, decimal interestPercent);
    //}


}

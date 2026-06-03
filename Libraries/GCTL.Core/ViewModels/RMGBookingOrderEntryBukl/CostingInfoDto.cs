namespace GCTL.Core.ViewModels.RMGBookingOrderEntryBukl
{
    public class CostingInfoDto : BaseViewModel
    {
        // --- RMG_CostingInfo (ci.*) ---
        public int AutoID { get; set; }
        public string CostingID { get; set; }
        public DateTime EntryDate { get; set; }
        public string BuyerID { get; set; }
        public string StyleID { get; set; }
        public string MasterPurchaseOrder { get; set; }
        public string PoNo { get; set; }
        public string IntegraJobNO { get; set; }
        public string ExportLCNoSC { get; set; }
        public DateTime? ShipmentDate { get; set; }
        public string FactorySuplier { get; set; }
        public string IssuedBy { get; set; }
        public string CheckedBy { get; set; }
        public decimal SubTotalAmountSHHKG { get; set; }
        public decimal SubTotalAmountBDT { get; set; }
        public decimal SubTotalAmountTHB { get; set; }
        public decimal DamagePercentage { get; set; }
        public decimal DamageAmountSHHKG { get; set; }
        public decimal DamageAmountBDT { get; set; }
        public decimal DamageAmountTHB { get; set; }
        public decimal InterestOverheadPercentage { get; set; }
        public decimal InterestOverheadSHHKG { get; set; }
        public decimal InterestOverheadBDT { get; set; }
        public decimal InterestOverheadTHB { get; set; }
        public decimal TotalAmountSHHKG { get; set; }
        public decimal TotalAmountBDT { get; set; }
        public decimal TotalAmountTHB { get; set; }
        public decimal TotalMaterialCostOverseas { get; set; }
        public decimal TotalMaterialCostBDT { get; set; }
        public decimal TotalMaterialCostBKK { get; set; }
        public decimal CMAndProfit { get; set; }
        public decimal HandlingCharge { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal FFPrice { get; set; }
        //public string LUser { get; set; }
        //public DateTime? LDate { get; set; }
        //public string LIP { get; set; }
        //public string LMAC { get; set; }
        //public DateTime? ModifyDate { get; set; }
        public string EmployeID { get; set; }
        public string CompanyCode { get; set; }
        public decimal? MaterialCostPerUnit { get; set; }
        public decimal? CMProfitUPerUnit { get; set; }
        public decimal? HandlingChargePerUnit { get; set; }
        public decimal? SubTotalByPerPcsSHHKG { get; set; }
        public decimal? SubTotalByPerPcsBDT { get; set; }
        public decimal? SubTotalByPerPcsTHB { get; set; }
        public decimal? ProductionUpCharge { get; set; }

        // --- RMG_CostingDetails (cd.*) ---
        public int Id { get; set; }
        public string CostingDetailsID { get; set; }
        public string DetailCostingID { get; set; }
        public int SLNO { get; set; }
        public string BookingItemTypeID { get; set; }
        public string ItemID { get; set; }
        public string Description { get; set; }
        public string Width { get; set; }
        public string ColorID { get; set; }
        public string SupplierID { get; set; }
        public string DetailPoNo { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? Consumption { get; set; }
        public decimal? Extra { get; set; }
        public decimal? TotalQuantity { get; set; }
        public string TotalQuantityUnit { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public string TotalPriceCurrencyId { get; set; }
        public decimal? DetailTotalAmountSHHKG { get; set; }
        public decimal? DetailTotalAmountBDT { get; set; }
        public decimal? DetailTotalAmountTHB { get; set; }
        public string ResponsibleBy { get; set; }
        public string DetailLUser { get; set; }
        public string BookinOrderNO { get; set; }
    }
}

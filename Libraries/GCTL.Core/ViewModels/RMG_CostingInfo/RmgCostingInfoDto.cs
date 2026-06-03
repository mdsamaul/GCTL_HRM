namespace GCTL.Core.ViewModels.RMG_CostingInfo
{
    public class RmgCostingInfoDto : BaseViewModel
    {
        public int AutoId { get; set; }
        public string CostingId { get; set; } = "";
        public DateTime EntryDate { get; set; }
        public string BuyerId { get; set; } = "";
        public string StyleId { get; set; } = "";
        public string MasterPurchaseOrder { get; set; } = "";
        public string PoNo { get; set; } = "";
        public string IntegraJobNo { get; set; } = "";
        public string ExportLcnoSc { get; set; } = "";
        public DateTime? ShipmentDate { get; set; }
        public string FactorySuplier { get; set; } = "";
        public string IssuedBy { get; set; } = "";
        public string CheckedBy { get; set; } = "";

        public decimal SubTotalAmountShhkg { get; set; }
        public decimal SubTotalAmountBdt { get; set; }
        public decimal SubTotalAmountThb { get; set; }

        public decimal DamagePercentage { get; set; }
        public decimal DamageAmountShhkg { get; set; }
        public decimal DamageAmountBdt { get; set; }
        public decimal DamageAmountThb { get; set; }

        public decimal InterestOverheadPercentage { get; set; }
        public decimal InterestOverheadShhkg { get; set; }
        public decimal InterestOverheadBdt { get; set; }
        public decimal InterestOverheadThb { get; set; }

        public decimal TotalAmountShhkg { get; set; }
        public decimal TotalAmountBdt { get; set; }
        public decimal TotalAmountThb { get; set; }

        public decimal TotalMaterialCostOverseas { get; set; }
        public decimal TotalMaterialCostBdt { get; set; }
        public decimal TotalMaterialCostBkk { get; set; }

        public decimal CmandProfit { get; set; }
        public decimal HandlingCharge { get; set; }
        public decimal ProductionUpCharge { get; set; }
        public decimal GrandTotal { get; set; }
        public decimal Ffprice { get; set; }

        public decimal? SubTotalByPerPcsShhkg { get; set; }
        public decimal? SubTotalByPerPcsBdt { get; set; }
        public decimal? SubTotalByPerPcsThb { get; set; }
        public decimal? CmprofitUperUnit { get; set; }
        public decimal? HandlingChargePerUnit { get; set; }
        public string ShowCreateDate { get; set; } = "";
        public string ShowModifyDate { get; set; } = "";
    }
}
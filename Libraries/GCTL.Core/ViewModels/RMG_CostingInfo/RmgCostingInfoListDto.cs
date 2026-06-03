namespace GCTL.Core.ViewModels.RMG_CostingInfo
{
    public class RmgCostingInfoListDto
    {
        public int AutoId { get; set; }
        public string CostingId { get; set; } = "";
        public DateTime EntryDate { get; set; }
        public string IntegraJobNo { get; set; } = "";
        public string StyleId { get; set; } = "";
        public string StyleName { get; set; } = "";
        public string MasterPurchaseOrder { get; set; } = "";
        public string PoNo { get; set; } = "";
        public string ExportLcnoSc { get; set; } = "";
        public string IssuedBy { get; set; } = "";
        public string IssuedName { get; set; } = "";
        public string CheckedBy { get; set; } = "";
        public string CheckedName { get; set; } = "";
        public string CreateDate { get; set; } = "";
        public string ModifyDate { get; set; } = "";
    }

}

using GCTL.Core.ViewModels;

namespace GCTL.UI.Core.Views.RMGProdOrderInformationEntry
{
    public class RMG_Prod_Temp_ColorSizeBreakupDto: BaseViewModel
    {
        public decimal? TC { get; set; }
        public string? BreakNo { get; set; }
        public string? DetailOrderId { get; set; }
        public string? ColorId { get; set; }
        public string? SizeId { get; set; }
        public int? Quantity { get; set; }
        public string? UnitTypeId { get; set; }
        public string? Remarks { get; set; }
        //public string? LUser { get; set; }
        //public DateTime? LDate { get; set; }
        //public string? LIP { get; set; }
        //public string? LMAC { get; set; }
        //public DateTime? ModifyDate { get; set; }
        //public string? CompanyCode { get; set; }
        public string? IntegraJOBNo { get; set; }
        public string? PONo { get; set; }
        public List<string>? ColorIds { get; set; }    
        public List<string>? SizeIds { get; set; }    
    }
}

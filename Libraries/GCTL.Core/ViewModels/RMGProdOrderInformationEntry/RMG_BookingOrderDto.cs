namespace GCTL.Core.ViewModels.RMGProdOrderInformationEntry
{
    public class RMG_BookingOrderDto : BaseViewModel
    {
        public decimal TC { get; set; }
        public string? BookinOrderNO { get; set; }
        public DateTime BookinDate { get; set; }
        public string? BuyerID { get; set; }
        public string? StyleID { get; set; }
        public string? MasterPurchaseOrder { get; set; }
        public string? PoNo { get; set; }
        public string? IntegraJobNO { get; set; }
        public string? PurchasedOfficer { get; set; }
        public string? Remarks { get; set; }
        public string? LUser { get; set; }
        public DateTime? LDate { get; set; }
        public string? LIP { get; set; }
        public string? LMAC { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string EmployeID { get; set; }
        public string CompanyID { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? DeliveryMethod { get; set; }
        public string? PaymentTerms { get; set; }
        public string? TermsCondition { get; set; }
        public string? BookingType { get; set; }
        public string? BookingEntryType { get; set; }
        public string? WarehouseID { get; set; }
        public string? PINo { get; set; }
        public DateTime? PIDate { get; set; }
        public decimal? PIValue { get; set; }
        public string? PICurrencyId { get; set; }
    }
}

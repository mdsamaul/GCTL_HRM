namespace GCTL.Core.ViewModels.RMGProdOrderInformationEntry
{
    public class RMG_Prod_OrderDetailsDto : BaseViewModel
    {
        public decimal? TC { get; set; }
        public string? DetailOrderId { get; set; }
        public string? OrderId { get; set; }
        public DateTime? Date { get; set; }
        public string? ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public string? BrandId { get; set; }
        public string? Style { get; set; }
        public string? RefNo { get; set; }
        public string? HSCode { get; set; }
        public string? PurchaseOrder { get; set; }
        public DateTime? PODate { get; set; }
        public int? OrderQuantity { get; set; }
        public string? POUnitTypID { get; set; }
        public string? POUnitTyp { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? CurrencyId { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? MaterialInfo { get; set; }
        public string? PrintingInstruction { get; set; }
        public string? WashingInstruction { get; set; }
        public string? LabelInstruction { get; set; }
        public string? PackagingInstruction { get; set; }
        public string? OtherInstruction { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? DeliveryTerm { get; set; }
        public string? DeliveryMethod { get; set; }
        public string? PortOfLoading { get; set; }
        public string? PortOfLoadingName { get; set; }
        public string? PortOfDischarge { get; set; }
        public string? PortOfDischargeName { get; set; }
        //public string? LUser { get; set; }
        //public DateTime? LDate { get; set; }
        //public string? LIP { get; set; }
        //public string? LMAC { get; set; }
        //public DateTime? ModifyDate { get; set; }
        //public string? CompanyCode { get; set; }
        public string? SupplierId { get; set; }
        public string? PaymentTermsId { get; set; }
        public string? GarmentsTesting { get; set; }
        public string? GarmentsInstruction { get; set; }
        public string? GarmentReminderDay { get; set; }
        public string? GarmentReminderType { get; set; }
        public string? GarmnetRemainderMail { get; set; }
        public string? IsGarmentTestRecieved { get; set; }
        public string? GarmentTestAttachment { get; set; }
        public string? FebricTesting { get; set; }
        public string? FebricInstruction { get; set; }
        public string? FebricReminderDay { get; set; }
        public string? FebricReminderType { get; set; }
        public string? FebricRemainderMail { get; set; }
        public string? IsFebricTestRecieved { get; set; }
        public string? FebricTestAttachment { get; set; }
        public string? TransportNo { get; set; }
        public string? IntegraJobNO { get; set; }
        public string? MasterPurchaseOrder { get; set; }
        public decimal? Percentage1 { get; set; }
        public string? DeliveryMethod2 { get; set; }
        public decimal? Percentage2 { get; set; }
        public string? DeliveryMethod3 { get; set; }
        public string? ShowCreateDate { get; set; }
        public string? ShowModifyDate { get; set; }
        public decimal? Percentage3 { get; set; }
        public DateTime? XFactoryDate { get; set; }
    }

}

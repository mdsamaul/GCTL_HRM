using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.RMGBookingOrderEntryBukl
{
    public class RMGBookingOrderEntryBuklDto : BaseViewModel
    {
        [Display(Name = "TC")]
        public decimal Tc { get; set; }

        [Display(Name = "Booking Order No")]
        public string BookinOrderNo { get; set; }

        [Display(Name = "Booking Date")]
        public DateTime? BookinDate { get; set; }   // ✅ nullable

        [Display(Name = "Buyer")]
        public string BuyerId { get; set; }

        [Display(Name = "Style")]
        public string StyleId { get; set; }

        [Display(Name = "Master PO")]
        public string MasterPurchaseOrder { get; set; }

        [Display(Name = "PO No")]
        public string PoNo { get; set; }

        [Display(Name = "Integra Job No")]
        public string IntegraJobNo { get; set; }

        [Display(Name = "Purchased Officer")]
        public string PurchasedOfficer { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "Employee")]
        public string EmployeId { get; set; }

        [Display(Name = "Company")]
        public string CompanyId { get; set; }

        [Display(Name = "Delivery Date")]
        public DateTime? DeliveryDate { get; set; }

        [Display(Name = "Delivery Address")]
        public string DeliveryAddress { get; set; }

        [Display(Name = "Delivery Method")]
        public string DeliveryMethod { get; set; }

        [Display(Name = "Payment Terms")]
        public string PaymentTerms { get; set; }

        [Display(Name = "Terms & Conditions")]
        public string TermsCondition { get; set; }

        [Display(Name = "Booking Type")]
        public string BookingType { get; set; }

        [Display(Name = "Booking Entry Type")]
        public string BookingEntryType { get; set; }

        [Display(Name = "Warehouse")]
        public string WarehouseId { get; set; }

        [Display(Name = "PI No")]
        public string Pino { get; set; }

        [Display(Name = "PI Date")]
        public DateTime? Pidate { get; set; }

        [Display(Name = "PI Value")]
        public decimal? Pivalue { get; set; }

        [Display(Name = "PI Currency")]
        public string PicurrencyId { get; set; }

        [Display(Name = "Supplier")]
        public string SupplierId { get; set; }

        [Display(Name = "MRBP ID")]
        public string Mrbpid { get; set; }

        [Display(Name = "Entered From Page")]
        public string EnterFromPageName { get; set; }

        [Display(Name = "PI File Path")]
        public string PifilePath { get; set; }

        [Display(Name = "Selected Costing IDs")]
        public List<string> SelectedCostingIds { get; set; } = new();
    }
}

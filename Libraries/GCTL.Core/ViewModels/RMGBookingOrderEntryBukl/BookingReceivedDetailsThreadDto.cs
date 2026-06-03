using System.ComponentModel.DataAnnotations;

namespace GCTL.Core.ViewModels.RMGBookingOrderEntryBukl
{
    public class BookingReceivedDetailsThreadDto : BaseViewModel
    {

        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "Purchase Receive No")]
        public string PurchaseReceiveNo { get; set; }

        [Display(Name = "BRD ID")]
        public string BRDID { get; set; }

        [Display(Name = "SL No")]
        public int? SLNO { get; set; }

        [Display(Name = "Color")]
        public string ColorId { get; set; }

        [Display(Name = "Item")]
        public string ItemID { get; set; }

        [Display(Name = "Fabric Detail")]
        public string FebricDetail { get; set; }

        [Display(Name = "Thread Color")]
        public string ThreadColorId { get; set; }

        [Display(Name = "Order Qty")]
        public decimal? OrderQty { get; set; }

        [Display(Name = "Qty Unit")]
        public string QtyUnitID { get; set; }

        [Display(Name = "Thread Count")]
        public string ThreadCountID { get; set; }

        [Display(Name = "Ref. Code Pantone")]
        public string REFCODEPANTONE { get; set; }

        [Display(Name = "Consumption")]
        public decimal? Consumption { get; set; }

        [Display(Name = "Consumption Unit")]
        public string ConsumtionUnitID { get; set; }

        [Display(Name = "Total Qty")]
        public decimal? TotalQty { get; set; }

        [Display(Name = "Total Qty Unit")]
        public string TotalQtyUnitID { get; set; }

        [Display(Name = "Required Qty")]
        public decimal? ReqQty { get; set; }

        [Display(Name = "Thread Req Unit")]
        public string ThreadReqUnit { get; set; }

        [Display(Name = "Thread %")]
        public string Threadpercent { get; set; }

        [Display(Name = "Total Received Qty")]
        public decimal? TotalReceivedQty { get; set; }

        [Display(Name = "Current Receive Qty")]
        public decimal? CurrentReceiveQty { get; set; }

        [Display(Name = "Received Unit Type")]
        public string ReceivedUnitType { get; set; }

        [Display(Name = "Unit Price")]
        public decimal? UnitPrice { get; set; }

        [Display(Name = "Received Unit Price")]
        public decimal? ReceivedUnitPrice { get; set; }

        [Display(Name = "Total Price")]
        public decimal? TotalPrice { get; set; }

        [Display(Name = "Currency")]
        public string CurrencyID { get; set; }

        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        [Display(Name = "Employee")]
        public string EmployeeID { get; set; }

        [Display(Name = "Total Received Qty (Previous)")]
        public decimal? TotalReceivedQtyPre { get; set; }

        [Display(Name = "Pending Receive Qty")]
        public decimal? PendingReceiveQty { get; set; }

        [Display(Name = "Pending Receive Qty (Previous)")]
        public decimal? PendingReceiveQtyPre { get; set; }

        [Display(Name = "Integra Job No")]
        public string IntegraJobNO { get; set; }

        [Display(Name = "PO No")]
        public string PoNo { get; set; }
    }
}

namespace GCTL.Core.ViewModels.RMGBookingOrderEntryBukl
{
    public class BaseBookingDto
    {
        public string PoNo { get; set; }
        public string Item { get; set; }
        public string Description { get; set; }
        public string ColorName { get; set; }
        public decimal GarmentQty { get; set; }
        public string GarmentQtyUnitName { get; set; }
        public decimal Consumption { get; set; }
        public string ConsumptionUnitName { get; set; }
        public decimal TotalQty { get; set; }
        public string TotalQtyUnitName { get; set; }
        public decimal OrderQty { get; set; }
        public string OrderQtyUnitName { get; set; }
        public decimal Percentage { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string CurrencyName { get; set; }
        public string Remarks { get; set; }
    }

    // ==================== Carton DTO ====================
    public class CartonBookingDto : BaseBookingDto
    {
        public int Id { get; set; }
        public string PurchaseReceiveNo { get; set; }
        public string BRDID { get; set; }
        public int? SLNO { get; set; }
        public string ItemID { get; set; }
        public string ColorID { get; set; }
        public string SizeID { get; set; }
        public string SizeName { get; set; }
        public string Refcode { get; set; }
        public double? CartonLength { get; set; }
        public string LeangthUnitID { get; set; }
        public string LengthUnitName { get; set; }
        public double? CartonWidth { get; set; }
        public string WidthUnitID { get; set; }
        public string WidthUnitName { get; set; }
        public double? CatonHeight { get; set; }
        public string HeightUnitID { get; set; }
        public string HeightUnitName { get; set; }
        public string OrderUnitID { get; set; }
        public string ConsumptionUnitID { get; set; }
        public string RequiredQtyUnitID { get; set; }
        public decimal? CartonPercent { get; set; }
        public decimal? TotalReceivedQty { get; set; }
        public decimal? CurrentReceiveQty { get; set; }
        public string ReceivedUnitType { get; set; }
        public decimal? ReceivedUnitPrice { get; set; }
        public string CurrencyID { get; set; }
        public string EmployeeID { get; set; }
        public decimal? TotalReceivedQtyPre { get; set; }
        public decimal? PendingReceiveQty { get; set; }
        public decimal? PendingReceiveQtyPre { get; set; }
        public string IntegraJobNO { get; set; }
    }

    // ==================== Thread DTO ====================
    public class ThreadBookingDto : BaseBookingDto
    {
        public int Id { get; set; }
        public string PurchaseReceiveNo { get; set; }
        public string BRDID { get; set; }
        public int? SLNO { get; set; }
        public string ColorId { get; set; }
        public string ItemID { get; set; }
        public string FebricDetail { get; set; }
        public string ThreadColorId { get; set; }
        public string QtyUnitID { get; set; }
        public string ThreadCountID { get; set; }
        public string ThreadCountName { get; set; }
        public string REFCODEPANTONE { get; set; }
        public string ConsumtionUnitID { get; set; }
        public string TotalQtyUnitID { get; set; }
        public decimal? ReqQty { get; set; }
        public string ThreadReqUnit { get; set; }
        public string Threadpercent { get; set; }
        public decimal? TotalReceivedQty { get; set; }
        public decimal? CurrentReceiveQty { get; set; }
        public string ReceivedUnitType { get; set; }
        public decimal? ReceivedUnitPrice { get; set; }
        public string CurrencyID { get; set; }
        public string EmployeeID { get; set; }
        public decimal? TotalReceivedQtyPre { get; set; }
        public decimal? PendingReceiveQty { get; set; }
        public decimal? PendingReceiveQtyPre { get; set; }
        public string IntegraJobNO { get; set; }
    }

    // ==================== Poly DTO ====================
    public class PolyBookingDto : BaseBookingDto
    {
        public int Id { get; set; }
        public string PurchaseReceiveNo { get; set; }
        public string BRDID { get; set; }
        public int? SerialNo { get; set; }
        public string ItemID { get; set; }
        public string ItemDescription { get; set; }
        public string ColorID { get; set; }
        public string RefernceCode { get; set; }
        public double? Length { get; set; }
        public string LengthUnitID { get; set; }
        public string LengthUnitName { get; set; }
        public double? Width { get; set; }
        public string WidthUnitID { get; set; }
        public string WidthUnitName { get; set; }
        public double? Flap { get; set; }
        public string FlapUnitID { get; set; }
        public string FlapUnitName { get; set; }
        public double? Guest { get; set; }
        public string GuestUnitID { get; set; }
        public string GuestUnitName { get; set; }
        public string GarmentQtyUnitID { get; set; }
        public string ConsumptionUnitID { get; set; }
        public string TotalQtyUnitID { get; set; }
        public decimal? TotalReceivedQty { get; set; }
        public decimal? CurrentReceiveQty { get; set; }
        public string ReceivedUnitType { get; set; }
        public decimal? ReceivedUnitPrice { get; set; }
        public string CurrencyID { get; set; }
        public string EmployeeID { get; set; }
        public decimal? TotalReceivedQtyPre { get; set; }
        public decimal? PendingReceiveQty { get; set; }
        public decimal? PendingReceiveQtyPre { get; set; }
        public string IntegraJobNO { get; set; }
    }

    // ==================== Button DTO ====================
    public class ButtonBookingDto : BaseBookingDto
    {
        public int Id { get; set; }
        public string PurchaseReceiveNo { get; set; }
        public string BRDID { get; set; }
        public int? SerialNo { get; set; }
        public string FabricColorID { get; set; }
        public string ItemID { get; set; }
        public string ColorID { get; set; }
        public string SizeID { get; set; }
        public string IDNo { get; set; }
        public string GermentsQtyUnitID { get; set; }
        public string ConsumptionUnitID { get; set; }
        public string TotalQtyUnitID { get; set; }
        public string OrderQtyUnitID { get; set; }
        public decimal? TotalReceivedQty { get; set; }
        public decimal? CurrentReceiveQty { get; set; }
        public string ReceivedUnitType { get; set; }
        public decimal? ReceivedUnitPrice { get; set; }
        public string CurrencyID { get; set; }
        public string EmployeeID { get; set; }
        public decimal? TotalReceivedQtyPre { get; set; }
        public decimal? PendingReceiveQty { get; set; }
        public decimal? PendingReceiveQtyPre { get; set; }
        public string IntegraJobNO { get; set; }
    }

    // ==================== Extra DTO ====================
    public class ExtraBookingDto : BaseBookingDto
    {
        public int Id { get; set; }
        public string PurchaseReceiveNo { get; set; }
        public string BRDID { get; set; }
        public int? SLNO { get; set; }
        public string FabricColorID { get; set; }
        public string ItemID { get; set; }
        public string ColorID { get; set; }
        public string OrderQtyIUnitD { get; set; }
        public string ConsumptionUnitID { get; set; }
        public string TotalQtyUnitID { get; set; }
        public decimal? ReqQty { get; set; }
        public string ReqQtyUnitID { get; set; }
        public decimal? TotalReceivedQty { get; set; }
        public decimal? CurrentReceiveQty { get; set; }
        public string ReceivedUnitType { get; set; }
        public decimal? ReceivedUnitPrice { get; set; }
        public string CurrencyID { get; set; }
        public string EmployeeID { get; set; }
        public decimal? TotalReceivedQtyPre { get; set; }
        public decimal? PendingReceiveQty { get; set; }
        public decimal? PendingReceiveQtyPre { get; set; }
        public string IntegraJobNO { get; set; }
    }

    // ==================== Febric DTO ====================
    public class FebricBookingDto : BaseBookingDto
    {
        public int Id { get; set; }
        public string PurchaseReceiveNo { get; set; }
        public string BRDID { get; set; }
        public string ColorId { get; set; }
        public string FabricItemId { get; set; }
        public string ItemID { get; set; }
        public string FebricDetails { get; set; }
        public string Refcode { get; set; }
        public decimal? OrderQty { get; set; }
        public string QtyUnit { get; set; }
        public decimal? Consumption { get; set; }
        public string ConsumtionUnit { get; set; }
        public decimal? TotalFebricQty { get; set; }
        public decimal? Percentage { get; set; }
        public decimal? TotalReceivedQty { get; set; }
        public decimal? CurrentReceiveQty { get; set; }
        public string ReceivedUnitType { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? ReceivedUnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public string CurrencyID { get; set; }
        public string EmployeeID { get; set; }
        public int? SLNO { get; set; }
        public decimal? TotalReceivedQtyPre { get; set; }
        public decimal? PendingReceiveQty { get; set; }
        public decimal? PendingReceiveQtyPre { get; set; }
        public string IntegraJobNO { get; set; }
        public string PoNo { get; set; }
    }

    // ==================== View Model ====================
    public class BookingOrderEntryBuklViewModel
    {
        public string Code { get; set; }
        public string Breadcrumb { get; set; }
        public BookingOrderEntryBuklSetup BookingOrderEntryBuklSetup { get; set; } = new BookingOrderEntryBuklSetup();
    }

    public class BookingOrderEntryBuklSetup
    {
        public string BookinOrderNo { get; set; }
        public string BookingType { get; set; }
    }


    public class ItemTypeViewModel
    {
        public string BookingItemTypeID { get; set; }
        public string BookingItemType { get; set; }
    }
    public class ItemTypeFilterDto
    {
        public string BookingType { get; set; }
        public List<string> CostingId { get; set; }
    }
    public class EmployeeDto
    {
        public string EmployeeID { get; set; }
        public string FullName { get; set; }
    }

}

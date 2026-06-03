namespace GCTL.Core.ViewModels.RMGBookingOrderEntryBukl
{
#nullable enable
    public class BookingItemUpdateDto
    {
        // Primary Key (Mandatory for update) - Collected as "Id" from JS
        public int Id { get; set; }
        public string BookingType { get; set; } = null!; // Collected as "BookingType" from JS

        // Core Fields
        public string? ItemId { get; set; }
        public string? Description { get; set; }
        public string? ColorId { get; set; }
        public string? PoNo { get; set; }
        public string? IntegraJobNo { get; set; }

        // Dimensions (Carton: 04)
        public decimal? CartonLength { get; set; }
        public decimal? CartonWidth { get; set; }
        public decimal? CatonHeight { get; set; }
        public string? LeangthUnitID { get; set; }
        public string? WidthUnitID { get; set; }
        public string? HeightUnitID { get; set; }
        public string? SizeId { get; set; }

        // Dimensions (Thread: 07)
        public string? ThreadCountID { get; set; }

        // Dimensions (Poly: 03)
        public decimal? Length { get; set; }
        public string? LengthUnitID { get; set; }
        public decimal? Width { get; set; }
        public decimal? Flap { get; set; }
        public string? FlapUnitID { get; set; }
        public decimal? Guest { get; set; }
        public string? GuestUnitID { get; set; }

        // Quantity & Consumption
        public decimal? GarmentQty { get; set; }
        public string? GarmentQtyUnitID { get; set; }
        public decimal? Consumption { get; set; }
        public string? ConsumptionUnitID { get; set; }
        public decimal? TotalQty { get; set; }
        public string? TotalQtyUnitID { get; set; }
        public decimal? OrderQty { get; set; }
        public string? OrderQtyUnitID { get; set; }

        // Price
        public decimal? Percentage { get; set; }
        public decimal? UnitPrice { get; set; } // Maps to existingItem.ReceivedUnitPrice
        public decimal? TotalPrice { get; set; }
        public string? CurrencyId { get; set; }

        // Other
        public string? Remarks { get; set; }
    }
}

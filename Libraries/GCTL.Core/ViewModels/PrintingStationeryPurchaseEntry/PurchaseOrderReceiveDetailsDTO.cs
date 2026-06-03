using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.PrintingStationeryPurchaseEntry
{
    public class PurchaseOrderReceiveDetailsDTO:BaseViewModel
    {
        public decimal TC { get; set; }
        public string? PurchaseOrderReceiveDetailsID { get; set; }
        public string PurchaseReceiveNo { get; set; } = string.Empty;
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public string? BrandID { get; set; }
        public string? BrandName { get; set; }
        public string? ModelID { get; set; }
        public string? ModelName { get; set; }
        public string? SizeID { get; set; }
        public string? SizeName { get; set; }
        public string? WarrantyPeriod { get; set; }
        public string? WarrantyPeriodName { get; set; }
        public string? WarrentyTypeID { get; set; }
        public string? WarrentyTypeName { get; set; }
        public decimal? ReqQty { get; set; }
        public string? UnitTypID { get; set; }
        public string? UnitTypName { get; set; }
        public decimal? UnitPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public int? SLNO { get; set; }
    }

}

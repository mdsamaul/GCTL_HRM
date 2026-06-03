using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.ProductStockHistoryReport
{
    public class ProductStockHistoryReportSetupViewModel : BaseViewModel
    {
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public string? Description { get; set; }
        public string? BrandID { get; set; }
        public string? BrandName { get; set; }
        public string? CatagoryID { get; set; }
        public string? CatagoryName { get; set; }
        public string? ModelID { get; set; }
        public string? ModelName { get; set; }
        public string? SizeID { get; set; }
        public string? SizeName { get; set; }
        public string? UnitId { get; set; }
        public string? UnitName { get; set; }
        public decimal? UnitPrice { get; set; }
        public string? OpeningQty { get; set; }
        public string? ReceivedQty { get; set; }
        public string? IssuedQty { get; set; }
        public string? StockQty { get; set; }
        public string? BalanceQty { get; set; }
        public decimal? StockValue { get; set; }
    }
}

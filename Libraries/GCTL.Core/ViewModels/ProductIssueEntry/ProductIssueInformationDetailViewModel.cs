using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.ProductIssueEntry
{
    public class ProductIssueInformationDetailViewModel:BaseViewModel
    {
        public decimal TC { get; set; }
        public string? PIDID { get; set; } 
        public string? IssueNo { get; set; } 
        public string? ProductCode { get; set; }
        public string? ProductName { get; set; }
        public string? BrandID { get; set; }
        public string? BrandName { get; set; }
        public string? ModelID { get; set; }
        public string? ModelName { get; set; }
        public string? SizeID { get; set; }
        public string? SizeName { get; set; }
        public string? UnitTypID { get; set; }
        public string? UnitTypName { get; set; }
        public decimal? StockQty { get; set; }
        public decimal? IssueQty { get; set; }       
        public string? FloorCode { get; set; }
        public string? FloorName { get; set; }
        public string? Description { get; set; }
    }
}

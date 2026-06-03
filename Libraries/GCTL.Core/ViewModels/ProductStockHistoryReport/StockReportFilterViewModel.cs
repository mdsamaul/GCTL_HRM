using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.ProductStockHistoryReport
{
    public class StockReportFilterViewModel : BaseViewModel
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<string>? ProductCodes { get; set; }
        public List<string>? CategoryIds { get; set; }
        public List<string>? BrandIds { get; set; }
        public List<string>? ModelIds { get; set; }
    }
}

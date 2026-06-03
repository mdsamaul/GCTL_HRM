using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.PrintingStationeryPurchaseReport
{
    public class PrintingStationeryPurchaseReportResultDto
    {
        public decimal PurchaseOrderReceiveDetailsID { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }

        public string CatagoryID { get; set; }
        public string CatagoryName { get; set; }

        public string BrandID { get; set; }
        public string BrandName { get; set; }

        public string ModelID { get; set; }
        public string ModelName { get; set; }

        public string SizeID { get; set; }
        public string SizeName { get; set; }

        public string WarrantyPeriod { get; set; }
        public string PeriodName { get; set; }

        public DateTime ReceiveDate { get; set; }
        public string ReqQty { get; set; }
        public string UnitTypeName { get; set; }

        public decimal PurchaseCost { get; set; }
        public decimal TotalPrice { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.ProductIssueReport
{
    public class ProductIssueReportSetupViewModel : BaseViewModel
    {      
        public string IssueNo { get; set; }
        public decimal IssueQty { get; set; }
        public string CatagoryID { get; set; }
        public string CatagoryName { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string BrandID { get; set; }
        public string BrandName { get; set; }
        public string ModelID { get; set; }
        public string ModelName { get; set; }
        public string SizeID { get; set; }
        public string SizeName { get; set; }
        public string UnitTypID { get; set; }
        public string UnitTypeName { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentName { get; set; }
        public string EmployeeID { get; set; }
        public string FullName { get; set; }
        public string FloorCode { get; set; }
        public string FloorName { get; set; }
    }
}

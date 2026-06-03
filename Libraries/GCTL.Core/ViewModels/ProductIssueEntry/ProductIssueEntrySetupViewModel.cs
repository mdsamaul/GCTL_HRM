using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.ProductIssueEntry
{
    public class ProductIssueEntrySetupViewModel : BaseViewModel
    {
        public decimal TC { get; set; }
        public string? MainCompanyCode { get; set; }
        public string IssueNo { get; set; }
        public DateTime? IssueDate { get; set; }
        public string? ShowIssueDate { get; set; }
        public string? DepartmentCode { get; set; }
        public string? EmployeeID { get; set; }
        public string? IssuedBy { get; set; }
        public string? Remarks { get; set; }  
        public string? CompanyCode { get; set; }
        public string? FloorCode { get; set; }
        public string? ShowCreateDate { get; set; }
        public string? ShowModifyDate { get; set; }

        // Details list: INV_ProductIssueInformationDetails
        public List<ProductIssueInformationDetailViewModel>? Details { get; set; } = new();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.InvDefSupplierType
{
    public class InvDefSupplierTypeSetupViewModel:BaseViewModel
    {
        public decimal AutoId { get; set; } 
        public string SupplierTypeId { get; set; } = string.Empty;
        public string SupplierType { get; set; } = string.Empty;
        public string? ShortName { get; set; }
        public string ShowCreateDate { get; set; }
        public string ShowModifyDate { get; set; }
        public string CompanyCode { get; set; }
    }
}

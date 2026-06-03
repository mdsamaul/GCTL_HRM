using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.SupplierType
{
    public class SupplierTypeSetupViewModel : BaseViewModel
    {
        public int SupplierTypeCode { get; set; }
        public string SupplierTypeId { get; set; }
        [Required(ErrorMessage = "Supplier TypeName is required")]
        public string SupplierTypeName { get; set; }
    }
}

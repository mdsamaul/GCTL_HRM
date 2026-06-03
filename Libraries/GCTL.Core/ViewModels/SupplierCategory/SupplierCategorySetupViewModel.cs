using GCTL.Core.ViewModels.SupplierType;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.SupplierCategory
{
    public class SupplierCategorySetupViewModel : BaseViewModel
    {
        public int SupplierCategoryCode { get; set; }
        public string SupplierCategoryId { get; set; }
        [Required(ErrorMessage = "Supplier Category is required")]
        public string SupplierCategory { get; set; }
    }
}

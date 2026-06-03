using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.SupplierOrigin
{
    public class SupplierOriginSetupViewModel : BaseViewModel
    {
        public int Tc { get; set; }
        public string SupplierOriginId { get; set; }
        [Required(ErrorMessage = "Supplier Origin is required")]
        public string SupplierOrigin { get; set; }
    }
}

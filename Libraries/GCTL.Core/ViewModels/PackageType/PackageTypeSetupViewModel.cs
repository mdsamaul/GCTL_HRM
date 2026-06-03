using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.PackageType
{
    public class PackageTypeSetupViewModel : BaseViewModel
    {
        public int Tc { get; set; }
        public string PackageTypeId { get; set; }
        [Required(ErrorMessage = "Package Type is required")]
        public string PackageType { get; set; }
    }
}

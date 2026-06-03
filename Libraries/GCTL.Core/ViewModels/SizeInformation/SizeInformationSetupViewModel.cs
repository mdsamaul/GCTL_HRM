using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.SizeInformation
{
    public class SizeInformationSetupViewModel : BaseViewModel
    {
        public decimal Tc { get; set; }
        public string SizeId { get; set; }
        [Required(ErrorMessage = "Size is required")]
        public string Size { get; set; }
        public string Detail { get; set; }
        public int? Slno { get; set; }
    }
}

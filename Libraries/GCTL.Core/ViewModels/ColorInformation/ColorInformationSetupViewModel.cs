using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.ColorInformation
{
    public class ColorInformationSetupViewModel : BaseViewModel
    {
        public decimal Tc { get; set; }
        public string ColorId { get; set; }
        [Required(ErrorMessage = "Color is required")]
        public string Color { get; set; }
        public string Detail { get; set; }
    }
}

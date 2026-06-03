using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.StyleInformation
{
    public class StyleInformationSetupViewModel : BaseViewModel
    {
        public decimal Tc { get; set; }
        public string StyleId { get; set; }
        [Required(ErrorMessage = "Style is required")]
        public string Style { get; set; }
        public string ShortName { get; set; }
        [Required(ErrorMessage = "BuyerId is required")]
        public string BuyerId { get; set; }
        public string Name { get; set; }
    }
}

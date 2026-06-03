using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.SeasonInformation
{
    public class SeasonInformationSetupViewModel : BaseViewModel
    {
        public decimal Tc { get; set; }
        public string SeasonId { get; set; }
        [Required(ErrorMessage = "Season is required")]
        public string Season { get; set; }
        public string Detail { get; set; }
    }
}

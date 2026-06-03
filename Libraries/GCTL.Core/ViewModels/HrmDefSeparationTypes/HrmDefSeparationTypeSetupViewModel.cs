using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HrmDefSeparationTypes
{
    public class HrmDefSeparationTypeSetupViewModel : BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string SeparationTypeId { get; set; }
        [Required(ErrorMessage = "Separation Type is required")]
        public string SeparationType { get; set; }
    }
}

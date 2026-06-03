using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.SeparationTypes
{
    public class SeparationTypesSetupViewModel : BaseViewModel
    {
        public decimal SeparationTypeCode { get; set; }
        public string SeparationTypeId { get; set; }
        [Required]
        public string SeparationType { get; set; }
    }
}

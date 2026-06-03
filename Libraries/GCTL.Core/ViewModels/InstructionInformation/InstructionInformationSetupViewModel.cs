using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.InstructionInformation
{
    public class InstructionInformationSetupViewModel : BaseViewModel
    {
        public decimal Tc { get; set; }
        public string InstructionId { get; set; }
        [Required(ErrorMessage = "Instruction is required")]
        public string Instruction { get; set; }
        public string CompanyCode { get; set; }
    }
}

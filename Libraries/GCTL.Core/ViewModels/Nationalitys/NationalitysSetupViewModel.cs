using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.Nationalitys
{
    public class NationalitysSetupViewModel : BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string NationalityCode { get; set; }
        [Required(ErrorMessage = "Nationality is required")]
        public string Nationality { get; set; }
    }
}

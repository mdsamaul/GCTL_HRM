using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.GarmentsTest
{
    public class GarmentsTestSetupViewModel : BaseViewModel
    {
        public int Tc { get; set; }
        public string GarmentsTestD { get; set; }
        [Required(ErrorMessage = "Garments Tes tName is required")]
        public string GarmentsTestName { get; set; }
        public string Details { get; set; }
        public string EmployeeId { get; set; }
        public string CompanyId { get; set; }
    }
}

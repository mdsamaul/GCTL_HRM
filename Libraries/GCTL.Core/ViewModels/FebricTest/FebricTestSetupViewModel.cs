using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.FebricTest
{
    public class FebricTestSetupViewModel : BaseViewModel
    {
        public int Tc { get; set; }
        public string FebricTestD { get; set; }
        [Required(ErrorMessage = "Febric Test Name required")]
        public string FebricTestName { get; set; }
        public string Details { get; set; }
        public string EmployeeId { get; set; }
        public string CompanyId { get; set; }
    }
}

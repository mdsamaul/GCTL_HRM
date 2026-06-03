using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.ThreadCount
{
    public class ThreadCountSetupViewModel : BaseViewModel
    {
        public int Tc { get; set; }
        public string ThreadCountId { get; set; }
        [Required(ErrorMessage = "Thread Count Name is required")]
        public string ThreadCountName { get; set; }
        public string ShortName { get; set; }
    }
}

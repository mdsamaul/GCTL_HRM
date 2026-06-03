using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HrmDefPerformance
{
    public class HrmDefPerformanceSetupViewModel : BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string PerformanceCode { get; set; }
        public string JobTitleId { get; set; }
        [Required(ErrorMessage = "JobTitle is required")]
        public string JobTitle { get; set; }
        public string Performance { get; set; }
        public string PerformanceShortName { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HrmDefPerformance2
{
    public class HrmDefPerformance2SetupViewModel : BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string PerformanceCode { get; set; }
        public string JobTitleId { get; set; }
        [Required(ErrorMessage = "Performance is required")]
        public string Performance { get; set; }
        public string PerformanceShortName { get; set; }
        public string JobTitle { get; set; }


    }
}

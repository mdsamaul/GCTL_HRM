using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRMDefExamTitles
{
    public class HRMDefExamTitlesSetupViewModel:BaseViewModel
    {
        public decimal AutoId { get; set; }
        public string ExamTitleCode { get; set; }
        public string ExamTitleName { get; set; }
        public string ShortName { get; set; }
    
    }
}

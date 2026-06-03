using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.AdvanceLoanAdjustment
{
    public class DataTableRequest
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SearchValue { get; set; }
        public string Department { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public string sortColumn  { get; set; }
        public string sortDirection { get; set; }
    }
}

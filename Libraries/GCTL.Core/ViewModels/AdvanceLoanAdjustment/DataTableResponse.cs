using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.AdvanceLoanAdjustment
{
    public class DataTableResponse<T>
    {
        public List<T> Data { get; set; }
        public int TotalRecords { get; set; }
        public int FilteredRecords { get; set; }
    }
}

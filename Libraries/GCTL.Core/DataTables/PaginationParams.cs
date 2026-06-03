using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.DataTables
{
    public class PaginationParams
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
    }
}

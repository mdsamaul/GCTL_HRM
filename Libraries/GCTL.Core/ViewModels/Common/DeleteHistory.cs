using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.Common
{
    public class DeleteHistoryDto
    {
        public int AutoId { get; set; }
        public long DHID { get; set; } 
        public string TableName { get; set; }

        public string Field1 { get; set; }
        public string Field2 { get; set; }
        public string Field3 { get; set; }
        public string Field4 { get; set; }
        
        //public string Field100 { get; set; }

        public DateTime DeletedOn { get; set; }
    }
}

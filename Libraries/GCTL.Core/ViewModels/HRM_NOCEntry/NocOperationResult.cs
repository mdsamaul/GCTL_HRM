using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HRM_NOCEntry
{
    public class NocOperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public long AutoId { get; set; }
        public string NocId { get; set; }
        public DateTime? LDate { get; set; }
        public DateTime? ModifyDate { get; set; }
    }
}

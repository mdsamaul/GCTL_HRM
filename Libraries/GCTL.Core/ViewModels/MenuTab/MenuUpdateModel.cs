using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.MenuTab
{
    public class MenuUpdateModel
    {
        public int AutoId { get; set; }
        public string Title { get; set; }
        public string ControllerName { get; set; }
        public string ViewName { get; set; }
        public string TableName { get; set; }
        public string Icon { get; set; }
        public bool IsActive { get; set; }
    }
}

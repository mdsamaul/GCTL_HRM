using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.MenuTab
{
    public class MenuTabViewModel
    {
   
        public string MenuCode { get; set; }
        public string Title { get; set; }
        public string ControllerName { get; set; }
        public int OrderBy { get; set; }
        public string ParentId { get; set; }
        public string ViewName { get; set; }
        public bool IsActive { get; set; }
        public string Icon { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.MenuTab
{
    public class MenuTabPostViewModel
    {
    
        public int AutoId { get; set; }
        public string MenuId { get; set; } // Assumes values like "002"
        public string Title { get; set; }
        public string ControllerName { get; set; }
        public int OrderBy { get; set; } // Assumes an integer for ordering
        public string ViewName { get; set; }
        public bool IsActive { get; set; }
        public string Icon { get; set; }
        public string ParentId { get; set; } // Nullable string for ParentId
        public string ChildId { get; set; } // Nullable string for ChildId
        public string GrandChildId { get; set; } // Nullable string for GrandChildId
    }
}

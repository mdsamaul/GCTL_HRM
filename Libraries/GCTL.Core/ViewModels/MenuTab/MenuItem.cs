using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.MenuTab
{
    public class MenuItem
    {
        public string MenuId { get; set; }
        public string Title { get; set; }
        public string ParentId { get; set; }
        public int? OrderBy { get; set; }
        public string ControllerName { get; set; }
        public string ViewName { get; set; }
        public string Icon { get; set; }
        public bool IsActive { get; set; }
        public string AccessCodeId { get; set; }
        public string AccessCodeName { get; set; }
        public string PageUrl { get; set; }
        public bool CheckAdd { get; set; }
        public bool CheckEdit { get; set; }
        public bool CheckDelete { get; set; }
        public bool CheckPrint { get; set; }
        public List<MenuItem> Children { get; set; }
    }
}

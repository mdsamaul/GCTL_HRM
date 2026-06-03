using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.MenuTab
{
    public class MenuAccessDto
    {
        public string AccessCodeId { get; set; }
        public string AccessCodeName { get; set; }
        public List<MenuItemDto> MenuAccessList { get; set; }
    }
}

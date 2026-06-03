using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.PrintingStationeryPurchaseReport
{
    public class PrintingStationeryDropdownDto
    {
        public List<DropdownItemDto> CategoryIds { get; set; }
        public List<DropdownItemDto> ProductIds { get; set; }
        public List<DropdownItemDto> BrandIds { get; set; }
        public List<DropdownItemDto> ModelIds { get; set; }
    }
    public class DropdownItemDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}

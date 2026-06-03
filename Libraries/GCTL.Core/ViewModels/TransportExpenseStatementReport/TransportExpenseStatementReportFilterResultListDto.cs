using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.TransportExpenseStatementReport
{
    public class TransportExpenseStatementReportFilterResultListDto
    {
        public List<DropdownItemDto> TransportTypeIds { get; set; }
        public List<DropdownItemDto> TransportIds { get; set; }
        public List<DropdownItemDto> DriverEmployeeIds { get; set; }
    }

    public class DropdownItemDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}

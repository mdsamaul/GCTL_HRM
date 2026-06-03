using GCTL.Core.ViewModels.EmployeeOfficialInfoReport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.EmployeeOfficialInfoReport
{
    public interface IEmployeeOfficialInfoReportService
    {

        Task<OfficialInfoDropdownResponse> GetOfficialInfoDropdownAsync(OfficialInfoFilterVm filters);
        Task<EmployeeReportGroupedDto> GetEmployeeOfficialInfoReport(OfficialInfoReportFilterVm ModelData);
        Task<bool> PagePermissionAsync(string accessCode);

    }
}

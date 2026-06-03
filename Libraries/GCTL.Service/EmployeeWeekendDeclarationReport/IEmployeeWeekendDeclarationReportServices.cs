using GCTL.Core.ViewModels.HRM_EmployeeWeekendDeclaration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.EmployeeWeekendDeclarationReport
{
    public interface IEmployeeWeekendDeclarationReportServices
    {
        Task<bool> PagePermissionAsync(string accessCode);
        Task<EmployeeFilterResultDto> GetRosterDataAsync(EmployeeFilterDto FilterData);
        Task<EmployeeFilterResultDto> GetRosterDataPdfAsync(EmployeeFilterDto FilterData);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.ViewModels.RosterScheduleApproval;
using GCTL.Core.ViewModels.RosterScheduleEntry;
using GCTL.Core.ViewModels.RosterScheduleReport;

namespace GCTL.Service.RosterScheduleReport
{
    public interface IRosterScheduleReportServices
    {
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
        Task<RosterReportFilterListDto> GetRosterDataAsync(RosterReportFilterDto filter);
        Task<RosterReportFilterListDto> GetRosterDataPdfAsync(RosterReportFilterDto filter);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.ViewModels.RosterScheduleApproval;
using GCTL.Core.ViewModels.RosterScheduleEntry;
using GCTL.UI.Core.Views.RosterScheduleApproval;

namespace GCTL.Service.RosterScheduleApproval
{
    public interface IRosterScheduleApprovalService
    {
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
        Task<(bool isSuccess, string isMessage)> ApprovalRosterServices(ApprovalRequest modelData);
        Task<List<RosterScheduleEntrySetupViewModel>> GetRosterScheduleGridService();

        Task<RosterFilterListDto> GetFilterDropdownsAsync(RosterFilterDto filter);
        Task<RosterFilterListDto> GetRosterGridDataAsync(RosterFilterDto filter);

    }
}

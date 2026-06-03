using GCTL.Core.ViewModels.ManualEntryApproval;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.ManualEntryApprovalService
{
    public interface IManualEntryApprovalService
    {
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
        Task<(bool isSuccess, string isMessage)> ApprovalManualEntries(ManualApprovalRequest modelData);
        Task<List<ManualEntryApprovalSetupViewModelDto>> GetManualEntryGridService();
        Task<ManualEntryApprovalFilterListDto> GetManualEntryDataAsync(ManualEntryApprovalFilterDto filter);

    }
}

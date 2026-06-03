using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.ProbationPeriodExtension;

namespace GCTL.Service.ProbationPeriodExtension
{
    public interface IProbationPeriodExtensionService
    {
        Task<ProbationExtensionResultViewModel> GetProbationExtensionDataAsync(string employeeId, string companyCode);

        Task<List<ProbationPeriodExtensionGetAll>> GetAllAsync();
        Task<ProbationPeriodExtensionSetupViewModel> GetByIdAsync(string code);
        Task<bool> SaveAsync(ProbationPeriodExtensionSetupViewModel entityVM);
        Task<bool> UpdateAsync(ProbationPeriodExtensionSetupViewModel entityVM);

        IEnumerable<CommonSelectModel> SelectionProbationPeriodExtensionTypeAsync();

        Task<bool> DeleteTab(List<string> ids);
        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string employeeCode, string exc);

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

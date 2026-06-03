using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.InstructionInformation;

namespace GCTL.Service.InstructionInformation
{
    public interface IInstructionInformationService
    {
        Task<List<InstructionInformationSetupViewModel>> GetAllAsync();
        Task<InstructionInformationSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(InstructionInformationSetupViewModel entityVM);
        Task<bool> UpdateAsync(InstructionInformationSetupViewModel vm);

        Task<IEnumerable<CommonSelectModel>> SelectionInstructionAsync();

        Task<bool> DeleteTab(List<string> ids);

        Task<bool> IsExistByCodeAsync(string code);
        Task<bool> IsExistAsync(string name);
        Task<bool> IsExistAsync(string name, string typeCode);

        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
    }
}

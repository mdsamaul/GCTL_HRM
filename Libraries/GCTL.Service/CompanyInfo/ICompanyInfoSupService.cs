using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.CompanyInfo;

namespace GCTL.Service.CompanyInfo
{
    public interface ICompanyInfoSupService
    {
        Task<List<CompanyInfoSetupViewModel>> GetAllAsync();
        Task<CompanyInfoSetupViewModel> GetByIdAsync(string code);

        Task<bool> SaveAsync(CompanyInfoSetupViewModel entityVM);
        Task<bool> UpdateAsync(CompanyInfoSetupViewModel entityVM);

        Task<IEnumerable<CommonSelectModel>> SelectionCompanyInfoAsync();
        Task<IEnumerable<CommonSelectModel>> SelectionBuyerCompanyInfoAsync();

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

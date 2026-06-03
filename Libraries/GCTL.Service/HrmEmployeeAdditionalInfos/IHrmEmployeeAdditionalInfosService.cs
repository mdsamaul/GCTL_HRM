using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HrmEmployeeAdditionalInfos;
using GCTL.Data.Models;

namespace GCTL.Service.HrmEmployeeAdditionalInfos
{
    public interface IHrmEmployeeAdditionalInfosService
    {
        Task<List<HrmEmployeeAdditionalInfoSetupViewModel>> GetAllAsync();
        Task<HrmEmployeeAdditionalInfoSetupViewModel> GetByIdAsync(string id);

        Task<bool> SaveAsync(HrmEmployeeAdditionalInfoSetupViewModel entityVM, string CompanyCode);
        Task<bool> UpdateAsync(HrmEmployeeAdditionalInfoSetupViewModel entityVM);

        HrmEmployeeAdditionalInfo GetLeaveType(string code);
        // bool DeleteLeaveType(string id);
        Task<bool> DeleteTab(List<string> ids, DeleteHistoryViewModel model);
        Task<string> GenerateNextCode();

        Task<HrmEmployeeAdditionalInfoSetupViewModel> GetEmployeeByCode(string employeeId);
        Task<List<HrmEmployeeAdditionalInfoSetupViewModel>> GetComapnyByBranchCode(string companyCode);
        Task<List<HrmEmployeeAdditionalInfoSetupViewModel>> GetComapnyByCode(string companyCode);

        Task<bool> IsExistByCodeAsync(string code, string employeeCode);
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);

    }
}
using GCTL.Core.ViewModels.AccessCodes;
using GCTL.Core.ViewModels.Common;
using GCTL.Data.Models;

namespace GCTL.Service.Users
{
    public interface IAccessCodeService
    {
        List<CoreAccessCode> GetAccessCodes();
        List<CoreAccessCode> GetUniqueAccessCodes();
        List<AccessCodeModel> GetMenus();
        CoreAccessCode GetAccessCode(string code);
        Task<List<AccessCodeModel>> GetAccessCodesAsync(string accessCode);
        bool DeleteAccessCode(string id);
        CoreAccessCode SaveAccessCode(CoreAccessCode entity);
        bool IsAccessCodeExistByCode(string code);
        bool IsAccessCodeExist(string name);
        bool IsAccessCodeExist(string name, string typeCode);
        IEnumerable<CommonSelectModel> AccessCodeSelection();
        List<AccessCodeModel> GetPermissionsAccessCode(string accessCode = "");
        bool HasPermission(string accessCode);
        bool SetPermissions(AccessCodeSetupViewModel model);
        void UpdateAccessCode(CoreMenuTab2 entity);
      
    }
}

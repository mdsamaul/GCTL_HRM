using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.Users;
using GCTL.Data.Models;

namespace GCTL.Service.Users
{
    public interface IUserService
    {
        List<CoreUserInfo> GetUsers();
        List<UserViewModel> GetAllUsers();
        CoreUserInfo GetUser(int id);
        CoreUserInfo GetUser(string employeeId);
        UserViewModel GetUserByEmployee(string employeeId);
        bool DeleteUser(int id);
        CoreUserInfo SaveUser(CoreUserInfo entity);
        bool IsUserExistById(int id);
        bool IsUserExist(int id, string userName);
        bool IsUserExistByName(string username);
        bool IsUserExistByName(string username, string employeeId);
        IEnumerable<CommonSelectModel> PreparerSelection(DefaultRoles role, string lUser);
    }
}

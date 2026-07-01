using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.Users;
using GCTL.Data.Models;

namespace GCTL.Service.Users
{
    public interface IUserService
    {
        Task<IEnumerable<CoreUserInfo>> GetUsers();
        //List<CoreUserInfo> GetUsers();
        Task<List<UserViewModel>> GetAllUsers();
        Task<List<CommonSelectModel>> GetEmployees();
        Task<CoreUserInfo> GetUser(int id);
        Task<int>  GetIdByEmployee(string code);
        Task<int> GetIdByUser(string code);
        Task<CoreUserInfo> GetBaseEmpData(string code);
        Task<CoreUserInfo> GetUser(string employeeId);
        Task<UserViewModel> GetUserByEmployee(string employeeId);
        Task<UserViewModel> GetEmployeeDetails(string employeeId);
        Task<UserViewModel> GetEmployeeDetailsByUser(string username);
        Task<bool> DeleteUser(int id, DeleteHistoryViewModel dm);
        Task<CoreUserInfo> SaveUser(CoreUserInfo entity);
        Task<bool> IsUserExistById(int id);
        Task<bool> IsUserExist(int id, string userName);
        Task<bool> IsUserExistByName(string username);
        Task<bool> IsUserExistByName(string username, string employeeId);
        Task<IEnumerable<CommonSelectModel>> PreparerSelection(DefaultRoles role, string lUser);

        Task<List<CommonSelectModel>> GetCompanies();
        Task<List<CommonSelectModel>> GetBranch();
        Task<List<CommonSelectModel>> GetDepartments();
    }
}

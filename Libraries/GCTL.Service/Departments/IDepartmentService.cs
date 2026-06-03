using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Data.Models;

namespace GCTL.Service.Departments
{
    public interface IDepartmentService
    {
        List<HrmDefDepartment> GetDepartments();
        HrmDefDepartment GetDepartment(string code);
        Task<(bool success, bool refSuccess, string message)> DeleteDepartment(List<string> ids, DeleteHistoryViewModel model);
        HrmDefDepartment SaveDepartment(HrmDefDepartment entity);
        bool IsDepartmentExistByCode(string code);
        bool IsDepartmentExist(string name);
        bool IsDepartmentExist(string name, string typeCode);
        IEnumerable<CommonSelectModel> DepartmentSelection();
        bool SavePermission(string accessCode);
        bool UpdatePermission(string accessCode);
        bool DeletePermission(string accessCode);
    }
}
using GCTL.Core.ViewModels.Employees;
using GCTL.Data.Models;
using System.Collections;

namespace GCTL.Service.Employees
{
    public interface IEmployeeService
    {
        List<HrmEmployee> GetEmployees();
        HrmEmployee GetEmployee(string id);
        EmployeeViewModel GetEmployeeByCode(string employeeId);

        Task<EmployeeViewModel> GetEmployeeDetailsByCode(string code);
        bool DeleteEmployee(string id);    
        HrmEmployee SaveEmployee(HrmEmployee entity);
        bool IsEmployeeExistByCode(string id);
        bool IsEmployeeExist(string name);
        bool IsEmployeeExist(string name, string employeeId);
        IEnumerable GetEmployeeSelections();
        IEnumerable GetEmployeeDropSelections();
    }
}
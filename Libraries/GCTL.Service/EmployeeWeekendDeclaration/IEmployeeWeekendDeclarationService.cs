using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.Companies;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HRM_EmployeeWeekendDeclaration;
using GCTL.Data.Models;

namespace GCTL.Service.EmployeeWeekendDeclaration
{
    public interface IEmployeeWeekendDeclarationService
    {
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
        List<CoreCompany> GetAllCompany();
        Task<EmployeeFilterResultDto> GetFilterDataAsync(EmployeeFilterDto filter);
        Task<(bool isSuccess, string message, object data)> SaveSelectedDatesAndEmployeesAsync(HRM_EmployeeWeekendDeclarationDto model);
        Task<(bool isSuccess, string message, object data)> SaveSelectedDatesAndEmployeesFromExcelAsync(HRM_EmployeeWeekendDeclarationDto model);
        Task<bool> BulkDeleteAsync(List<decimal> ids, DeleteHistoryViewModel dmodel);
        Task<List<EmployeeWeekendDeclarationViewModelShow>> GetWeekendEmpDecService();
        Task<EmployeeWeekendDeclarationViewModelShow?> GetEmployeeWeekendDeclarationByIdAsync(string id);
        Task<(bool IsSuccess, string Message)> UpdateEmployeeWeekendDeclarationAsync(string id, string weekendDate, string remarks);
        Task<byte[]> GenerateEmployeeWeekendDeclarationExcelAsync();
    }
}

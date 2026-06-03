using GCTL.Core.ViewModels.HRLettersReportViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HRLettersReport
{
    public interface IHRLettersReportService
    {
        Task<bool> PagePermissionAsync(string accessCode);
        Task<bool> SavePermissionAsync(string accessCode);
        Task<bool> UpdatePermissionAsync(string accessCode);
        Task<bool> DeletePermissionAsync(string accessCode);
        Task<FullEmployeeDetailsGetByIdViewModel> GetByEmployeeCodeAsync(string employeeCode);
        Task<byte[]> GeneratePdfAsync(HRLetterReportRequestViewModel request);
        Task<string> SaveOrUpdateLetterAsync(SaveOrUpdateLetterRequestDto dto);
        Task<IEnumerable<EmployeeByLetterTypeDto>> GetEmployeesByLetterTypeAsync(string letterTypeId, string companyCode);
    }
}

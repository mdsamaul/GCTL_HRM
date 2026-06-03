using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.EmployeeGeneralInfoReport;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.EmployeeGeneralInfoReport
{
    public interface IEmployeeGeneralInfoReportService
    {
      
        Task<List<CommonSelectModel>> GetComapnyByBranchCode(List<string> companyCode);


        Task<List<dynamic>> EmployeeGeneralInfoReport
        (
            List<string> departmentCode, List<string> designationCodes,
            List<string> employeeCodes, List<string> branchCodes,
        List<string> companyCodes, 
        string nationalityCode,string genderCode,string bloodGroupCode,
        string religionCode, string maritalStatusCode);

       

        #region Filter by Company, Branch, Department, Designation
        Task<List<CommonSelectModelDD>> GetBranchByCompanyId(List<string> companyIds);
        Task<List<CommonSelectModelDD>> GetDepartmentByCompanyId(List<string> companyIds);
        Task<List<CommonSelectModelDD>> GetDesignationByCompanyId(List<string> companyIds);
        Task<List<CommonSelectModelDD>> GetEmployeeByCompanyId(List<string> companyIds);

        Task<List<CommonSelectModelDD>> GetDepartmentByBranchId(List<string> branchIds);
        Task<List<CommonSelectModelDD>> GetDesignationByBranchId(List<string> branchIds);
        Task<List<CommonSelectModelDD>> GetEmployeeByBranchId(List<string> branchIds);

        Task<List<CommonSelectModelDD>> GetDesignationByDepartmentId(List<string> departmentIds);
        Task<List<CommonSelectModelDD>> GetEmployeeByDepartmentId(List<string> departmentIds);

        Task<List<CommonSelectModelDD>> GetEmployeeByDesignationId(List<string> designationIds);
        #endregion

        Task<bool> PagePermissionAsync(string accessCode);
    }
}

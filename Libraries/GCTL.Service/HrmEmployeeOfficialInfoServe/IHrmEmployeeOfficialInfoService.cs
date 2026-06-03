using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GCTL.Core.ViewModels.Employees;
//using GCTL.Core.ViewModels.HrmLeaveApplicationEntry;
using GCTL.Data.Models;

namespace GCTL.Service.HrmEmployeeOfficialInfoServe
{
    public interface IHrmEmployeeOfficialInfoService
    {
        List<HrmEmployeeOfficialInfo> GetEmployeesByCompCode(string compCode);
        //List<EmpInfoViewModel> GetEmployeesByCompDept(string compCode, string? deptCode, string? branchCode);
        //Task<EmpInfoViewModel> GetEmployeesByEmpId(string EmpId);


    }
}

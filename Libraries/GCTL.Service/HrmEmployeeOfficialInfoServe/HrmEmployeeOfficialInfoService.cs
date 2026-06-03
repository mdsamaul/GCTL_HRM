using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
//using GCTL.Core.ViewModels.HrmLeaveApplicationEntry;
using GCTL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;

namespace GCTL.Service.HrmEmployeeOfficialInfoServe
{
    public class HrmEmployeeOfficialInfoService : IHrmEmployeeOfficialInfoService
    {
        private readonly GCTL_ERP_DB_DatapathContext _context;

        public HrmEmployeeOfficialInfoService(GCTL_ERP_DB_DatapathContext context)
        {
            _context = context;
        }


        //public List<EmpInfoViewModel> GetEmployeesByCompDept(string compCode, string? deptCode, string? branchCode)
        //{
        //    // var employees = _context.HrmEmployeeOfficialInfo.Where(e => e.CompanyCode == compCode && e.DepartmentCode == deptCode && e.BranchCode == branchCode).ToList();

        //    if (compCode !=null && deptCode == null && branchCode == null)
        //    {
        //        var empCom = (from e in _context.HrmEmployeeOfficialInfo
        //                      where e.CompanyCode == compCode 
        //                      join h in _context.HrmEmployee on e.EmployeeId equals h.EmployeeId into he
        //                      from h in he.DefaultIfEmpty()
        //                      select new EmpInfoViewModel
        //                      {
        //                          ReportingTo = e.ReportingTo,
        //                          HOD = e.Hod,
        //                          DepartmentCode = e.DepartmentCode,
        //                          DesignationCode = e.DesignationCode,

        //                          EmployeeFirstName = h.FirstName,
        //                          EmployeeLastName = h.LastName,

        //                          EmployeeId = h.EmployeeId,

        //                      }).ToList();
        //        return empCom;
        //    }
        //    else if (compCode != null && deptCode != null && branchCode == null)
        //    {
        //        var empCom = (from e in _context.HrmEmployeeOfficialInfo
        //                      where e.CompanyCode == compCode && e.DepartmentCode == deptCode
        //                      join h in _context.HrmEmployee on e.EmployeeId equals h.EmployeeId into he
        //                      from h in he.DefaultIfEmpty()
        //                      select new EmpInfoViewModel
        //                      {
        //                          ReportingTo = e.ReportingTo,
        //                          HOD = e.Hod,
        //                          DepartmentCode = e.DepartmentCode,
        //                          DesignationCode = e.DesignationCode,

        //                          EmployeeFirstName = h.FirstName,
        //                          EmployeeLastName = h.LastName,

        //                          EmployeeId = h.EmployeeId,

        //                      }).ToList();
        //        return empCom;
        //    }
        //    else if (compCode != null && deptCode == null && branchCode != null)
        //    {
        //        var empCom = (from e in _context.HrmEmployeeOfficialInfo
        //                      where e.CompanyCode == compCode && e.DepartmentCode == branchCode
        //                      join h in _context.HrmEmployee on e.EmployeeId equals h.EmployeeId into he
        //                      from h in he.DefaultIfEmpty()
        //                      select new EmpInfoViewModel
        //                      {
        //                          ReportingTo = e.ReportingTo,
        //                          HOD = e.Hod,
        //                          DepartmentCode = e.DepartmentCode,
        //                          DesignationCode = e.DesignationCode,

        //                          EmployeeFirstName = h.FirstName,
        //                          EmployeeLastName = h.LastName,

        //                          EmployeeId = h.EmployeeId,

        //                      }).ToList();
        //        return empCom;
        //    }
        //    else if (compCode != null && deptCode != null && branchCode != null)
        //    {
        //        var empCom = (from e in _context.HrmEmployeeOfficialInfo
        //                      where e.CompanyCode == compCode && e.DepartmentCode == deptCode && e.BranchCode == branchCode
        //                      join h in _context.HrmEmployee on e.EmployeeId equals h.EmployeeId into he
        //                      from h in he.DefaultIfEmpty()
        //                      select new EmpInfoViewModel
        //                      {
        //                          ReportingTo = e.ReportingTo,
        //                          HOD = e.Hod,
        //                          DepartmentCode = e.DepartmentCode,
        //                          DesignationCode = e.DesignationCode,

        //                          EmployeeFirstName = h.FirstName,
        //                          EmployeeLastName = h.LastName,

        //                          EmployeeId = h.EmployeeId,

        //                      }).ToList();
        //        return empCom;
        //    }


        //    var emp = (from e in _context.HrmEmployeeOfficialInfo
        //               where e.CompanyCode == compCode || e.DepartmentCode == deptCode || e.BranchCode == branchCode
        //               join h in _context.HrmEmployee on e.EmployeeId equals h.EmployeeId into he
        //               from h in he.DefaultIfEmpty()
        //               select new EmpInfoViewModel
        //               {
        //                   ReportingTo = e.ReportingTo,
        //                   HOD = e.Hod,
        //                   DepartmentCode = e.DepartmentCode,
        //                   DesignationCode = e.DesignationCode,

        //                   EmployeeFirstName = h.FirstName,
        //                   EmployeeLastName = h.LastName,

        //                   EmployeeId = h.EmployeeId,

        //               }).ToList();


        //       var  employees = _context.HrmEmployeeOfficialInfo.Where(e => e.CompanyCode == compCode || e.DepartmentCode == deptCode || e.BranchCode == branchCode).ToList();
            
        //    return emp;
        //}

        public  List<HrmEmployeeOfficialInfo> GetEmployeesByCompCode(string compCode)
        {
            var employees =  _context.HrmEmployeeOfficialInfo.Where(e => e.CompanyCode == compCode).ToList();
            return employees;
        }

        //public async Task< EmpInfoViewModel> GetEmployeesByEmpId(string EmpId)
        //{
        //    var query = await (from h in _context.HrmEmployee
        //                 where h.EmployeeId == EmpId
        //                 join e in _context.HrmEmployeeOfficialInfo on h.EmployeeId equals e.EmployeeId into he
        //                from e in he.DefaultIfEmpty()
        //                join dep in _context.HrmDefDepartment on e.DepartmentCode equals dep.DepartmentCode into ed
        //                from dep in ed.DefaultIfEmpty()
        //                join desig in _context.HrmDefDesignation on e.DesignationCode equals desig.DesignationCode into edg
        //                from desig in edg.DefaultIfEmpty()
        //                join r in _context.HrmEmployee on e.ReportingTo equals r.EmployeeId into er
        //                from r in er.DefaultIfEmpty()
        //                join hod in _context.HrmEmployee on e.Hod equals hod.EmployeeId into eh
        //                from hod in eh.DefaultIfEmpty()
        //                 join com in _context.CoreCompany on h.CompanyCode equals com.CompanyCode into ch
        //                 from com in ch.DefaultIfEmpty()

        //                join branch in _context.CoreBranch on e.BranchCode equals branch.BranchCode into brnc
        //                from branch in brnc.DefaultIfEmpty()



        //                select new EmpInfoViewModel
        //                 {
        //                    ReportingTo = e.ReportingTo,
        //                    HOD = e.Hod,
        //                    DepartmentCode = e.DepartmentCode,
        //                    DesignationCode = e.DesignationCode,
        //                    DepartmentName = dep.DepartmentName,
        //                    DepartmentShortName = dep.DepartmentShortName,
        //                    DesignationName = desig.DesignationName,
        //                    DesignationShortName = desig.DesignationShortName,
        //                    EmployeeFirstName = h.FirstName,
        //                    EmployeeLastName = h.LastName,
        //                    ReportingFirstName = r.FirstName,
        //                    ReportingLastName = r.LastName,
        //                    HODFirstName = hod.FirstName,
        //                    HODLastName = hod.LastName,
        //                    CompanyName = com.CompanyName,
        //                    CompanyCode = com.CompanyCode,
        //                    EmployeeId = h.EmployeeId,
        //                    JoiningDate = e.JoiningDate,
        //                    BranchName = branch.BranchName,
        //                    GrossSalary = e.GrossSalary,
        //                    ConfirmationDate = e.ConfirmeDate,
        //                    IsExpatriate = e.IsExpatriate,
        //                    ExpatriateBasicSalary = e.ExpatriateBasicSalary,
        //                    ExpatriateConveyance = e.ExpatriateConveyance,
        //                    ExpatriateHouseRent = e.ExpatriateHouseRent,
        //                    ExpatriateMedical = e.ExpatriateMedical,
        //                    Lfa = e.Lfa


                            
        //                   // HodId = hod.EmployeeId,
        //                   // HodFirstName = hod.FirstName,
        //                   // HodLastName = hod.LastName,
        //                    //ISuperVisorId = r.EmployeeId,
        //                   // ISuperVisorName = r.FirstName
        //                 }).FirstOrDefaultAsync();

        //    var employees =  _context.HrmEmployeeOfficialInfo.Where(e => e.EmployeeId == EmpId).ToList();
        //    return query;
        //}

        
    }
}

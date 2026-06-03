using AutoMapper;
using Dapper;
using DocumentFormat.OpenXml.Math;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.EmployeeGeneralInfoReport;

using GCTL.Data.Models;
using GCTL.Service.HrmEmployees2;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Humanizer.In;

namespace GCTL.Service.EmployeeGeneralInfoReport
{
    public class EmployeeGeneralInfoReportService: IEmployeeGeneralInfoReportService
    {
        private readonly IRepository<HrmEmployee> _hrmEmployeeRepository;
        private readonly IRepository<CoreBranch> coreBranch;
        private readonly IRepository<HrmDefDepartment> deptment;
        private readonly IRepository<HrmDefDesignation> designation;
        private readonly IRepository<HrmEmployeeOfficialInfo> _empOfficialRepository;
        private readonly IConfiguration _configuration;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;

       

        public EmployeeGeneralInfoReportService(IRepository<HrmEmployee> hrmEmployeeRepository, IRepository<CoreBranch> coreBranch, IRepository<HrmDefDepartment> deptment, IRepository<HrmDefDesignation> designation, IRepository<HrmEmployeeOfficialInfo> empOfficialRepository, IConfiguration configuration, IRepository<CoreAccessCode> accessCodeRepository)
        {
            _hrmEmployeeRepository = hrmEmployeeRepository;
            this.coreBranch = coreBranch;
            this.deptment = deptment;
            this.designation = designation;
            _empOfficialRepository = empOfficialRepository;
            _configuration = configuration;
            this.accessCodeRepository = accessCodeRepository;
        }

        private string BuildQueryString(DynamicParameters parameters)
        {
            var queryBuilder = new StringBuilder();
            queryBuilder.Append("EXEC EmployeeGeneralInfoReportSP ");

            foreach (var param in parameters.ParameterNames)
            {
                var value = parameters.Get<object>(param);
                // If the value is null or whitespace, default to a single space (' ')
                string output = (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                    ? "' '"
                    : $"'{value}'";
                queryBuilder.Append($"@{param} = {output}, ");
            }

            if (queryBuilder.Length > 0)
            {
                queryBuilder.Length -= 2; 
            }

            return queryBuilder.ToString();
        }





        public async Task<List<dynamic>> EmployeeGeneralInfoReport(List<string> departmentCode, List<string> designationCodes,
            List<string> employeeCodes, List<string> branchCodes,
        List<string> companyCodes, 
        string nationalityCode, string genderCode, string bloodGroupCode,
        string religionCode, string maritalStatusCode
    )
        {
            var queryParameters = new DynamicParameters();

            // Prepare parameters
            queryParameters.Add("@DepartmentCodes", string.Join(",", departmentCode ?? new List<string>()));
            queryParameters.Add("@DesignationCodes", string.Join(",", designationCodes ?? new List<string>()));
            queryParameters.Add("@EmployeeCodes", string.Join(",", employeeCodes ?? new List<string>()));
            queryParameters.Add("@BranchCodes", string.Join(",", branchCodes ?? new List<string>()));
            queryParameters.Add("@CompanyCodes", string.Join(",", companyCodes ?? new List<string>()));
            queryParameters.Add("@GenderCode", genderCode ?? (object)null);
            queryParameters.Add("@NationalityCode", nationalityCode ?? (object)null);
            queryParameters.Add("@BloodGroupCode", bloodGroupCode ?? (object)null);
            queryParameters.Add("@ReligionCode", religionCode ?? (object)null);
            queryParameters.Add("@MaritalStatusCode", maritalStatusCode ?? (object)null);

            var query = BuildQueryString(queryParameters);


            try
            {
                using (var connection = new SqlConnection(_configuration.GetConnectionString("ApplicationDbConnection")))
                {

                    await connection.OpenAsync();
                    var result = (await connection.QueryAsync<dynamic>(query, queryParameters)).ToList();

                    return result;

                }
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error fetching official info: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return new List<dynamic>();
            }
        }

       

        #region Filtering Data
      
       

        public async Task<List<CommonSelectModel>> GetComapnyByBranchCode(List<string> companyCode)
        {
            try
            {
                var result = await (
                                    from br in coreBranch.All().AsNoTracking()
                                       
                                    where companyCode.Contains(br.CompanyCode)
                                    select new CommonSelectModel
                                    {
                                        Code = br.BranchCode,
                                        Name = br.BranchName

                                    }).ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }
        #endregion

        //
        #region GetBranchByCompanyId
        public async Task<List<CommonSelectModelDD>> GetBranchByCompanyId(List<string> companyIds)
        {

            if (companyIds == null || !companyIds.Any())
            {
                return new List<CommonSelectModelDD>();
            }

            var result = await (from b in coreBranch.All().AsNoTracking()

                                where companyIds.Contains(b.CompanyCode)

                                select new CommonSelectModelDD
                                {
                                    BranchCode = b.BranchCode,
                                    BranchName = b.BranchName
                                }).ToListAsync();
            return result;


        }
        #endregion


        #region GetDepartmentByCompanyId
        public async Task<List<CommonSelectModelDD>> GetDepartmentByCompanyId(List<string> companyIds)
        {
            if (companyIds == null || !companyIds.Any())
            {
                return new List<CommonSelectModelDD>();
            }

            var result = await (from dep in deptment.All().AsNoTracking()

                                where companyIds.Contains(dep.CompanyCode)

                                select new CommonSelectModelDD
                                {
                                    DepartmentCode = dep.DepartmentCode,
                                    DepartmentName = dep.DepartmentName
                                }).ToListAsync();
            return result;
        }
        #endregion


        #region GetDesignationByCompanyId
        public async Task<List<CommonSelectModelDD>> GetDesignationByCompanyId(List<string> companyIds)
        {
            if (companyIds == null || !companyIds.Any())
            {
                return new List<CommonSelectModelDD>();
            }

            var result = await (from des in designation.All().AsNoTracking()

                                where companyIds.Contains(des.CompanyCode)

                                select new CommonSelectModelDD
                                {
                                    DesignationCode = des.DesignationCode,
                                    DesignationName = des.DesignationName
                                }).ToListAsync();
            return result;
        }
        #endregion


        #region GetEmployeeByCompanyId
        public async Task<List<CommonSelectModelDD>> GetEmployeeByCompanyId(List<string> companyIds)
        {
            if (companyIds == null || !companyIds.Any())
            {
                return new List<CommonSelectModelDD>();
            }

            var result = await (from e in _hrmEmployeeRepository.All().AsNoTracking()

                                where companyIds.Contains(e.CompanyCode)

                                select new CommonSelectModelDD
                                {
                                    EmployeeId = e.EmployeeId,
                                    EmployeeName = $"{e.FirstName} {e.LastName}"
                                }).ToListAsync();
            return result;
        }
        #endregion


        #region GetDepartmentByBranchId
        public async Task<List<CommonSelectModelDD>> GetDepartmentByBranchId(List<string> branchIds)
        {
            if (branchIds == null || !branchIds.Any())
            {
                return new List<CommonSelectModelDD>();
            }

            var result = await (from oi in _empOfficialRepository.All().AsNoTracking()

                                where branchIds.Contains(oi.BranchCode)

                                join dep in deptment.All().AsNoTracking() on oi.DepartmentCode equals dep.DepartmentCode into depGroup
                                from dep in depGroup.DefaultIfEmpty()

                                join br in coreBranch.All().AsNoTracking() on oi.BranchCode equals br.BranchCode into brGroup
                                from br in brGroup.DefaultIfEmpty()

                                select new CommonSelectModelDD
                                {
                                    DepartmentCode = oi.DepartmentCode,
                                    DepartmentName = dep.DepartmentName
                                }).Distinct().ToListAsync();

            return result;
        }
        #endregion


        #region GetDesignationByBranchId
        public async Task<List<CommonSelectModelDD>> GetDesignationByBranchId(List<string> branchIds)
        {
            if (branchIds == null || !branchIds.Any())
            {
                return new List<CommonSelectModelDD>();
            }

            var result = await (from oi in _empOfficialRepository.All().AsNoTracking()

                                where branchIds.Contains(oi.BranchCode)

                                join des in designation.All().AsNoTracking() on oi.DesignationCode equals des.DesignationCode into desGroup
                                from des in desGroup.DefaultIfEmpty()

                                join br in coreBranch.All().AsNoTracking() on oi.BranchCode equals br.BranchCode into brGroup
                                from br in brGroup.DefaultIfEmpty()

                                select new CommonSelectModelDD
                                {
                                    DesignationCode = oi.DesignationCode,
                                    DesignationName = des.DesignationName
                                }).Distinct().ToListAsync();

            return result;
        }
        #endregion


        #region GetEmployeeByBranchId
        public async Task<List<CommonSelectModelDD>> GetEmployeeByBranchId(List<string> branchIds)
        {
            if (branchIds == null || !branchIds.Any())
            {
                return new List<CommonSelectModelDD>();
            }

            var result = await (from oi in _empOfficialRepository.All().AsNoTracking()

                                where branchIds.Contains(oi.BranchCode)

                                join e in _hrmEmployeeRepository.All().AsNoTracking() on oi.EmployeeId equals e.EmployeeId into eGroup
                                from e in eGroup.DefaultIfEmpty()

                                join br in coreBranch.All().AsNoTracking() on oi.BranchCode equals br.BranchCode into brGroup
                                from br in brGroup.DefaultIfEmpty()

                                select new CommonSelectModelDD
                                {
                                    EmployeeId = oi.EmployeeId,
                                    EmployeeName = $"{e.FirstName} {e.LastName}"
                                }).Distinct().ToListAsync();

            return result;
        }
        #endregion


        #region GetDesignationByDepartmentId
        public async Task<List<CommonSelectModelDD>> GetDesignationByDepartmentId(List<string> departmentIds)
        {
            if (departmentIds == null || !departmentIds.Any())
            {
                return new List<CommonSelectModelDD>();
            }

            var result = await (from oi in _empOfficialRepository.All().AsNoTracking()

                                where departmentIds.Contains(oi.DepartmentCode)

                                join des in designation.All().AsNoTracking() on oi.DesignationCode equals des.DesignationCode into desGroup
                                from des in desGroup.DefaultIfEmpty()

                                select new CommonSelectModelDD
                                {
                                    DesignationCode = oi.DesignationCode,
                                    DesignationName = des.DesignationName
                                }).Distinct().ToListAsync();

            return result; ;
        }
        #endregion


        #region GetEmployeeByDepartmentId
        public async Task<List<CommonSelectModelDD>> GetEmployeeByDepartmentId(List<string> departmentIds)
        {
            if (departmentIds == null || !departmentIds.Any())
            {
                return new List<CommonSelectModelDD>();
            }

            var result = await (from oi in _empOfficialRepository.All().AsNoTracking()

                                where departmentIds.Contains(oi.DepartmentCode)

                                join e in _hrmEmployeeRepository.All().AsNoTracking() on oi.EmployeeId equals e.EmployeeId into eGroup
                                from e in eGroup.DefaultIfEmpty()

                                select new CommonSelectModelDD
                                {
                                    EmployeeId = oi.EmployeeId,
                                    EmployeeName = $"{e.FirstName} {e.LastName}"
                                }).Distinct().ToListAsync();

            return result; ;
        }
        #endregion


        #region GetEmployeeByDesignationId
        public async Task<List<CommonSelectModelDD>> GetEmployeeByDesignationId(List<string> designationIds)
        {
            if (designationIds == null || !designationIds.Any())
            {
                return new List<CommonSelectModelDD>();
            }

            var result = await (from oi in _empOfficialRepository.All().AsNoTracking()

                                where designationIds.Contains(oi.DesignationCode)

                                join e in _hrmEmployeeRepository.All().AsNoTracking() on oi.EmployeeId equals e.EmployeeId into eGroup
                                from e in eGroup.DefaultIfEmpty()

                                select new CommonSelectModelDD
                                {
                                    EmployeeId = oi.EmployeeId,
                                    EmployeeName = $"{e.FirstName} {e.LastName}"
                                }).Distinct().ToListAsync();

            return result; ;
        }
        #endregion


        #region EmployeeSelection
        public IEnumerable<CommonSelectModel> EmployeeSelection()
        {
            return _hrmEmployeeRepository.All()
                .Select(x => new CommonSelectModel
                {
                    Code = x.EmployeeId,
                    Name = $"{x.FirstName} {x.LastName} ({x.EmployeeId})"
                });
        }


        #endregion
        //

        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Employee General Info Report" && x.TitleCheck);
        }

    }

}

//SP

//drop proc EmployeeGeneralInfoReportSP

//CREATE PROCEDURE EmployeeGeneralInfoReportSP
//    @DepartmentCodes NVARCHAR(MAX) ,
//    @DesignationCodes NVARCHAR(MAX) ,
//    @EmployeeCodes NVARCHAR(MAX) ,
//    @BranchCodes NVARCHAR(MAX) ,
//    @CompanyCodes NVARCHAR(MAX) ,
//    @GenderCode NVARCHAR(50) ,
//    @BloodGroupCode NVARCHAR(50),
//    @NationalityCode NVARCHAR(50) ,
//    @ReligionCode NVARCHAR(50) ,
//    @MaritalStatusCode NVARCHAR(50)


//AS
//BEGIN
//    SET NOCOUNT ON;

//select isNull(emp.EmployeeID,'') as EmployeeID , 
//   isnull(emp.FirstName + '' + emp.LastName, '') as EmployeeName,
//   ISNULL(emp.FatherName, '') AS FatherName,
//    ISNULL(emp.MotherName, '') AS MotherName,
//    ISNULL(n.Nationality, '') AS Nationality,
//      ISNULL(com.CompanyName, '') AS CompanyName,
//       ISNULL(emp.NationalIDNO, '') AS NationalIDNO,
//ISNULL(
//    CASE
//        WHEN emp.DateOfBirthCertificate = '1900-01-01' THEN ''
//        ELSE CONVERT(NVARCHAR, emp.DateOfBirthCertificate, 120)
//    END, ''
//) AS DateOfBirthCertificate,

//    ISNULL(emp.PlaceOfBirth, '') AS PlaceOfBirth,
//    ISNULL(s.Sex, '') AS Sex,
//    ISNULL(b.BloodGroup, '') AS BloodGroup,
//    ISNULL(r.Religion, '') AS Religion,
//    ISNULL(c.PresentAddress, '') AS PresentAddress,
//    ISNULL(c.ParmanentAddress, '') AS ParmanentAddress,
//    ISNULL(emp.Telephone, '') AS Telephone,
//    ISNULL(emp.PersonalEmail, '') AS PersonalEmail,
//    ISNULL(emp.TinNo, '') AS TinNo,
//    ISNULL(m.MaritalStatus, '') AS MaritalStatus,
//    ISNULL(dept.DepartmentName, '') AS DepartmentName,
//    ISNULL(desi.DesignationName, '') AS DesignationName

//from  HRM_Employee as emp   
// LEFT outer JOIN   HRM_Def_Nationality as n 
// on emp.NationalityCode=n.NationalityCode
//left outer join  HRM_Def_Sex  s
//on emp.SexCode=s.SexCode
//left outer join  HRM_Def_BloodGroup b
//on emp.BloodGroupCode=b.BloodGroupCode
//left outer join HRM_Def_Religion as r
//on emp.ReligionCode=r.ReligionCode
//left outer join HRM_Def_MaritalStatus  as m
//on emp.MaritalStatusCode=m.MaritalStatusCode
//left outer join HRM_EmployeeOfficialInfo  as offi
//on emp.EmployeeID=offi.EmployeeID
// left outer JOIN HRM_Def_Designation AS desi ON offi.DesignationCode = desi.DesignationCode
// left outer JOIN HRM_Def_Department AS dept ON offi.DepartmentCode = dept.DepartmentCode
// left outer JOIN Core_Branch AS br ON offi.BranchCode = br.BranchCode
// left outer join  HRM_EmployeeContactInfo  as c
// on emp.EmployeeID=c.EmployeeID
// left outer join Core_Company as com
// on emp.CompanyCode=com.CompanyCode
//    WHERE 
//        (@DepartmentCodes = '' OR offi.DepartmentCode IN (Select * from SplitString(@DepartmentCodes, ','))) AND
//        (@DesignationCodes ='' OR offi.DesignationCode IN (Select * from SplitString(@DesignationCodes, ','))) AND
//        (@EmployeeCodes='' OR offi.EmployeeID IN (Select * from SplitString(@EmployeeCodes, ','))) AND
//        (@BranchCodes ='' OR offi.BranchCode IN (Select * from SplitString(@BranchCodes, ','))) AND
//        (@CompanyCodes ='' OR offi.CompanyCode IN (Select * from SplitString(@CompanyCodes, ','))) AND
//        (@GenderCode ='' OR s.SexCode = @GenderCode) AND
//        (@BloodGroupCode ='' OR b.BloodGroupCode = @BloodGroupCode) AND
//        (@NationalityCode ='' OR n.NationalityCode = @NationalityCode) AND
//        (@ReligionCode ='' OR r.ReligionCode = @ReligionCode) AND
//        (@MaritalStatusCode ='' OR m.MaritalStatusCode = @MaritalStatusCode) 
//        ORDER BY emp.EmployeeID;
//END;


//EXEC EmployeeGeneralInfoReportSP
//    @DepartmentCodes = '001,004,005,006',
//    @DesignationCodes = '',
//    @EmployeeCodes = '',
//    @BranchCodes = '001',
//    @CompanyCodes = '',
//    @GenderCode = '',
//    @BloodGroupCode = '',
//    @NationalityCode = '',
//    @ReligionCode = '',
//    @MaritalStatusCode = ''

//select*from HRM_Def_Designation



//select*from HRM_EmployeeOfficialInfo

//select*from HRM_Employee

//select*from HRM_Def_Designation

//select*from HRM_Def_Department

//select * from Core_Branch

//select*from HRM_EIS_Def_EmploymentNature

//select*from HRM_Def_EmpType

//select*from HRM_Def_EmployeeStatus

//select*from HRM_Separation

//select*from HRM_EmployeeAdditionalInfo

//select CompanyName from Core_Company

//select *from HRM_Def_BloodGroup 

//select *from HRM_Def_Nationality

//select* from HRM_Def_MaritalStatus

//select*from  HRM_Def_Sex

//select*from HRM_Def_Religion

//select*from HRM_EmployeeContactInfo


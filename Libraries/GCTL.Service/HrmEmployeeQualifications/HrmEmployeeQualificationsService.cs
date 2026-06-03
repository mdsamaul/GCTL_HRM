using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.Companies;
using GCTL.Core.ViewModels.HrmEmployeeEducations;
using GCTL.Core.ViewModels.HrmEmployeeFamilys;
using GCTL.Core.ViewModels.HrmEmployeeQualifications;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HrmEmployeeQualifications
{
    public class HrmEmployeeQualificationsService : AppService<HrmEmployeeQualification>, IHrmEmployeeQualificationsService
    {
        private readonly IRepository<HrmEmployeeQualification> hrmEmployeeQualification;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;
        private readonly IRepository<HrmDefInstitute> hrmDefInstitute;
        private readonly IRepository<HrmDefDegree> hrmdegDegree;
        private readonly IRepository<HrmEmployee> hrmEmployee;
        private readonly IRepository<HrmDefDepartment> departmentRepository;
        private readonly IRepository<HrmDefDesignation> designationRepository;
        private readonly IRepository<HrmEmployeeOfficialInfo> hrmEmpOffialInfo;

        string strMaxNO = string.Empty;
        private const string TableName = "HRM_EmployeeQualification";
        private const string ColumnName = "EmpQualificationID";

        public HrmEmployeeQualificationsService(IRepository<HrmEmployeeQualification> hrmEmployeeQualification, IRepository<HrmEmployeeOfficialInfo> hrmEmpOffialInfo, IRepository<CoreAccessCode> accessCodeRepository, ICommonService commonService, IRepository<HrmDefInstitute> hrmDefInstitute, IRepository<HrmDefDegree> hrmdegDegree, IRepository<CoreCompany> coreCompanyRepository, IRepository<HrmEmployee> hrmEmployee, IRepository<HrmDefDepartment> departmentRepository, IRepository<HrmDefDesignation> designationRepository) : base(hrmEmployeeQualification)
        {
            this.hrmEmployeeQualification = hrmEmployeeQualification;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
            this.hrmDefInstitute = hrmDefInstitute;
            this.hrmdegDegree = hrmdegDegree;
            this.hrmEmployee = hrmEmployee;
            this.departmentRepository = departmentRepository;
            this.designationRepository = designationRepository;
            this.hrmEmpOffialInfo = hrmEmpOffialInfo;
        }

        public async Task<List<HrmEmployeeQualificationsSetupViewModel>> GetAllAsync(string employeeId)
        {
            var data = await (from quali in hrmEmployeeQualification.All().AsNoTracking()
                              where quali.EmployeeId == employeeId
                              join inst in hrmDefInstitute.All().AsNoTracking()
                              on quali.InstitueCode equals inst.InstituteCode into qualiInstJoin
                              from inst in qualiInstJoin.DefaultIfEmpty()
                              join deg in hrmdegDegree.All().AsNoTracking()
                              on quali.CourseTitleCode equals deg.DegreeCode into qualiDegreeJoin
                              from deg in qualiDegreeJoin.DefaultIfEmpty()

                              join emp in hrmEmployee.All().AsNoTracking()
                              on quali.EmployeeId equals emp.EmployeeId into empEduEmpJoin
                              from emp in empEduEmpJoin.DefaultIfEmpty()


                              join ofEmp in hrmEmpOffialInfo.All().AsNoTracking()
                             on quali.EmployeeId equals ofEmp.EmployeeId into eduOffJoin
                              from ofEmp in eduOffJoin.DefaultIfEmpty()

                              join desi in designationRepository.All().AsNoTracking()
                              on ofEmp.DesignationCode equals desi.DesignationCode into edudesiJoin
                              from desi in edudesiJoin.DefaultIfEmpty()

                              join dept in departmentRepository.All().AsNoTracking()
                              on ofEmp.DepartmentCode equals dept.DepartmentCode into eduDeptJoin
                              from dept in eduDeptJoin.DefaultIfEmpty()



                              select new HrmEmployeeQualificationsSetupViewModel
                              {
                                  EmpQualificationId = quali.EmpQualificationId,
                                  Instituteaddress = quali.Instituteaddress,
                                  ResultDivision = quali.ResultDivision,
                                  YearofPasssing = quali.YearofPasssing,
                                  Dueration = quali.Dueration,
                                  DuratioinType = quali.DuratioinType,
                                  CourseCode = quali.CourseCode,
                                  CourseTittleName = deg.DegreeName,
                                  InstituteName = inst.InstituteName,
                                  EmployeeId = quali.EmployeeId
                              }
                             ).ToListAsync();
            return data;
        }

        //

        public async Task<List<HrmEmployeeQualificationsSetupViewModel>> GetEmployeeByCompanyCode(string companyCode)
        {
            try
            {


                var result = await (from e in hrmEmployee.All().AsNoTracking()

                                    where (e.CompanyCode == companyCode)

                                    select new HrmEmployeeQualificationsSetupViewModel
                                    {
                                        EmployeeId = e.EmployeeId,
                                        EmployeeName = string.Format("{0} {1} ({2})", e.FirstName, e.LastName, e.EmployeeId),
                                    }).ToListAsync();



                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }

        }
        public async Task<HrmEmployeeQualificationsSetupViewModel> GetEmployeeNameDesDeptByCode(string employeeId)
        {
            try
            {

                var result = await (

                                    from e in hrmEmployee.All().AsNoTracking()

                                    join oi in hrmEmpOffialInfo.All()
                                        .Select(x => new { x.EmployeeId, x.DepartmentCode, x.DesignationCode })
                                        .AsNoTracking() on e.EmployeeId equals oi.EmployeeId into oiGroup
                                    from oi in oiGroup.DefaultIfEmpty()

                                    join des in designationRepository.All()
                                        .Select(x => new { x.DesignationCode, x.DesignationName })
                                        .AsNoTracking() on oi.DesignationCode equals des.DesignationCode into desGroup
                                    from des in desGroup.DefaultIfEmpty()

                                    join dep in departmentRepository.All()
                                        .Select(x => new { x.DepartmentCode, x.DepartmentName })
                                        .AsNoTracking() on oi.DepartmentCode equals dep.DepartmentCode into depGroup
                                    from dep in depGroup.DefaultIfEmpty()

                                    where e.EmployeeId == employeeId

                                    select new HrmEmployeeQualificationsSetupViewModel
                                    {
                                        EmployeeId = e.EmployeeId,
                                        EmployeeName = $"{e.FirstName} {e.LastName}" ?? " ",
                                        DesignationName = des.DesignationName ?? "",
                                        DepartmentName = dep.DepartmentName ?? "",


                                    }).FirstOrDefaultAsync();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }


        }

        //
        public async Task<HrmEmployeeQualificationsSetupViewModel> GetByIdAsync(string code)
        {
            var data = await (from quali in hrmEmployeeQualification.All().Where(x => x.EmpQualificationId == code).AsNoTracking()
                              join inst in hrmDefInstitute.All().AsNoTracking()
                              on quali.InstitueCode equals inst.InstituteCode into qualiInstJoin
                              from inst in qualiInstJoin.DefaultIfEmpty()
                              join deg in hrmdegDegree.All().AsNoTracking()
                              on quali.CourseTitleCode equals deg.DegreeCode into qualiDegreeJoin
                              from deg in qualiDegreeJoin.DefaultIfEmpty()

                              join emp in hrmEmployee.All().AsNoTracking()
                              on quali.EmployeeId equals emp.EmployeeId into empEduEmpJoin
                              from emp in empEduEmpJoin.DefaultIfEmpty()

                              join ofEmp in hrmEmpOffialInfo.All().AsNoTracking()
                                    on quali.EmployeeId equals ofEmp.EmployeeId into eduOffJoin
                              from ofEmp in eduOffJoin.DefaultIfEmpty()

                              join desi in designationRepository.All().AsNoTracking()
                              on ofEmp.DesignationCode equals desi.DesignationCode into edudesiJoin
                              from desi in edudesiJoin.DefaultIfEmpty()

                              join dept in departmentRepository.All().AsNoTracking()
                              on ofEmp.DepartmentCode equals dept.DepartmentCode into eduDeptJoin
                              from dept in eduDeptJoin.DefaultIfEmpty()


                              select new HrmEmployeeQualificationsSetupViewModel
                              {
                                  AutoId = quali.AutoId,
                                  EmpQualificationId = quali.EmpQualificationId,
                                  Instituteaddress = quali.Instituteaddress,
                                  ResultDivision = quali.ResultDivision,
                                  YearofPasssing = quali.YearofPasssing,
                                  Dueration = quali.Dueration,
                                  DuratioinType = quali.DuratioinType,
                                  Achievment = quali.Achievment,
                                  Remarks = quali.Remarks,
                                  CourseCode = quali.CourseCode,
                                  Luser = quali.Luser,
                                  Ldate = quali.Ldate,
                                  ModifyDate = quali.ModifyDate,
                                  UserInfoEmployeeId = quali.UserInfoEmployeeId,
                                  Lip = quali.Lip,
                                  Lmac = quali.Lmac,
                                  CourseTitleCode = quali.CourseTitleCode,
                                  CourseTittleName = deg.DegreeName,
                                  InstitueCode = quali.InstitueCode,

                                  InstituteName = inst.InstituteName,

                                  CompanyCode = quali.CompanyCode,

                                  EmployeeId = quali.EmployeeId,
                                  EmployeeName = $"{emp.FirstName} {emp.LastName}",
                                  DepartmentName = dept.DepartmentName ?? "",
                                  DesignationName = desi.DesignationName ?? "",
                              }).FirstOrDefaultAsync();
            return data;
        }

        public async Task<bool> SaveAsync(HrmEmployeeQualificationsSetupViewModel entityVM, string CompanyCode)
        {
            await hrmEmployeeQualification.BeginTransactionAsync();
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 6);
            try
            {
                HrmEmployeeQualification entity = new HrmEmployeeQualification();
                entity.EmpQualificationId = strMaxNO;
                entity.CompanyCode = entityVM.CompanyCode ?? string.Empty;
                entity.EmployeeId = entityVM.EmployeeId ?? string.Empty;
                entity.CourseCode = entityVM.CourseCode ?? string.Empty;
                entity.CourseTitleCode = entityVM.CourseTitleCode ?? string.Empty;
                entity.InstitueCode = entityVM.InstitueCode ?? string.Empty;
                entity.ResultDivision = entityVM.ResultDivision ?? string.Empty;
                entity.Instituteaddress = entityVM.Instituteaddress ?? string.Empty;
                entity.YearofPasssing = entityVM.YearofPasssing ?? string.Empty;
                entity.CompanyCode = entityVM.CompanyCode ?? string.Empty;
                entity.Dueration = entityVM.Dueration ?? string.Empty;
                entity.DuratioinType = entityVM.DuratioinType ?? string.Empty;
                entity.Achievment = entityVM.Achievment ?? string.Empty;
                entity.Remarks = entityVM.Remarks ?? string.Empty;
                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.CompanyCode = CompanyCode;
                entity.UserInfoEmployeeId = entityVM.UserInfoEmployeeId ?? string.Empty;
                entity.Ldate = DateTime.Now;
                await hrmEmployeeQualification.AddAsync(entity);
                await hrmEmployeeQualification.CommitTransactionAsync();
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await hrmEmployeeQualification.RollbackTransactionAsync();
                return false;
            }
        }

        public async Task<bool> UpdateAsync(HrmEmployeeQualificationsSetupViewModel entityVM)
        {
            await hrmEmployeeQualification.BeginTransactionAsync();
            try
            {

                var entity = await hrmEmployeeQualification.GetByIdAsync(entityVM.EmpQualificationId);
                if (entity == null)
                {
                    await hrmEmployeeQualification.RollbackTransactionAsync();
                    return false;
                }
                entity.EmpQualificationId = entityVM.EmpQualificationId;
                entity.CompanyCode = entityVM.CompanyCode ?? string.Empty;
                entity.EmployeeId = entityVM.EmployeeId ?? string.Empty;
                entity.CourseCode = entityVM.CourseCode ?? string.Empty;
                entity.CourseTitleCode = entityVM.CourseTitleCode ?? string.Empty;
                entity.InstitueCode = entityVM.InstitueCode ?? string.Empty;
                entity.ResultDivision = entityVM.ResultDivision ?? string.Empty;
                entity.Instituteaddress = entityVM.Instituteaddress ?? string.Empty;
                entity.YearofPasssing = entityVM.YearofPasssing ?? string.Empty;
                entity.CompanyCode = entityVM.CompanyCode ?? string.Empty;
                entity.Dueration = entityVM.Dueration ?? string.Empty;
                entity.DuratioinType = entityVM.DuratioinType ?? string.Empty;
                entity.Achievment = entityVM.Achievment ?? string.Empty;
                entity.Remarks = entityVM.Remarks ?? string.Empty;
                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.UserInfoEmployeeId = entityVM.UserInfoEmployeeId;
                entity.ModifyDate = DateTime.Now;
                await hrmEmployeeQualification.UpdateAsync(entity);
                await hrmEmployeeQualification.CommitTransactionAsync();
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await hrmEmployeeQualification.RollbackTransactionAsync();
                return false;
            }
        }

        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await hrmEmployeeQualification.All().Where(x => ids.Contains(x.EmpQualificationId)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }

            hrmEmployeeQualification.Delete(entity);

            return true;
        }

        public Task<bool> IsExistByAsync(string code)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> IsExistAsync(string code, string employeeCode, string courseTypeId, string couresetitleID)
        {
            return await hrmEmployeeQualification.All().AnyAsync(x => x.EmployeeId == employeeCode && x.CourseCode == courseTypeId && x.CourseTitleCode == couresetitleID && x.EmpQualificationId != code);
        }
        public Task<bool> IsExistAsync(string employeeCode, string typeCode, string name)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<CommonSelectModel>> DropSelection()
        {
            var data = await hrmEmployeeQualification.All()
                .Select(x => new CommonSelectModel
                {
                    Code = x.EmployeeId,
                    Name = x.Remarks
                }).ToListAsync();
            return data;

        }

        #region Acccess Permission

        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Professional Qualification Entry" && x.TitleCheck);
        }

        public async Task<bool> SavePermissonAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Professional Qualification Entry" && x.CheckAdd);
        }

        public async Task<bool> UpdateParmissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Professional Qualification Entry" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Professional Qualification Entry" && x.CheckDelete);

        }
        #endregion 
    }
}
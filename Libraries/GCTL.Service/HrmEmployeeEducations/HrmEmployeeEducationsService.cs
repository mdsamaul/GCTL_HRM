using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HrmEmployeeEducations;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HrmEmployeeEducations
{
    public class HrmEmployeeEducationsService : AppService<HrmEmployeeEducation>, IHrmEmployeeEducationsService
    {
        private readonly IRepository<HrmEmployeeEducation> hrmEmpEduInfo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;
        private readonly IRepository<CoreCompany> coreCompanyRepository;
        private readonly IRepository<CoreBranch> coreBranchRepository;
        private readonly IRepository<HrmEmployee> hrmEmployee;
        private readonly IRepository<HrmEmployeeOfficialInfo> hrmEmpOffialInfo;
        private readonly IRepository<HrmDefDepartment> departmentRepository;
        private readonly IRepository<HrmDefDesignation> designationRepository;
        private readonly IRepository<HrmDefDegree> hrmDegree;
        private readonly IRepository<HrmDefBoardCountryName> hrmBoardCountryName;
        private readonly IRepository<HrmDefExamTitle> hrmDefexamTitle;
        private readonly IRepository<HrmDefInstitute> hrmDefInstitute;
        private readonly IRepository<HrmDefExamGroupInfo> hrmExamGroupInfo;

        string strMaxNO = string.Empty;
        private const string TableName = "HRM_EmployeeEducation";
        private const string ColumnName = "EmpEduCode";

        public HrmEmployeeEducationsService(IRepository<HrmEmployeeEducation> hrmEmpEduInfo, IRepository<HrmEmployee> hrmEmployee,
            IRepository<CoreAccessCode> accessCodeRepository,
            IRepository<HrmEmployeeOfficialInfo> hrmEmpOffialInfo,
            ICommonService commonService, IRepository<CoreCompany> coreCompanyRepository,
            IRepository<CoreBranch> coreBranchRepository, 
            IRepository<HrmDefDepartment> departmentRepository,
            IRepository<HrmDefDesignation> designationRepository,
            IRepository<HrmDefDegree> hrmDegree, 
            IRepository<HrmDefBoardCountryName> hrmBoardCountryName, 
            IRepository<HrmDefExamTitle> hrmDefexamTitle, 
            IRepository<HrmDefInstitute> hrmDefInstitute, IRepository<HrmDefExamGroupInfo> hrmExamGroupInfo) : base(hrmEmpEduInfo)
        {
            this.hrmEmpEduInfo = hrmEmpEduInfo;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
            this.coreCompanyRepository = coreCompanyRepository;
            this.departmentRepository = departmentRepository;
            this.designationRepository = designationRepository;
            this.hrmDegree = hrmDegree;
            this.coreBranchRepository = coreBranchRepository;
            this.hrmBoardCountryName = hrmBoardCountryName;
            this.hrmDefexamTitle = hrmDefexamTitle;
            this.hrmDefInstitute = hrmDefInstitute;
            this.hrmExamGroupInfo = hrmExamGroupInfo;
            this.hrmEmployee = hrmEmployee;
            this.hrmEmpOffialInfo = hrmEmpOffialInfo;
        }

        public async Task<List<HrmEmployeeEducationsSetupViewModel>> GetAllAsync(string employeeId)
        {
            var data = await (from empEdu in hrmEmpEduInfo.All().AsNoTracking()
                              where empEdu.EmployeeId == employeeId


                              join emp in hrmEmployee.All().AsNoTracking()
                              on empEdu.EmployeeId equals emp.EmployeeId into empEduEmpJoin
                              from emp in empEduEmpJoin.DefaultIfEmpty()



                              join degree in hrmDegree.All().AsNoTracking()
                              on empEdu.DegreeCode equals degree.DegreeCode into empEduDegreeJoin
                              from degree in empEduDegreeJoin.DefaultIfEmpty()

                              join boardCountry in hrmBoardCountryName.All().AsNoTracking()
                              on empEdu.BoardCode equals boardCountry.BoardCode into boardEmpEduJoin
                              from boardCountry in boardEmpEduJoin.DefaultIfEmpty()

                              join examTitle in hrmDefexamTitle.All().AsNoTracking()
                              on empEdu.ExamTitleCode equals examTitle.ExamTitleCode into examTitleEmpEduJoin
                              from examTitle in examTitleEmpEduJoin.DefaultIfEmpty()

                              join institute in hrmDefInstitute.All().AsNoTracking()
                              on empEdu.InstitueCode equals institute.InstituteCode into empEduInstituteJoin
                              from institute in empEduInstituteJoin.DefaultIfEmpty()

                              join examGroup in hrmExamGroupInfo.All().AsNoTracking()
                              on empEdu.GroupCode equals examGroup.GroupCode into empEduGroupJoin
                              from examGroup in empEduGroupJoin.DefaultIfEmpty()

                              select new HrmEmployeeEducationsSetupViewModel
                              {
                                  EmployeeId = emp.EmployeeId,
                                  EmpEduCode = empEdu.EmpEduCode,
                                  DegreeCode = empEdu.DegreeCode,
                                  DegreeName = degree.DegreeName,
                                  ExamTitleName = examTitle.ExamTitleName,
                                  InstituteName = institute.InstituteName,
                                  GroupName = examGroup.GroupName,
                                  BoardName = boardCountry.BoardName,
                                  ResultDivision = empEdu.ResultDivision,
                                  CgpaMarks = empEdu.CgpaMarks,
                                  ScaleOf = empEdu.ScaleOf,
                                  YearofPasssing = empEdu.YearofPasssing,
                                  Dueration = empEdu.Dueration,
                                  DuratioinType = empEdu.DuratioinType,
                              }).ToListAsync();
            return data;
        }




        public async Task<HrmEmployeeEducationsSetupViewModel> GetByIdAsync(string code)
        {
            var data = await (from empEdu in hrmEmpEduInfo.All().AsNoTracking()
                              join empEduComp in coreCompanyRepository.All().AsNoTracking()
                              on empEdu.CompanyCode equals empEduComp.CompanyCode into emdEduCompJoin
                              from empEduComp in emdEduCompJoin.DefaultIfEmpty()
                              where empEdu.EmpEduCode == code
                              join emp in hrmEmployee.All().AsNoTracking()
                              on empEdu.EmployeeId equals emp.EmployeeId into empEduEmpJoin
                              from emp in empEduEmpJoin.DefaultIfEmpty()

                              join coreBra in coreBranchRepository.All().AsNoTracking()
                              on empEdu.BranchCode equals coreBra.BranchCode into empEduBranchJoin
                              from coreBra in empEduBranchJoin.DefaultIfEmpty()

                              join degree in hrmDegree.All().AsNoTracking()
                              on empEdu.DegreeCode equals degree.DegreeCode into empEduDegreeJoin
                              from degree in empEduDegreeJoin.DefaultIfEmpty()

                              join boardCountry in hrmBoardCountryName.All().AsNoTracking()
                              on empEdu.BoardCode equals boardCountry.BoardCode into boardEmpEduJoin
                              from boardCountry in boardEmpEduJoin.DefaultIfEmpty()

                              join examTitle in hrmDefexamTitle.All().AsNoTracking()
                              on empEdu.ExamTitleCode equals examTitle.ExamTitleCode into examTitleEmpEduJoin
                              from examTitle in examTitleEmpEduJoin.DefaultIfEmpty()

                              join institute in hrmDefInstitute.All().AsNoTracking()
                              on empEdu.InstitueCode equals institute.InstituteCode into empEduInstituteJoin
                              from institute in empEduInstituteJoin.DefaultIfEmpty()

                              join examGroup in hrmExamGroupInfo.All().AsNoTracking()
                              on empEdu.GroupCode equals examGroup.GroupCode into empEduGroupJoin
                              from examGroup in empEduGroupJoin.DefaultIfEmpty()

                              join ofEmp in hrmEmpOffialInfo.All().AsNoTracking()
                           on empEdu.EmployeeId equals ofEmp.EmployeeId into eduOffJoin
                              from ofEmp in eduOffJoin.DefaultIfEmpty()

                              join desi in designationRepository.All().AsNoTracking()
                              on ofEmp.DesignationCode equals desi.DesignationCode into edudesiJoin
                              from desi in edudesiJoin.DefaultIfEmpty()

                              join dept in departmentRepository.All().AsNoTracking()
                              on ofEmp.DepartmentCode equals dept.DepartmentCode into eduDeptJoin
                              from dept in eduDeptJoin.DefaultIfEmpty()

                              select new HrmEmployeeEducationsSetupViewModel
                              {
                                  AutoId = empEdu.AutoId,
                                  EmpEduCode = empEdu.EmpEduCode,

                                  DegreeCode = empEdu.DegreeCode,
                                  DegreeName = degree.DegreeName,

                                  ExamTitleCode = empEdu.ExamTitleCode,
                                  ExamTitleName = examTitle.ExamTitleName,

                                  InstitueCode = empEdu.InstitueCode,
                                  InstituteName = institute.InstituteName,

                                  GroupCode = empEdu.GroupCode,
                                  GroupName = examGroup.GroupName,

                                  BoardCode = empEdu.BoardCode,
                                  BoardName = boardCountry.BoardName,

                                  ResultDivision = empEdu.ResultDivision,
                                  CgpaMarks = empEdu.CgpaMarks,
                                  ScaleOf = empEdu.ScaleOf,
                                  YearofPasssing = empEdu.YearofPasssing,
                                  Dueration = empEdu.Dueration,
                                  DuratioinType = empEdu.DuratioinType,
                                  Achievment = empEdu.Achievment,
                                  Remarks = empEdu.Achievment,
                                  Luser = empEdu.Luser,

                                  Lmac = empEdu.Lmac,
                                  Lip = empEdu.Lip,
                                  Ldate = empEdu.Ldate,
                                  ModifyDate = empEdu.ModifyDate,

                                  CompanyCode = empEdu.CompanyCode,
                                  ComapanyName = empEduComp.CompanyName,
                                  BranchCode = empEdu.BranchCode,
                                  BranchName = coreBra.BranchName,
                                  EmployeeId = empEdu.EmployeeId,
                                  EmployeeName = $"{emp.FirstName} {emp.LastName}",

                                  DesignationName = desi.DesignationName ?? "",
                                  DepartmentName = dept.DepartmentName ?? "",

                              }).FirstOrDefaultAsync();
            return data;
        }


        public IEnumerable<CommonSelectModel> SelectionHrmDefEmpEduTypeAsync()
        {

            var data = hrmEmpEduInfo.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.EmpEduCode,
                           Name = x.DuratioinType,
                       });
            return data;
        }


        public async Task<bool> SaveAsync(HrmEmployeeEducationsSetupViewModel entityVM)
        {
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 3);
            await hrmEmpEduInfo.BeginTransactionAsync();
            try
            {

                HrmEmployeeEducation entity = new HrmEmployeeEducation();
                entity.EmpEduCode = strMaxNO;
                entity.CompanyCode = entityVM.CompanyCode ?? string.Empty;
                entity.BranchCode = entityVM?.BranchCode ?? string.Empty;
                entity.EmployeeId = entityVM.EmployeeId ?? string.Empty;
                entity.DegreeCode = entityVM.DegreeCode ?? string.Empty;
                entity.ExamTitleCode = entityVM.ExamTitleCode ?? string.Empty;
                entity.InstitueCode = entityVM.InstitueCode ?? string.Empty;
                entity.BoardCode = entityVM.BoardCode ?? string.Empty;
                entity.GroupCode = entityVM.GroupCode ?? string.Empty;
                entity.ResultDivision = entityVM.ResultDivision ?? string.Empty;
                entity.CgpaMarks = entityVM.CgpaMarks ?? string.Empty;
                entity.ScaleOf = entityVM.ScaleOf ?? string.Empty;
                entity.YearofPasssing = entityVM.YearofPasssing ?? string.Empty;
                entity.Dueration = entityVM.Dueration ?? string.Empty;
                entity.DuratioinType = entityVM.DuratioinType ?? string.Empty;
                entity.Achievment = entityVM.Achievment ?? string.Empty;
                entity.Remarks = entityVM.Remarks ?? string.Empty;
                entity.Luser = entityVM.Luser;
                entity.BranchCode = entityVM.BranchCode ?? string.Empty;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                await hrmEmpEduInfo.AddAsync(entity);
                await hrmEmpEduInfo.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await hrmEmpEduInfo.RollbackTransactionAsync();

                return false;
            }
        }

        public async Task<bool> UpdateAsync(HrmEmployeeEducationsSetupViewModel entityVM)
        {
            await hrmEmpEduInfo.BeginTransactionAsync();
            try
            {

                var entity = await hrmEmpEduInfo.GetByIdAsync(entityVM.EmpEduCode);
                if (entity == null)
                {
                    await hrmEmpEduInfo.RollbackTransactionAsync();
                    return false;
                }
                entity.EmpEduCode = entityVM.EmpEduCode;
                entity.CompanyCode = entityVM.CompanyCode ?? string.Empty;
                entity.EmployeeId = entityVM.EmployeeId ?? string.Empty;
                entity.DegreeCode = entityVM.DegreeCode ?? string.Empty;
                entity.ExamTitleCode = entityVM.ExamTitleCode ?? string.Empty;
                entity.InstitueCode = entityVM.InstitueCode ?? string.Empty;
                entity.BoardCode = entityVM.BoardCode ?? string.Empty;
                entity.GroupCode = entityVM.GroupCode ?? string.Empty;
                entity.ResultDivision = entityVM.ResultDivision ?? string.Empty;
                entity.CgpaMarks = entityVM.CgpaMarks ?? string.Empty;
                entity.ScaleOf = entityVM.ScaleOf ?? string.Empty;
                entity.YearofPasssing = entityVM.YearofPasssing ?? string.Empty;
                entity.CompanyCode = entityVM.CompanyCode ?? string.Empty;
                entity.Dueration = entityVM.Dueration ?? string.Empty;
                entity.DuratioinType = entityVM.DuratioinType ?? string.Empty;
                entity.Achievment = entityVM.Achievment ?? string.Empty;
                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.BranchCode = entityVM.BranchCode ?? string.Empty;
                entity.ModifyDate = DateTime.Now;
                await hrmEmpEduInfo.UpdateAsync(entity);
                await hrmEmpEduInfo.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await hrmEmpEduInfo.RollbackTransactionAsync();
                return false;
            }
        }


        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await hrmEmpEduInfo.All().Where(x => ids.Contains(x.EmpEduCode)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }

            hrmEmpEduInfo.Delete(entity);

            return true;
        }


        public async Task<HrmEmployeeEducationsSetupViewModel> GetEmployeeNameDesDeptByCode(string employeeId)
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

                                    select new HrmEmployeeEducationsSetupViewModel
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

        public async Task<List<HrmEmployeeEducationsSetupViewModel>> GetEmployeeByCompanyCode(string companyCode)
        {
            try
            {


                var result = await (from e in hrmEmployee.All().AsNoTracking()

                                    where (e.CompanyCode == companyCode)

                                    select new HrmEmployeeEducationsSetupViewModel
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

        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await hrmEmpEduInfo.All().AnyAsync(x => x.EmpEduCode == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await hrmEmpEduInfo.All().AnyAsync(x => x.EmployeeId == name);
        }

        public async Task<bool> IsExistAsync(string employeeCode, string typeCode, string degreeCode, string eduCode)
        {
            return await hrmEmpEduInfo.All().AnyAsync(x => x.EmployeeId == employeeCode && x.DegreeCode == degreeCode && x.ExamTitleCode == typeCode && x.EmpEduCode != eduCode);
        }



        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Educational Info Entry" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Educational Info Entry" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Educational Info Entry" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Educational Info Entry" && x.CheckDelete);
        }
        #endregion
    }
}
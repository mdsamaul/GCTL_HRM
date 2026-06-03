using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.EmployeeReferenceInfos;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.EmployeeReferenceInfos
{
    public class EmployeeReferenceInfosService : AppService<HrmEmployeeReferenceInfo>, IEmployeeReferenceInfosService
    {
        #region Service & Rep
        private readonly IRepository<HrmEmployeeReferenceInfo> employeeReferenceInfosRepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;
        private readonly IRepository<CoreCompany> coreCompanyRepository;
        private readonly IRepository<CoreBranch> coreBranchRepository;
        private readonly IRepository<HrmDefDepartment> departmentRepository;
        private readonly IRepository<HrmDefDesignation> designationRepository;
        private readonly IRepository<HrmDefRelationship> relationshipRepository;
        private readonly IRepository<HrmDefNationality> nationalityRepository;
        private readonly IRepository<HrmEmployee> hrmEmployee;
        private readonly IRepository<HrmEmployeeOfficialInfo> hrmEmpOffialInfo;

        string strMaxNO = string.Empty;
        private const string TableName = "HRM_EmployeeReferenceInfo";
        private const string ColumnName = "EmpReferenceID";

        public EmployeeReferenceInfosService(
           IRepository<HrmEmployeeReferenceInfo> employeeReferenceInfosRepository,
           IRepository<CoreAccessCode> accessCodeRepository,
           ICommonService commonService,
           IRepository<CoreCompany> coreCompanyRepository,
           IRepository<CoreBranch> coreBranchRepository,
           IRepository<HrmDefDepartment> departmentRepository,
           IRepository<HrmDefDesignation> designationRepository,
           IRepository<HrmDefRelationship> relationshipRepository,
           IRepository<HrmDefNationality> nationalityRepository,
           IRepository<HrmEmployeeOfficialInfo> hrmEmpOffialInfo,
           IRepository<HrmEmployee> hrmEmployee

           )

: base(employeeReferenceInfosRepository)
        {
            this.employeeReferenceInfosRepository = employeeReferenceInfosRepository;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
            this.coreCompanyRepository = coreCompanyRepository;
            this.coreBranchRepository = coreBranchRepository;
            this.departmentRepository = departmentRepository;
            this.designationRepository = designationRepository;
            this.relationshipRepository = relationshipRepository;
            this.nationalityRepository = nationalityRepository;
            this.hrmEmployee = hrmEmployee;
            this.hrmEmpOffialInfo = hrmEmpOffialInfo;
        }

        #endregion

        #region GetAllAsync

        public async Task<List<EmployeeReferenceInfosSetupViewModel>> GetAllAsync(string employeeId)
        {
            var data = await (from emp in employeeReferenceInfosRepository.All().AsNoTracking()

                       where emp.EmployeeId == employeeId
                       
                       join hrmDef in relationshipRepository.All().AsNoTracking()
                       on emp.RelationId equals hrmDef.RelationshipCode into empEmContactRelation1Join
                       from hrmDef in empEmContactRelation1Join.DefaultIfEmpty()

                       join ofEmp in hrmEmpOffialInfo.All().AsNoTracking()
                       on emp.EmployeeId equals ofEmp.EmployeeId into eduOffJoin
                       from ofEmp in eduOffJoin.DefaultIfEmpty()

                       join desi in designationRepository.All().AsNoTracking()
                       on emp.Designation equals desi.DesignationCode into edudesiJoin
                       from desi in edudesiJoin.DefaultIfEmpty()

                       join HrmNat in nationalityRepository.All().AsNoTracking()
                       on emp.NationalityCode equals HrmNat.NationalityCode into empNationalityJoin
                       from HrmNat in empNationalityJoin.DefaultIfEmpty()

                       select new EmployeeReferenceInfosSetupViewModel
                       {
                           EmpReferenceId = emp.EmpReferenceId,
                           DesignationName = desi.DesignationName ?? "",
                           Nationality = HrmNat.Nationality,
                           ReferenceName = emp.ReferenceName,
                           OrganizationName = emp.OrganizationName,
                           RefAddress = emp.RefAddress,
                           RelationName = hrmDef.Relationship,
                           MobileNumber = emp.MobileNumber,
                           EmployeeId = emp.EmployeeId,
                       }).ToListAsync();
            return data;
        }

        #endregion

        #region GetByIdAsync

        public async Task<EmployeeReferenceInfosSetupViewModel> GetByIdAsync(string id)
        {
            var data = await (from emp in employeeReferenceInfosRepository.All().AsNoTracking()

                       join hrmEmp in hrmEmployee.All().AsNoTracking()
                       on emp.EmployeeId equals hrmEmp.EmployeeId into empEduEmpJoin
                       from hrmEmp in empEduEmpJoin.DefaultIfEmpty()
                  
                       join Hrmemp in coreBranchRepository.All().AsNoTracking()
                       on emp.BranchCode equals Hrmemp.BranchCode into empBranchNameJoin
                       from Hrmemp in empBranchNameJoin.DefaultIfEmpty()
                  
                       join ofEmp in hrmEmpOffialInfo.All().AsNoTracking()
                       on emp.EmployeeId equals ofEmp.EmployeeId into eduOffJoin
                       from ofEmp in eduOffJoin.DefaultIfEmpty()
                  
                       join desi in designationRepository.All().AsNoTracking()
                       on ofEmp.DesignationCode equals desi.DesignationCode into edudesiJoin
                       from desi in edudesiJoin.DefaultIfEmpty()
                  
                       join dept in departmentRepository.All().AsNoTracking()
                       on ofEmp.DepartmentCode equals dept.DepartmentCode into eduDeptJoin
                       from dept in eduDeptJoin.DefaultIfEmpty()
                  
                       join hrmDef in relationshipRepository.All().AsNoTracking()
                  
                       on emp.RelationId equals hrmDef.RelationshipCode into empEmContactRelation1Join
                       from hrmDef in empEmContactRelation1Join.DefaultIfEmpty()
                  
                       join empComp in coreCompanyRepository.All().AsNoTracking()
                           on emp.CompanyCode equals empComp.CompanyCode into empComJoin
                       from empComp in empComJoin.DefaultIfEmpty()
                  
                       join HrmNat in nationalityRepository.All().AsNoTracking()
                       on emp.NationalityCode equals HrmNat.NationalityCode into empNationalityJoin
                       from HrmNat in empNationalityJoin.DefaultIfEmpty()
                  
                       where emp.EmpReferenceId == id
                  
                       select new EmployeeReferenceInfosSetupViewModel
                       {
                           AutoId = emp.AutoId,
                           EmpReferenceId = emp.EmpReferenceId,
                           EmployeeId = emp.EmployeeId,
                           EmployeeName = $"{hrmEmp.FirstName}{hrmEmp.LastName}",
                           DesignationName = desi.DesignationName ?? "",
                           DepartmentName = dept.DepartmentName ?? "",
                           NationalityCode = emp.NationalityCode,
                           Nationality = HrmNat.Nationality,
                           BranchCode = emp.BranchCode,
                           BranchName = Hrmemp.BranchName,
                           CompanyCode = emp.CompanyCode,
                           CompanyName = empComp.CompanyName,
                           ReferenceName = emp.ReferenceName,
                           OrganizationName = emp.OrganizationName,
                           Designation = emp.Designation,
                           RefAddress = emp.RefAddress,
                           RelationId = emp.RelationId,
                           RelationName = hrmDef.Relationship,
                           MobileNumber = emp.MobileNumber,
                           PhoneNumber = emp.PhoneNumber,
                           Fax = emp.Fax,
                           Email = emp.Email,
                           Luser = emp.Luser,
                           Lmac = emp.Lmac,
                           Lip = emp.Lip,
                           Ldate = emp.Ldate,
                           ModifyDate = emp.ModifyDate,
                       }).FirstOrDefaultAsync();

            return data;
        }

        #endregion

        #region SelectionTypeAsync

        public IEnumerable<CommonSelectModel> SelectionReferenceTypeAsync()
        {
            var data = employeeReferenceInfosRepository.All()
               .Select(x => new CommonSelectModel
               {
                   Code = x.EmpReferenceId,
                   Name = x.ReferenceName,
               });
            return data;
        }

        #endregion

        #region SaveAsync

        public async Task<bool> SaveAsync(EmployeeReferenceInfosSetupViewModel entityVM, string CompanyCode)
        {
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 4);
            await employeeReferenceInfosRepository.BeginTransactionAsync();
            try
            {

                HrmEmployeeReferenceInfo entity = new HrmEmployeeReferenceInfo();
                entity.EmpReferenceId = strMaxNO;
                entity.EmployeeId = entityVM.EmployeeId;
                entity.ReferenceName = entityVM.ReferenceName;
                entity.OrganizationName = entityVM.OrganizationName;
                entity.Designation = entityVM.Designation;
                entity.RefAddress = entityVM.RefAddress;
                entity.RelationId = entityVM.RelationId;
                entity.MobileNumber = entityVM.MobileNumber;
                entity.PhoneNumber = entityVM.PhoneNumber;
                entity.Fax = entityVM.Fax;
                entity.Email = entityVM.Email;
                entity.CompanyCode = entityVM.CompanyCode;
                entity.NationalityCode = entityVM.NationalityCode;
                entity.BranchCode = entityVM.BranchCode;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                entity.CompanyCode = CompanyCode;
                entity.UserInfoEmployeeId = entityVM.UserInfoEmployeeId ?? string.Empty;
                await employeeReferenceInfosRepository.AddAsync(entity);
                await employeeReferenceInfosRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await employeeReferenceInfosRepository.RollbackTransactionAsync();

                return false;
            }
        }

        #endregion

        #region UpdateAsync

        public async Task<bool> UpdateAsync(EmployeeReferenceInfosSetupViewModel entityVM)
        {
            await employeeReferenceInfosRepository.BeginTransactionAsync();
            try
            {

                var entity = await employeeReferenceInfosRepository.GetByIdAsync(entityVM.EmpReferenceId);
                if (entity == null)
                {
                    await employeeReferenceInfosRepository.RollbackTransactionAsync();
                    return false;
                }
                entity.EmpReferenceId = entityVM.EmpReferenceId;
                entity.EmployeeId = entityVM.EmployeeId;
                entity.ReferenceName = entityVM.ReferenceName;
                entity.OrganizationName = entityVM.OrganizationName;
                entity.Designation = entityVM.Designation;
                entity.RefAddress = entityVM.RefAddress;
                entity.RelationId = entityVM.RelationId;
                entity.MobileNumber = entityVM.MobileNumber;
                entity.PhoneNumber = entityVM.PhoneNumber;
                entity.Fax = entityVM.Fax;
                entity.Email = entityVM.Email;
                entity.CompanyCode = entityVM.CompanyCode;
                entity.NationalityCode = entityVM.NationalityCode;
                entity.BranchCode = entityVM.BranchCode;
                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.ModifyDate = DateTime.Now;
                await employeeReferenceInfosRepository.UpdateAsync(entity);
                await employeeReferenceInfosRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await employeeReferenceInfosRepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region Delete

        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await employeeReferenceInfosRepository.All().Where(x => ids.Contains(x.EmpReferenceId)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }

            employeeReferenceInfosRepository.Delete(entity);

            return true;
        }

        #endregion

        #region Company, Branch, Department

        public async Task<List<EmployeeReferenceInfosSetupViewModel>> GetEmployeeByCompanyCode(string companyCode)
        {
            try
            {
                var result = await (from e in hrmEmployee.All().AsNoTracking()

                       where (e.CompanyCode == companyCode)
                       select new EmployeeReferenceInfosSetupViewModel
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

        //Branch filter
        public async Task<List<EmployeeReferenceInfosSetupViewModel>> GetComapnyByBranchCode(string companyCode)
        {
            try
            {
                var result = await (
                       from br in coreBranchRepository.All().AsNoTracking()
                       where (br.CompanyCode == companyCode)
                       select new EmployeeReferenceInfosSetupViewModel
                       {
                    
                           BranchCode = br.BranchCode,
                           CoreBranchName = br.BranchName
                    
                       }).ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<EmployeeReferenceInfosSetupViewModel> GetEmployeeNameDesDeptByCode(string employeeId)
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

                            select new EmployeeReferenceInfosSetupViewModel
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

        #endregion

        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await employeeReferenceInfosRepository.All().AnyAsync(x => x.EmpReferenceId == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await employeeReferenceInfosRepository.All().AnyAsync(x => x.EmployeeId == name);
        }

        public async Task<bool> IsExistAsync(string employeeCode, string typeCode, string name, string empReferenceId)
        {
            return await employeeReferenceInfosRepository.All().AnyAsync(x => x.EmployeeId == employeeCode && x.ReferenceName == name && x.EmpReferenceId != empReferenceId);
        }
        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Employee Reference Info" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Employee Reference Info" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Employee Reference Info" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Employee Reference Info" && x.CheckDelete);
        }
        #endregion
    }
}

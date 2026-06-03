using GCTL.Core.Data;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.HrmEmployeeAdditionalInfos;
using GCTL.Core.ViewModels.HrmEmployeeEducations;
using GCTL.Core.ViewModels.HrmEmployeeFamilys;
using GCTL.Data.Models;
using GCTL.Service.BranchesTypeInfo;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HrmEmployeeFamilys
{
    public class HrmEmployeeFamilysService : AppService<HrmEmployeeFamily>, IHrmEmployeeFamilysService
    {
        private readonly IRepository<HrmEmployeeFamily> hrmEmpFamily;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;
        private readonly IRepository<CoreCompany> coreCompanyRepository;
        private readonly IRepository<CoreBranch> coreBranchRepository;
        private readonly IRepository<HrmEmployee> hrmEmployee;
        private readonly IRepository<HrmDefDepartment> departmentRepository;
        private readonly IRepository<HrmDefDesignation> designationRepository;
        private readonly IRepository<HrmDefRelationship> hrmRelationShip;
        private readonly IRepository<HrmDefBloodGroup> bloodGroupRepository;
        private readonly IRepository<HrmDefOccupation> occupationRepository;
        private readonly IRepository<HrmEmployeeOfficialInfo> hrmEmpOffialInfo;

        string strMaxNO = string.Empty;
        private const string TableName = "HRM_EmployeeFamily";
        private const string ColumnName = "EmpFamilyID";

        public HrmEmployeeFamilysService(IRepository<HrmEmployeeFamily> hrmEmpFamily,
            IRepository<HrmEmployee> hrmEmployee,
            IRepository<CoreAccessCode> accessCodeRepository,
            ICommonService commonService, IRepository<CoreCompany> coreCompanyRepository,
            IRepository<CoreBranch> coreBranchRepository,
            IRepository<HrmDefRelationship> hrmRelationShip,
            IRepository<HrmDefBloodGroup> bloodGroupRepository,
            IRepository<HrmDefOccupation> occupationRepository,
            IRepository<HrmDefDepartment> departmentRepository,
            IRepository<HrmEmployeeOfficialInfo> hrmEmpOffialInfo,
            IRepository<HrmDefDesignation> designationRepository) : base(hrmEmpFamily)
        {
            this.hrmEmpFamily = hrmEmpFamily;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
            this.coreCompanyRepository = coreCompanyRepository;
            this.departmentRepository = departmentRepository;
            this.designationRepository = designationRepository;
            this.coreBranchRepository = coreBranchRepository;
            this.hrmEmployee = hrmEmployee;
            this.bloodGroupRepository = bloodGroupRepository;
            this.occupationRepository = occupationRepository;
            this.hrmRelationShip = hrmRelationShip;
            this.hrmEmpOffialInfo = hrmEmpOffialInfo;
        }


        //
        public async Task<List<HrmEmployeeFamilysSetViewModel>> GetEmployeeByCompanyCode(string companyCode)
        {
            try
            {


                var result = await (from e in hrmEmployee.All().AsNoTracking()

                                    where (e.CompanyCode == companyCode)

                                    select new HrmEmployeeFamilysSetViewModel
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
        public async Task<HrmEmployeeFamilysSetViewModel> GetEmployeeNameDesDeptByCode(string employeeId)
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

                                    select new HrmEmployeeFamilysSetViewModel
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


        public async Task<List<HrmEmployeeFamilysSetViewModel>> GetAllAsync(string employeeId)
        {
            var data = await (from empFamily in hrmEmpFamily.All().AsNoTracking()
                              where empFamily.EmployeeId == employeeId

                              join rela in hrmRelationShip.All().AsNoTracking()
                              on empFamily.RelationshipId equals rela.RelationshipCode into empFamilyRelationShipJoin
                              from rela in empFamilyRelationShipJoin.DefaultIfEmpty()

                              join occu in occupationRepository.All().AsNoTracking()
                              on empFamily.OccupationId equals occu.OccupationCode into empFamilyOccupationJoin
                              from occu in empFamilyOccupationJoin.DefaultIfEmpty()
                              select new HrmEmployeeFamilysSetViewModel
                              {
                                  EmpFamilyId = empFamily.EmpFamilyId,
                                  RelationShipName = rela.Relationship,
                                  OccupationName = occu.Occupation,
                                  Name = empFamily.Name,
                                  DateOfBirth = empFamily.DateOfBirth.ToString("dd/MM/yyyy"),
                                  AddressDetails = empFamily.AddressDetails,
                                  Phone = empFamily.Phone,
                                  EmployeeId = empFamily.EmployeeId
                              }).ToListAsync();
            return data;
        }




        public async Task<HrmEmployeeFamilysSetViewModel> GetByIdAsync(string code)
        {
            var data = await (from empFamily in hrmEmpFamily.All().AsNoTracking()
                              join empFamilyComp in coreCompanyRepository.All().AsNoTracking()
                              on empFamily.CompanyCode equals empFamilyComp.CompanyCode into emdEduCompJoin
                              from empFamilyComp in emdEduCompJoin.DefaultIfEmpty()

                              where empFamily.EmpFamilyId == code

                              join emp in hrmEmployee.All().AsNoTracking()
                              on empFamily.EmployeeId equals emp.EmployeeId into empFamilyEmpJoin
                              from emp in empFamilyEmpJoin.DefaultIfEmpty()

                              join coreBra in coreBranchRepository.All().AsNoTracking()
                              on empFamily.BranchCode equals coreBra.BranchCode into empFamilyBranchJoin
                              from coreBra in empFamilyBranchJoin.DefaultIfEmpty()

                              join ofEmp in hrmEmpOffialInfo.All().AsNoTracking()
                                      on empFamily.EmployeeId equals ofEmp.EmployeeId into eduOffJoin
                              from ofEmp in eduOffJoin.DefaultIfEmpty()

                              join desi in designationRepository.All().AsNoTracking()
                              on ofEmp.DesignationCode equals desi.DesignationCode into edudesiJoin
                              from desi in edudesiJoin.DefaultIfEmpty()

                              join dept in departmentRepository.All().AsNoTracking()
                              on ofEmp.DepartmentCode equals dept.DepartmentCode into eduDeptJoin
                              from dept in eduDeptJoin.DefaultIfEmpty()

                              join rela in hrmRelationShip.All().AsNoTracking()
                              on empFamily.RelationshipId equals rela.RelationshipCode into empFamilyRelationShipJoin
                              from rela in empFamilyRelationShipJoin.DefaultIfEmpty()

                              join occu in occupationRepository.All().AsNoTracking()
                              on empFamily.OccupationId equals occu.OccupationCode into empFamilyOccupationJoin
                              from occu in empFamilyOccupationJoin.DefaultIfEmpty()

                              join blood in bloodGroupRepository.All().AsNoTracking()
                              on empFamily.BloodGroupId equals blood.BloodGroupCode into empFamilyBloodJoin
                              from blood in empFamilyBloodJoin.DefaultIfEmpty()

                              select new HrmEmployeeFamilysSetViewModel
                              {
                                  AutoId = empFamily.AutoId,
                                  EmpFamilyId = empFamily.EmpFamilyId,

                                  RelationshipId = empFamily.RelationshipId,
                                  RelationShipName = rela.Relationship,

                                  BloodGroupId = empFamily.BloodGroupId,
                                  BloodGroupName = blood.BloodGroup,

                                  OccupationId = empFamily.OccupationId,
                                  OccupationName = occu.Occupation,
                                  Name = empFamily.Name,
                                  DateOfBirth = empFamily.DateOfBirth.ToString("dd/MM/yyyy"),
                                  AddressDetails = empFamily.AddressDetails,
                                  Phone = empFamily.Phone,
                                  Email = empFamily.Email,
                                  Luser = empFamily.Luser,
                                  Lmac = empFamily.Lmac,
                                  Lip = empFamily.Lip,
                                  Ldate = empFamily.Ldate,
                                  ModifyDate = empFamily.ModifyDate,
                                  CompanyCode = empFamily.CompanyCode,
                                  ComapanyName = empFamilyComp.CompanyName,
                                  BranchCode = empFamily.BranchCode,
                                  BranchName = coreBra.BranchName,
                                  EmployeeId = empFamily.EmployeeId,
                                  EmployeeName = $"{emp.FirstName} {emp.LastName}",

                                  DepartmentName = dept.DepartmentName ?? "",
                                  DesignationName = desi.DesignationName ?? "",

                              }).FirstOrDefaultAsync();
            return data;
        }


        public IEnumerable<CommonSelectModel> SelectionHrmDefEmpFamilyTypeAsync()
        {

            var data = hrmEmpFamily.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.EmpFamilyId,
                           Name = x.Name,
                       });
            return data;
        }


        public async Task<bool> SaveAsync(HrmEmployeeFamilysSetViewModel entityVM, string CompanyCode)
        {
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 3);
            await hrmEmpFamily.BeginTransactionAsync();
            try
            {

                HrmEmployeeFamily entity = new HrmEmployeeFamily();
                entity.EmpFamilyId = strMaxNO;
                entity.CompanyCode = entityVM.CompanyCode ?? string.Empty;
                entity.EmployeeId = entityVM.EmployeeId ?? string.Empty;
                entity.RelationshipId = entityVM.RelationshipId ?? string.Empty;
                entity.Name = entityVM.Name ?? string.Empty;
                entity.OccupationId = entityVM.OccupationId ?? string.Empty;
                entity.BloodGroupId = entityVM.BloodGroupId ?? string.Empty;
                entity.DateOfBirth = entityVM.DateOfBirth.ToDate();
                entity.AddressDetails = entityVM.AddressDetails ?? string.Empty;
                entity.Phone = entityVM.Phone ?? string.Empty;
                entity.Email = entityVM.Email ?? string.Empty;
                entity.Luser = entityVM.Luser;
                entity.BranchCode = entityVM.BranchCode ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.Ldate = DateTime.Now;
                entity.CompanyCode = CompanyCode;
                entity.BranchCode = entityVM?.BranchCode ?? string.Empty;
                entity.UserInfoEmployeeId = entityVM.UserInfoEmployeeId ?? string.Empty;

                await hrmEmpFamily.AddAsync(entity);
                await hrmEmpFamily.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await hrmEmpFamily.RollbackTransactionAsync();

                return false;
            }
        }

        public async Task<bool> UpdateAsync(HrmEmployeeFamilysSetViewModel entityVM)
        {

            await hrmEmpFamily.BeginTransactionAsync();
            try
            {

                var entity = await hrmEmpFamily.GetByIdAsync(entityVM.EmpFamilyId);
                if (entity == null)
                {
                    await hrmEmpFamily.RollbackTransactionAsync();
                    return false;
                }
                entityVM.EmpFamilyId = entity.EmpFamilyId;
                entity.CompanyCode = entityVM.CompanyCode ?? string.Empty;
                entity.EmployeeId = entityVM.EmployeeId ?? string.Empty;
                entity.RelationshipId = entityVM.RelationshipId ?? string.Empty;
                entity.Name = entityVM.Name ?? string.Empty;
                entity.OccupationId = entityVM.OccupationId ?? string.Empty;
                entity.BloodGroupId = entityVM.BloodGroupId ?? string.Empty;
                entity.DateOfBirth = entityVM.DateOfBirth.ToDate();
                entity.AddressDetails = entityVM.AddressDetails ?? string.Empty;
                entity.Phone = entityVM.Phone ?? string.Empty;
                entity.Email = entityVM.Email ?? string.Empty;
                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.BranchCode = entityVM.BranchCode ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.ModifyDate = DateTime.Now;
                await hrmEmpFamily.UpdateAsync(entity);
                await hrmEmpFamily.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await hrmEmpFamily.RollbackTransactionAsync();
                return false;
            }
        }


        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await hrmEmpFamily.All().Where(x => ids.Contains(x.EmpFamilyId)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }

            hrmEmpFamily.Delete(entity);

            return true;
        }


        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await hrmEmpFamily.All().AnyAsync(x => x.EmpFamilyId == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await hrmEmpFamily.All().AnyAsync(x => x.EmployeeId == name);
        }

        public async Task<bool> IsExistAsync(string employeeCode, string typeCode, string name)
        {
            return await hrmEmpFamily.All().AnyAsync(x => x.EmployeeId == employeeCode && x.Name == name && x.EmpFamilyId != typeCode);
        }



        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Family Info Entry" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Family Info Entry" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Family Info Entry" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Family Info Entry" && x.CheckDelete);
        }
        #endregion
    }
}
using GCTL.Core.Data;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.SeparationInfos;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.SeparationInfos
{
    public class SeparationInfosService: AppService<HrmSeparation>, ISeparationInfosService
    {
        private readonly IRepository<HrmSeparation> separationInfosRepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;
        private readonly IRepository<CoreCompany> coreCompanyRepository;
        private readonly IRepository<HrmDefDepartment> departmentRepository;
        private readonly IRepository<HrmDefDesignation> designationRepository;
        private readonly IRepository<HrmDefSeparationType> separationTypeRepository;
        private readonly IRepository<HrmEmployee> hrmEmployee;
        private readonly IRepository<HrmEmployeeOfficialInfo> hrmEmpOffialInfo;
        private readonly IRepository<CoreUserInfo> coreUserInfo;

        string strMaxNO = string.Empty;
        private const string TableName = "HRM_Separation";
        private const string ColumnName = "SeparationId";

        public SeparationInfosService(IRepository<HrmSeparation> separationInfosRepository,
            IRepository<CoreAccessCode> accessCodeRepository,
            ICommonService commonService,
            IRepository<CoreCompany> coreCompanyRepository,
            IRepository<HrmDefDepartment> departmentRepository,
            IRepository<HrmDefDesignation> designationRepository,
            IRepository<HrmDefSeparationType> separationTypeRepository,
            IRepository<HrmEmployeeOfficialInfo> hrmEmpOffialInfo,
            IRepository<HrmEmployee> hrmEmployee,
            IRepository<CoreUserInfo> coreUserInfo

            )
    : base(separationInfosRepository)
        {
            this.separationInfosRepository = separationInfosRepository;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
            this.coreCompanyRepository = coreCompanyRepository;
            this.departmentRepository = departmentRepository;
            this.designationRepository = designationRepository;
            this.separationTypeRepository = separationTypeRepository;
            this.hrmEmployee = hrmEmployee;
            this.hrmEmpOffialInfo = hrmEmpOffialInfo;
            this.coreUserInfo = coreUserInfo;

        }

        public async Task<List<SeparationInfosSetupViewModel>> GetAllAsync()
        {
            var data = await (from empFamily in separationInfosRepository.All().AsNoTracking()

                              join empFamilyComp in coreCompanyRepository.All().AsNoTracking()
                              on empFamily.CompanyCode equals empFamilyComp.CompanyCode into emdEduCompJoin
                              from empFamilyComp in emdEduCompJoin.DefaultIfEmpty()

                              join emp in hrmEmployee.All().AsNoTracking()
                              on empFamily.EmployeeId equals emp.EmployeeId into empEmpJoin
                              from emp in empEmpJoin.DefaultIfEmpty()

                              join ofEmp in hrmEmpOffialInfo.All().AsNoTracking()
                              on empFamily.EmployeeId equals ofEmp.EmployeeId into eduOffJoin
                              from ofEmp in eduOffJoin.DefaultIfEmpty()

                              join desi in designationRepository.All().AsNoTracking()
                              on ofEmp.DesignationCode equals desi.DesignationCode into edudesiJoin
                              from desi in edudesiJoin.DefaultIfEmpty()

                              join dept in departmentRepository.All().AsNoTracking()
                              on ofEmp.DepartmentCode equals dept.DepartmentCode into eduDeptJoin
                              from dept in eduDeptJoin.DefaultIfEmpty()

                              join sep in separationTypeRepository.All().AsNoTracking()
                              on empFamily.SeparationTypeId equals sep.SeparationTypeId into empFamilySepJoin
                              from sep in empFamilySepJoin.DefaultIfEmpty()

                              select new SeparationInfosSetupViewModel
                              {
                                  SeparationCode = empFamily.SeparationCode,
                                  SeparationId = empFamily.SeparationId,
                                  SeparationDate = empFamily.SeparationDate.ToString("dd/MM/yyyy"),
                                  SeparationTypeId = empFamily.SeparationTypeId,
                                  SeparationType = sep.SeparationType,
                                  FinalPayment = empFamily.FinalPayment,
                                  IsPaid = empFamily.IsPaid,
                                  Remark = empFamily.Remark,
                                  RefLetterNo = empFamily.RefLetterNo,
                                  RefLetterDate = empFamily.RefLetterDate.Value.ToString("dd/MM/yyyy"),
                                  Luser = empFamily.Luser,
                                  Lmac = empFamily.Lmac,
                                  Lip = empFamily.Lip,
                                  Ldate = empFamily.Ldate,
                                  ModifyDate = empFamily.ModifyDate,
                                  CompanyCode = empFamily.CompanyCode,
                                  CompanyName = empFamilyComp.CompanyName,
                                  EmployeeId = empFamily.EmployeeId,
                                  EmployeeName = $"{emp.FirstName} {emp.LastName}",
                                  DesignationName = desi.DesignationName ?? "",
                                  DepartmentName = dept.DepartmentName ?? "",

                              }).ToListAsync();
            return data;
        }


        public async Task<SeparationInfosSetupViewModel> GetByIdAsync(string code)
        {
            var data = await (from empFamily in separationInfosRepository.All().AsNoTracking()

                              join empFamilyComp in coreCompanyRepository.All().AsNoTracking()
                              on empFamily.CompanyCode equals empFamilyComp.CompanyCode into emdEduCompJoin
                              from empFamilyComp in emdEduCompJoin.DefaultIfEmpty()

                              where empFamily.SeparationId == code

                              join emp in hrmEmployee.All().AsNoTracking()
                              on empFamily.EmployeeId equals emp.EmployeeId into empEmpJoin
                              from emp in empEmpJoin.DefaultIfEmpty()

                              join ofEmp in hrmEmpOffialInfo.All().AsNoTracking()
                              on empFamily.EmployeeId equals ofEmp.EmployeeId into eduOffJoin
                              from ofEmp in eduOffJoin.DefaultIfEmpty()

                              join desi in designationRepository.All().AsNoTracking()
                              on ofEmp.DesignationCode equals desi.DesignationCode into edudesiJoin
                              from desi in edudesiJoin.DefaultIfEmpty()

                              join dept in departmentRepository.All().AsNoTracking()
                              on ofEmp.DepartmentCode equals dept.DepartmentCode into eduDeptJoin
                              from dept in eduDeptJoin.DefaultIfEmpty()

                              join sep in separationTypeRepository.All().AsNoTracking()
                              on empFamily.SeparationTypeId equals sep.SeparationTypeId into empFamilySepJoin
                              from sep in empFamilySepJoin.DefaultIfEmpty()

                              select new SeparationInfosSetupViewModel
                              {
                                  
                                  SeparationCode = empFamily.SeparationCode,
                                  SeparationId = empFamily.SeparationId,
                                  SeparationDate = empFamily.SeparationDate.ToString("dd/MM/yyyy"),
                                  SeparationTypeId = empFamily.SeparationTypeId,
                                  SeparationType = sep.SeparationType,
                                  FinalPayment = empFamily.FinalPayment,
                                  IsPaid = empFamily.IsPaid,
                                  Remark = empFamily.Remark,
                                  RefLetterNo = empFamily.RefLetterNo,
                                  RefLetterDate = empFamily.RefLetterDate.Value.ToString("dd/MM/yyyy"),
                                  Luser = empFamily.Luser,
                                  Lmac = empFamily.Lmac,
                                  Lip = empFamily.Lip,
                                  Ldate = empFamily.Ldate,
                                  ModifyDate = empFamily.ModifyDate,
                                  CompanyCode = empFamily.CompanyCode,
                                  CompanyName = empFamilyComp.CompanyName,
                                  EmployeeId = empFamily.EmployeeId,
                                  EmployeeName = $"{emp.FirstName} {emp.LastName}",
                                  DesignationName = desi != null ? desi.DesignationName : "",
                                  DepartmentName = dept != null ? dept.DepartmentName : "",

                              }).FirstOrDefaultAsync();
            return data;
        }

        public IEnumerable<CommonSelectModel> SelectionSeparationInfosTypeAsync()
        {
            var data = separationInfosRepository.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.SeparationId,
                           Name = x.Remark,
                       });
            return data;
        }

        public async Task<bool> SaveAsync(SeparationInfosSetupViewModel entityVM)
        {
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 4);
            await separationInfosRepository.BeginTransactionAsync();
            try
            {
                HrmSeparation entity = new HrmSeparation();
                entity.SeparationCode = entityVM.SeparationCode;
                entity.SeparationId = strMaxNO;
                entity.CompanyCode = entityVM.CompanyCode;
                entity.EmployeeId = entityVM.EmployeeId ?? string.Empty;
                entity.SeparationDate = entityVM.SeparationDate.ToDate();
                entity.SeparationTypeId = entityVM.SeparationTypeId ?? string.Empty;
                entity.FinalPayment = entityVM.FinalPayment;
                entity.IsPaid = entityVM.IsPaid ??string.Empty;
                entity.Remark = entityVM.Remark ?? string.Empty;
                entity.RefLetterNo = entityVM.RefLetterNo ?? string.Empty;
                entity.RefLetterDate = entityVM.RefLetterDate.ToDate();
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.Ldate = DateTime.Now;
                await separationInfosRepository.AddAsync(entity);
                await separationInfosRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await separationInfosRepository.RollbackTransactionAsync();

                return false;
            }
        }

        public async Task<bool> UpdateAsync(SeparationInfosSetupViewModel entityVM)
        {
            await separationInfosRepository.BeginTransactionAsync();
            try
            {

                var entity = await separationInfosRepository.GetByIdAsync(entityVM.SeparationId);
                if (entity == null)
                {
                    await separationInfosRepository.RollbackTransactionAsync();
                    return false;
                }
                entity.SeparationCode = entityVM.SeparationCode;
                entity.SeparationId = entityVM.SeparationId;
                entity.CompanyCode = entityVM.CompanyCode;
                entity.EmployeeId = entityVM.EmployeeId ?? string.Empty;
                entity.SeparationDate = entityVM.SeparationDate.ToDate();
                entity.SeparationTypeId = entityVM.SeparationTypeId ?? string.Empty;
                entity.FinalPayment = entityVM.FinalPayment;
                entity.IsPaid = entityVM.IsPaid ?? string.Empty;
                entity.Remark = entityVM.Remark ?? string.Empty;
                entity.RefLetterNo = entityVM.RefLetterNo;
                entity.RefLetterDate = entityVM.RefLetterDate.ToDate();
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.ModifyDate = DateTime.Now;
                await separationInfosRepository.UpdateAsync(entity);


                var ofcInfo = hrmEmpOffialInfo.GetAll().FirstOrDefault(e => e.EmployeeId == entityVM.EmployeeId);

                if (entityVM.SeparationTypeId == "02" )
                {
                    ofcInfo.EmployeeStatus = "02";
                }
                else if (entityVM.SeparationTypeId == "01")
                {
                    ofcInfo.EmployeeStatus = "01";
                }
                else if (entityVM.SeparationTypeId == "03")
                {
                    ofcInfo.EmployeeStatus = "02";
                }

                hrmEmpOffialInfo.Update(ofcInfo);

                


                await separationInfosRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await separationInfosRepository.RollbackTransactionAsync();
                return false;
            }
        }



        #region GetEmployee with Dept, Designation, EmpOfficialInfo

        public async Task<SeparationInfosSetupViewModel> GetEmployeeByCode(string employeeId)
        {
            try
            {
                if (string.IsNullOrEmpty(employeeId)) return null;

                return await (
                    from e in hrmEmployee.All().AsNoTracking()
                    join oi in hrmEmpOffialInfo.All()
                        .Select(x => new { x.EmployeeId, x.DepartmentCode, x.DesignationCode }).AsNoTracking()
                        on e.EmployeeId equals oi.EmployeeId into oiGroup
                    from oi in oiGroup.DefaultIfEmpty()
                    join des in designationRepository.All()
                        .Select(x => new { x.DesignationCode, x.DesignationName }).AsNoTracking()
                        on oi.DesignationCode equals des.DesignationCode into desGroup
                    from des in desGroup.DefaultIfEmpty()
                    join dep in departmentRepository.All()
                        .Select(x => new { x.DepartmentCode, x.DepartmentName }).AsNoTracking()
                        on oi.DepartmentCode equals dep.DepartmentCode into depGroup
                    from dep in depGroup.DefaultIfEmpty()
                    where e.EmployeeId == employeeId
                    select new SeparationInfosSetupViewModel
                    {
                        EmployeeId = e.EmployeeId,
                        EmployeeName = $"{e.FirstName} {e.LastName}",
                        DesignationName = des.DesignationName ?? "",
                        DepartmentName = dep.DepartmentName ?? ""
                    }).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<List<SeparationInfosSetupViewModel>> GetComapnyByCode(string companyCode)
        {
            try
            {
                if (string.IsNullOrEmpty(companyCode)) return new List<SeparationInfosSetupViewModel>();

                return await (
                    from e in hrmEmployee.All().AsNoTracking()
                    where e.CompanyCode == companyCode
                    select new SeparationInfosSetupViewModel
                    {
                        EmployeeId = e.EmployeeId,
                        EmployeeName = $"{e.FirstName} {e.LastName} ({e.EmployeeId})"
                    }).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        #endregion

        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await separationInfosRepository.All().Where(x => ids.Contains(x.SeparationId)).ToListAsync();

            foreach (var item in entity)
            {
                if (item != null)
                {
                    var empOffice = hrmEmpOffialInfo.GetAll().FirstOrDefault(e => e.EmployeeId == item.EmployeeId);

                    if (empOffice != null)
                    {
                        empOffice.EmployeeStatus = "02";
                        hrmEmpOffialInfo.Update(empOffice);

                    }

                }                   
            }

            foreach (var item in entity)
            {
                if (item != null)
                {
                    var userInfo = coreUserInfo.GetAll().FirstOrDefault(e => e.EmployeeId == item.EmployeeId);

                    if (userInfo != null)
                    {
                        userInfo.Status = "1";
                    coreUserInfo.Update(userInfo);
                    }

                }                
            }

            if (!entity.Any())
            {
                return false;
            }

            separationInfosRepository.Delete(entity);

            return true;
        }

        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await separationInfosRepository.All().AnyAsync(x => x.CompanyCode == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await separationInfosRepository.All().AnyAsync(x => x.EmployeeId == name);
        }

        public async Task<bool> IsExistAsync(string employeeCode, string typeCode, string name)
        {
            return await separationInfosRepository.All().AnyAsync(x => x.EmployeeId == employeeCode && x.CompanyCode == name && x.SeparationId != typeCode);
        }
        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Separation Info Entry" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Separation Info Entry" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Separation Info Entry" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Separation Info Entry" && x.CheckDelete);
        }
        #endregion
    }
}

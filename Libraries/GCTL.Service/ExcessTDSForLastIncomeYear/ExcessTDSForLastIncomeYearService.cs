using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.ExcessTDSForLastIncomeYear;

using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;

namespace GCTL.Service.ExcessTDSForLastIncomeYear
{
    public class ExcessTDSForLastIncomeYearService : AppService<HrmPayExcessTdsforLastIncomeYearEntry>, IExcessTDSForLastIncomeYearService
    {
        #region Service & Repository
        private readonly IRepository<HrmPayExcessTdsforLastIncomeYearEntry> excessTDSForLastIncomeYearRepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;
        private readonly IRepository<CoreCompany> coreCompanyRepository;
        private readonly IRepository<CoreBranch> coreBranchRepository;
        private readonly IRepository<HrmEmployee> hrmEmployee;
        private readonly IRepository<HrmEmployeeOfficialInfo> hrmEmpOffialInfo;
        private readonly IRepository<HrmDefDepartment> departmentRepository;
        private readonly IRepository<HrmDefDesignation> designationRepository;
        private readonly IRepository<HrmDefEmpType> emploreeTyperepository;
        private readonly IRepository<HrmDefEmployeeStatus> emploreeEmployeeStatusRepository;
        private readonly IRepository<HrmEisDefEmploymentNature> employmentNatureRepository;
        private readonly IRepository<AccFinancialYear> accFinancialYearRepository;

        string strMaxNO = string.Empty;
        private const string TableName = "HRM_PAY_ExcessTDSForLastIncomeYearEntry";
        private const string ColumnName = "ETDSLIYID";

        public ExcessTDSForLastIncomeYearService(
            IRepository<HrmPayExcessTdsforLastIncomeYearEntry> excessTDSForLastIncomeYearRepository,
            IRepository<CoreAccessCode> accessCodeRepository,
            ICommonService commonService,
            IRepository<CoreCompany> coreCompanyRepository,
            IRepository<CoreBranch> coreBranchRepository,
            IRepository<HrmEmployee> hrmEmployee,
            IRepository<HrmEmployeeOfficialInfo> hrmEmpOffialInfo,
            IRepository<HrmDefDepartment> departmentRepository,
            IRepository<HrmDefDesignation> designationRepository,
            IRepository<HrmDefEmpType> emploreeTyperepository,
            IRepository<HrmDefEmployeeStatus> emploreeEmployeeStatusRepository,
            IRepository<HrmEisDefEmploymentNature> employmentNatureRepository,
            IRepository<AccFinancialYear> accFinancialYearRepository

            )
    : base(excessTDSForLastIncomeYearRepository)
        {
            this.excessTDSForLastIncomeYearRepository = excessTDSForLastIncomeYearRepository;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
            this.coreCompanyRepository = coreCompanyRepository;
            this.coreBranchRepository = coreBranchRepository;
            this.hrmEmployee = hrmEmployee;
            this.hrmEmpOffialInfo = hrmEmpOffialInfo;
            this.departmentRepository = departmentRepository;
            this.designationRepository = designationRepository;
            this.emploreeTyperepository = emploreeTyperepository;
            this.emploreeEmployeeStatusRepository = emploreeEmployeeStatusRepository;
            this.employmentNatureRepository = employmentNatureRepository;
            this.accFinancialYearRepository = accFinancialYearRepository;
        }

        #endregion

        #region GetFilterDataAsync 

        public async Task<ExcessTDSForLastIncomeYearFilterListDto> GetFilterDataAsync(ExcessTDSForLastIncomeYearFilterDto filter)
        {
            try
            {
                var query = from e in hrmEmployee.All()
                            join eoi in hrmEmpOffialInfo.All() on e.EmployeeId equals eoi.EmployeeId
                            join dg in designationRepository.All() on eoi.DesignationCode equals dg.DesignationCode into dg_join
                            from dg in dg_join.DefaultIfEmpty()
                            join dp in departmentRepository.All() on eoi.DepartmentCode equals dp.DepartmentCode into dp_join
                            from dp in dp_join.DefaultIfEmpty()
                            join cb in coreBranchRepository.All() on eoi.BranchCode equals cb.BranchCode into cb_join
                            from cb in cb_join.DefaultIfEmpty()
                            join cc in coreCompanyRepository.All() on eoi.CompanyCode equals cc.CompanyCode into cc_join
                            from cc in cc_join.DefaultIfEmpty()
                            join et in emploreeTyperepository.All() on eoi.EmpTypeCode equals et.EmpTypeCode into et_join
                            from et in et_join.DefaultIfEmpty()
                            join es in emploreeEmployeeStatusRepository.All() on eoi.EmployeeStatus equals es.EmployeeStatusId into es_join
                            from es in es_join.DefaultIfEmpty()
                            join en in employmentNatureRepository.All() on eoi.EmploymentNatureId equals en.EmploymentNatureId into en_join
                            from en in en_join.DefaultIfEmpty()
                            select new
                            {
                                e.EmployeeId,
                                EmployeeName = (e.FirstName ?? "") + " " + (e.LastName ?? ""),
                                eoi.CompanyCode,
                                cc.CompanyName,
                                cb.BranchCode,
                                cb.BranchName,
                                dp.DepartmentCode,
                                dp.DepartmentName,
                                dg.DesignationCode,
                                dg.DesignationName,
                                et.EmpTypeCode,
                                et.EmpTypeName,
                                eoi.EmployeeStatus,
                                StatusName = es.EmployeeStatus,
                                EmploymentNatureId = en.EmploymentNatureId,
                                EmploymentNature = en.EmploymentNature,
                                eoi.JoiningDate,
                                ConfirmDate = eoi.ConfirmeDate // Corrected from ConfirmeDate
                            };

                // Apply filters
                if (filter.CompanyCodes?.Any() == true)
                    query = query.Where(x => x.CompanyCode != null && filter.CompanyCodes.Contains(x.CompanyCode));

                if (filter.BranchCodes?.Any() == true)
                    query = query.Where(x => x.BranchCode != null && filter.BranchCodes.Contains(x.BranchCode));

                if (filter.DepartmentCodes?.Any() == true)
                    query = query.Where(x => x.DepartmentCode != null && filter.DepartmentCodes.Contains(x.DepartmentCode));

                if (filter.DesignationCodes?.Any() == true)
                    query = query.Where(x => x.DesignationCode != null && filter.DesignationCodes.Contains(x.DesignationCode));

                if (filter.EmployeeIDs?.Any() == true)
                    query = query.Where(x => x.EmployeeId != null && filter.EmployeeIDs.Contains(x.EmployeeId));

                if (filter.EmployeeTypeCodes?.Any() == true)
                    query = query.Where(x => x.EmpTypeCode != null && filter.EmployeeTypeCodes.Contains(x.EmpTypeCode));

                if (filter.EmployeeStatuses?.Any() == true)
                    query = query.Where(x => x.EmployeeStatus != null && filter.EmployeeStatuses.Contains(x.EmployeeStatus));

                if (filter.ActivityStatuses?.Any() == true && !filter.ActivityStatuses.Contains("02,01"))
                    query = query.Where(x => x.EmployeeStatus != null && filter.ActivityStatuses.Contains(x.EmployeeStatus));

                if (filter.EmploymentNatureId?.Any() == true)
                {
                    query = query.Where(x => x.EmploymentNatureId != null && filter.EmploymentNatureId.Contains(x.EmploymentNatureId));
                }

                if (filter.EmployeeIDs?.Any() == true)
                {
                    query = query.Where(x => x.EmployeeId != null && filter.EmployeeIDs.Contains(x.EmployeeId));
                }

                if (filter.FromDate.HasValue)
                    query = query.Where(x => x.JoiningDate.HasValue && x.JoiningDate.Value.Date >= filter.FromDate.Value.Date);

                if (filter.ToDate.HasValue)
                    query = query.Where(x => x.JoiningDate.HasValue && x.JoiningDate.Value.Date <= filter.ToDate.Value.Date);

                var result = new ExcessTDSForLastIncomeYearFilterListDto
                {
                    Date = DateTime.Now,
                    Companies = await query
                        .Where(x => x.CompanyCode != null && x.CompanyName != null)
                        .Select(x => new ExcessTDSForLastIncomeYearFilterResultDto { Code = x.CompanyCode, Name = x.CompanyName })
                        .Distinct().ToListAsync(),
                    Branches = await query
                        .Where(x => x.BranchCode != null && x.BranchName != null)
                        .Select(x => new ExcessTDSForLastIncomeYearFilterResultDto { Code = x.BranchCode, Name = x.BranchName })
                        .Distinct().ToListAsync(),
                    Departments = await query
                        .Where(x => x.DepartmentCode != null && x.DepartmentName != null)
                        .Select(x => new ExcessTDSForLastIncomeYearFilterResultDto { Code = x.DepartmentCode, Name = x.DepartmentName })
                        .Distinct().ToListAsync(),
                    Designations = await query
                        .Where(x => x.DesignationCode != null && x.DesignationName != null)
                        .Select(x => new ExcessTDSForLastIncomeYearFilterResultDto { Code = x.DesignationCode, Name = x.DesignationName })
                        .Distinct().ToListAsync(),
                    EmployeeTypes = await query
                        .Where(x => x.EmpTypeCode != null && x.EmpTypeName != null)
                        .Select(x => new ExcessTDSForLastIncomeYearFilterResultDto { Code = x.EmpTypeCode, Name = x.EmpTypeName })
                        .Distinct().ToListAsync(),
                    EmploymentNature = await query
                        .Where(x => x.EmploymentNatureId != null && x.EmploymentNature != null)
                        .Select(x => new ExcessTDSForLastIncomeYearFilterResultDto { Code = x.EmploymentNatureId, Name = x.EmploymentNature })
                        .Distinct().ToListAsync(),
                    ActivityStatuses = await query
                        .Where(x => x.EmployeeStatus != null && x.StatusName != null)
                        .Select(x => new ExcessTDSForLastIncomeYearFilterResultDto { Code = x.EmployeeStatus, Name = x.StatusName })
                        .Distinct().ToListAsync(),
                    Employees = await query.Where(x => x.EmployeeId != null && x.EmployeeName != null)
                        .Select(x => new ExcessTDSForLastIncomeYearFilterResultDto
                        {
                            Code = x.EmployeeId,
                            Name = x.EmployeeName,
                            EmpId = x.EmployeeId,
                            EmployeeId = x.EmployeeId,
                            CompanyName = x.CompanyName ?? "",
                            BranchName = x.BranchName ?? "",
                            DepartmentName = x.DepartmentName ?? "",
                            DesignationName = x.DesignationName ?? "",
                            EmployeeType = x.EmpTypeName ?? "",
                            EmploymentNature = x.EmploymentNature ?? "",
                            JoiningDate = x.JoiningDate,
                            ConfirmeDate = x.ConfirmDate
                        }).Distinct().ToListAsync(),
                };

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetFilterDataAsync: {ex.Message}");
                throw;
            }
        }


        #endregion

        #region PopulateEmployee
        public async Task<object> GetPopulateEmployee(string employeeId)
        {
            var data = await (from fooAllo in excessTDSForLastIncomeYearRepository.All().AsNoTracking()

                              where fooAllo.EmployeeId == employeeId

                              join ofEmp in hrmEmpOffialInfo.All().AsNoTracking()
                              on fooAllo.EmployeeId equals ofEmp.EmployeeId into eduOffJoin
                              from ofEmp in eduOffJoin.DefaultIfEmpty()

                              join desi in designationRepository.All().AsNoTracking()
                              on ofEmp.DesignationCode equals desi.DesignationCode into edudesiJoin
                              from desi in edudesiJoin.DefaultIfEmpty()

                              join dept in departmentRepository.All().AsNoTracking()
                              on ofEmp.DepartmentCode equals dept.DepartmentCode into eduDeptJoin
                              from dept in eduDeptJoin.DefaultIfEmpty()

                              join hrmcom in coreCompanyRepository.All().AsNoTracking()
                              on fooAllo.CompanyCode equals hrmcom.CompanyCode into empComNameJoin
                              from hrmcom in empComNameJoin.DefaultIfEmpty()

                              join emp in hrmEmployee.All().AsNoTracking()
                              on fooAllo.EmployeeId equals emp.EmployeeId into empEduEmpJoin
                              from emp in empEduEmpJoin.DefaultIfEmpty()

                              join bra in coreBranchRepository.All().AsNoTracking()
                              on ofEmp.BranchCode equals bra.BranchCode into empBranchNameJoin
                              from bra in empBranchNameJoin.DefaultIfEmpty()

                              join et in emploreeTyperepository.All().AsNoTracking()
                              on ofEmp.EmpTypeCode equals et.EmpTypeCode into etGroup
                              from et in etGroup.DefaultIfEmpty()

                              join es in emploreeEmployeeStatusRepository.All().AsNoTracking()
                              on ofEmp.EmployeeStatus equals es.EmployeeStatusId into esGroup
                              from es in esGroup.DefaultIfEmpty()

                              join empnes in employmentNatureRepository.All().AsNoTracking()
                              on ofEmp.EmploymentNatureId equals empnes.EmploymentNatureId into empnesGroup
                              from empnes in empnesGroup.DefaultIfEmpty()

                              select new
                              {
                                  Code = fooAllo.EmployeeId,
                                  Name = $"{emp.FirstName} {emp.LastName}",
                                  Designation = desi.DesignationName,
                                  Branch = bra.BranchName,
                                  Company = hrmcom.CompanyName,
                                  Department = dept.DepartmentName,
                                  EmployeeType = et.EmpTypeName,
                                  JoiningDate = ofEmp.JoiningDate.HasValue ? ofEmp.JoiningDate.Value.ToString() : "",
                                  EmploymentNature = empnes.EmploymentNature,
                                  MobileAllowance = ofEmp.MobileAllowance,
                                  EmployeeStatus = es.EmployeeStatus,
                                  ConfirmeDate = ofEmp.ConfirmeDate.HasValue ? ofEmp.ConfirmeDate.Value.ToString("dd/MM/yyyy") : "",

                              }).Distinct().ToListAsync();

            return data;
        }

        #endregion

        #region SaveAsync

        public async Task<bool> SaveAsync(ExcessTDSForLastIncomeYearSetupViewModel entityVM, string CompanyCode)
        {
            await excessTDSForLastIncomeYearRepository.BeginTransactionAsync();
            try
            {
                // Get next available ID
                var allRecords = await excessTDSForLastIncomeYearRepository.GetAllAsync();
                int nextId = (allRecords.Any() ? allRecords.Select(r => int.Parse(r.Etdsliyid)).Max() : 0) + 1;

                if (entityVM.AutoId == 0)
                {
                    // New record(s)
                    var employees = entityVM.SelectedEmployeeIds?.Count > 0 ? entityVM.SelectedEmployeeIds : new List<string> { entityVM.EmployeeId };

                    foreach (var empId in employees)
                    {

                        bool isExist = excessTDSForLastIncomeYearRepository.GetAll().Any(x => x.EmployeeId == empId && x.FinancialCodeNo == entityVM.FinancialCodeNo && x.EffectiveDate == entityVM.EffectiveDate && x.Tdsamount == entityVM.Tdsamount);
                        if (!isExist)
                        {
                            var data = new HrmPayExcessTdsforLastIncomeYearEntry
                            {
                                Etdsliyid = nextId.ToString().PadLeft(8, '0'),
                                EmployeeId = empId ?? string.Empty,
                                FinancialCodeNo = entityVM.FinancialCodeNo ?? string.Empty,
                                Tdsamount = entityVM.Tdsamount,
                                EffectiveDate = entityVM.EffectiveDate,
                                IsfullAmountAdjust = entityVM.IsfullAmountAdjust,
                                Remark = entityVM.Remark ?? string.Empty,
                                SalaryMonth = entityVM.SalaryMonth ?? string.Empty,
                                SalaryYear = entityVM.SalaryYear ?? string.Empty,
                                ApprovedStatus = entityVM.ApprovedStatus ?? string.Empty,
                                Luser = entityVM.Luser ?? string.Empty,
                                Ldate = DateTime.Now,
                                Lip = entityVM.Lip ?? string.Empty,
                                Lmac = entityVM.Lmac ?? string.Empty,
                                CompanyCode = CompanyCode ?? ""
                            };

                            await excessTDSForLastIncomeYearRepository.AddAsync(data);
                            nextId++;
                        }
                    }
                }
                else
                {
                    // Update existing record
                    var existingEntity = await excessTDSForLastIncomeYearRepository.GetByIdAsync(entityVM.Etdsliyid);
                    if (existingEntity != null)
                    {
                        existingEntity.EmployeeId = entityVM.EmployeeId ?? string.Empty;
                        existingEntity.FinancialCodeNo = entityVM.FinancialCodeNo ?? string.Empty;
                        existingEntity.EffectiveDate = entityVM.EffectiveDate;
                        existingEntity.Tdsamount = entityVM.Tdsamount;
                        existingEntity.IsfullAmountAdjust = entityVM.IsfullAmountAdjust;
                        existingEntity.Remark = entityVM.Remark;
                        existingEntity.SalaryMonth = entityVM.SalaryMonth ?? string.Empty;
                        existingEntity.SalaryYear = entityVM.SalaryYear ?? string.Empty;
                        existingEntity.ApprovedStatus = entityVM.ApprovedStatus ?? string.Empty;
                        existingEntity.Luser = entityVM.Luser ?? string.Empty;
                        existingEntity.Ldate = DateTime.Now;
                        existingEntity.Lip = entityVM.Lip ?? string.Empty;
                        existingEntity.Lmac = entityVM.Lmac ?? string.Empty;
                        existingEntity.CompanyCode = entityVM.CompanyCode ?? string.Empty;

                        await excessTDSForLastIncomeYearRepository.UpdateAsync(existingEntity);
                    }
                }

                await excessTDSForLastIncomeYearRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error message: {ex.Message}");
                await excessTDSForLastIncomeYearRepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region UpdateAsync

        public async Task<bool> UpdateAsync(ExcessTDSForLastIncomeYearSetupViewModel entityVM)
        {
            await excessTDSForLastIncomeYearRepository.BeginTransactionAsync();

            try
            {
                // Make sure ID is received
                if (string.IsNullOrWhiteSpace(entityVM.Etdsliyid))
                {
                    await excessTDSForLastIncomeYearRepository.RollbackTransactionAsync();
                    return false;
                }

                var entity = await excessTDSForLastIncomeYearRepository.GetByIdAsync(entityVM.Etdsliyid);

                if (entity == null)
                {
                    await excessTDSForLastIncomeYearRepository.RollbackTransactionAsync();
                    return false;
                }

                bool isExist = excessTDSForLastIncomeYearRepository.GetAll().Any(x => x.EmployeeId == entityVM.EmployeeId && x.FinancialCodeNo == entityVM.FinancialCodeNo && x.EffectiveDate == entityVM.EffectiveDate && x.Tdsamount == entityVM.Tdsamount && x.AutoId != entityVM.AutoId);
                if (isExist)
                {
                    return false;
                }
                // Update fields

                entity.EmployeeId = entityVM.EmployeeId ?? string.Empty;
                entity.FinancialCodeNo = entityVM.FinancialCodeNo ?? string.Empty;
                entity.EffectiveDate = entityVM.EffectiveDate;
                entity.Tdsamount = entityVM.Tdsamount;
                entity.IsfullAmountAdjust = entityVM.IsfullAmountAdjust;
                entity.Remark = entityVM.Remark;
                entity.SalaryMonth = entityVM.SalaryMonth ?? string.Empty; ;
                entity.SalaryYear = entityVM.SalaryYear ?? string.Empty; ;
                entity.ApprovedStatus = entityVM.ApprovedStatus ?? string.Empty; ;
                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.ModifyDate = DateTime.Now;

                await excessTDSForLastIncomeYearRepository.UpdateAsync(entity);
                await excessTDSForLastIncomeYearRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                await excessTDSForLastIncomeYearRepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region GetAllAsync

        public async Task<List<ExcessTDSForLastIncomeYearSetupViewModel>> GetAllAsync()
        {

            var data = await (from inc in excessTDSForLastIncomeYearRepository.All().AsNoTracking()
                              join emp in hrmEmployee.All().AsNoTracking()
                              on inc.EmployeeId equals emp.EmployeeId into empJoin
                              from emp in empJoin.DefaultIfEmpty()
                              join empOff in hrmEmpOffialInfo.All().AsNoTracking()
                              on emp.EmployeeId equals empOff.EmployeeId into empOffJoin
                              from empOff in empOffJoin.DefaultIfEmpty()
                              join dept in departmentRepository.All().AsNoTracking()
                              on empOff.DepartmentCode equals dept.DepartmentCode into deptJoin
                              from dept in deptJoin.DefaultIfEmpty()
                              join desig in designationRepository.All().AsNoTracking()
                              on empOff.DesignationCode equals desig.DesignationCode into desigJoin
                              from desig in desigJoin.DefaultIfEmpty()
                              join afy in accFinancialYearRepository.All().AsNoTracking()
                              on inc.FinancialCodeNo equals afy.FinancialCodeNo into afyJoin
                              from afy in afyJoin.DefaultIfEmpty()

                              select new ExcessTDSForLastIncomeYearSetupViewModel
                              {
                                  AutoId = inc.AutoId,
                                  Etdsliyid = inc.Etdsliyid,
                                  EmployeeId = emp.EmployeeId,
                                  EmployeeName = $"{emp.FirstName} {emp.LastName}",
                                  DesignationName = desig.DesignationName,
                                  FinancialCodeNo = inc.FinancialCodeNo,
                                  Name = afy.Name,
                                  Tdsamount = inc.Tdsamount,
                                  EffectiveDate = inc.EffectiveDate,
                                  IsfullAmountAdjust = inc.IsfullAmountAdjust == "true" || inc.IsfullAmountAdjust == "1" ? "Yes" : "No",
                                  //IsfullAmountAdjust = inc.IsfullAmountAdjust,
                                  Remark = inc.Remark,

                              }).ToListAsync();
            return data;
        }

        #endregion

        #region GetByIdAsync
        public async Task<ExcessTDSForLastIncomeYearSetupViewModel> GetByIdAsync(string code)
        {
            var data = await (from inc in excessTDSForLastIncomeYearRepository.All().AsNoTracking()

                              where inc.Etdsliyid == code

                              select new ExcessTDSForLastIncomeYearSetupViewModel
                              {
                                  AutoId = inc.AutoId,
                                  Etdsliyid = inc.Etdsliyid,
                                  FinancialCodeNo = inc.FinancialCodeNo,
                                  EffectiveDate = inc.EffectiveDate,
                                  Tdsamount = inc.Tdsamount,
                                  IsfullAmountAdjust = inc.IsfullAmountAdjust,
                                  Remark = inc.Remark,
                                  SalaryMonth = inc.SalaryMonth,
                                  SalaryYear = inc.SalaryYear,
                                  Ldate = inc.Ldate,
                                  ModifyDate = inc.ModifyDate,
                                  EmployeeId = inc.EmployeeId,

                              }).FirstOrDefaultAsync();
            return data;
        }

        #endregion

        #region SelectionTypeAsync

        public IEnumerable<CommonSelectModel> SelectionExcessTDSForLastIncomeYearAsync()
        {
            var data = excessTDSForLastIncomeYearRepository.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.Etdsliyid,
                           Name = x.Remark,
                       });
            return data;
        }

        #endregion

        #region DeleteTab
        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await excessTDSForLastIncomeYearRepository.All().Where(x => ids.Contains(x.Etdsliyid)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }

            excessTDSForLastIncomeYearRepository.Delete(entity);

            return true;
        }

        #endregion

        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await excessTDSForLastIncomeYearRepository.All().AnyAsync(x => x.Etdsliyid == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await excessTDSForLastIncomeYearRepository.All().AnyAsync(x => x.EmployeeId == name);
        }

        public async Task<bool> IsExistAsync(string employeeCode, string fyear, DateTime efctive, decimal amount)
        {
            var result = excessTDSForLastIncomeYearRepository.All().FirstOrDefault(e => e.EmployeeId == employeeCode);

            return await excessTDSForLastIncomeYearRepository.All().AnyAsync(x => x.EmployeeId == employeeCode && x.FinancialCodeNo == fyear && x.EffectiveDate == efctive && x.Tdsamount == amount);

        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Excess TDS For Last Income Year" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Excess TDS For Last Income Year" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Excess TDS For Last Income Year" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Excess TDS For Last Income Year" && x.CheckDelete);
        }

        #endregion
    }
}

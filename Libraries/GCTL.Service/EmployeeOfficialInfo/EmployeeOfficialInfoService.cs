using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HrmEmployeeOfficialInfo;
using GCTL.Data.Models;
using GCTL.Service.DeleteHistories;
using Microsoft.EntityFrameworkCore;

namespace GCTL.Service.EmployeeOfficialInfo
{
    public class EmployeeOfficialInfoService : AppService<HrmEmployeeOfficialInfo>, IEmployeeOfficialInfoService
    {
        #region Repositories
        private readonly IRepository<HrmEmployeeOfficialInfo> _hrmEmployeeOfficialInfoRepository;
        private readonly IRepository<CoreAccessCode> _coreAccessCodeRepository;
        private readonly IRepository<HrmEmployee> _employeeRepository;
        private readonly IRepository<CoreCompany> _companyRepository;
        private readonly IRepository<HrmDefDepartment> _departmentRepository;
        private readonly IRepository<HmsShift> _shiftRepository;
        private readonly IRepository<CoreBranch> _branchRepository;
        private readonly IRepository<HrmDefDivision> _divisionRepository;
        private readonly IRepository<HrmDefDesignation> _designationRepository;
        private readonly IRepository<HrmDefEmployeeStatus> _employeeStatusRepository;
        private readonly IRepository<HrmDefGrade> _gradeRepository;
        private readonly IRepository<HrmEisDefEmploymentNature> _employmentNature;
        private readonly IRepository<HrmDefEmpType> _empTypeRepository;
        private readonly IRepository<CorePeriodInfo> corePeriodInforepository;
        private readonly IRepository<HrmAtdHoliday> atdHolidayRepository;
        private readonly IRepository<HrmAtdCompanyWeekEnd> companyWeekEndRepository;
        private readonly IDeleteHistoryService deleteHistoryService;

        string strMaxNO = string.Empty;
        private const string TableName = "HRM_EmployeeOfficialInfo";
        private const string ColumnName = "EmployeeID";

        public EmployeeOfficialInfoService(
            IRepository<HrmEmployeeOfficialInfo> hrmEmployeeOfficialInfoRepository,
            IRepository<CoreAccessCode> coreAccessCodeRepository,
            IRepository<HrmEmployee> employeeRepository,
            IRepository<CoreCompany> companyRepository,
            IRepository<HrmDefDepartment> departmentRepository,
            IRepository<HrmDefEmpType> defEmpTypeRepository,
            IRepository<HmsShift> shiftRepository,
            IRepository<CoreBranch> branchRepository,
            IRepository<HrmDefDivision> divisionRepository,
            IRepository<HrmDefDesignation> designationRepository,
            IRepository<HrmDefEmployeeStatus> employeeStatusRepository,
            IRepository<HrmDefGrade> gradeRepository,
            IRepository<HrmEisDefEmploymentNature> employmentNature,
            IRepository<HrmDefEmpType> empTypeRepository,
            IRepository<CorePeriodInfo> corePeriodInforepository,
            IRepository<HrmAtdHoliday> atdHolidayRepository,
            IRepository<HrmAtdCompanyWeekEnd> companyWeekEndRepository,
            IDeleteHistoryService deleteHistoryService

            )
    : base(hrmEmployeeOfficialInfoRepository)
        {
            _hrmEmployeeOfficialInfoRepository = hrmEmployeeOfficialInfoRepository;
            _coreAccessCodeRepository = coreAccessCodeRepository;
            _employeeRepository = employeeRepository;
            _companyRepository = companyRepository;
            _departmentRepository = departmentRepository;
            _empTypeRepository = defEmpTypeRepository;
            _shiftRepository = shiftRepository;
            _branchRepository = branchRepository;
            _divisionRepository = divisionRepository;
            _designationRepository = designationRepository;
            _employeeStatusRepository = employeeStatusRepository;
            _gradeRepository = gradeRepository;
            _employmentNature = employmentNature;
            _empTypeRepository = empTypeRepository;
            this.corePeriodInforepository = corePeriodInforepository;
            this.atdHolidayRepository = atdHolidayRepository;
            this.companyWeekEndRepository = companyWeekEndRepository;
            this.deleteHistoryService = deleteHistoryService;
        }

        #endregion

        #region Permissions
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await _coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Employee Information System" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await _coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Employee Information System" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await _coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Employee Information System" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await _coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Employee Information System" && x.CheckDelete);
        }

        #endregion

        #region GetAllAsync

        public async Task<List<HrmEmployeeOfficialInfoSetupViewModel>> GetAllAsync()
        {
            var data = await (
                from oi in _hrmEmployeeOfficialInfoRepository.All()
                                        .AsNoTracking()
                                        .OrderByDescending(x => x.AutoId)

                join emp in _employeeRepository.All().AsNoTracking()
                    on oi.EmployeeId equals emp.EmployeeId into empGroup
                from emp in empGroup.DefaultIfEmpty()

                join dep in _departmentRepository.All().AsNoTracking()
                    on oi.DepartmentCode equals dep.DepartmentCode into depGroup
                from dep in depGroup.DefaultIfEmpty()

                join des in _designationRepository.All().AsNoTracking()
                    on oi.DesignationCode equals des.DesignationCode into desGroup
                from des in desGroup.DefaultIfEmpty()

                select new HrmEmployeeOfficialInfoSetupViewModel
                {
                    AutoId = oi.AutoId,
                    EmployeeId = oi.EmployeeId,
                    FirstName = emp != null ? emp.FirstName : "",
                    LastName = emp != null ? emp.LastName : "",
                    DepartmentName = dep != null ? dep.DepartmentName : "",
                    DesignationName = des != null ? des.DesignationName : "",
                    JoiningDate = oi.JoiningDate,
                    Ldate = oi.Ldate,
                    ModifyDate = oi.ModifyDate,
                }).ToListAsync();

            return data;
        }

        #endregion

        #region GetByIdAsync
        public async Task<HrmEmployeeOfficialInfoSetupViewModel> GetByIdAsync(string code)
        {
            var data = await (from oi in _hrmEmployeeOfficialInfoRepository.All().AsNoTracking()

                              join emp in _employeeRepository.All().AsNoTracking() on oi.EmployeeId 
                              equals emp.EmployeeId into empGroup
                              from emp in empGroup.DefaultIfEmpty()

                              join dep in _departmentRepository.All().AsNoTracking() on oi.DepartmentCode 
                              equals dep.DepartmentCode into depGroup
                              from dep in depGroup.DefaultIfEmpty()

                              join des in _designationRepository.All().AsNoTracking() on oi.DesignationCode 
                              equals des.DesignationCode into desGroup
                              from des in desGroup.DefaultIfEmpty()

                              where oi.EmployeeId == code

                              select new HrmEmployeeOfficialInfoSetupViewModel
                              {
                                  AutoId = oi.AutoId,
                                  EmployeeId = oi.EmployeeId,
                                  FullName = $"{emp.FirstName} {emp.LastName}",
                                  OfficialInfoCompanyCode = oi.CompanyCode,
                                  OfficialInfoBranchCode = oi.BranchCode,
                                  DivisionCode = oi.DivisionCode,
                                  DepartmentCode = oi.DepartmentCode,
                                  DepartmentName = dep.DepartmentName,
                                  DesignationCode = oi.DesignationCode,
                                  DesignationName = des.DesignationName,
                                  EmpTypeCode = oi.EmpTypeCode,
                                  GradeCode = oi.GradeCode,
                                  EmploymentNatureId = oi.EmploymentNatureId,
                                  GrossSalary = oi.GrossSalary,
                                  CurrencyCode = oi.CurrencyCode,
                                  PaymentPeriodId = oi.PaymentPeriodId,
                                  DisbursementMethodId = oi.DisbursementMethodId,
                                  ShiftCode = oi.ShiftCode,
                                  EmployeeStatus = oi.EmployeeStatus,
                                  ReportingTo = oi.ReportingTo,
                                  Hod = oi.Hod,
                                  MobileNo = oi.MobileNo,
                                  Email = oi.Email,
                                  AppointmentLetterNo = oi.AppointmentLetterNo,
                                  AppointmentLetterDate = oi.AppointmentLetterDate.HasValue ? oi.AppointmentLetterDate.Value : null,
                                  JoiningDate = oi.JoiningDate.HasValue ? oi.JoiningDate.Value : null,
                                  JoiningSalary = oi.JoiningSalary,
                                  ProbationPeriodType = oi.ProbationPeriodType,
                                  ProbationPeriod = oi.ProbationPeriod,
                                  ConfirmeDate = oi.ConfirmeDate.HasValue ? oi.ConfirmeDate.Value : null,
                                  CompanyCodeSession = oi.CompanyCodeSession,
                                  UserInfoEmployeeId = oi.UserInfoEmployeeId,
                                  StepNoId = oi.StepNoId,
                                  TecnicalSkillTypeId = oi.TecnicalSkillTypeId,
                                  SalaryScaleId = oi.SalaryScaleId,
                                  ContractEndDate = oi.ContractEndDate.HasValue ? oi.ContractEndDate.Value : null,
                                  SectionCode = oi.SectionCode,
                                  LineCode = oi.LineCode,
                                  AttendenceId = oi.AttendenceId,
                                  IsExpatriate = oi.IsExpatriate,
                                  ExpatriateBasicSalary = oi.ExpatriateBasicSalary,
                                  ExpatriateHouseRent = oi.ExpatriateHouseRent,
                                  ExpatriateConveyance = oi.ExpatriateConveyance,
                                  ExpatriateMedical = oi.ExpatriateMedical,
                                  Lfa = oi.Lfa,
                                  MobileAllowance = oi.MobileAllowance,
                                  ConfirmationRefNo = oi.ConfirmationRefNo,
                                  ProbationEffectDate = oi.ProbationEffectDate.HasValue ? oi.ProbationEffectDate.Value : null,
                                  ModeOfPaymentInBankPercentage = oi.ModeOfPaymentInBankPercentage,
                                  IsLunchBilEligible = oi.IsLunchBilEligible,
                                  IsExtraDutyEligible = oi.IsExtraDutyEligible,
                                  IsOverTimeEligible = oi.IsOverTimeEligible,
                                  IsGovtHolidayEligible = oi.IsGovtHolidayEligible,
                                  IsAttendanceBonusEligible = oi.IsAttendanceBonusEligible,
                                  PayId = oi.PayId,

                                  Ldate = oi.Ldate,
                                  ModifyDate = oi.ModifyDate,
                                  Luser = oi.Luser,
                                  Lip = oi.Lip,
                                  Lmac = oi.Lmac

                              }).FirstOrDefaultAsync();
            return data;
        }

        #endregion

        #region SaveAsync
        public async Task<bool> SaveAsync(HrmEmployeeOfficialInfoSetupViewModel model)
        {
            await _hrmEmployeeOfficialInfoRepository.BeginTransactionAsync();
            try
            {
                HrmEmployeeOfficialInfo officialInfo = new HrmEmployeeOfficialInfo();
                officialInfo.EmployeeId = model.EmployeeId;
                officialInfo.CompanyCode = model.OfficialInfoCompanyCode;
                officialInfo.BranchCode = model.OfficialInfoBranchCode ?? string.Empty;
                officialInfo.DivisionCode = model.DivisionCode;
                officialInfo.DepartmentCode = model.DepartmentCode;
                officialInfo.DesignationCode = model.DesignationCode;
                officialInfo.EmpTypeCode = model.EmpTypeCode ?? string.Empty;
                officialInfo.GradeCode = model.GradeCode ?? string.Empty;
                officialInfo.EmploymentNatureId = model.EmploymentNatureId ?? string.Empty;
                officialInfo.GrossSalary = model.GrossSalary;
                officialInfo.CurrencyCode = model.CurrencyCode ?? string.Empty;
                officialInfo.PaymentPeriodId = model.PaymentPeriodId ?? string.Empty;
                officialInfo.DisbursementMethodId = model.DisbursementMethodId ?? string.Empty;
                officialInfo.ShiftCode = model.ShiftCode ?? string.Empty;
                officialInfo.EmployeeStatus = model.EmployeeStatus ?? string.Empty;
                officialInfo.ReportingTo = model.ReportingTo ?? string.Empty;
                officialInfo.Hod = model.Hod ?? string.Empty;
                officialInfo.MobileNo = model.MobileNo ?? string.Empty;
                officialInfo.Email = model.Email ?? string.Empty;
                officialInfo.AppointmentLetterNo = model.AppointmentLetterNo ?? string.Empty;
                officialInfo.AppointmentLetterDate = model.AppointmentLetterDate;
                officialInfo.JoiningDate = model.JoiningDate;
                officialInfo.JoiningSalary = model.JoiningSalary;
                officialInfo.ProbationPeriodType = model.ProbationPeriodType ?? string.Empty;
                officialInfo.ProbationPeriod = model.ProbationPeriod ?? string.Empty;
                officialInfo.ConfirmeDate = model.ConfirmeDate;
                officialInfo.CompanyCodeSession = model.CompanyCodeSession ?? string.Empty;
                officialInfo.StepNoId = model.StepNoId;
                officialInfo.TecnicalSkillTypeId = model.TecnicalSkillTypeId;
                officialInfo.SalaryScaleId = model.SalaryScaleId;
                officialInfo.ContractEndDate = model.ContractEndDate;
                officialInfo.SectionCode = model.SectionCode;
                officialInfo.LineCode = model.LineCode;
                officialInfo.AttendenceId = model.AttendenceId;
                officialInfo.IsExpatriate = model.IsExpatriate;
                officialInfo.ExpatriateBasicSalary = model.ExpatriateBasicSalary;
                officialInfo.ExpatriateHouseRent = model.ExpatriateHouseRent;
                officialInfo.ExpatriateConveyance = model.ExpatriateConveyance;
                officialInfo.ExpatriateMedical = model.ExpatriateMedical;
                officialInfo.Lfa = model.Lfa;
                officialInfo.MobileAllowance = model.MobileAllowance;
                officialInfo.ConfirmationRefNo = model.ConfirmationRefNo;
                officialInfo.ProbationEffectDate = model.ProbationEffectDate;
                officialInfo.ModeOfPaymentInBankPercentage = model.ModeOfPaymentInBankPercentage;
                officialInfo.IsLunchBilEligible = model.IsLunchBilEligible;
                officialInfo.IsExtraDutyEligible = model.IsExtraDutyEligible;
                officialInfo.IsOverTimeEligible = model.IsOverTimeEligible;
                officialInfo.IsGovtHolidayEligible = model.IsGovtHolidayEligible;
                officialInfo.IsAttendanceBonusEligible = model.IsAttendanceBonusEligible;
                officialInfo.PayId = model.PayId;
                officialInfo.UserInfoEmployeeId = model.UserInfoEmployeeId;

                officialInfo.Ldate = DateTime.Now;
                officialInfo.Luser = model.Luser;
                officialInfo.Lip = model.Lip;
                officialInfo.Lmac = model.Lmac;

                await _hrmEmployeeOfficialInfoRepository.AddAsync(officialInfo);
                await _hrmEmployeeOfficialInfoRepository.CommitTransactionAsync();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error! {ex.Message}");
                await _hrmEmployeeOfficialInfoRepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region UpdateAsync
        public async Task<bool> UpdateAsync(HrmEmployeeOfficialInfoSetupViewModel model)
        {
            await _hrmEmployeeOfficialInfoRepository.BeginTransactionAsync();
            //var joiningDate = model.JoiningDate.ToDate();
            try
            {
                var result = await _hrmEmployeeOfficialInfoRepository.GetByIdAsync(model.AutoId);
                if(result == null)
                {
                    await _hrmEmployeeOfficialInfoRepository.RollbackTransactionAsync();
                    return false;
                }

                result.EmployeeId = model.EmployeeId;
                result.CompanyCode = model.OfficialInfoCompanyCode;
                result.BranchCode = model.OfficialInfoBranchCode ?? string.Empty;
                result.DivisionCode = model.DivisionCode;
                result.DepartmentCode = model.DepartmentCode;
                result.DesignationCode = model.DesignationCode;
                result.EmpTypeCode = model.EmpTypeCode ?? string.Empty;
                result.GradeCode = model.GradeCode ?? string.Empty;
                result.EmploymentNatureId = model.EmploymentNatureId ?? string.Empty;
                result.GrossSalary = model.GrossSalary;
                result.CurrencyCode = model.CurrencyCode ?? string.Empty;
                result.PaymentPeriodId = model.PaymentPeriodId ?? string.Empty;
                result.DisbursementMethodId = model.DisbursementMethodId ?? string.Empty;
                result.ShiftCode = model.ShiftCode ?? string.Empty;
                result.EmployeeStatus = model.EmployeeStatus ?? string.Empty;
                result.ReportingTo = model.ReportingTo ?? string.Empty;
                result.Hod = model.Hod ?? string.Empty;
                result.MobileNo = model.MobileNo ?? string.Empty;
                result.Email = model.Email ?? string.Empty;
                result.AppointmentLetterNo = model.AppointmentLetterNo ?? string.Empty;
                result.AppointmentLetterDate = model.AppointmentLetterDate;
                result.JoiningDate = model.JoiningDate;
                result.JoiningSalary = model.JoiningSalary;
                result.ProbationPeriodType = model.ProbationPeriodType ?? string.Empty;
                result.ProbationPeriod = model.ProbationPeriod ?? string.Empty;
                result.ConfirmeDate = model.ConfirmeDate;
                result.CompanyCodeSession = model.CompanyCodeSession ?? string.Empty;
                result.StepNoId = model.StepNoId;
                result.TecnicalSkillTypeId = model.TecnicalSkillTypeId;
                result.SalaryScaleId = model.SalaryScaleId;
                result.ContractEndDate = model.ContractEndDate;
                result.SectionCode = model.SectionCode;
                result.LineCode = model.LineCode;
                result.AttendenceId = model.AttendenceId;
                result.IsExpatriate = model.IsExpatriate;
                result.ExpatriateBasicSalary = model.ExpatriateBasicSalary;
                result.ExpatriateHouseRent = model.ExpatriateHouseRent;
                result.ExpatriateConveyance = model.ExpatriateConveyance;
                result.ExpatriateMedical = model.ExpatriateMedical;
                result.Lfa = model.Lfa;
                result.MobileAllowance = model.MobileAllowance;
                result.ConfirmationRefNo = model.ConfirmationRefNo;
                result.ProbationEffectDate = model.ProbationEffectDate;
                result.ModeOfPaymentInBankPercentage = model.ModeOfPaymentInBankPercentage;
                result.IsLunchBilEligible = model.IsLunchBilEligible;
                result.IsExtraDutyEligible = model.IsExtraDutyEligible;
                result.IsOverTimeEligible = model.IsOverTimeEligible;
                result.IsGovtHolidayEligible = model.IsGovtHolidayEligible;
                result.IsAttendanceBonusEligible = model.IsAttendanceBonusEligible;
                result.PayId = model.PayId;
                result.UserInfoEmployeeId = model.UserInfoEmployeeId;

                result.Luser = model.Luser;
                result.Lip = model.Lip;
                result.Lmac = model.Lmac;
                result.ModifyDate = DateTime.Now; ;

                await _hrmEmployeeOfficialInfoRepository.UpdateAsync(result);
                await _hrmEmployeeOfficialInfoRepository.CommitTransactionAsync();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error! {ex.Message}");
                await _hrmEmployeeOfficialInfoRepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region GetEmployeeDetailsByCode
        public async Task<HrmEmployeeOfficialInfoSetupViewModel> GetEmployeeDetailsByCode(string code)
        {
            var result = await (from emp in _employeeRepository.All().AsNoTracking()

                                join oi in _hrmEmployeeOfficialInfoRepository.All().AsNoTracking() on emp.EmployeeId
                                equals oi.EmployeeId into oiGroup
                                from oi in oiGroup.DefaultIfEmpty()

                                join dep in _departmentRepository.All().AsNoTracking() on oi.DepartmentCode
                                equals dep.DepartmentCode into depGroup
                                from dep in depGroup.DefaultIfEmpty()

                                join des in _designationRepository.All().AsNoTracking() on oi.DesignationCode
                                equals des.DesignationCode into desGroup
                                from des in desGroup.DefaultIfEmpty()

                                where emp.EmployeeId == code

                                select new HrmEmployeeOfficialInfoSetupViewModel
                                {
                                    EmployeeId = emp.EmployeeId,
                                    FullName = $"{emp.FirstName} {emp.LastName}",
                                    DesignationName = des.DesignationName,
                                    DepartmentName = dep.DepartmentName,

                                }).FirstOrDefaultAsync();
            return result;
        }
        #endregion

        #region DeleteTab

        public async Task<bool> DeleteTab(List<string> ids, DeleteHistoryViewModel model)
        {
            var entity = _hrmEmployeeOfficialInfoRepository.All().Where(x => ids.Contains(x.EmployeeId)).ToList();

            if (!entity.Any())
            {
                return false;
            }

            _hrmEmployeeOfficialInfoRepository.Delete(entity);

            model.tableName = TableName;
            await deleteHistoryService.LogDeletedRecordsAsync(
                entity, model
            );

            return true;

        }

        #endregion

        #region IsExistsByCode
        public async Task<bool> IsExistsByCode(string code)
        {
            return await _hrmEmployeeOfficialInfoRepository.All().AnyAsync(x => x.EmployeeId == code);
        }

        #endregion

        #region EmployeeSelection

        public IEnumerable<CommonSelectModel> EmployeeSelection()
        {
            return _employeeRepository.All()
                .Select(x => new CommonSelectModel
                {
                    Code = x.EmployeeId,
                    Name = $"{x.FirstName} {x.LastName} ({x.EmployeeId})"
                });
        }

        #endregion

        #region GetHolidayAndWeekendAsync

        public async Task<List<HolidayWeekenderOFFDto>> GetHolidayAndWeekendAsync(int year)
        {
            var result = new List<HolidayWeekenderOFFDto>();

            // 🎯 Holidays
            var holidays = await atdHolidayRepository.All()
                .Where(x => x.FromDate.Year == year)
                .ToListAsync();

            foreach (var h in holidays)
            {
                result.Add(new HolidayWeekenderOFFDto
                {
                    Date = h.FromDate.ToString("yyyy-MM-dd"),
                    Title = h.HolidayName,
                    Type = "holiday"
                });
            }

            // 🎯 Weekend (Company-wise) - Filter by year
            var weekends = await companyWeekEndRepository.All()
                .Where(x => x.EffectiveDate.Year == year)
                .ToListAsync();

            // Split comma-separated weekend days
            var weekendDays = weekends
                .SelectMany(x => x.Weekend.Split(','))
                .Select(d => d.Trim())
                .Distinct()
                .ToList();

            // Whole year loop
            var start = new DateTime(year, 1, 1);
            var end = new DateTime(year, 12, 31);

            for (var date = start; date <= end; date = date.AddDays(1))
            {
                if (weekendDays.Contains(date.DayOfWeek.ToString()))
                {
                    result.Add(new HolidayWeekenderOFFDto
                    {
                        Date = date.ToString("yyyy-MM-dd"),
                        Title = $"Weekend",
                        Type = "weekend"
                    });
                }
            }

            return result;
        }

        #endregion

    }
}

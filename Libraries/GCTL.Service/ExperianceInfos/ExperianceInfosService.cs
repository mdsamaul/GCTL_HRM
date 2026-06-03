using DocumentFormat.OpenXml.Bibliography;
using GCTL.Core.Data;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.ExperianceInfos;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.ExperianceInfos
{
    public class ExperianceInfosService: AppService<HrmEmployeeExp>, IExperianceInfosService
    {
        #region Service & Rep
        private readonly IRepository<HrmEmployeeExp> experianceInfosRepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;
        private readonly IRepository<CoreCompany> coreCompanyRepository;
        private readonly IRepository<HrmDefDepartment> departmentRepository;
        private readonly IRepository<HrmDefDesignation> designationRepository;
        private readonly IRepository<HrmDefCompanyInfo> companyRepository;
        private readonly IRepository<HrmDefEmpType> empTypeRepository;
        private readonly IRepository<HrmEmployee> hrmEmployee;
        private readonly IRepository<HrmEmployeeOfficialInfo> hrmEmpOffialInfo;
        private readonly IRepository<HrmAtdHoliday> atdHolidayRepository;
        private readonly IRepository<HrmAtdCompanyWeekEnd> companyWeekEndRepository;

        string strMaxNO = string.Empty;
        private const string TableName = "HRM_EmployeeExp";
        private const string ColumnName = "EmpExpID";

        public ExperianceInfosService(
            IRepository<HrmEmployeeExp> experianceInfosRepository,
            IRepository<CoreAccessCode> accessCodeRepository, 
            ICommonService commonService,
            IRepository<CoreCompany> coreCompanyRepository, 
            IRepository<HrmDefDepartment> departmentRepository,
            IRepository<HrmDefDesignation> designationRepository,
            IRepository<HrmDefCompanyInfo> companyRepository,
            IRepository<HrmDefEmpType> empTypeRepository,
            IRepository<HrmEmployee> hrmEmployee,
            IRepository<HrmEmployeeOfficialInfo> hrmEmpOffialInfo,
            IRepository<HrmAtdHoliday> atdHolidayRepository,
            IRepository<HrmAtdCompanyWeekEnd> companyWeekEndRepository

            )
    : base(experianceInfosRepository)
        {
            this.experianceInfosRepository = experianceInfosRepository;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
            this.coreCompanyRepository = coreCompanyRepository;
            this.departmentRepository = departmentRepository;
            this.designationRepository = designationRepository;
            this.companyRepository = companyRepository;
            this.empTypeRepository = empTypeRepository;
            this.hrmEmployee = hrmEmployee;
            this.hrmEmpOffialInfo = hrmEmpOffialInfo;
            this.atdHolidayRepository = atdHolidayRepository;
            this.companyWeekEndRepository = companyWeekEndRepository;
        }

        #endregion

        #region GetAllAsync

        public async Task<List<ExperianceInfosSetupViewModel>> GetAllAsync(string employeeId)
        {
            var data = await (from empEdu in experianceInfosRepository.All().AsNoTracking()

                              where empEdu.EmployeeId == employeeId

                              join ofEmp in hrmEmpOffialInfo.All().AsNoTracking()
                              on empEdu.EmployeeId equals ofEmp.EmployeeId into eduOffJoin
                              from ofEmp in eduOffJoin.DefaultIfEmpty()

                              join desi in designationRepository.All().AsNoTracking()
                              on empEdu.DesignationId equals desi.DesignationCode into edudesiJoin
                              from desi in edudesiJoin.DefaultIfEmpty()

                              join hrmcom in companyRepository.All().AsNoTracking()
                              on empEdu.CompanyNameId equals hrmcom.CompanyNameId into empComNameJoin
                              from hrmcom in empComNameJoin.DefaultIfEmpty()

                              select new ExperianceInfosSetupViewModel
                              {
                                  EmpExpId = empEdu.EmpExpId,
                                  CompanyName1 = hrmcom.CompanyName,
                                  Responsibilities = empEdu.Responsibilities,
                                  FromDate = empEdu.FromDate.ToString("dd/MM/yyyy"),
                                  ToDate = empEdu.ToDate.ToString("dd/MM/yyyy"),
                                  Salary = empEdu.Salary,
                                  EmployeeId=empEdu.EmployeeId,
                                  DesignationName = desi.DesignationName ?? "",

                                  Experience = string.Format("{0:F2} Years",
                                  (empEdu.ToDate - empEdu.FromDate).Days / 365.0 +
                                  ((empEdu.ToDate - empEdu.FromDate).Days % 365) / 365.0),

                                  //Experience = string.Format("{0} Years, {1} Months, {2} Days, {3} Hours",
                                  //(empEdu.ToDate - empEdu.FromDate).Days / 365,
                                  //((empEdu.ToDate - empEdu.FromDate).Days % 365) / 30,
                                  //((empEdu.ToDate - empEdu.FromDate).Days % 365) % 30,
                                  //(empEdu.ToDate - empEdu.FromDate).Hours)

                              }).ToListAsync();
            return data;
        }

        #endregion

        #region GetByIdAsync

        public async Task<ExperianceInfosSetupViewModel> GetByIdAsync(string code)
        {
            var data = await (from empEdu in experianceInfosRepository.All().AsNoTracking()

                              join empEduComp in coreCompanyRepository.All().AsNoTracking()
                              on empEdu.CompanyCode equals empEduComp.CompanyCode into emdEduCompJoin
                              from empEduComp in emdEduCompJoin.DefaultIfEmpty()

                              where empEdu.EmpExpId == code

                              join emp in hrmEmployee.All().AsNoTracking()
                              on empEdu.EmployeeId equals emp.EmployeeId into empEduEmpJoin
                              from emp in empEduEmpJoin.DefaultIfEmpty()

                              join ofEmp in hrmEmpOffialInfo.All().AsNoTracking()
                              on empEdu.EmployeeId equals ofEmp.EmployeeId into eduOffJoin
                              from ofEmp in eduOffJoin.DefaultIfEmpty()
                               
                              join desi in designationRepository.All().AsNoTracking()
                              on ofEmp.DesignationCode equals desi.DesignationCode into edudesiJoin
                              from desi in edudesiJoin.DefaultIfEmpty()

                              join dept in departmentRepository.All().AsNoTracking()
                              on ofEmp.DepartmentCode equals dept.DepartmentCode into eduDeptJoin
                              from dept in eduDeptJoin.DefaultIfEmpty()

                              join hrmcom in companyRepository.All().AsNoTracking()
                              on empEdu.CompanyNameId equals hrmcom.CompanyNameId into empComNameJoin
                              from hrmcom in empComNameJoin.DefaultIfEmpty()

                              join hrmjob in empTypeRepository.All().AsNoTracking()
                              on empEdu.JobNatureId equals hrmjob.EmpTypeCode into empJobNatureJoin
                              from hrmjob in empJobNatureJoin.DefaultIfEmpty()

                              select new ExperianceInfosSetupViewModel
                              {
                                  AutoId = empEdu.AutoId,
                                  EmpExpId = empEdu.EmpExpId,
                                  BusinessType = empEdu.BusinessType,
                                  CompanyNameId = empEdu.CompanyNameId,
                                  CompanyName1 = hrmcom.CompanyName,
                                  Address = empEdu.Address,
                                  DepartmentId = empEdu.DepartmentId,
                                  DepartmentName = dept.DepartmentName,
                                  DesignationId = empEdu.DesignationId,
                                  DesignationName = desi.DesignationName,
                                  Responsibilities = empEdu.Responsibilities,
                                  JobNatureId = empEdu.JobNatureId,
                                  FromDate = empEdu.FromDate.ToString("yyyy/MM/dd"),
                                  ToDate = empEdu.ToDate.ToString("yyyy/MM/dd"),
                                  Salary = empEdu.Salary,
                                  Remarks = empEdu.Remarks,
                                  Luser = empEdu.Luser,

                                  Lmac = empEdu.Lmac,
                                  Lip = empEdu.Lip,
                                  Ldate = empEdu.Ldate,
                                  ModifyDate = empEdu.ModifyDate,

                                  CompanyCode = empEdu.CompanyCode,
                                  CompanyName = empEduComp.CompanyName,
                                  EmployeeId = empEdu.EmployeeId,
                                  EmployeeName = $"{emp.FirstName} {emp.LastName}",

                                  Experience = string.Format("{0:F2} Years",
                                  (empEdu.ToDate - empEdu.FromDate).Days / 365.0 +
                                  ((empEdu.ToDate - empEdu.FromDate).Days % 365) / 365.0),

                                  //Experience = string.Format("{0} Years, {1} Months, {2} Days, {3} Hours",
                                  //(empEdu.ToDate - empEdu.FromDate).Days / 365,
                                  //((empEdu.ToDate - empEdu.FromDate).Days % 365) / 30,
                                  //((empEdu.ToDate - empEdu.FromDate).Days % 365) % 30,
                                  //(empEdu.ToDate - empEdu.FromDate).Hours)

                              }).FirstOrDefaultAsync();
            return data;
        }

        #endregion

        #region SelectionTypeAsync

        public IEnumerable<CommonSelectModel> SelectionExperianceInfosTypeAsync()
        {
            var data = experianceInfosRepository.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.EmpExpId,
                           Name = x.BusinessType,
                       });
            return data;
        }

        #endregion

        #region SaveAsync

        public async Task<bool> SaveAsync(ExperianceInfosSetupViewModel entityVM, string CompanyCode)
        {
            commonService.FindMaxNo(ref strMaxNO, ColumnName, TableName, 4);
            await experianceInfosRepository.BeginTransactionAsync();
            try
            {
                HrmEmployeeExp entity = new HrmEmployeeExp();
                entity.EmpExpId = strMaxNO;
                entity.CompanyCode = entityVM.CompanyCode;
                entity.EmployeeId = entityVM.EmployeeId;
                entity.CompanyNameId = entityVM.CompanyNameId;
                entity.BusinessType = entityVM.BusinessType;
                entity.Address = entityVM.Address;
                entity.DepartmentId = entityVM.DepartmentId;
                entity.DesignationId = entityVM.DesignationId;
                entity.Responsibilities = entityVM.Responsibilities;
                entity.JobNatureId = entityVM.JobNatureId;
                entity.FromDate = DateTime.ParseExact(entityVM.FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture); //entityVM.FromDate.ToDate();
                entity.ToDate = DateTime.ParseExact(entityVM.ToDate, "yyyy-MM-dd", CultureInfo.InvariantCulture); // entityVM.ToDate.ToDate();
                entity.Salary = entityVM.Salary;
                entity.Remarks = entityVM.Remarks;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.Ldate = DateTime.Now;
                entity.CompanyCode = CompanyCode;
                entity.UserInfoEmployeeId = entityVM.UserInfoEmployeeId;
                await experianceInfosRepository.AddAsync(entity);
                await experianceInfosRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"error message {ex.Message}");
                await experianceInfosRepository.RollbackTransactionAsync();

                return false;
            }
        }

        #endregion

        #region UpdateAsync

        public async Task<bool> UpdateAsync(ExperianceInfosSetupViewModel entityVM)
        {
            await experianceInfosRepository.BeginTransactionAsync();
            try
            {
                var entity = await experianceInfosRepository.GetByIdAsync(entityVM.EmpExpId);
                if (entity == null)
                {
                    await experianceInfosRepository.RollbackTransactionAsync();
                    return false;
                }
                entity.EmpExpId = entityVM.EmpExpId;
                entity.CompanyCode = entityVM.CompanyCode;
                entity.EmployeeId = entityVM.EmployeeId;
                entity.CompanyNameId = entityVM.CompanyNameId;
                entity.BusinessType = entityVM.BusinessType;
                entity.Address = entityVM.Address;
                entity.DepartmentId = entityVM.DepartmentId;
                entity.DesignationId = entityVM.DesignationId;
                entity.Responsibilities = entityVM.Responsibilities;
                entity.JobNatureId = entityVM.JobNatureId;
                entity.FromDate = DateTime.ParseExact(entityVM.FromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture); // entityVM.FromDate.ToDate();
                entity.ToDate = DateTime.ParseExact(entityVM.ToDate, "yyyy-MM-dd", CultureInfo.InvariantCulture); //entityVM.ToDate.ToDate();
                entity.Salary = entityVM.Salary;
                entity.Remarks = entityVM.Remarks;
                entity.Luser = entityVM.Luser;
                entity.Lip = entityVM.Lip;
                entity.Lmac = entityVM.Lmac;
                entity.ModifyDate = DateTime.Now;
                await experianceInfosRepository.UpdateAsync(entity);
                await experianceInfosRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred : {ex.Message}");
                await experianceInfosRepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region Delete

        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await experianceInfosRepository.All().Where(x => ids.Contains(x.EmpExpId)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }

            experianceInfosRepository.Delete(entity);

            return true;
        }

        #endregion

        #region Company, Branch, Department

        public async Task<List<ExperianceInfosSetupViewModel>> GetEmployeeByCompanyCode(string companyCode)
        {
            try
            {
                var result = await (from e in hrmEmployee.All().AsNoTracking()

                             where (e.CompanyCode == companyCode)
                             select new ExperianceInfosSetupViewModel
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

        public async Task<ExperianceInfosSetupViewModel> GetEmployeeNameDesDeptByCode(string employeeId)
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

                                    select new ExperianceInfosSetupViewModel
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
            return await experianceInfosRepository.All().AnyAsync(x => x.CompanyNameId == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await experianceInfosRepository.All().AnyAsync(x => x.EmployeeId == name);
        }

        public async Task<bool> IsExistAsync(string employeeCode, string typeCode, string name)
        {
            return await experianceInfosRepository.All().AnyAsync(x => x.EmployeeId == employeeCode && x.CompanyNameId == name && x.EmpExpId != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Experiance Info" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Experiance Info" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Experiance Info" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Experiance Info" && x.CheckDelete);
        }
        #endregion

        #region GetHolidayAndWeekendAsync

        public async Task<List<HolidayWeekenderEXPDto>> GetHolidayAndWeekendAsync(int year)
        {
            var result = new List<HolidayWeekenderEXPDto>();

            // 🎯 Holidays
            var holidays = await atdHolidayRepository.All()
                .Where(x => x.FromDate.Year == year)
                .ToListAsync();

            foreach (var h in holidays)
            {
                result.Add(new HolidayWeekenderEXPDto
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
                    result.Add(new HolidayWeekenderEXPDto
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

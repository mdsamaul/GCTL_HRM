using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.ProbationPeriodExtension;
using GCTL.Data.Models;
using GCTL.Service.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace GCTL.Service.ProbationPeriodExtension
{
    public class ProbationPeriodExtensionService : AppService<HrmDefProbationPeriodExtension>, IProbationPeriodExtensionService
    {
        #region Service & repository
        private readonly IRepository<HrmDefProbationPeriodExtension> probationPeriodExtensionrepository;
        private readonly IRepository<CorePeriodInfo> corePeriodInforepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly ICommonService commonService;
        private readonly IRepository<CoreCompany> coreCompanyRepository;
        private readonly IRepository<HrmDefDepartment> departmentRepository;
        private readonly IRepository<HrmDefDesignation> designationRepository;
        private readonly IRepository<HrmDefSeparationType> separationTypeRepository;
        private readonly IRepository<HrmEmployee> hrmEmployee;
        private readonly IRepository<HrmEmployeeOfficialInfo> hrmEmpOffialInfo;
        private readonly IRepository<CoreUserInfo> coreUserInfo;
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        string strMaxNO = string.Empty;

        private const string TableName = "HRM_Def_ProbationPeriodExtension";
        private const string ColumnName = "PPEID";

        public IConfiguration Configuration { get; }

        public ProbationPeriodExtensionService(
            IRepository<HrmDefProbationPeriodExtension> probationPeriodExtensionrepository,
            IRepository<CorePeriodInfo> corePeriodInforepository,
             IRepository<CoreAccessCode> accessCodeRepository,
            ICommonService commonService,
            IRepository<CoreCompany> coreCompanyRepository,
            IRepository<HrmDefDepartment> departmentRepository,
            IRepository<HrmDefDesignation> designationRepository,
            IRepository<HrmDefSeparationType> separationTypeRepository,
            IRepository<HrmEmployeeOfficialInfo> hrmEmpOffialInfo,
            IRepository<HrmEmployee> hrmEmployee,
            IRepository<CoreUserInfo> coreUserInfo,

            IConfiguration configuration

            )
    : base(probationPeriodExtensionrepository)
        {
            this.probationPeriodExtensionrepository = probationPeriodExtensionrepository;
            this.corePeriodInforepository = corePeriodInforepository;
            this.accessCodeRepository = accessCodeRepository;
            this.commonService = commonService;
            this.coreCompanyRepository = coreCompanyRepository;
            this.departmentRepository = departmentRepository;
            this.designationRepository = designationRepository;
            this.separationTypeRepository = separationTypeRepository;
            this.hrmEmployee = hrmEmployee;
            this.hrmEmpOffialInfo = hrmEmpOffialInfo;
            this.coreUserInfo = coreUserInfo;
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("ApplicationDbConnection");
        }

        #endregion

        #region GetProbationExtensionDataAsync
        public async Task<ProbationExtensionResultViewModel> GetProbationExtensionDataAsync(string employeeId, string companyCode)
        {
            try
            {
                var fullList = new List<ProbationExtensionViewModel>();

                using (var conn = new SqlConnection(_connectionString))
                using (var cmd = new SqlCommand("ProcProbationPeriodExtensionEntry", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EmployeeId", (object)employeeId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CompanyCode", (object)companyCode ?? DBNull.Value);

                    await conn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var joiningDateValue = reader["JoiningDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["JoiningDate"]);
                            var contractEndDateValue = reader["ContractEndDate"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(reader["ContractEndDate"]);
                            fullList.Add(new ProbationExtensionViewModel
                            {
                                EmployeeID = reader["EmployeeID"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                CompanyCode = reader["CompanyCode"].ToString(),
                                CompanyName = reader["CompanyName"].ToString(),
                                DesignationName = reader["DesignationName"].ToString(),
                                DepartmentName = reader["DepartmentName"].ToString(),
                                JoiningDate = joiningDateValue,
                                showJoiningDate = joiningDateValue?.ToString("dd/MM/yyyy"),
                                GrossSalary = reader["GrossSalary"] as decimal?,
                                ProbationPeriod = reader["ProbationPeriod"].ToString(),
                                ContractEndDate = contractEndDateValue,
                                showContractEndDate = contractEndDateValue?.ToString("dd/MM/yyyy"),
                                DurationSinceJoining = reader["DurationSinceJoining"].ToString(),

                            });
                        }
                    }
                }

                // Get unique companies and employees
                var companyList = fullList
                    .GroupBy(x => new { x.CompanyCode, x.CompanyName })
                    .Select(g => new CompanyInfo1
                    {
                        CompanyCode = g.Key.CompanyCode,
                        CompanyName = g.Key.CompanyName
                    }).ToList();

                var employeeList = fullList
                    .GroupBy(x => new { x.EmployeeID, x.FullName })
                    .Select(g => new EmployeeInfo
                    {
                        EmployeeID = g.Key.EmployeeID,
                        FullName = g.Key.FullName
                    }).ToList();

                return new ProbationExtensionResultViewModel
                {
                    FullList = fullList,
                    CompanyList = companyList,
                    EmployeeList = employeeList
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetching probation extension data", ex);
            }
        }

        #endregion

        #region GetAllAsync

        public async Task<List<ProbationPeriodExtensionGetAll>> GetAllAsync()
        {
            var data = await (
                from pro in probationPeriodExtensionrepository.All().AsNoTracking()

                join emp in hrmEmployee.All().AsNoTracking()
                on pro.EmployeeId equals emp.EmployeeId into empJoin
                from emp in empJoin.DefaultIfEmpty()

                join perod in corePeriodInforepository.All().AsNoTracking()
                on pro.PeriodInfoId equals perod.PeriodInfoId into perodJoin
                from perod in perodJoin.DefaultIfEmpty()

                select new ProbationPeriodExtensionGetAll
                {
                    // pro, perod,
                    AutoId = pro.AutoId,
                    Ppeid = pro.Ppeid,
                    EmployeeId = emp.EmployeeId,
                    EmployeeName = $"{emp.FirstName} {emp.LastName}",
                    Wef = pro.Wef == null ? null : ((DateTime)pro.Wef).ToString("dd/MM/yyyy"),
                    ExtensionSalary = pro.ExtensionSalary,
                    RefLetterNo = pro.RefLetterNo,
                    RefLetterDate = pro.RefLetterDate == null ? null : ((DateTime)pro.RefLetterDate).ToString("dd/MM/yyyy"),
                    ExtendedPeriod = pro.ExtendedPeriod + " " + perod.PeriodName
                }
            ).ToListAsync();

            //var test = data;
            //throw  new NotImplementedException();
            return data;
        }

        #endregion

        #region GetByIdAsync

        public async Task<ProbationPeriodExtensionSetupViewModel> GetByIdAsync(string code)
        {
            var data = await (

                from pro in probationPeriodExtensionrepository.All().AsNoTracking()
                where pro.Ppeid == code

                join emp in hrmEmployee.All().AsNoTracking()
                on pro.EmployeeId equals emp.EmployeeId into empJoin
                from emp in empJoin.DefaultIfEmpty()

                join empOff in hrmEmpOffialInfo.All().AsNoTracking()
                on pro.EmployeeId equals empOff.EmployeeId into empOffJoin
                from empOff in empOffJoin.DefaultIfEmpty()

                join desi in designationRepository.All().AsNoTracking()
                on empOff.DesignationCode equals desi.DesignationCode into desiJoin
                from desi in desiJoin.DefaultIfEmpty()

                join dept in departmentRepository.All().AsNoTracking()
                on empOff.DepartmentCode equals dept.DepartmentCode into deptJoin
                from dept in deptJoin.DefaultIfEmpty()

                join comp in coreCompanyRepository.All().AsNoTracking()
                on empOff.CompanyCode equals comp.CompanyCode into compJoin
                from comp in compJoin.DefaultIfEmpty()

                select new
                {
                    pro,
                    emp,
                    empOff,
                    desi,
                    dept,
                    comp
                }
            ).FirstOrDefaultAsync();

            if (data == null) return null;

            // Parse ExtendedPeriod
            string extendedPeriod = data.pro.ExtendedPeriod ?? "";
            string numberPart = new string(extendedPeriod.TakeWhile(char.IsDigit).ToArray());
            string stringPart = new string(extendedPeriod.SkipWhile(c => char.IsDigit(c) || c == ' ').ToArray());
            var periodInfo = await corePeriodInforepository.All().AsNoTracking()
                                .FirstOrDefaultAsync(p => p.PeriodName.ToLower() == stringPart.ToLower());

            string periodInfoId = periodInfo?.PeriodInfoId;

            // Final ViewModel mapping
            var result = new ProbationPeriodExtensionSetupViewModel
            {
                AutoId = data.pro.AutoId,
                Ppeid = data.pro.Ppeid,
                ExtendedPeriod = data.pro.ExtendedPeriod,
                Extended = numberPart,
                Period = stringPart,
                PeriodInfoId = data.pro.PeriodInfoId,
                ProbationPeriodType = data.pro.PeriodInfoId,

                Wef = data.pro.Wef?.ToString("dd/MM/yyyy"),
                ExtensionSalary = data.pro.ExtensionSalary,
                RefLetterNo = data.pro.RefLetterNo,
                RefLetterDate = data.pro.RefLetterDate?.ToString("dd/MM/yyyy"),
                Remarks = data.pro.Remarks,
                Luser = data.pro.Luser,
                Lmac = data.pro.Lmac,
                Lip = data.pro.Lip,
                Ldate = data.pro.Ldate,
                ModifyDate = data.pro.ModifyDate,
                EmployeeId = data.pro.EmployeeId,

                EmployeeName = $"{data.emp?.FirstName} {data.emp?.LastName}".Trim(),
                DesignationName = data.desi?.DesignationName ?? "",
                DepartmentName = data.dept?.DepartmentName ?? "",
                JoiningDate = data.empOff?.JoiningDate,
                GrossSalary = data.empOff.GrossSalary,
                ProbationPeriod = data.empOff.ProbationPeriod?.ToString(),
                ContractEndDate = data.empOff?.ContractEndDate
            };

            return result;
        }

        #endregion

        #region SaveAsync

        public async Task<bool> SaveAsync(ProbationPeriodExtensionSetupViewModel entityVM)
        {
            await probationPeriodExtensionrepository.BeginTransactionAsync();

            try
            {
                var name = corePeriodInforepository.All().Where(e => e.PeriodInfoId == entityVM.Period).FirstOrDefault();

                // Get next PPEID
                var allRecords = await probationPeriodExtensionrepository.GetAllAsync();
                int nextId = (allRecords.Any() ? allRecords.Select(r => int.Parse(r.Ppeid)).Max() : 0) + 1;
                string newPpeid = nextId.ToString().PadLeft(8, '0');

                if (entityVM.AutoId == 0)
                {
                    // Check for duplicate based on EmployeeId and ExtendedPeriod
                    bool isExist = probationPeriodExtensionrepository
                        .GetAll()
                        .Any(x => x.EmployeeId == entityVM.EmployeeId && x.ExtendedPeriod == entityVM.ExtendedPeriod);

                    if (!isExist)
                    {
                        var newEntity = new HrmDefProbationPeriodExtension
                        {
                            Ppeid = newPpeid,
                            EmployeeId = entityVM.EmployeeId ?? string.Empty,
                            ExtendedPeriod = entityVM.Extended,
                            PeriodInfoId = entityVM.Period,
                            Wef = !string.IsNullOrWhiteSpace(entityVM.Wef) ? DateTime.ParseExact(entityVM.Wef, "yyyy-MM-dd", CultureInfo.InvariantCulture) : DateTime.MinValue,
                            ExtensionSalary = entityVM.ExtensionSalary,
                            RefLetterNo = entityVM.RefLetterNo,
                            RefLetterDate = !string.IsNullOrWhiteSpace(entityVM.RefLetterDate) ? DateTime.ParseExact(entityVM.RefLetterDate, "yyyy-MM-dd", CultureInfo.InvariantCulture) : (DateTime?)null,
                            Remarks = entityVM.Remarks ?? string.Empty,
                            Luser = entityVM.Luser ?? string.Empty,
                            Ldate = DateTime.Now,
                            Lip = entityVM.Lip ?? string.Empty,
                            Lmac = entityVM.Lmac ?? string.Empty,
                            CompanyCode = entityVM.CompanyCode ?? string.Empty,
                            UserEmployeeId = entityVM.UserInfoEmployeeId ?? string.Empty,
                        };

                        await probationPeriodExtensionrepository.AddAsync(newEntity);
                    }
                    else
                    {
                        await probationPeriodExtensionrepository.RollbackTransactionAsync();
                        return false; // Already exists
                    }
                }
                else
                {
                    // Update logic
                    var existingEntity = await probationPeriodExtensionrepository.GetByIdAsync(entityVM.Ppeid);
                    if (existingEntity != null)
                    {
                        existingEntity.EmployeeId = entityVM.EmployeeId ?? string.Empty;
                        existingEntity.ExtendedPeriod = entityVM.ExtendedPeriod ?? string.Empty;
                        existingEntity.Wef = !string.IsNullOrWhiteSpace(entityVM.Wef) ? DateTime.ParseExact(entityVM.Wef, "yyyy-MM-dd", CultureInfo.InvariantCulture) : DateTime.MinValue;
                        existingEntity.ExtensionSalary = entityVM.ExtensionSalary;
                        existingEntity.RefLetterNo = entityVM.RefLetterNo ?? string.Empty;
                        existingEntity.RefLetterDate = !string.IsNullOrWhiteSpace(entityVM.RefLetterDate) ? DateTime.ParseExact(entityVM.RefLetterDate, "yyyy-MM-dd", CultureInfo.InvariantCulture) : (DateTime?)null;
                        existingEntity.Remarks = entityVM.Remarks ?? string.Empty;
                        existingEntity.Luser = entityVM.Luser ?? string.Empty;
                        existingEntity.Ldate = DateTime.Now;
                        existingEntity.Lip = entityVM.Lip ?? string.Empty;
                        existingEntity.Lmac = entityVM.Lmac ?? string.Empty;
                        existingEntity.CompanyCode = entityVM.CompanyCode ?? string.Empty;
                        existingEntity.ModifyDate = DateTime.Now;

                        await probationPeriodExtensionrepository.UpdateAsync(existingEntity);
                    }
                }

                await probationPeriodExtensionrepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error message: {ex.Message}");
                await probationPeriodExtensionrepository.RollbackTransactionAsync();
                return false;
            }
        }


        #endregion

        #region UpdateAsync

        public async Task<bool> UpdateAsync(ProbationPeriodExtensionSetupViewModel entityVM)
        {
            await probationPeriodExtensionrepository.BeginTransactionAsync();

            try
            {
                // Validate Ppeid is provided
                if (string.IsNullOrWhiteSpace(entityVM.Ppeid))
                {
                    await probationPeriodExtensionrepository.RollbackTransactionAsync();
                    return false;
                }

                // Retrieve existing entity
                var entity = await probationPeriodExtensionrepository.GetByIdAsync(entityVM.Ppeid);
                if (entity == null)
                {
                    await probationPeriodExtensionrepository.RollbackTransactionAsync();
                    return false;
                }

                // Get period name for ExtendedPeriod
                var name = corePeriodInforepository.All().Where(e => e.PeriodInfoId == entityVM.Period).FirstOrDefault();

                // Update fields
                entity.EmployeeId = entityVM.EmployeeId ?? string.Empty;
                entity.ExtendedPeriod = entityVM.Extended ?? string.Empty;
                entity.PeriodInfoId = entityVM.Period;
                entity.Wef = DateTime.TryParseExact(entityVM.Wef, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedWef)
                    ? parsedWef
                    : null;
                entity.ExtensionSalary = entityVM.ExtensionSalary ?? 0;
                entity.RefLetterNo = entityVM.RefLetterNo ?? string.Empty;
                entity.RefLetterDate = DateTime.TryParseExact(entityVM.RefLetterDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedRefLetterDate)
                    ? parsedRefLetterDate
                    : null;
                entity.Remarks = entityVM.Remarks ?? string.Empty;
                entity.Luser = entityVM.Luser ?? string.Empty;
                entity.Lip = entityVM.Lip ?? string.Empty;
                entity.Lmac = entityVM.Lmac ?? string.Empty;
                entity.CompanyCode = entityVM.CompanyCode ?? string.Empty;
                entity.ModifyDate = DateTime.Now;

                await probationPeriodExtensionrepository.UpdateAsync(entity);
                await probationPeriodExtensionrepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
                await probationPeriodExtensionrepository.RollbackTransactionAsync();
                return false;
            }
        }

        #endregion

        #region SelectionTypeAsync
        public IEnumerable<CommonSelectModel> SelectionProbationPeriodExtensionTypeAsync()
        {

            var data = probationPeriodExtensionrepository.All()
                       .Select(x => new CommonSelectModel
                       {
                           Code = x.Ppeid,
                           Name = x.ExtendedPeriod,
                       });
            return data;
        }

        #endregion

        #region DeleteTab
        public async Task<bool> DeleteTab(List<string> ids)
        {
            var entity = await probationPeriodExtensionrepository.All().Where(x => ids.Contains(x.Ppeid)).ToListAsync();

            if (!entity.Any())
            {
                return false;
            }

            probationPeriodExtensionrepository.Delete(entity);

            return true;
        }

        #endregion

        #region Duplicate Check 
        public async Task<bool> IsExistByCodeAsync(string code)
        {
            return await probationPeriodExtensionrepository.All().AnyAsync(x => x.Ppeid == code);
        }

        public async Task<bool> IsExistAsync(string name)
        {
            return await probationPeriodExtensionrepository.All().AnyAsync(x => x.EmployeeId == name);
        }

        public async Task<bool> IsExistAsync(string employeeCode, string exc)
        {
            var result = probationPeriodExtensionrepository.All().FirstOrDefault(e => e.EmployeeId == employeeCode);

            return await probationPeriodExtensionrepository.All().AnyAsync(x => x.EmployeeId == employeeCode && x.ExtendedPeriod == exc /*&& x.SalaryYear == year*/);

            // return await salaryOnHoldrepository.All().AnyAsync(x => x.EmployeeId == employeeCode && x.SalaryMonth == month && x.SalaryYear != typeCode);
        }

        #endregion

        #region Permission all type
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Probation Period Extension" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Probation Period Extension" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Probation Period Extension" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Probation Period Extension" && x.CheckDelete);
        }
        #endregion

    }
}

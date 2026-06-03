using GCTL.Core.Data;
using GCTL.Core.ViewModels.ManualEntryApproval;
using GCTL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.ManualEntryApprovalService
{
    public class ManualEntryApprovalService : AppService<HrmAtdManual>, IManualEntryApprovalService
    {
        private readonly IRepository<HrmAtdManual> atdManualRepo;
        private readonly IRepository<GCTL_ERP_DB_DatapathContext> _context;
        private readonly IRepository<HrmEmployee> employeeRepo;
        private readonly IRepository<HrmEmployeeOfficialInfo> empOffRepo;
        private readonly IRepository<HrmDefDesignation> desiRepo;
        private readonly IRepository<HrmDefDepartment> depRepo;
        private readonly IRepository<HrmDefDivision> divRepo;
        private readonly IRepository<CoreBranch> branchRepo;
        private readonly IRepository<CoreCompany> companyRepo;
        private readonly IRepository<HrmDefEmployeeStatus> empStRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly IRepository<HrmAtdShift> shiftRepo;
        private readonly IRepository<HrmAtdAttendanceType> atdTypeRepo;
        private readonly string _connectionString;

        public ManualEntryApprovalService(
            IRepository<HrmAtdManual> AtdManualRepo,
            IRepository<GCTL_ERP_DB_DatapathContext> context,
            IRepository<HrmEmployee> employeeRepo,
            IRepository<HrmEmployeeOfficialInfo> empOffRepo,
            IRepository<HrmDefDesignation> desiRepo,
            IRepository<HrmDefDepartment> depRepo,
            IRepository<HrmDefDivision> divRepo,
            IRepository<CoreBranch> branchRepo,
            IRepository<CoreCompany> companyRepo,
            IRepository<HrmDefEmployeeStatus> empStRepo,
            IRepository<CoreAccessCode> accessCodeRepository,
            IRepository<HrmAtdShift> shiftRepo,
            IRepository<HrmAtdAttendanceType> atdTypeRepo,
            IConfiguration configuration
        ) : base(AtdManualRepo)
        {
            this.atdManualRepo = AtdManualRepo;
            _context = context;
            this.employeeRepo = employeeRepo;
            this.empOffRepo = empOffRepo;
            this.desiRepo = desiRepo;
            this.depRepo = depRepo;
            this.divRepo = divRepo;
            this.branchRepo = branchRepo;
            this.companyRepo = companyRepo;
            this.empStRepo = empStRepo;
            this.accessCodeRepository = accessCodeRepository;
            this.shiftRepo = shiftRepo;
            this.atdTypeRepo = atdTypeRepo;
            _connectionString = configuration.GetConnectionString("ApplicationDbConnection");
        }

        #region Permission all type

        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Manual Entry Approval" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Manual Entry Approval" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Manual Entry Approval" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Manual Entry Approval" && x.CheckDelete);
        }

        #endregion
        //public async Task<ManualEntryApprovalFilterListDto> GetManualEntryDataAsync(ManualEntryApprovalFilterDto filter)
        //{
        //    var query = from atdm in atdManualRepo.All()
        //                join eoi in empOffRepo.All() on atdm.EmployeeId equals eoi.EmployeeId
        //                join e in employeeRepo.All() on atdm.EmployeeId equals e.EmployeeId into empJoin
        //                from e in empJoin.DefaultIfEmpty()
        //                join dg in desiRepo.All() on eoi.DesignationCode equals dg.DesignationCode into dgJoin
        //                from dg in dgJoin.DefaultIfEmpty()
        //                join cb in branchRepo.All() on eoi.BranchCode equals cb.BranchCode into cbJoin
        //                from cb in cbJoin.DefaultIfEmpty()
        //                join dv in divRepo.All() on eoi.DivisionCode equals dv.DivisionCode into dvJoin
        //                from dv in dvJoin.DefaultIfEmpty()
        //                join dp in depRepo.All() on eoi.DepartmentCode equals dp.DepartmentCode into dpJoin
        //                from dp in dpJoin.DefaultIfEmpty()
        //                join empSt in empStRepo.All() on eoi.EmployeeStatus equals empSt.EmployeeStatusId into empStJoin
        //                from empSt in empStJoin.DefaultIfEmpty()
        //                join cp in companyRepo.All() on eoi.CompanyCode equals cp.CompanyCode into cpJoin
        //                from cp in cpJoin.DefaultIfEmpty()
        //                join atdt in atdTypeRepo.All() on atdm.AttendanceTypeCode equals atdt.AttendanceTypeCode into atdtJoin
        //                from atdt in atdtJoin.DefaultIfEmpty()

        //                where
        //                    (filter.FromDate == null || atdm.Date >= filter.FromDate) &&
        //                    (filter.ToDate == null || atdm.Date <= filter.ToDate) &&
        //                    (atdm.ApprovalStatus != "Approved")
        //                select new
        //                {
        //                    ManualCode = atdm.ManualCode ?? "",
        //                    EmpId = e.EmployeeId,
        //                    EmpName = (e.FirstName ?? "") + " " + (e.LastName ?? ""),
        //                    CompanyCode = eoi.CompanyCode ?? "",
        //                    BranchCode = cb.BranchCode ?? "",
        //                    DivisionCode = dv.DivisionCode ?? "",
        //                    DivisionName = dv.DivisionName ?? "",
        //                    DesignationCode = dg.DesignationCode ?? "",
        //                    DesignationName = dg.DesignationName ?? "",
        //                    DepartmentCode = dp.DepartmentCode ?? "",
        //                    DepartmentName = dp.DepartmentName ?? "",
        //                    BranchName = cb.BranchName ?? "",
        //                    CompanyName = cp.CompanyName ?? "",
        //                    EmployeeStatusCode = eoi.EmployeeStatus ?? "",
        //                    AttendanceTypeCode = atdm.AttendanceTypeCode ?? "",
        //                    Date = atdm.Date,
        //                    Time = atdm.Time,
        //                    Remark = atdm.Remarks ?? "",
        //                    empStatus = empSt.EmployeeStatus ?? "",
        //                    AttandanceType = atdt.AttendanceTypeName
        //                };

        //    if (filter.CompanyCodes?.Any() == true)
        //    {
        //        query = query.Where(x => x.CompanyCode != null && filter.CompanyCodes.Contains(x.CompanyCode));
        //    }
        //    if (filter.BranchCodes?.Any() == true)
        //    {
        //        query = query.Where(x => x.BranchCode != null && filter.BranchCodes.Contains(x.BranchCode));
        //    }
        //    if (filter.DivisionCodes?.Any() == true)
        //    {
        //        query = query.Where(x => x.DivisionCode != null && filter.DivisionCodes.Contains(x.DivisionCode));
        //    }
        //    if (filter.DepartmentCodes?.Any() == true)
        //    {
        //        query = query.Where(x => x.DepartmentCode != null && filter.DepartmentCodes.Contains(x.DepartmentCode));
        //    }
        //    if (filter.DesignationCodes?.Any() == true)
        //    {
        //        query = query.Where(x => x.DesignationCode != null && filter.DesignationCodes.Contains(x.DesignationCode));
        //    }
        //    if (filter.EmployeeStatuses?.Any() == true)
        //    {
        //        query = query.Where(x => x.EmployeeStatusCode != null && filter.EmployeeStatuses.Contains(x.EmployeeStatusCode));
        //    }
        //    if (filter.EmployeeIDs?.Any() == true)
        //    {
        //        query = query.Where(x => x.EmpId != null && filter.EmployeeIDs.Contains(x.EmpId));
        //    }

        //    var result = new ManualEntryApprovalFilterListDto
        //    {
        //        Companies = await query.Where(x => x.CompanyCode != null && x.CompanyName != null)
        //            .Select(x => new ManualEntryApprovalFilterResultDto { Code = x.CompanyCode, Name = x.CompanyName })
        //            .Distinct().ToListAsyncSafe(),

        //        Branches = await query.Where(x => x.BranchCode != null && x.BranchName != null)
        //            .Select(x => new ManualEntryApprovalFilterResultDto { Code = x.BranchCode, Name = x.BranchName })
        //            .Distinct().ToListAsyncSafe(),

        //        Divisions = await query.Where(x => x.DivisionCode != null && x.DivisionName != null)
        //            .Select(x => new ManualEntryApprovalFilterResultDto { Code = x.DivisionCode, Name = x.DivisionName })
        //            .Distinct().ToListAsyncSafe(),

        //        Departments = await query.Where(x => x.DepartmentCode != null && x.DepartmentName != null)
        //            .Select(x => new ManualEntryApprovalFilterResultDto { Code = x.DepartmentCode, Name = x.DepartmentName })
        //            .Distinct().ToListAsyncSafe(),

        //        Designations = await query.Where(x => x.DesignationCode != null && x.DesignationName != null)
        //            .Select(x => new ManualEntryApprovalFilterResultDto { Code = x.DesignationCode, Name = x.DesignationName })
        //            .Distinct().ToListAsyncSafe(),

        //        Employees = await query.Where(x => x.EmpId != null && x.EmpName != null)
        //            .Select(x => new ManualEntryApprovalFilterResultDto
        //            {
        //                Code = x.ManualCode,
        //                Name = x.EmpName,
        //                EmpId = x.EmpId,
        //                DesignationName = x.DesignationName ?? "",
        //                DepartmentName = x.DepartmentName ?? "",
        //                BranchName = x.BranchName ?? "",
        //                DivisionName = x.DivisionName ?? "",
        //                CompanyName = x.CompanyName ?? "",
        //                AttandanceType = x.AttandanceType ?? "",
        //                Date = x.Date,
        //                Time = x.Time.ToString("HH:mm"),
        //                ShowDate = x.Date.ToString("dd/MM/yyyy"),
        //                ShowTime = x.Time.ToString("hh:mm tt"),
        //                Remark = x.Remark ?? "",
        //                ManualId = x.ManualCode,
        //                EmployeeStatus = x.empStatus
        //            }).Distinct().ToListAsyncSafe(),

        //        ActivityStatuses = await query.Where(x => x.empStatus != null)
        //            .Select(x => new ManualEntryApprovalFilterResultDto { Code = x.EmployeeStatusCode, Name = x.empStatus })
        //            .Distinct().ToListAsyncSafe(),
        //    };

        //    return result;
        //}

        public async Task<List<ManualEntryBaseRow>> GetManualEntryBaseDataAsync(
     ManualEntryApprovalFilterDto filter)
        {
            var query =
                from eoi in empOffRepo.All().AsNoTracking()
                join atdm in atdManualRepo.All().AsNoTracking()
                    on eoi.EmployeeId equals atdm.EmployeeId
                join e in employeeRepo.All().AsNoTracking()
                    on eoi.EmployeeId equals e.EmployeeId into empJoin
                from e in empJoin.DefaultIfEmpty()
                join dg in desiRepo.All().AsNoTracking()
                    on eoi.DesignationCode equals dg.DesignationCode into dgJoin
                from dg in dgJoin.DefaultIfEmpty()
                join cb in branchRepo.All().AsNoTracking()
                    on eoi.BranchCode equals cb.BranchCode into cbJoin
                from cb in cbJoin.DefaultIfEmpty()
                join dv in divRepo.All().AsNoTracking()
                    on eoi.DivisionCode equals dv.DivisionCode into dvJoin
                from dv in dvJoin.DefaultIfEmpty()
                join dp in depRepo.All().AsNoTracking()
                    on eoi.DepartmentCode equals dp.DepartmentCode into dpJoin
                from dp in dpJoin.DefaultIfEmpty()
                join empSt in empStRepo.All().AsNoTracking()
                    on eoi.EmployeeStatus equals empSt.EmployeeStatusId into empStJoin
                from empSt in empStJoin.DefaultIfEmpty()
                join cp in companyRepo.All().AsNoTracking()
                    on eoi.CompanyCode equals cp.CompanyCode into cpJoin
                from cp in cpJoin.DefaultIfEmpty()
                join atdt in atdTypeRepo.All().AsNoTracking()
                    on atdm.AttendanceTypeCode equals atdt.AttendanceTypeCode into atdtJoin
                from atdt in atdtJoin.DefaultIfEmpty()

                where
                    (filter.FromDate == null || atdm.Date >= filter.FromDate) &&
                    (filter.ToDate == null || atdm.Date <= filter.ToDate) &&
                    atdm.ApprovalStatus != "Approved"

                select new ManualEntryBaseRow
                {
                    ManualCode = atdm.ManualCode,
                    EmpId = eoi.EmployeeId,
                    EmpName = (e.FirstName ?? "") + " " + (e.LastName ?? ""),
                    CompanyCode = eoi.CompanyCode,
                    CompanyName = cp.CompanyName,
                    BranchCode = eoi.BranchCode,
                    BranchName = cb.BranchName,
                    DivisionCode = eoi.DivisionCode,
                    DivisionName = dv.DivisionName,
                    DepartmentCode = eoi.DepartmentCode,
                    DepartmentName = dp.DepartmentName,
                    DesignationCode = eoi.DesignationCode,
                    DesignationName = dg.DesignationName,
                    EmployeeStatusCode = eoi.EmployeeStatus,
                    EmployeeStatusName = empSt.EmployeeStatus,
                    AttendanceType = atdt.AttendanceTypeName,
                    Date = atdm.Date,
                    Time = atdm.Time,
                    Remark = atdm.Remarks
                };

            // 🔥 ONLY DB HIT
            return await query.ToListAsyncSafe();
        }


        public async Task<ManualEntryApprovalFilterListDto> GetManualEntryDataAsync(ManualEntryApprovalFilterDto filter)
        {
            var baseData = await GetManualEntryBaseDataAsync(filter);

            IEnumerable<ManualEntryBaseRow> data = baseData;

            if (filter.CompanyCodes?.Any() == true)
                data = data.Where(x => filter.CompanyCodes.Contains(x.CompanyCode));

            if (filter.BranchCodes?.Any() == true)
                data = data.Where(x => filter.BranchCodes.Contains(x.BranchCode));

            if (filter.DivisionCodes?.Any() == true)
                data = data.Where(x => filter.DivisionCodes.Contains(x.DivisionCode));

            if (filter.DepartmentCodes?.Any() == true)
                data = data.Where(x => filter.DepartmentCodes.Contains(x.DepartmentCode));

            if (filter.DesignationCodes?.Any() == true)
                data = data.Where(x => filter.DesignationCodes.Contains(x.DesignationCode));

            if (filter.EmployeeStatuses?.Any() == true)
                data = data.Where(x => filter.EmployeeStatuses.Contains(x.EmployeeStatusCode));

            if (filter.EmployeeIDs?.Any() == true)
                data = data.Where(x => filter.EmployeeIDs.Contains(x.EmpId));

            return new ManualEntryApprovalFilterListDto
            {
                Companies = baseData
                    .Where(x => !string.IsNullOrEmpty(x.CompanyCode))
                    .Select(x => new ManualEntryApprovalFilterResultDto
                    {
                        Code = x.CompanyCode,
                        Name = x.CompanyName
                    })
                    .DistinctBy(x => x.Code).ToList(),

                Branches = baseData
                    .Where(x => !string.IsNullOrEmpty(x.BranchCode))
                    .Select(x => new ManualEntryApprovalFilterResultDto
                    {
                        Code = x.BranchCode,
                        Name = x.BranchName
                    })
                    .DistinctBy(x => x.Code).ToList(),

                Divisions = baseData
                    .Where(x => !string.IsNullOrEmpty(x.DivisionCode))
                    .Select(x => new ManualEntryApprovalFilterResultDto
                    {
                        Code = x.DivisionCode,
                        Name = x.DivisionName
                    })
                    .DistinctBy(x => x.Code).ToList(),

                Departments = baseData
                    .Where(x => !string.IsNullOrEmpty(x.DepartmentCode))
                    .Select(x => new ManualEntryApprovalFilterResultDto
                    {
                        Code = x.DepartmentCode,
                        Name = x.DepartmentName
                    })
                    .DistinctBy(x => x.Code).ToList(),

                Designations = baseData
                    .Where(x => !string.IsNullOrEmpty(x.DesignationCode))
                    .Select(x => new ManualEntryApprovalFilterResultDto
                    {
                        Code = x.DesignationCode,
                        Name = x.DesignationName
                    })
                    .DistinctBy(x => x.Code).ToList(),

                Employees = data.Select(x => new ManualEntryApprovalFilterResultDto
                {
                    Code = x.EmpId,
                    ManualId = x.ManualCode,
                    Name = x.EmpName,
                    EmpId = x.EmpId,
                    DepartmentName = x.DepartmentName,
                    DesignationName = x.DesignationName,
                    BranchName = x.BranchName,
                    DivisionName = x.DivisionName,
                    CompanyName = x.CompanyName,
                    AttandanceType = x.AttendanceType,
                    Date = x.Date,
                    ShowDate = x.Date.ToString("dd/MM/yyyy"),
                    ShowTime = x.Time.HasValue ? x.Time.Value.ToString("hh:mm tt") : "",
                    Remark = x.Remark,
                    EmployeeStatus = x.EmployeeStatusName
                }).ToList(),

                ActivityStatuses = baseData
                    .Where(x => x.EmployeeStatusCode != null)
                    .Select(x => new ManualEntryApprovalFilterResultDto
                    {
                        Code = x.EmployeeStatusCode.ToString(),
                        Name = x.EmployeeStatusName
                    })
                    .DistinctBy(x => x.Code).ToList()
            };
        }




        public async Task<(bool isSuccess, string isMessage)> ApprovalManualEntries(ManualApprovalRequest modelData)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    // Convert list to comma separated string for SQL IN clause
                    var manualCodes = string.Join(",", modelData.CheckedApprovalList.Select(x => $"'{x}'"));

                    string sql = $@"
                UPDATE HRM_ATD_Manual
                SET ApprovalStatus = 'Approved',
                    ApprovalDatetime = GETDATE(),                    
                    ApprovedBy = @Luser
                WHERE ManualCode IN ({manualCodes})";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {                       
                        cmd.Parameters.AddWithValue("@Luser", modelData.Luser ?? "");

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        return (true, $"Approved successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, "Update failed: " + ex.Message);
            }
        }

        public async Task<List<ManualEntryApprovalSetupViewModelDto>> GetManualEntryGridService()
        {
            var query = from atdm in atdManualRepo.All()
                        join eoi in empOffRepo.All() on atdm.EmployeeId equals eoi.EmployeeId
                        join e in employeeRepo.All() on atdm.EmployeeId equals e.EmployeeId into empGroup
                        from e in empGroup.DefaultIfEmpty()
                        join d in desiRepo.All() on eoi.DesignationCode equals d.DesignationCode into desiGroup
                        from d in desiGroup.DefaultIfEmpty()
                        join atdt in atdTypeRepo.All() on atdm.AttendanceTypeCode equals atdt.AttendanceTypeCode into atdtJoin
                        from atdt in atdtJoin.DefaultIfEmpty()
                        where atdm.ApprovalStatus == "Approved"
                        select new ManualEntryApprovalSetupViewModelDto
                        {
                            AutoId = atdm.AutoId,
                            ManualCode = atdm.ManualCode ?? "",
                            BulkEntryId = atdm.BulkEntryId ?? "",
                            AttdEntryType = atdm.AttdEntryType ?? "",
                            EmployeeId = atdm.EmployeeId ?? "",
                            EmployeeName = (e.FirstName ?? "") + " " + (e.LastName ?? ""),
                            DesignationName = d.DesignationName ?? "",
                            AttendanceTypeCode = atdm.AttendanceTypeCode ?? "",
                            AttendanceTypeName = atdt.AttendanceTypeName ?? "",
                            Date = atdm.Date,
                            ShowDate = atdm.Date.ToString("dd/MM/yyyy"),
                            Time = atdm.Time,
                            Remarks = atdm.Remarks ?? "",
                            CompanyCode = atdm.CompanyCode ?? "",
                            Latitude = atdm.Latitude ?? "",
                            Longitude = atdm.Longitude ?? "",
                            EntryVia = atdm.EntryVia ?? "",
                            ApprovalStatus = atdm.ApprovalStatus ?? "",
                            ApprovedBy = atdm.ApprovedBy ?? "",
                            ApprovalDatetime = atdm.ApprovalDatetime,
                            ShowApprovalDatetime = atdm.ApprovalDatetime.HasValue? atdm.ApprovalDatetime.Value.ToString("dd/MM/yyyy HH:mm:ss tt"): "",
                            EntryUser = atdm.Luser ?? "",
                            MonthName = atdm.Date.ToString("MMMM"),
                            YearName = atdm.Date.Year.ToString(),
                            DayName = atdm.Date.DayOfWeek.ToString()
                        };

            return await query.ToListAsync();
        }
    }

        public static class QueryableExtensions
    {
        public static async Task<List<T>> ToListAsyncSafe<T>(this IQueryable<T> query, CancellationToken cancellationToken = default)
        {
            try
            {
                return await query.ToListAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error executing ToListAsync: {ex.Message}");
                throw;
            }
        }
    }
}
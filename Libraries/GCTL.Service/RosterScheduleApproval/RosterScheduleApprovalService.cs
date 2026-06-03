using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.PFAssignEntry;
using GCTL.Core.ViewModels.RosterScheduleApproval;
using GCTL.Core.ViewModels.RosterScheduleEntry;
using GCTL.Data.Models;
using GCTL.Service.EmployeeWeekendDeclaration;
using GCTL.UI.Core.Views.RosterScheduleApproval;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace GCTL.Service.RosterScheduleApproval
{
    public class RosterScheduleApprovalService : AppService<HrmRosterScheduleEntry>, IRosterScheduleApprovalService
    {
        private readonly IRepository<HrmRosterScheduleEntry> rosterApprovalRepo;
        private readonly IRepository<GCTL_ERP_DB_DatapathContext> _context;
        private readonly IRepository<HrmEmployee> employeeRepo;
        private readonly IRepository<HrmEmployeeOfficialInfo> empOffRepo;
        private readonly IRepository<HrmDefDesignation> desiRepo;
        private readonly IRepository<HrmDefDepartment> depRepo;
        private readonly IRepository<HrmDefDivision> divRepo;
        private readonly IRepository<CoreBranch> branchRepo;
        private readonly IRepository<CoreCompany> companyRepo;
        private readonly IRepository<HrmDefEmployeeStatus> empStRepo;
        private readonly IRepository<HrmAtdShift> shiftRepo;
        private readonly IRepository<HrmRosterScheduleEntry> rosterEntryRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly string _connectionString;

        public RosterScheduleApprovalService(
            IRepository<HrmRosterScheduleEntry> RosterApprovalRepo,
            IRepository<GCTL_ERP_DB_DatapathContext> context,
             IRepository<HrmEmployee> employeeRepo,
          IRepository<HrmEmployeeOfficialInfo> empOffRepo,
          IRepository<HrmDefDesignation> desiRepo,
          IRepository<HrmDefDepartment> depRepo,
          IRepository<HrmDefDivision> divRepo,
          IRepository<CoreBranch> branchRepo,
          IRepository<CoreCompany> companyRepo,
            IRepository<HrmDefEmployeeStatus> empStRepo,
            IRepository<HrmAtdShift> shiftRepo,
            IRepository<HrmRosterScheduleEntry> rosterEntryRepo,
            IRepository<CoreAccessCode> accessCodeRepository,
              IConfiguration configuration
            ) : base(RosterApprovalRepo)
        {
            this.rosterApprovalRepo = RosterApprovalRepo;
            _context = context;
            this.employeeRepo = employeeRepo;
            this.empOffRepo = empOffRepo;
            this.desiRepo = desiRepo;
            this.depRepo = depRepo;
            this.divRepo = divRepo;
            this.branchRepo = branchRepo;
            this.companyRepo = companyRepo;
            this.empStRepo = empStRepo;
            this.shiftRepo = shiftRepo;
            this.rosterEntryRepo = rosterEntryRepo;
            this.accessCodeRepository = accessCodeRepository;
            _connectionString = configuration.GetConnectionString("ApplicationDbConnection");
        }

        #region Permission all type

        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Roster Schedule Approval" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Roster Schedule Approval" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Roster Schedule Approval" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Roster Schedule Approval" && x.CheckDelete);
        }

        #endregion

       

        public async Task<RosterFilterListDto> GetFilterDropdownsAsync(RosterFilterDto filter)
        {
            var data = await BuildDropdownQuery(filter)
                .AsNoTracking()
                .ToListAsync();

            var result = new RosterFilterListDto
            {
                Companies = data
                    .Where(x => !string.IsNullOrEmpty(x.CompanyCode))
                    .GroupBy(x => new { x.CompanyCode, x.CompanyName })
                    .Select(g => new RosterFilterResultDto
                    {
                        Code = g.Key.CompanyCode,
                        Name = g.Key.CompanyName
                    }).ToList(),

                Branches = data
                    .Where(x => !string.IsNullOrEmpty(x.BranchCode))
                    .GroupBy(x => new { x.BranchCode, x.BranchName })
                    .Select(g => new RosterFilterResultDto
                    {
                        Code = g.Key.BranchCode,
                        Name = g.Key.BranchName
                    }).ToList(),

                Divisions = data
                    .Where(x => !string.IsNullOrEmpty(x.DivisionCode))
                    .GroupBy(x => new { x.DivisionCode, x.DivisionName })
                    .Select(g => new RosterFilterResultDto
                    {
                        Code = g.Key.DivisionCode,
                        Name = g.Key.DivisionName
                    }).ToList(),

                Departments = data
                    .Where(x => !string.IsNullOrEmpty(x.DepartmentCode))
                    .GroupBy(x => new { x.DepartmentCode, x.DepartmentName })
                    .Select(g => new RosterFilterResultDto
                    {
                        Code = g.Key.DepartmentCode,
                        Name = g.Key.DepartmentName
                    }).ToList(),

                Designations = data
                    .Where(x => !string.IsNullOrEmpty(x.DesignationCode))
                    .GroupBy(x => new { x.DesignationCode, x.DesignationName })
                    .Select(g => new RosterFilterResultDto
                    {
                        Code = g.Key.DesignationCode,
                        Name = g.Key.DesignationName
                    }).ToList(),

                ActivityStatuses = data
                    .Where(x => !string.IsNullOrEmpty(x.EmployeeStatusCode))
                    .GroupBy(x => new { x.EmployeeStatusCode, x.EmpStatus })
                    .Select(g => new RosterFilterResultDto
                    {
                        Code = g.Key.EmployeeStatusCode,
                        Name = g.Key.EmpStatus
                    }).ToList(),

                Employees = data
                    .Where(x => !string.IsNullOrEmpty(x.EmpId))
                    .GroupBy(x => new { x.EmpId, x.EmpName })
                    .Select(g => new RosterFilterResultDto
                    {
                        Code = g.Key.EmpId,
                        Name = g.Key.EmpName
                    }).ToList()
            };

            return result;
        }


        // Method 2: Get Grid Data with Server-Side Pagination
        public async Task<RosterFilterListDto> GetRosterGridDataAsync(RosterFilterDto filter)
        {
            var baseQuery = BuildBaseQuery(filter).AsNoTracking();

            // Get total count before pagination
            var totalRecords = await baseQuery.CountAsync();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(filter.SearchValue))
            {
                var searchTerm = filter.SearchValue.ToLower();
                baseQuery = baseQuery.Where(x =>
                    (x.EmpId != null && x.EmpId.ToLower().Contains(searchTerm)) ||
                    (x.EmpName != null && x.EmpName.ToLower().Contains(searchTerm)) ||
                    (x.DesignationName != null && x.DesignationName.ToLower().Contains(searchTerm)) ||
                    (x.DepartmentName != null && x.DepartmentName.ToLower().Contains(searchTerm)) ||
                    (x.ShiftName != null && x.ShiftName.ToLower().Contains(searchTerm))
                );
            }

            // Get filtered count
            var filteredRecords = await baseQuery.CountAsync();

            // Apply sorting
            baseQuery = ApplySorting(baseQuery, filter.SortColumn, filter.SortDirection);

            // Apply pagination
            var pagedQuery = baseQuery
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize);

            var employees = await pagedQuery
                .Select(x => new RosterFilterResultDto
                {
                    Code = x.EmpId,
                    Name = x.EmpName,
                    DesignationName = x.DesignationName ?? "",
                    DepartmentName = x.DepartmentName ?? "",
                    BranchName = x.BranchName ?? "",
                    DivisionName = x.DivisionName ?? "",
                    CompanyName = x.CompanyName ?? "",
                    RosterScheduleId = x.RosterId ?? "",
                    Date = x.Date,
                    DayName = x.Date.ToString("dddd"),
                    ShiftName = x.ShiftName ?? "",
                    Remark = x.Remark ?? "",
                    ShowDate = x.Date.ToString("dd/MM/yyyy")
                })
                .ToListAsyncSafe();

            return new RosterFilterListDto
            {
                Employees = employees,
                TotalRecords = totalRecords,
                FilteredRecords = filteredRecords
            };
        }
        
        // Helper method to build base query
        private IQueryable<RosterQueryDto> BuildBaseQuery(RosterFilterDto filter)
        {
            var baseQuery = from rse in rosterEntryRepo.All().AsNoTracking()
                            join eoi in empOffRepo.All().AsNoTracking() on rse.EmployeeId equals eoi.EmployeeId
                            join e in employeeRepo.All().AsNoTracking() on rse.EmployeeId equals e.EmployeeId into empJoin
                            from e in empJoin.DefaultIfEmpty()
                            join dg in desiRepo.All().AsNoTracking() on eoi.DesignationCode equals dg.DesignationCode into dgJoin
                            from dg in dgJoin.DefaultIfEmpty()
                            join cb in branchRepo.All().AsNoTracking() on eoi.BranchCode equals cb.BranchCode into cbJoin
                            from cb in cbJoin.DefaultIfEmpty()
                            join dv in divRepo.All().AsNoTracking() on eoi.DivisionCode equals dv.DivisionCode into dvJoin
                            from dv in dvJoin.DefaultIfEmpty()
                            join dp in depRepo.All().AsNoTracking() on eoi.DepartmentCode equals dp.DepartmentCode into dpJoin
                            from dp in dpJoin.DefaultIfEmpty()
                            join empSt in empStRepo.All().AsNoTracking() on eoi.EmployeeStatus equals empSt.EmployeeStatusId into empStJoin
                            from empSt in empStJoin.DefaultIfEmpty()
                            join cp in companyRepo.All().AsNoTracking() on eoi.CompanyCode equals cp.CompanyCode into cpJoin
                            from cp in cpJoin.DefaultIfEmpty()
                            join st in shiftRepo.All().AsNoTracking() on rse.ShiftCode equals st.ShiftCode into stJoin
                            from st in stJoin.DefaultIfEmpty()
                            where (filter.FromDate == null || rse.Date >= filter.FromDate) &&
                                  (filter.ToDate == null || rse.Date <= filter.ToDate) &&
                                  (rse.ApprovalStatus != "Approved")
                            select new
                            {
                                EmpId = e.EmployeeId,
                                FirstName = e.FirstName,
                                LastName = e.LastName,
                                CompanyCode = eoi.CompanyCode,
                                BranchCode = cb.BranchCode,
                                DivisionCode = dv.DivisionCode,
                                DivisionName = dv.DivisionName,
                                DesignationCode = dg.DesignationCode,
                                DesignationName = dg.DesignationName,
                                DepartmentCode = dp.DepartmentCode,
                                DepartmentName = dp.DepartmentName,
                                BranchName = cb.BranchName,
                                CompanyName = cp.CompanyName,
                                EmployeeStatusCode = eoi.EmployeeStatus,
                                Date = rse.Date,
                                RosterId = rse.RosterScheduleId,
                                ShiftCode = st.ShiftCode,
                                ShiftName = st.ShiftName,
                                ShiftStartTime = st.ShiftStartTime,
                                ShiftEndTime = st.ShiftEndTime,
                                Remark = rse.Remark,
                                EmpStatus = empSt.EmployeeStatus
                            };

            // Apply filters
            if (filter.CompanyCodes?.Any() == true)
                baseQuery = baseQuery.Where(x => x.CompanyCode != null && filter.CompanyCodes.Contains(x.CompanyCode));

            if (filter.BranchCodes?.Any() == true)
                baseQuery = baseQuery.Where(x => x.BranchCode != null && filter.BranchCodes.Contains(x.BranchCode));

            if (filter.DivisionCodes?.Any() == true)
                baseQuery = baseQuery.Where(x => x.DivisionCode != null && filter.DivisionCodes.Contains(x.DivisionCode));

            if (filter.DepartmentCodes?.Any() == true)
                baseQuery = baseQuery.Where(x => x.DepartmentCode != null && filter.DepartmentCodes.Contains(x.DepartmentCode));

            if (filter.DesignationCodes?.Any() == true)
                baseQuery = baseQuery.Where(x => x.DesignationCode != null && filter.DesignationCodes.Contains(x.DesignationCode));

            if (filter.EmployeeStatuses?.Any() == true)
                baseQuery = baseQuery.Where(x => x.EmployeeStatusCode != null && filter.EmployeeStatuses.Contains(x.EmployeeStatusCode));

            if (filter.EmployeeIDs?.Any() == true)
                baseQuery = baseQuery.Where(x => x.EmpId != null && filter.EmployeeIDs.Contains(x.EmpId));

            // Final projection to RosterQueryDto (done in-memory after filters)
            return baseQuery.Select(x => new RosterQueryDto
            {
                EmpId = x.EmpId,
                EmpName = (x.FirstName ?? "") + " " + (x.LastName ?? ""),
                CompanyCode = x.CompanyCode ?? "",
                BranchCode = x.BranchCode ?? "",
                DivisionCode = x.DivisionCode ?? "",
                DivisionName = x.DivisionName ?? "",
                DesignationCode = x.DesignationCode ?? "",
                DesignationName = x.DesignationName ?? "",
                DepartmentCode = x.DepartmentCode ?? "",
                DepartmentName = x.DepartmentName ?? "",
                BranchName = x.BranchName ?? "",
                CompanyName = x.CompanyName ?? "",
                EmployeeStatusCode = x.EmployeeStatusCode ?? "",
                Date = x.Date,
                RosterId = x.RosterId ?? "",
                ShiftCode = x.ShiftCode ?? "",
                ShiftName = (x.ShiftName ?? "") + " ( " + x.ShiftStartTime.ToString("hh:mm tt") + " - " + x.ShiftEndTime.ToString("hh:mm tt") + " )",
                Remark = x.Remark ?? "",
                EmpStatus = x.EmpStatus ?? ""
            });
        }

        private IQueryable<RosterQueryDto> BuildDropdownQuery(RosterFilterDto filter)
        {
            var baseQuery = from eoi in empOffRepo.All().AsNoTracking()
                            join e in employeeRepo.All().AsNoTracking() on eoi.EmployeeId equals e.EmployeeId into empJoin
                            from e in empJoin.DefaultIfEmpty()
                            join dg in desiRepo.All().AsNoTracking() on eoi.DesignationCode equals dg.DesignationCode into dgJoin
                            from dg in dgJoin.DefaultIfEmpty()
                            join cb in branchRepo.All().AsNoTracking() on eoi.BranchCode equals cb.BranchCode into cbJoin
                            from cb in cbJoin.DefaultIfEmpty()
                            join dv in divRepo.All().AsNoTracking() on eoi.DivisionCode equals dv.DivisionCode into dvJoin
                            from dv in dvJoin.DefaultIfEmpty()
                            join dp in depRepo.All().AsNoTracking() on eoi.DepartmentCode equals dp.DepartmentCode into dpJoin
                            from dp in dpJoin.DefaultIfEmpty()
                            join empSt in empStRepo.All().AsNoTracking() on eoi.EmployeeStatus equals empSt.EmployeeStatusId into empStJoin
                            from empSt in empStJoin.DefaultIfEmpty()
                            join cp in companyRepo.All().AsNoTracking() on eoi.CompanyCode equals cp.CompanyCode into cpJoin
                            from cp in cpJoin.DefaultIfEmpty()
                            select new RosterQueryDto
                            {
                                EmpId = e.EmployeeId,
                                EmpName = (e.FirstName ?? "") + " " + (e.LastName ?? ""),
                                CompanyCode = eoi.CompanyCode ?? "",
                                BranchCode = cb.BranchCode ?? "",
                                DivisionCode = dv.DivisionCode ?? "",
                                DivisionName = dv.DivisionName ?? "",
                                DesignationCode = dg.DesignationCode ?? "",
                                DesignationName = dg.DesignationName ?? "",
                                DepartmentCode = dp.DepartmentCode ?? "",
                                DepartmentName = dp.DepartmentName ?? "",
                                BranchName = cb.BranchName ?? "",
                                CompanyName = cp.CompanyName ?? "",
                                EmployeeStatusCode = eoi.EmployeeStatus ?? "",
                                EmpStatus = empSt.EmployeeStatus ?? ""
                            };

            // Apply filters if needed
            if (filter.CompanyCodes?.Any() == true)
                baseQuery = baseQuery.Where(x => filter.CompanyCodes.Contains(x.CompanyCode));

            if (filter.BranchCodes?.Any() == true)
                baseQuery = baseQuery.Where(x => filter.BranchCodes.Contains(x.BranchCode));

            if (filter.DivisionCodes?.Any() == true)
                baseQuery = baseQuery.Where(x => filter.DivisionCodes.Contains(x.DivisionCode));

            if (filter.DepartmentCodes?.Any() == true)
                baseQuery = baseQuery.Where(x => filter.DepartmentCodes.Contains(x.DepartmentCode));

            if (filter.DesignationCodes?.Any() == true)
                baseQuery = baseQuery.Where(x => filter.DesignationCodes.Contains(x.DesignationCode));

            if (filter.EmployeeStatuses?.Any() == true)
                baseQuery = baseQuery.Where(x => filter.EmployeeStatuses.Contains(x.EmployeeStatusCode));

            if (filter.EmployeeIDs?.Any() == true)
                baseQuery = baseQuery.Where(x => filter.EmployeeIDs.Contains(x.EmpId));

            return baseQuery;
        }

        // Helper method for sorting
        private IQueryable<RosterQueryDto> ApplySorting(IQueryable<RosterQueryDto> query, string sortColumn, string sortDirection)
        {
            if (string.IsNullOrWhiteSpace(sortColumn))
                return query.OrderBy(x => x.Date);

            var isAscending = sortDirection?.ToLower() == "asc";

            return sortColumn.ToLower() switch
            {
                "empid" => isAscending ? query.OrderBy(x => x.EmpId) : query.OrderByDescending(x => x.EmpId),
                "empname" => isAscending ? query.OrderBy(x => x.EmpName) : query.OrderByDescending(x => x.EmpName),
                "designation" => isAscending ? query.OrderBy(x => x.DesignationName) : query.OrderByDescending(x => x.DesignationName),
                "department" => isAscending ? query.OrderBy(x => x.DepartmentName) : query.OrderByDescending(x => x.DepartmentName),
                "date" => isAscending ? query.OrderBy(x => x.Date) : query.OrderByDescending(x => x.Date),
                _ => query.OrderBy(x => x.Date)
            };
        }








        public async Task<(bool isSuccess, string isMessage)> ApprovalRosterServices(ApprovalRequest modelData)
        {
            const int batchSize = 1000; 
            try
            {
                var allRosterIds = modelData.CheckedApprovalList;
                int total = allRosterIds.Count;
                int processed = 0;

                while (processed < total)
                {
                    var currentBatch = allRosterIds
                        .Skip(processed)
                        .Take(batchSize)
                        .ToList();

                    var rosterBatch = rosterEntryRepo
                        .GetAll()
                        .Where(x => currentBatch.Contains(x.RosterScheduleId))
                        .ToList();

                    foreach (var rosterData in rosterBatch)
                    {
                        rosterData.ApprovalStatus = "Approved"; 
                        rosterData.ApprovalDatetime = DateTime.Now;
                        rosterData.Remark = modelData.Remark ?? "";
                        rosterData.ApprovedBy = modelData.Luser;
                    }

                   await rosterEntryRepo.UpdateRangeAsync(rosterBatch);
                    processed += currentBatch.Count;
                }

                return (true, "Batch update successful.");
            }
            catch (Exception ex)
            {
                return (false, "Batch update failed: " + ex.Message);
            }
        }


        public async Task<List<RosterScheduleEntrySetupViewModel>> GetRosterScheduleGridService()
        {
            var query = from ras in rosterEntryRepo.All()
                        join eoi in empOffRepo.All() on ras.EmployeeId equals eoi.EmployeeId
                        join e in employeeRepo.All() on ras.EmployeeId equals e.EmployeeId into empGroup
                        from e in empGroup.DefaultIfEmpty()
                        join d in desiRepo.All() on eoi.DesignationCode equals d.DesignationCode into desiGroup
                        from d in desiGroup.DefaultIfEmpty()
                        join s in shiftRepo.All() on ras.ShiftCode equals s.ShiftCode into shiftGroup
                        from s in shiftGroup.DefaultIfEmpty()
                        where
                        (ras.ApprovalStatus == "Approved")
                        select new RosterScheduleEntrySetupViewModel
                        {
                            TC = ras.Tc,
                            RosterScheduleId = ras.RosterScheduleId,
                            EmployeeID = ras.EmployeeId,
                            Name = (e != null ? e.FirstName + " " + e.LastName : ""),
                            DesignationName = d != null ? d.DesignationName : "",
                            ShiftName = s.ShiftName + "( " + s.ShiftStartTime.ToString("hh:mm:tt") + " - " + s.ShiftEndTime.ToString("hh:mm:tt") + " )" ?? "",
                            Remark = ras.Remark ?? "",
                            ApprovalDatetime = ras.ApprovalDatetime,
                            ApprovalStatus = ras.ApprovalStatus ?? "",
                            ApprovedBy = ras.ApprovedBy ?? "",
                            Date = ras.Date,
                            Luser= ras.Luser,
                            ApprovalDatetimeShow = ras.Date.ToString("dd/MM/yyyy hh:mm tt")

                            //DayName = ras.ScheduleDate.HasValue ? ras.ScheduleDate.Value.DayOfWeek.ToString() : "",
                            //luser = ras.LastModifiedBy ?? "" // You can rename it
                        };

            return await query.ToListAsync();
        }     
        
    }

}

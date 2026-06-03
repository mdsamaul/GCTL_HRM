using DocumentFormat.OpenXml.Bibliography;
using GCTL.Core.Data;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.ManualAttendance;
using GCTL.Data.Models;
using GCTL.Service.DeleteHistories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace GCTL.Service.ManualAttendances
{
    public class ManualAttendanceService : AppService<HrmAtdManual>, IManualAttendanceService
    {
        #region Repositories
        private readonly IRepository<HrmAtdManual> _manualAttendanceRepository;
        private readonly IRepository<CoreAccessCode> _coreAccessCodeRepository;
        private readonly IRepository<HrmEmployee> _employeeRepository;
        private readonly IRepository<HrmEmployeeOfficialInfo> offEmpRepository;
        private readonly IRepository<HrmAtdAttendanceType> _attendanceTypeRepository;
        private readonly IRepository<HrmAtdShift> shiftRepository;
        private readonly IRepository<CoreCompany> _companyRepository;
        private readonly IRepository<HrmDefDesignation> _designationRepository;
        private readonly IRepository<HrmDefDepartment> _departmentRepository;
        private readonly IRepository<HrmRosterScheduleEntry> rosterScheduleEntryRepository;
        private readonly IRepository<HrmEmployeeOfficialInfo> _officialInfoRepository;
        private readonly IDeleteHistoryService deleteHistoryService;
        private readonly GCTL_ERP_DB_DatapathContext context;
        public ManualAttendanceService(
            IRepository<HrmAtdManual> manualAttendanceRepository,
            IRepository<CoreAccessCode> coreAccessCodeRepository,
            IRepository<HrmEmployee> employeeRepository,
            IRepository<HrmEmployeeOfficialInfo> offEmpRepository,
            IRepository<CoreCompany> companyRepository,
            IRepository<HrmDefDesignation> designationRepository,
            IRepository<HrmDefDepartment> departmentRepository,
            IRepository<HrmRosterScheduleEntry> RosterScheduleEntryRepository,
            IRepository<HrmAtdAttendanceType> attendanceTypeRepository,
            IRepository<HrmAtdShift> ShiftRepository,
            IRepository<HrmEmployeeOfficialInfo> officialInfoRepository,
             IDeleteHistoryService deleteHistoryService,
            GCTL_ERP_DB_DatapathContext context
            ) : base(manualAttendanceRepository)
        {
            _manualAttendanceRepository = manualAttendanceRepository;
            _coreAccessCodeRepository = coreAccessCodeRepository;
            _employeeRepository = employeeRepository;
            this.offEmpRepository = offEmpRepository;
            _companyRepository = companyRepository;
            _designationRepository = designationRepository;
            _departmentRepository = departmentRepository;
            rosterScheduleEntryRepository = RosterScheduleEntryRepository;
            _attendanceTypeRepository = attendanceTypeRepository;
            shiftRepository = ShiftRepository;
            _officialInfoRepository = officialInfoRepository;
            this.deleteHistoryService = deleteHistoryService;
            this.context = context;
        }
        #endregion


        #region Permissions
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await _coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Manual Attendance" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await _coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Manual Attendance" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await _coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Manual Attendance" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await _coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Manual Attendance" && x.CheckDelete);
        }
        #endregion


        #region GetAllAsync


        public async Task<(int totalRecords, List<ManualAttendanceSetupViewModel> data)> GetAllAsync(
        int skip,
        int pageSize,
        string sortColumn,
        string sortDirection,
        string searchValue)
        {
            var query = from ma in _manualAttendanceRepository.All().AsNoTracking()
                        where ma.AttdEntryType.Trim().Replace("  ", " ") == "Single Entry"
                        join e in _employeeRepository.All().AsNoTracking() on ma.EmployeeId equals e.EmployeeId into eGroup
                        from e in eGroup.DefaultIfEmpty()
                        join at in _attendanceTypeRepository.All().AsNoTracking() on ma.AttendanceTypeCode equals at.AttendanceTypeCode into atGroup
                        from at in atGroup.DefaultIfEmpty()
                        select new ManualAttendanceSetupViewModel
                        {
                            ManualCode = ma.ManualCode,
                            EmployeeId = ma.EmployeeId,
                            AttdEntryType = ma.AttdEntryType,
                            EmployeeFullName = $"{e.FirstName} {e.LastName}",
                            AttendanceTypeName = at.AttendanceTypeName,
                            DateFrom = ma.Date.ToString("dd/MM/yyyy"),
                            EntryTime = ma.Time,
                            ShowEntryTime = ma.Time.ToString("hh:mm:ss tt"),
                            Remarks = ma.Remarks,
                            Luser = ma.Luser
                        };

            // 🔹 Filtering
            if (!string.IsNullOrEmpty(searchValue))
            {
                query = query.Where(x => x.EmployeeId.Contains(searchValue)
                                      || x.EmployeeFullName.Contains(searchValue)
                                      || x.AttendanceTypeName.Contains(searchValue));
            }

            // 🔹 Sorting
            if (!string.IsNullOrEmpty(sortColumn))
            {
                query = sortColumn switch
                {
                    "ManualCode" => sortDirection == "asc" ? query.OrderBy(x => x.ManualCode) : query.OrderByDescending(x => x.ManualCode),
                    "EmployeeId" => sortDirection == "asc" ? query.OrderBy(x => x.EmployeeId) : query.OrderByDescending(x => x.EmployeeId),
                    "EmployeeFullName" => sortDirection == "asc" ? query.OrderBy(x => x.EmployeeFullName) : query.OrderByDescending(x => x.EmployeeFullName),
                    "AttendanceTypeName" => sortDirection == "asc" ? query.OrderBy(x => x.AttendanceTypeName) : query.OrderByDescending(x => x.AttendanceTypeName),
                    _ => query.OrderByDescending(x => x.ManualCode)
                };
            }

            int totalRecords = await query.CountAsync();
            var data = await query.Skip(skip).Take(pageSize).ToListAsync();

            return (totalRecords, data);
        }

        #endregion


        #region GetByCodeAsync
        public async Task<ManualAttendanceSetupViewModel> GetByIdAsync(string code)
        {
            var result = await (from ma in _manualAttendanceRepository.All().AsNoTracking()

                                join e in _employeeRepository.All().AsNoTracking() on ma.EmployeeId equals e.EmployeeId into eGroup
                                from e in eGroup.DefaultIfEmpty()

                                join oi in _officialInfoRepository.All().AsNoTracking() on e.EmployeeId equals oi.EmployeeId into oiGroup
                                from oi in oiGroup.DefaultIfEmpty()

                                join at in _attendanceTypeRepository.All().AsNoTracking() on ma.AttendanceTypeCode equals at.AttendanceTypeCode into atGroup
                                from at in atGroup.DefaultIfEmpty()

                                join cc in _companyRepository.All().AsNoTracking() on ma.CompanyCode equals cc.CompanyCode into ccGroup
                                from cc in ccGroup.DefaultIfEmpty()

                                join des in _designationRepository.All().AsNoTracking() on oi.DesignationCode equals des.DesignationCode into desGroup
                                from des in desGroup.DefaultIfEmpty()

                                join dep in _departmentRepository.All().AsNoTracking() on oi.DepartmentCode equals dep.DepartmentCode into depGroup
                                from dep in depGroup.DefaultIfEmpty()

                                where ma.EmployeeId == code

                                select new ManualAttendanceSetupViewModel
                                {
                                    CompanyName = cc.CompanyName,
                                    EmployeeId = ma.EmployeeId,
                                    EmployeeFullName = $"{e.FirstName} {e.LastName}",
                                    DesignationName = des.DesignationName,
                                    DepartmentName = dep.DepartmentName,
                                    AttendanceTypeName = at.AttendanceTypeName,
                                    DateFrom = ma.Date.ToString("dd/MM/yyyy"),
                                    EntryTime = ma.Time,
                                    Remarks = ma.Remarks
                                }).FirstOrDefaultAsync();
            return result;
        }
        #endregion


        #region SaveAsync
       

        public async Task<(bool IsSuccess, HrmAtdManual? SavedRecord)> SaveAsync(ManualAttendanceSetupViewModel model)
        {
            HrmAtdManual? lastSaved = null;
            await _manualAttendanceRepository.BeginTransactionAsync();
            try
            {
                // Step 1: Delete duplicates in bulk
                var duplicate = await _manualAttendanceRepository
                    .FindByAsync(x => x.Date >= model.DateFrom.ToDate() &&
                                      x.Date <= model.DateTo.ToDate() &&
                                      x.EmployeeId == model.EmployeeId);

                if (duplicate != null && duplicate.Any())
                {
                    await _manualAttendanceRepository.DeleteRangeAsync(duplicate);
                }

                // Step 2: Prepare data for bulk insert
                DateTime fromDate = model.DateFrom.ToDate();
                int noOfDays = (model.DateTo.ToDate() - model.DateFrom.ToDate()).Days;

                // Fetch all required data upfront (batch query optimization)
                var dates = Enumerable.Range(0, noOfDays + 1)
                    .Select(i => fromDate.AddDays(i))
                    .ToList();

                var rosterShifts = await rosterScheduleEntryRepository.All()
                    .AsNoTracking()
                    .Where(rse => rse.EmployeeId == model.EmployeeId && dates.Contains(rse.Date))
                    .Select(rse => new { rse.Date, rse.ShiftCode })
                    .ToListAsync();

                var defaultShift = await offEmpRepository.All()
                    .AsNoTracking()
                    .Where(oi => oi.EmployeeId == model.EmployeeId)
                    .Select(oi => oi.ShiftCode)
                    .FirstOrDefaultAsync();

                var shiftCodes = rosterShifts.Select(r => r.ShiftCode)
                    .Concat(new[] { defaultShift })
                    .Where(s => !string.IsNullOrEmpty(s))
                    .Distinct()
                    .ToList();

                var shiftTimes = await shiftRepository.All()
                    .Where(x => shiftCodes.Contains(x.ShiftCode))
                    .Select(c => new
                    {
                        c.ShiftCode,
                        c.ShiftStartTime,
                        c.ShiftEndTime
                    })
                    .ToListAsync();

                var shiftDict = shiftTimes.ToDictionary(s => s.ShiftCode);

               

                var nextCode = await context.HrmAtdManual
    .MaxAsync(x => (int?)Convert.ToInt32(x.ManualCode)) ?? 0;

                int startingCode = nextCode + 1;
                int currentCodeIndex = 0;




                // Step 4: Build records list for bulk insert
                var recordsToInsert = new List<HrmAtdManual>();

                for (int i = 0; i <= noOfDays; i++)
                {
                    var currentDate = fromDate.AddDays(i);

                    // Get shift for current date
                    var shiftCode = rosterShifts.FirstOrDefault(r => r.Date == currentDate)?.ShiftCode
                                    ?? defaultShift;

                    var newEntryTime = model.EntryTime;
                    var newExitTime = model.ExitTime;

                    if (!string.IsNullOrEmpty(shiftCode) && shiftDict.ContainsKey(shiftCode))
                    {
                        newEntryTime = shiftDict[shiftCode].ShiftStartTime;
                        newExitTime = shiftDict[shiftCode].ShiftEndTime;
                    }

                    // Entry record
                    var atdManual = new HrmAtdManual
                    {
                        ManualCode = (startingCode + currentCodeIndex++).ToString()??"",
                        BulkEntryId = model.BulkEntryId ?? string.Empty,
                        EmployeeId = model.EmployeeId ?? "",
                        AttendanceTypeCode = model.AttendanceTypeCode ?? "",
                        Date = currentDate,
                        Remarks = model.Remarks??"",
                        Luser = model.Luser ?? "",
                        Ldate = DateTime.Now,
                        Lip = model.Lip ?? "",
                        Lmac = model.Lmac ?? "",
                        CompanyCode = model.CompanyCode ?? "",
                        ApprovalStatus=model.ApprovalStatus??"",
                        ApprovedBy = model.ApprovedBy?? "",
                        ApprovalDatetime = model.ApprovalDatetime,
                        Latitude = model.Latitude??"",
                        Longitude = model.Longitude??"",
                        EntryVia = model.EntryVia??"",
                        MonthName = currentDate.ToString("MMMM")??"",
                        DayName = currentDate.ToString("dddd")??"",
                        YearName = currentDate.ToString("yyyy")??"",
                        
                    };

                    if (model.AttendanceTypeCode == "3")
                    {
                        atdManual.AttdEntryType = model.AttendanceTypeName;
                        atdManual.Time = newEntryTime;
                    }
                    else
                    {
                        atdManual.AttdEntryType = "Single Entry";
                        atdManual.Time = model.EntryTime;
                    }

                    recordsToInsert.Add(atdManual);
                    lastSaved = atdManual;

                    // Exit record if needed
                    if (model.ISBothInOutEntry)
                    {
                        var atdManualExit = new HrmAtdManual
                        {
                            ManualCode = (startingCode + currentCodeIndex++).ToString() ?? "",
                            BulkEntryId = model.BulkEntryId ?? string.Empty,
                            EmployeeId = model.EmployeeId ?? "",
                            AttendanceTypeCode = model.AttendanceTypeCodeTwo ?? "",
                            Date = currentDate,
                            Time = newExitTime,
                            Remarks = model.Remarks ?? "",
                            Luser = model.Luser ?? "",
                            Ldate = DateTime.Now,
                            Lip = model.Lip ?? "",
                            Lmac = model.Lmac ?? "",
                            CompanyCode = model.CompanyCode ?? "",
                            ApprovalStatus = model.ApprovalStatus ?? "",
                            ApprovedBy = model.ApprovedBy ?? "",
                            ApprovalDatetime = model.ApprovalDatetime,
                            Latitude = model.Latitude ?? "",
                            Longitude = model.Longitude ?? "",
                            EntryVia = model.EntryVia ?? "",
                            MonthName = currentDate.ToString("MMMM") ?? "",
                            DayName = currentDate.ToString("dddd") ?? "",
                            YearName = currentDate.ToString("yyyy") ?? "",
                        };

                        if (model.AttendanceTypeCode == "3")
                        {
                            atdManualExit.AttdEntryType = model.AttendanceTypeName;
                            atdManualExit.Time = newEntryTime;
                        }
                        else
                        {
                            atdManualExit.AttdEntryType = "Single Entry";
                            atdManualExit.Time = model.ExitTime;
                        }

                        recordsToInsert.Add(atdManualExit);
                    }
                }

                // Step 5: Bulk insert all records at once
                await _manualAttendanceRepository.AddRangeAsync(recordsToInsert);
                await _manualAttendanceRepository.CommitTransactionAsync();

                return (true, lastSaved);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error message: {ex.Message}");
                await _manualAttendanceRepository.RollbackTransactionAsync();
                return (false, null);
            }
        }



        #endregion


        #region DeleteAsync
        
        public async Task<(bool IsSuccess, HrmAtdManual? DeletedRecord)> DeleteAsync(
    List<string> ids,
    List<string> selectedEmployeeIds,
    string attendanceTypeCode,
    string fromDate,
    string toDate,
    bool isBothInOutEntry,
    DeleteHistoryViewModel deleteModel)
        {
            if ((ids == null || !ids.Any()) && (selectedEmployeeIds == null || !selectedEmployeeIds.Any()))
                return (false, null);

            bool isDateRangeEmpty = string.IsNullOrEmpty(fromDate) && string.IsNullOrEmpty(toDate);

            IQueryable<HrmAtdManual> query;

            if (isDateRangeEmpty)
            {
                query = _manualAttendanceRepository.All()
                    .Where(x => ids.Contains(x.ManualCode));

                if (!isBothInOutEntry && !string.IsNullOrEmpty(attendanceTypeCode))
                    query = query.Where(x => x.AttendanceTypeCode == attendanceTypeCode);
            }
            else
            {
                DateTime dateFrom = fromDate.ToDate();
                DateTime dateTo = toDate.ToDate();

                query = _manualAttendanceRepository.All()
                    .Where(x => selectedEmployeeIds.Contains(x.EmployeeId) || ids.Contains(x.ManualCode));

                if (!isBothInOutEntry && !string.IsNullOrEmpty(attendanceTypeCode))
                    query = query.Where(x => x.AttendanceTypeCode == attendanceTypeCode);

                query = query.Where(x => x.Date >= dateFrom && x.Date <= dateTo);
            }

            var allAutoIds = await query.Select(x => x.AutoId).ToListAsync();
            
            if (!allAutoIds.Any())
                return (false, null);

            HrmAtdManual? firstDeleted = null;
            const int batchSize = 5000;

            for (int i = 0; i < allAutoIds.Count; i += batchSize)
            {
                var batchIds = allAutoIds.Skip(i).Take(batchSize).ToList();

                var entitiesToDelete = await _manualAttendanceRepository.All()
                    .Where(x => batchIds.Contains(x.AutoId))
                    .ToListAsync();

                if (entitiesToDelete.Any())
                {
                    firstDeleted ??= entitiesToDelete.First();
                    await _manualAttendanceRepository.DeleteRangeAsync(entitiesToDelete);

                    deleteModel.tableName = _manualAttendanceRepository.GetTableName();
                    await deleteHistoryService.LogDeletedRecordsAsync(entitiesToDelete, deleteModel);
                    await _manualAttendanceRepository.CommitTransactionAsync();
                }
            }

            return (true, firstDeleted);
        }
        #endregion


        #region GetEmployeeByCompany 
        public async Task<List<ManualAttendanceSetupViewModel>> GetEmployeeByCompany(string companyId)
        {
            var result = await (from oi in _officialInfoRepository.All().AsNoTracking()

                                where oi.CompanyCode == companyId

                                join c in _companyRepository.All().AsNoTracking() on oi.CompanyCode equals c.CompanyCode

                                join e in _employeeRepository.All().AsNoTracking() on oi.EmployeeId equals e.EmployeeId into eGroup
                                from e in eGroup.DefaultIfEmpty()

                                select new ManualAttendanceSetupViewModel
                                {
                                    EmployeeId = oi.EmployeeId,
                                    EmployeeFullName = $"{e.FirstName} {e.LastName} ({oi.EmployeeId})"
                                }).Distinct().ToListAsync();
            return result;
        }
        #endregion


        #region GetCompanyDataById
        public async Task<List<ManualAttendanceSetupViewModel>> GetCompanyDataById(string companyId)
        {
            var result = await (from ma in _manualAttendanceRepository.All().AsNoTracking()

                                join c in _companyRepository.All().AsNoTracking() on ma.CompanyCode equals c.CompanyCode into cGroup
                                from c in cGroup.DefaultIfEmpty()

                                join e in _employeeRepository.All().AsNoTracking() on ma.EmployeeId equals e.EmployeeId into eGroup
                                from e in eGroup.DefaultIfEmpty()

                                join at in _attendanceTypeRepository.All().AsNoTracking() on ma.AttendanceTypeCode equals at.AttendanceTypeCode into atGroup
                                from at in atGroup.DefaultIfEmpty()

                                where ma.CompanyCode == companyId orderby ma.AutoId descending

                                select new ManualAttendanceSetupViewModel
                                {
                                    ManualCode = ma.ManualCode,
                                    CompanyCode = ma.CompanyCode,
                                    EmployeeId = ma.EmployeeId,
                                    EmployeeFullName = $"{e.FirstName} {e.LastName}",
                                    AttendanceTypeName = at.AttendanceTypeName,
                                    DateFrom = ma.Date.ToString("dd/MM/yyyy"),
                                    EntryTime = ma.Time,
                                    Remarks = ma.Remarks,
                                    Luser = ma.Luser
                                }).ToListAsync();
            return result;
        }
        #endregion


        #region GetEmployeeDetailsById & GetEmployeeDataById
        public async Task<ManualAttendanceSetupViewModel> GetEmployeeDetailsById(string id)
        {
            var result = await (from e in _employeeRepository.All().AsNoTracking()

                                join oi in _officialInfoRepository.All().AsNoTracking() on e.EmployeeId equals oi.EmployeeId into oiGroup
                                from oi in oiGroup.DefaultIfEmpty()

                                join des in _designationRepository.All().AsNoTracking() on oi.DesignationCode equals des.DesignationCode into desGroup
                                from des in desGroup.DefaultIfEmpty()

                                join dep in _departmentRepository.All().AsNoTracking() on oi.DepartmentCode equals dep.DepartmentCode into depGroup
                                from dep in depGroup.DefaultIfEmpty()

                                where e.EmployeeId == id 

                                select new ManualAttendanceSetupViewModel
                                {
                                    EmployeeId = e.EmployeeId,
                                    EmployeeFullName = $"{e.FirstName} {e.LastName}",
                                    DesignationName = des.DesignationName,
                                    DepartmentName = dep.DepartmentName
                                }).FirstOrDefaultAsync();
            return result;
        }
        #endregion


        #region GetEmployeeDataById


        public async Task<(int totalRecords, List<ManualAttendanceSetupViewModel> data)> GetEmployeeDataByIdAsync(
        string employeeId,
        int skip,
        int pageSize,
        string searchValue,
        string sortColumn,
        string sortDirection)
        {
            var query = from ma in _manualAttendanceRepository.All().AsNoTracking()
                        join c in _companyRepository.All().AsNoTracking() on ma.CompanyCode equals c.CompanyCode into cGroup
                        from c in cGroup.DefaultIfEmpty()
                        join e in _employeeRepository.All().AsNoTracking() on ma.EmployeeId equals e.EmployeeId into eGroup
                        from e in eGroup.DefaultIfEmpty()
                        join at in _attendanceTypeRepository.All().AsNoTracking() on ma.AttendanceTypeCode equals at.AttendanceTypeCode into atGroup
                        from at in atGroup.DefaultIfEmpty()
                        where ma.EmployeeId == employeeId orderby ma.AutoId descending
                        select new ManualAttendanceSetupViewModel
                        {
                            ManualCode = ma.ManualCode,
                            CompanyCode = ma.CompanyCode,
                            EmployeeId = ma.EmployeeId,
                            EmployeeFullName = $"{e.FirstName} {e.LastName}",
                            AttendanceTypeName = at.AttendanceTypeName,
                            DateFrom = ma.Date.ToString("dd/MM/yyyy"),
                            EntryTime = ma.Time,
                            ShowEntryTime = ma.Time.ToString("hh:mm:ss tt"),
                            Remarks = ma.Remarks,
                            Luser = ma.Luser
                        };

            // 🔹 Filtering
            if (!string.IsNullOrEmpty(searchValue))
            {
                query = query.Where(x => x.EmployeeFullName.Contains(searchValue)
                                      || x.EmployeeId.Contains(searchValue)
                                      || x.AttendanceTypeName.Contains(searchValue));
            }

            // 🔹 Sorting
            if (!string.IsNullOrEmpty(sortColumn))
            {
                query = sortColumn switch
                {
                    "manualCode" => sortDirection == "asc" ? query.OrderBy(x => x.ManualCode) : query.OrderByDescending(x => x.ManualCode),
                    "employeeId" => sortDirection == "asc" ? query.OrderBy(x => x.EmployeeId) : query.OrderByDescending(x => x.EmployeeId),
                    "employeeFullName" => sortDirection == "asc" ? query.OrderBy(x => x.EmployeeFullName) : query.OrderByDescending(x => x.EmployeeFullName),
                    "attendanceTypeName" => sortDirection == "asc" ? query.OrderBy(x => x.AttendanceTypeName) : query.OrderByDescending(x => x.AttendanceTypeName),
                    _ => query.OrderByDescending(x => x.ManualCode)
                };
            }

            int totalRecords = await query.CountAsync();
            var data = await query.Skip(skip).Take(pageSize).ToListAsync();

            return (totalRecords, data);
        }

        #endregion


        #region GenerateNextCode
        public async Task<string> GenerateNextCode()
        {
            var codes = await _manualAttendanceRepository.GetAllAsync();
            int nextCode = 1;
            if (codes.Any())
            {
                var lastCode = codes.Max(x => int.TryParse(x.ManualCode, out int parsedCode) ? parsedCode : 0);
                nextCode = lastCode + 1;
            }
            return nextCode.ToString();
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

        public async Task<ShiftTimeDto?> SandRTimeByEmployeeAsync(string employeeId, DateTime formDate)
        {
            try
            {

                var shiftCode = await rosterScheduleEntryRepository.All()
                    .AsNoTracking()
                    .Where(rse => rse.EmployeeId == employeeId && rse.Date == formDate)
                    .Select(rse => rse.ShiftCode)
                    .FirstOrDefaultAsync();

                if (string.IsNullOrEmpty(shiftCode))
                {
                    shiftCode = await offEmpRepository.All()
                        .AsNoTracking()
                        .Where(oi => oi.EmployeeId == employeeId)
                        .Select(oi => oi.ShiftCode).FirstOrDefaultAsync();
                }

                if (!string.IsNullOrEmpty(shiftCode))
                {
                    var shiftTimes = await shiftRepository.All()
                        .Where(x => x.ShiftCode == shiftCode)
                        .Select(c => new ShiftTimeDto
                        {
                            ShiftStartTime = c.ShiftStartTime,
                            ShiftEndTime = c.ShiftEndTime,
                            ShowShiftStartTime = c.ShiftStartTime.ToString("hh:mm:ss tt"),
                            ShowShiftEndTime = c.ShiftEndTime.ToString("hh:mm:ss tt")

                        })
                        .FirstOrDefaultAsync();

                    return shiftTimes;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SandRTimeByEmployeeAsync: {ex.Message}");
                throw;
            }
        }
        #endregion
    }
}

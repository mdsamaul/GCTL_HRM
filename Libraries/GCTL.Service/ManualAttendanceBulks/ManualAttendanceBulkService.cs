using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using GCTL.Core.Data;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.ManualAttendanceBulk;
using GCTL.Data.Models;
using GCTL.Service.DeleteHistories;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;

namespace GCTL.Service.ManualAttendanceBulks
{
    public class ManualAttendanceBulkService : AppService<HrmAtdManual>, IManualAttendanceBulkService
    {
        #region Repositories
        private readonly IRepository<HrmAtdManual> _manualAttendanceBulkRepository;
        private readonly IRepository<CoreAccessCode> _coreAccessCodeRepository;
        private readonly IRepository<HrmEmployee> _employeeRepository;
        private readonly IRepository<HrmAtdAttendanceType> _attendanceTypeRepository;
        private readonly IRepository<CoreCompany> _companyRepository;
        private readonly IRepository<CoreBranch> _branchRepository;
        private readonly IRepository<HrmRosterScheduleEntry> rosterScheduleEntryRepository;
        private readonly IRepository<HrmAtdShift> shiftRepository;
        private readonly IDeleteHistoryService deleteHistoryService;
        private readonly IRepository<HrmAtdMachineData> machineDataRepository;
        private readonly IRepository<HrmDefDesignation> _designationRepository;
        private readonly IRepository<HrmDefDepartment> _departmentRepository;
        private readonly IRepository<HrmEmployeeOfficialInfo> _officialInfoRepository;

        public ManualAttendanceBulkService(
            IRepository<HrmAtdManual> manualAttendanceBulkRepository,
            IRepository<CoreAccessCode> coreAccessCodeRepository,
            IRepository<HrmEmployee> employeeRepository,
            IRepository<CoreCompany> companyRepository,
            IRepository<HrmDefDesignation> designationRepository,
            IRepository<HrmDefDepartment> departmentRepository,
            IRepository<HrmAtdAttendanceType> attendanceTypeRepository,
            IRepository<CoreBranch> branchRepository,
            IRepository<HrmRosterScheduleEntry> RosterScheduleEntryRepository,
            IRepository<HrmAtdShift> ShiftRepository,
           IDeleteHistoryService deleteHistoryService,
            IRepository<HrmAtdMachineData> machineDataRepository,
            IRepository<HrmEmployeeOfficialInfo> officialInfoRepository) : base(manualAttendanceBulkRepository)
        {
            _manualAttendanceBulkRepository = manualAttendanceBulkRepository;
            _coreAccessCodeRepository = coreAccessCodeRepository;
            _employeeRepository = employeeRepository;
            _companyRepository = companyRepository;
            _designationRepository = designationRepository;
            _departmentRepository = departmentRepository;
            _attendanceTypeRepository = attendanceTypeRepository;
            _branchRepository = branchRepository;
            rosterScheduleEntryRepository = RosterScheduleEntryRepository;
            shiftRepository = ShiftRepository;
            this.deleteHistoryService = deleteHistoryService;
            this.machineDataRepository = machineDataRepository;
            _officialInfoRepository = officialInfoRepository;
        }
        #endregion


        #region Permissions
        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await _coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Manual Attendance (Bulk)" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await _coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Manual Attendance (Bulk)" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await _coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Manual Attendance (Bulk)" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await _coreAccessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Manual Attendance (Bulk)" && x.CheckDelete);
        }
        #endregion


        #region GetAllAsync
        
        public async Task<(int totalRecords, List<ManualAttendanceBulkSetupViewModel> data)> GetAllPagedAsync(
    string companyId,
    int skip,
    int pageSize,
    string sortColumn,
    string sortDirection,
    string searchValue)
        {
            var query = from ma in _manualAttendanceBulkRepository.All().AsNoTracking()
                        where ma.AttdEntryType == "Bulk Entry"
                        join e in _employeeRepository.All().AsNoTracking() on ma.EmployeeId equals e.EmployeeId into eGroup
                        from e in eGroup.DefaultIfEmpty()
                        join at in _attendanceTypeRepository.All().AsNoTracking() on ma.AttendanceTypeCode equals at.AttendanceTypeCode into atGroup
                        from at in atGroup.DefaultIfEmpty()
                        select new ManualAttendanceBulkSetupViewModel
                        {
                            ManualCode = ma.ManualCode,
                            BulkEntryId = ma.BulkEntryId,
                            EmployeeId = ma.EmployeeId,
                            EmployeeFullName = (e.FirstName + " " + e.LastName),
                            AttendanceTypeName = at.AttendanceTypeName,
                            DateFrom = ma.Date.ToString("dd/MM/yyyy"),
                            EntryTime = ma.Time,
                            ShowEntryTime = ma.Time.ToString("hh:mm:ss tt"),
                            Remarks = ma.Remarks,
                            Luser = ma.Luser,
                            CompanyCode = ma.CompanyCode
                        };

            if (!string.IsNullOrEmpty(companyId))
            {
                query = query.Where(ma => ma.CompanyCode == companyId);
            }

            if (!string.IsNullOrEmpty(searchValue))
            {
                query = query.Where(x =>
                    x.EmployeeId.Contains(searchValue) ||
                    x.EmployeeFullName.Contains(searchValue) ||
                    x.AttendanceTypeName.Contains(searchValue));
            }

            if (!string.IsNullOrEmpty(sortColumn))
            {
                query = sortColumn switch
                {
                    "manualCode" => sortDirection == "asc" ? query.OrderBy(x => x.ManualCode) : query.OrderByDescending(x => x.ManualCode),
                    "employeeId" => sortDirection == "asc" ? query.OrderBy(x => x.EmployeeId) : query.OrderByDescending(x => x.EmployeeId),
                    "employeeFullName" => sortDirection == "asc" ? query.OrderBy(x => x.EmployeeFullName) : query.OrderByDescending(x => x.EmployeeFullName),
                    _ => query.OrderByDescending(x => x.ManualCode)
                };
            }

            var totalRecords = await query.CountAsync();
            var data = await query.Skip(skip).Take(pageSize).ToListAsync();

            return (totalRecords, data);
        }
        #endregion


        #region GetEmployeeByCompanyId
        public async Task<List<ManualAttendanceBulkSetupViewModel>> GetEmployeeByCompanyId(string companyId = null)
        {
            try
            {
                var result = await (from oi in _officialInfoRepository.All().AsNoTracking()

                                    where oi.CompanyCode == companyId

                                    join c in _companyRepository.All().AsNoTracking() on oi.CompanyCode equals c.CompanyCode

                                    join e in _employeeRepository.All().AsNoTracking() on oi.EmployeeId equals e.EmployeeId into eGroup
                                    from e in eGroup.DefaultIfEmpty()
                                    where oi.EmployeeStatus == "01"
                                    select new ManualAttendanceBulkSetupViewModel
                                    {
                                        EmployeeId = oi.EmployeeId,
                                        EmployeeFullName = $"{e.FirstName} {e.LastName} ({oi.EmployeeId})"
                                    }).Distinct().ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occured in GetEmployeeByCompanyId method: {ex.Message}");
                throw;
            }
        }
        #endregion


        #region GetByCodeAsync
        public async Task<ManualAttendanceBulkSetupViewModel> GetByIdAsync(string code)
        {
            var result = await (from ma in _manualAttendanceBulkRepository.All().AsNoTracking()

                                where ma.EmployeeId == code

                                select new ManualAttendanceBulkSetupViewModel
                                {
                                    EmployeeId = ma.EmployeeId,
                                    DateFrom = ma.Date.ToString("dd/MM/yyyy"),
                                    EntryTime = ma.Time,
                                    Remarks = ma.Remarks
                                }).FirstOrDefaultAsync();
            return result;
        }
        #endregion


        #region SaveAsync

        public async Task<bool> SaveAsync(ManualAttendanceBulkSetupViewModel model, List<string> selectedEmployeeIds)
        {
            await _manualAttendanceBulkRepository.BeginTransactionAsync();
            try
            {
                DateTime dateFrom = model.DateFrom.ToDate();
                DateTime dateTo = model.DateTo.ToDate();
                int noOfDays = (dateTo - dateFrom).Days + 1;

                // 1. Duplicate Delete (bulk)
                var duplicates = await _manualAttendanceBulkRepository
                    .FindByAsync(x => x.Date >= dateFrom &&
                                     x.Date <= dateTo &&
                                     selectedEmployeeIds.Contains(x.EmployeeId));

                if (duplicates?.Any() == true)
                    await _manualAttendanceBulkRepository.DeleteRangeAsync(duplicates);

                // 2. Fetch roster shifts
                var dates = Enumerable.Range(0, noOfDays).Select(i => dateFrom.AddDays(i)).ToList();

                var rosterShifts = await rosterScheduleEntryRepository.All()
                    .Where(rse => selectedEmployeeIds.Contains(rse.EmployeeId) && dates.Contains(rse.Date))
                    .Select(rse => new { rse.EmployeeId, rse.Date, rse.ShiftCode })
                    .ToListAsync();

                // 3. Default shift fetch
                var defaultShifts = await _officialInfoRepository.All()
                    .Where(x => selectedEmployeeIds.Contains(x.EmployeeId))
                    .Select(x => new { x.EmployeeId, x.ShiftCode })
                    .ToListAsync();

                // 4. Shift Codes
                var shiftCodes = rosterShifts.Select(r => r.ShiftCode)
                    .Concat(defaultShifts.Select(d => d.ShiftCode))
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Distinct()
                    .ToList();

                // 5. Shift times dictionary
                var shiftTimes = await shiftRepository.All()
                    .Where(x => shiftCodes.Contains(x.ShiftCode))
                    .Select(x => new { x.ShiftCode, x.ShiftStartTime, x.ShiftEndTime })
                    .ToListAsync();

                var shiftDict = shiftTimes.ToDictionary(s => s.ShiftCode);

                // 6. Manual Code Generate Start
                var lastManualCode = await _manualAttendanceBulkRepository.All()
                    .OrderByDescending(x => x.AutoId)
                    .Select(x => x.ManualCode)
                    .FirstOrDefaultAsync();

                int startingCode = string.IsNullOrEmpty(lastManualCode) ? 1 : Convert.ToInt32(lastManualCode) + 1;
                int currentIndex = 0;

                var recordsToInsert = new List<HrmAtdManual>();

                // 7. Generate Records Parallel
                var results = await Task.WhenAll(
                    selectedEmployeeIds.Select(async emp =>
                    {
                        var empRoster = rosterShifts.Where(r => r.EmployeeId == emp).ToList();
                        var empDefShift = defaultShifts.FirstOrDefault(s => s.EmployeeId == emp)?.ShiftCode;

                        var empRecords = new List<HrmAtdManual>();

                        for (int i = 0; i < noOfDays; i++)
                        {
                            DateTime currentDate = dateFrom.AddDays(i);

                            DateTime entry = model.EntryTime;
                            DateTime exit = model.ExitTime;

                            string shiftCode = empRoster.FirstOrDefault(r => r.Date == currentDate)?.ShiftCode ?? empDefShift;

                            // 👇 Apply roster shift ONLY for AttendanceTypeCode = "3"
                            if (model.AttendanceTypeCode == "3" && !string.IsNullOrEmpty(shiftCode) && shiftDict.ContainsKey(shiftCode))
                            {
                                entry = shiftDict[shiftCode].ShiftStartTime;
                                exit = shiftDict[shiftCode].ShiftEndTime;
                            }

                            //string bulkEntryId = $"{currentDate:yyyyMMdd}-{entry:HHmmss}-{exit:HHmmss}";
                            string bulkEntryId = $"{currentDate:yyyyMMdd}-{entry:HHmmss}";

                            // Entry record
                            empRecords.Add(new HrmAtdManual
                            {
                                ManualCode = (startingCode + Interlocked.Increment(ref currentIndex)).ToString()??"",
                                BulkEntryId = bulkEntryId??"",
                                EmployeeId = emp??"",
                                AttendanceTypeCode = model.AttendanceTypeCode??"",
                                Date = currentDate,
                                Time = entry,
                                AttdEntryType = "Bulk Entry"??"",
                                Remarks = model.Remarks??"",
                                CompanyCode = model.CompanyCode??"",
                                Luser = model.Luser??"",
                                Ldate = DateTime.Now,
                                Lip = model.Lip??"",
                                Lmac = model.Lmac??"",                                
                                ApprovalStatus = string.Empty,
                                ApprovedBy = string.Empty,
                                ApprovalDatetime = null,
                                Latitude = model.Latitude ?? "",
                                Longitude = model.Longitude ?? "",
                                EntryVia = model.EntryVia ?? "",
                                MonthName = currentDate.ToString("MMMM") ?? "",
                                DayName = currentDate.ToString("dddd") ?? "",
                                YearName = currentDate.ToString("yyyy") ?? "",
                            });

                            if (model.ISBothInOutEntry)
                            {
                                empRecords.Add(new HrmAtdManual
                                {
                                    ManualCode = (startingCode + Interlocked.Increment(ref currentIndex)).ToString()??"",
                                    BulkEntryId = bulkEntryId??"",
                                    EmployeeId = emp??"",
                                    AttendanceTypeCode = model.AttendanceTypeCodeTwo??"",
                                    Date = currentDate,
                                    Time = model.AttendanceTypeCode == "3" ? exit : model.ExitTime,
                                    AttdEntryType = "Bulk Entry"??"",
                                    Remarks = model.Remarks??"",
                                    CompanyCode = model.CompanyCode??"",
                                    Luser = model.Luser??"",
                                    Ldate = DateTime.Now,
                                    Lip = model.Lip??"",
                                    Lmac = model.Lmac??"",
                                    ApprovalStatus =  string.Empty,
                                    ApprovedBy =  string.Empty,
                                    ApprovalDatetime = null,
                                    Latitude = model.Latitude ?? "",
                                    Longitude = model.Longitude ?? "",
                                    EntryVia = model.EntryVia ?? "",
                                    MonthName = currentDate.ToString("MMMM") ?? "",
                                    DayName = currentDate.ToString("dddd") ?? "",
                                    YearName = currentDate.ToString("yyyy") ?? "",
                                });
                            }
                        }

                        return empRecords;
                    })
                );

                foreach (var x in results)
                    recordsToInsert.AddRange(x);

                // 8. Batch Insert (5000)
                const int batchSize = 5000;
                for (int i = 0; i < recordsToInsert.Count; i += batchSize)
                    await _manualAttendanceBulkRepository.AddRangeAsync(recordsToInsert.Skip(i).Take(batchSize).ToList());

                await _manualAttendanceBulkRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR: {ex.Message}");
                await _manualAttendanceBulkRepository.RollbackTransactionAsync();
                return false;
            }
        }



        // Helper method to generate codes in batch
        //private async Task<List<string>> GenerateNextCodesBatch(int count)
        //{
        //    var codes = new List<string>(count);

        //    var startingCode = await GetNextAvailableCodeNumber();
        //    for (int i = 0; i < count; i++)
        //    {
        //        codes.Add($"MAN{(startingCode + i):D10}");
        //    }

        //    return codes;
        //}

        //private async Task<long> GetNextAvailableCodeNumber()
        //{

        //    return 1;
        //}
        #endregion


        #region DeleteAsync

       

        public async Task<bool> DeleteAsync(List<string> selectedEmployeeIds, string attendanceTypeCode, string fromDate, string toDate, bool isBothInOutEntry, DeleteHistoryViewModel deleteModel)
        {
            await _manualAttendanceBulkRepository.BeginTransactionAsync();
            try
            {
                if (selectedEmployeeIds == null || !selectedEmployeeIds.Any())
                    return false;

                DateTime dateFrom = fromDate.ToDate();
                DateTime dateTo = toDate.ToDate();

                var query = _manualAttendanceBulkRepository.All()
                    .Where(x => x.Date >= dateFrom &&
                                x.Date <= dateTo &&
                                selectedEmployeeIds.Contains(x.EmployeeId));

                if (!isBothInOutEntry && !string.IsNullOrEmpty(attendanceTypeCode))
                    query = query.Where(x => x.AttendanceTypeCode == attendanceTypeCode);

                const int batchSize = 5000;
                int totalDeleted = 0;

                while (true)
                {
                    var batchIds = await query
                        .Select(x => x.AutoId)
                        .Take(batchSize)
                        .ToListAsync();

                    if (!batchIds.Any())
                        break;

                    var entitiesToDelete = await _manualAttendanceBulkRepository.All()
                        .Where(x => batchIds.Contains(x.AutoId))
                        .ToListAsync();

                    if (entitiesToDelete.Any())
                    {
                        await _manualAttendanceBulkRepository.DeleteRangeAsync(entitiesToDelete);
                        deleteModel.tableName = _manualAttendanceBulkRepository.GetTableName();
                        await deleteHistoryService.LogDeletedRecordsAsync(entitiesToDelete, deleteModel);
               
                        totalDeleted += entitiesToDelete.Count;
                    }
                   
                    if (batchIds.Count < batchSize)
                        break;
                }

                await _manualAttendanceBulkRepository.CommitTransactionAsync();
                return totalDeleted > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete Error: {ex.Message}");
                await _manualAttendanceBulkRepository.RollbackTransactionAsync();
                throw;
            }
        }

        #endregion


        #region GetBranchByCompanyId
        public async Task<List<ManualAttendanceBulkSetupViewModel>> GetBranchByCompanyId(string companyId)
        {
            var result = await (from b in _branchRepository.All().AsNoTracking()

                                where b.CompanyCode == companyId

                                select new ManualAttendanceBulkSetupViewModel
                                {
                                    BranchCode = b.BranchCode,
                                    BranchName = b.BranchName
                                }).ToListAsync();
            return result;
        }
        #endregion


        #region  GetCompanyDataById
        public async Task<List<ManualAttendanceBulkSetupViewModel>> GetCompanyDataById(string companyId)
        {
            var result = await (from ma in _manualAttendanceBulkRepository.All().AsNoTracking()

                                where ma.CompanyCode == companyId

                                join c in _companyRepository.All().AsNoTracking() on ma.CompanyCode equals c.CompanyCode into cGroup
                                from c in cGroup.DefaultIfEmpty()

                                join e in _employeeRepository.All().AsNoTracking() on ma.EmployeeId equals e.EmployeeId into eGroup
                                from e in eGroup.DefaultIfEmpty()

                                join at in _attendanceTypeRepository.All().AsNoTracking() on ma.AttendanceTypeCode equals at.AttendanceTypeCode into atGroup
                                from at in atGroup.DefaultIfEmpty()

                                select new ManualAttendanceBulkSetupViewModel
                                {
                                    ManualCode = ma.ManualCode,
                                    CompanyCode = ma.CompanyCode,
                                    EmployeeId = ma.EmployeeId,
                                    EmployeeFullName = $"{e.FirstName} {e.LastName}",
                                    AttendanceTypeName = at.AttendanceTypeName,
                                    DateFrom = ma.Date.ToString("yyyyyy/MM/dd"),
                                    EntryTime = ma.Time,
                                    Remarks = ma.Remarks,
                                    Luser = ma.Luser
                                }).ToListAsync();
            return result;
        }
        #endregion


        #region GetDepartmentByCompanyId
        public async Task<List<ManualAttendanceBulkSetupViewModel>> GetDepartmentByCompanyId(string companyId)
        {
            var result = await (from d in _departmentRepository.All().AsNoTracking()

                                where d.CompanyCode == companyId

                                join c in _companyRepository.All().AsNoTracking() on d.CompanyCode equals c.CompanyCode into cGroup
                                from c in cGroup.DefaultIfEmpty()

                                select new ManualAttendanceBulkSetupViewModel
                                {
                                    DepartmentCode = d.DepartmentCode,
                                    DepartmentName = d.DepartmentName
                                }).ToListAsync();
            return result;
        }
        #endregion


        #region GetDepartmentByBranchId
        public async Task<List<ManualAttendanceBulkSetupViewModel>> GetDepartmentByBranchId(string branchId)
        {
            if (branchId == null || !branchId.Any())
            {
                return new List<ManualAttendanceBulkSetupViewModel>();
            }

            var result = await (from oi in _officialInfoRepository.All().AsNoTracking()

                                where oi.BranchCode == branchId

                                join d in _departmentRepository.All().AsNoTracking() on oi.DepartmentCode equals d.DepartmentCode into dGroup
                                from d in dGroup.DefaultIfEmpty()

                                join b in _branchRepository.All().AsNoTracking() on oi.BranchCode equals b.BranchCode into bGroup
                                from b in bGroup.DefaultIfEmpty()

                                select new ManualAttendanceBulkSetupViewModel
                                {
                                    DepartmentCode = oi.DepartmentCode,
                                    DepartmentName = d.DepartmentName
                                }).Distinct().ToListAsync();
            return result;
        }
        #endregion


        #region GetDesignationByBranchId
        public async Task<List<ManualAttendanceBulkSetupViewModel>> GetDesignationByBranchId(string branchId)
        {
            if (branchId == null || !branchId.Any())
            {
                return new List<ManualAttendanceBulkSetupViewModel>();
            }

            var result = await (from oi in _officialInfoRepository.All().AsNoTracking()

                                where oi.BranchCode == branchId

                                join d in _designationRepository.All().AsNoTracking() on oi.DesignationCode equals d.DesignationCode into dGroup
                                from d in dGroup.DefaultIfEmpty()

                                join b in _branchRepository.All().AsNoTracking() on oi.BranchCode equals b.BranchCode into bGroup
                                from b in bGroup.DefaultIfEmpty()

                                select new ManualAttendanceBulkSetupViewModel
                                {
                                    DesignationCode = oi.DesignationCode,
                                    DesignationName = d.DesignationName
                                }).Distinct().ToListAsync();
            return result;
        }
        #endregion


        #region GetEmployeeByBranchId
        public async Task<List<ManualAttendanceBulkSetupViewModel>> GetEmployeeByBranchId(string companyId, string branchId)
        {
            if (branchId == null || !branchId.Any())
            {
                return new List<ManualAttendanceBulkSetupViewModel>();
            }

            var result = await (from oi in _officialInfoRepository.All().AsNoTracking()

                                    //where oi.CompanyCode == companyId && oi.BranchCode == branchId
                                where (string.IsNullOrEmpty(companyId) || oi.CompanyCode == companyId) && (string.IsNullOrEmpty(branchId) || oi.BranchCode == branchId)

                                join e in _employeeRepository.All().AsNoTracking() on oi.EmployeeId equals e.EmployeeId into eGroup
                                from e in eGroup.DefaultIfEmpty()

                                join br in _branchRepository.All().AsNoTracking() on oi.BranchCode equals br.BranchCode into brGroup
                                from br in brGroup.DefaultIfEmpty()

                                select new ManualAttendanceBulkSetupViewModel
                                {
                                    EmployeeId = oi.EmployeeId
                                }).Distinct().ToListAsync();

            return result;
        }
        #endregion


        #region GetDesignationByCompanyId 
        public async Task<List<ManualAttendanceBulkSetupViewModel>> GetDesignationByCompanyId(string companyId)
        {
            var result = await (from d in _designationRepository.All().AsNoTracking()

                                where d.CompanyCode == companyId

                                join c in _companyRepository.All().AsNoTracking() on d.CompanyCode equals c.CompanyCode into cGroup
                                from c in cGroup.DefaultIfEmpty()

                                select new ManualAttendanceBulkSetupViewModel
                                {
                                    DesignationCode = d.DesignationCode,
                                    DesignationName = d.DesignationName
                                }).ToListAsync();
            return result;
        }
        #endregion


        #region GetDesignationByDepartmentId
        public async Task<List<ManualAttendanceBulkSetupViewModel>> GetDesignationByDepartmentId(List<string> departmentId)
        {
            var result = await (from oi in _officialInfoRepository.All().AsNoTracking()

                                where departmentId.Contains(oi.DepartmentCode)

                                join des in _designationRepository.All().AsNoTracking() on oi.DesignationCode equals des.DesignationCode into desGroup
                                from des in desGroup.DefaultIfEmpty()

                                join d in _departmentRepository.All().AsNoTracking() on oi.DepartmentCode equals d.DepartmentCode into dGroup
                                from d in dGroup.DefaultIfEmpty()

                                select new ManualAttendanceBulkSetupViewModel
                                {
                                    DesignationCode = oi.DesignationCode,
                                    DesignationName = des.DesignationName
                                }).Distinct().ToListAsync();
            return result;
        }
       
        #endregion


        #region GetEmployeeByDepartmentId
        public async Task<List<ManualAttendanceBulkSetupViewModel>> GetEmployeeByDepartmentId(string companyId, string branchId, List<string> departmentId, string selectedListType, string selectedActiveStatus)
        {
            if (departmentId == null || !departmentId.Any())
            {
                return new List<ManualAttendanceBulkSetupViewModel>();
            }

            var result = await (from oi in _officialInfoRepository.All().AsNoTracking()

                                    //where oi.CompanyCode == companyId && oi.BranchCode == branchId && oi.DepartmentCode == departmentId
                                where (string.IsNullOrEmpty(companyId) || oi.CompanyCode == companyId) &&
                                (string.IsNullOrEmpty(branchId) || oi.BranchCode == branchId) &&
                                (departmentId.Contains(oi.DepartmentCode))&&
                                (string.IsNullOrEmpty(selectedActiveStatus) || oi.EmployeeStatus == selectedActiveStatus)
                                //(string.IsNullOrEmpty(departmentId) || oi.DepartmentCode == departmentId)

                                join e in _employeeRepository.All().AsNoTracking() on oi.EmployeeId equals e.EmployeeId into eGroup
                                from e in eGroup.DefaultIfEmpty()

                                join dep in _departmentRepository.All().AsNoTracking() on oi.DepartmentCode equals dep.DepartmentCode into depGroup
                                from dep in depGroup.DefaultIfEmpty()
                                join shif in shiftRepository.All().AsNoTracking() on oi.ShiftCode equals shif.ShiftCode into shiftGroup
                                from shif in shiftGroup.DefaultIfEmpty()

                                select new ManualAttendanceBulkSetupViewModel
                                {
                                    EmployeeId = oi.EmployeeId,
                                    shift = shif.ShiftName,
                                    InTime = shif.ShiftStartTime,
                                    OutTime = shif.ShiftEndTime,
                                    ShowInTime= shif.ShiftStartTime.ToString("hh:mm:ss tt"),
                                    ShowOutTime= shif.ShiftEndTime.ToString("hh:mm:ss tt")

                                }).Distinct().ToListAsync();

            return result;
        }
        #endregion


        #region GetEmployeeByDesignationId
       

        public async Task<List<ManualAttendanceBulkSetupViewModel>> GetEmployeeByDesignationId(
    string companyId,
    string branchId,
    List<string> departmentId,
    List<string> designationId,
    string selectedListType,
    string selectedActiveStatus)
        {
            try
            {
                var query = from oi in _officialInfoRepository.All().AsNoTracking()
                            where
                            (string.IsNullOrEmpty(companyId) || oi.CompanyCode == companyId) &&
                            (string.IsNullOrEmpty(branchId) || oi.BranchCode == branchId) &&
                            (departmentId == null || departmentId.Count == 0 || departmentId.Contains(oi.DepartmentCode)) &&
                            (designationId == null || designationId.Count == 0 || designationId.Contains(oi.DesignationCode)) &&
                            (string.IsNullOrEmpty(selectedActiveStatus) || oi.EmployeeStatus == selectedActiveStatus)

                            join e in _employeeRepository.All().AsNoTracking()
                                on oi.EmployeeId equals e.EmployeeId into eGroup
                            from e in eGroup.DefaultIfEmpty()

                            join des in _designationRepository.All().AsNoTracking()
                                on oi.DesignationCode equals des.DesignationCode into desGroup
                            from des in desGroup.DefaultIfEmpty()

                            join shif in shiftRepository.All().AsNoTracking()
                                on oi.ShiftCode equals shif.ShiftCode into shiftGroup
                            from shif in shiftGroup.DefaultIfEmpty()

                            select new ManualAttendanceBulkSetupViewModel
                            {
                                EmployeeId = oi.EmployeeId,
                                shift = shif != null ? shif.ShiftName : "",

                                InTime = shif != null ? shif.ShiftStartTime : null,
                                OutTime = shif != null ? shif.ShiftEndTime : null,

                                ShowInTime = "",
                                ShowOutTime = ""
                            };

                var result = await query.Distinct().ToListAsync();

                Parallel.ForEach(result, item =>
                {
                    item.ShowInTime = item.InTime?.ToString("hh:mm:ss tt") ?? "";
                    item.ShowOutTime = item.OutTime?.ToString("hh:mm:ss tt") ?? "";
                });

                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion


        #region GetEmployeeDetailsById
        public async Task<ManualAttendanceBulkSetupViewModel> GetEmployeeDetailsById(string id)
        {
            var result = await (from e in _employeeRepository.All().AsNoTracking()

                                    //join des in _designationRepository.All().AsNoTracking() on e.DesignationCode equals des.DesignationCode into desGroup
                                    //from des in desGroup.DefaultIfEmpty()

                                    //join dep in _departmentRepository.All().AsNoTracking() on e.DepartmentCode equals dep.DepartmentCode into depGroup
                                    //from dep in depGroup.DefaultIfEmpty()

                                where e.EmployeeId == id

                                select new ManualAttendanceBulkSetupViewModel
                                {
                                    EmployeeFullName = $"{e.FirstName} {e.LastName}",
                                    //DesignationName = des.DesignationName,
                                    //DepartmentName = dep.DepartmentName
                                }).FirstOrDefaultAsync();
            return result;
        }
        #endregion


        #region GetEmployeeDataById
        public async Task<List<ManualAttendanceBulkSetupViewModel>> GetEmployeeDataById(string employeeId)
        {
            var result = await (from ma in _manualAttendanceBulkRepository.All().AsNoTracking()

                                join c in _companyRepository.All().AsNoTracking() on ma.CompanyCode equals c.CompanyCode into cGroup
                                from c in cGroup.DefaultIfEmpty()

                                join e in _employeeRepository.All().AsNoTracking() on ma.EmployeeId equals e.EmployeeId into eGroup
                                from e in eGroup.DefaultIfEmpty()

                                join at in _attendanceTypeRepository.All().AsNoTracking() on ma.AttendanceTypeCode equals at.AttendanceTypeCode into atGroup
                                from at in atGroup.DefaultIfEmpty()

                                where ma.EmployeeId == employeeId

                                select new ManualAttendanceBulkSetupViewModel
                                {
                                    ManualCode = ma.ManualCode,
                                    CompanyCode = ma.CompanyCode,
                                    EmployeeId = ma.EmployeeId,
                                    EmployeeFullName = $"{e.FirstName} {e.LastName}",
                                    AttendanceTypeName = at.AttendanceTypeName,
                                    DateFrom = ma.Date.ToString("yyyy/MM/dd"),
                                    EntryTime = ma.Time,
                                    Remarks = ma.Remarks,
                                    Luser = ma.Luser
                                }).ToListAsync();
            return result;
        }
        #endregion


        #region GenerateNextCode
        public async Task<string> GenerateNextCode()
        {
            var codes = await _manualAttendanceBulkRepository.GetAllAsync();
            int nextCode = 1;
            if (codes.Any())
            {
                var lastCode = codes.Max(x => int.TryParse(x.ManualCode, out int parsedCode) ? parsedCode : 0);
                nextCode = lastCode + 1;
            }
            return nextCode.ToString();
        }
        #endregion


        #region GenerateNextBulkEntryCode
        public string GenerateNextBulkEntryCode(ManualAttendanceBulkSetupViewModel model)
        {
            //var code = model.DateFrom.ToDate().ToString("yyyyMMdd") + "-" + model.EntryTime.ToString("hhmmss") + "-" + model.ExitTime.ToString("hhmmss") ?? "";
            var code = model.DateFrom.ToDate().ToString("yyyyMMdd") + "-" + model.EntryTime.ToString("hhmmss") + "-" + model.ExitTime.ToString("hhmmss") ?? "";
            return code;
        }
        #endregion
    }
}
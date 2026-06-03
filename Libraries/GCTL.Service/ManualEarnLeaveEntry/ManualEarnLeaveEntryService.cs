using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Bibliography;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HRM_EmployeeWeekendDeclaration;
using GCTL.Core.ViewModels.ManualEarnLeaveEntry;
using GCTL.Data.Models;
using GCTL.Service.DeleteHistories;
using GCTL.Service.Employees;
using GCTL.Service.EmployeeWeekendDeclaration;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace GCTL.Service.ManualEarnLeaveEntry
{
    public class ManualEarnLeaveEntryService : AppService<HrmEarnLeaveEntry>, IManualEarnLeaveEntryService
    {
        private readonly IRepository<HrmEarnLeaveEntry> earnLeaveEntryRepository;
        private readonly GCTL_ERP_DB_DatapathContext _context;
        private readonly IRepository<HrmEmployee> employeeRepo;
        private readonly IRepository<HrmEmployeeOfficialInfo> empOffRepo;
        private readonly IRepository<HrmDefDesignation> desiRepo;
        private readonly IRepository<HrmDefDepartment> depRepo;
        private readonly IRepository<HrmDefDivision> divRepo;
        private readonly IRepository<CoreBranch> branchRepo;
        private readonly IRepository<CoreCompany> companyRepo;
        private readonly IRepository<HrmDefEmpType> defEmpTypeRepo;
        private readonly IRepository<HrmEisDefEmploymentNature> empNatureRepo;
        private readonly IRepository<HrmDefEmployeeStatus> empStatusRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly IRepository<HrmSeparation> SeparationRepo;
        private readonly IDeleteHistoryService deleteHistoryService;

        public ManualEarnLeaveEntryService(IRepository<HrmEarnLeaveEntry> earnLeaveEntryRepository, 
            GCTL_ERP_DB_DatapathContext context,
          IRepository<HrmEmployee> employeeRepo,
          IRepository<HrmEmployeeOfficialInfo> empOffRepo,
          IRepository<HrmDefDesignation> desiRepo,
          IRepository<HrmDefDepartment> depRepo,
          IRepository<HrmDefDivision> divRepo,
          IRepository<CoreBranch> branchRepo,
          IRepository<CoreCompany> companyRepo,
          IRepository<HrmDefEmpType> defEmpTypeRepo,
          IRepository<HrmEisDefEmploymentNature> empNatureRepo,
          IRepository<HrmDefEmployeeStatus> empStatusRepo,
          IRepository<CoreAccessCode> accessCodeRepository,
          IRepository<HrmSeparation> SeparationRepo,
            IDeleteHistoryService deleteHistoryService
            ):base(earnLeaveEntryRepository)
        {
            this.earnLeaveEntryRepository = earnLeaveEntryRepository;
            _context = context;
            this.employeeRepo = employeeRepo;
            this.empOffRepo = empOffRepo;
            this.desiRepo = desiRepo;
            this.depRepo = depRepo;
            this.divRepo = divRepo;
            this.branchRepo = branchRepo;
            this.companyRepo = companyRepo;
            this.defEmpTypeRepo = defEmpTypeRepo;
            this.empNatureRepo = empNatureRepo;
            this.empStatusRepo = empStatusRepo;
            this.accessCodeRepository = accessCodeRepository;
            this.SeparationRepo = SeparationRepo;
            this.deleteHistoryService = deleteHistoryService;
        }

        #region Permission all type

        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Earn Leave" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Earn Leave" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Earn Leave" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Earn Leave" && x.CheckDelete);
        }

        #endregion
        public async Task<ManualEarnLeaveEntryEmployeeFilterListDto> GetFilterDataAsync(ManualEarnLeaveEntryEmployeeFilterDto filter)
        {          

            var query = from e in employeeRepo.All()
                        join eoi in empOffRepo.All() on e.EmployeeId equals eoi.EmployeeId
                        join dg in desiRepo.All() on eoi.DesignationCode equals dg.DesignationCode into dg_join
                        from dg in dg_join.DefaultIfEmpty()
                        join dp in depRepo.All() on eoi.DepartmentCode equals dp.DepartmentCode into dp_join
                        from dp in dp_join.DefaultIfEmpty()
                        join dv in divRepo.All() on eoi.DivisionCode equals dv.DivisionCode into dv_join
                        from dv in dv_join.DefaultIfEmpty()
                        join cb in branchRepo.All() on eoi.BranchCode equals cb.BranchCode into cb_join
                        from cb in cb_join.DefaultIfEmpty()
                        join cc in companyRepo.All() on eoi.CompanyCode equals cc.CompanyCode into cc_join
                        from cc in cc_join.DefaultIfEmpty()
                        join et in defEmpTypeRepo.All() on eoi.EmpTypeCode equals et.EmpTypeCode into et_join
                        from et in et_join.DefaultIfEmpty()
                        join en in empNatureRepo.All() on eoi.EmploymentNatureId equals en.EmploymentNatureId into en_join
                        from en in en_join.DefaultIfEmpty()
                        join empStatus in empStatusRepo.All() on eoi.EmployeeStatus equals empStatus.EmployeeStatusId into empStatus_join
                        from empStatus in empStatus_join.DefaultIfEmpty()
                        join sp in SeparationRepo.All() on e.EmployeeId equals sp.EmployeeId into sp_join
                        from sp in sp_join.DefaultIfEmpty()

               select new 
                        {
                            EmpID = e.EmployeeId,
                            EmployeeName = e.FirstName??"" + e.LastName??"",
                            CompanyCode = eoi.CompanyCode,
                            BranchCode= cb.BranchCode,
                            DivisionCode = dv.DivisionCode,
                            DivisionName = dv.DivisionName,
                            DesignationCode = dg.DesignationCode,
                            DesignationName = dg.DesignationName,
                            DepartmentCode = dp.DepartmentCode,
                            DepartmentName = dp.DepartmentName,
                            BranchName = cb.BranchName,
                            CompanyName = cc.CompanyName,
                            EmployeeStatusCode = eoi.EmployeeStatus,
                            EmployeeTypeName = et.EmpTypeName,
                            EmploymentNature = en.EmploymentNature,
                            JoiningDate = eoi.JoiningDate,
                            EndDate = (eoi.EmployeeStatus == "01" ? DateTime.Now : sp.SeparationDate),
                            DaysInYear = ((((eoi.EmployeeStatus == "01" ? DateTime.Now : sp.SeparationDate).Year % 4 == 0)
                     && ((eoi.EmployeeStatus == "01" ? DateTime.Now : sp.SeparationDate).Year % 100 != 0))
                     || ((eoi.EmployeeStatus == "01" ? DateTime.Now : sp.SeparationDate).Year % 400 == 0)) ? 366 : 365,
                            ServiceDuration = (eoi.JoiningDate.HasValue ?
                       ((DateTime.Now.Year - eoi.JoiningDate.Value.Year) +
                       ((DateTime.Now.Month - eoi.JoiningDate.Value.Month) / 12.0) +
                       ((DateTime.Now.Day - eoi.JoiningDate.Value.Day) /
                       (DateTime.IsLeapYear(DateTime.Now.Year) ? 366.0 : 365.0)))
                       : 0),
                            SeparationDate = sp.SeparationDate,
                            ServiceDuration2 = eoi.JoiningDate.HasValue
                         ? (DateTime.Now.Year - eoi.JoiningDate.Value.Year).ToString() + "Y" +
                           (DateTime.Now.Month - eoi.JoiningDate.Value.Month).ToString() + "M-" +
                           (DateTime.Now.Day - eoi.JoiningDate.Value.Day).ToString() + "D"
                         : "N/A"
                        };

            if(filter.CompanyCodes?.Any() == true)
            {
                query = query.Where(x => x.CompanyCode != null && filter.CompanyCodes.Contains(x.CompanyCode));
            }
            if(filter.BranchCodes?.Any() == true)
            {
                query = query.Where(x => x.BranchCode != null & filter.BranchCodes.Contains(x.BranchCode));
            }
            if (filter.DivisionCodes?.Any() == true)
            {
                query = query.Where(x => x.DivisionCode != null && filter.DivisionCodes.Contains(x.DivisionCode));
            }
            if (filter.DepartmentCodes?.Any()== true)
            {
                query = query.Where(x => x.DepartmentCode != null && filter.DepartmentCodes.Contains(x.DepartmentCode));
            }
            if (filter.DesignationCodes?.Any() == true)
            {
                query = query.Where(x => x.DesignationCode != null && filter.DesignationCodes.Contains(x.DesignationCode));
            }
            if (filter.EmployeeIDs?.Any() == true)
            {
                query = query.Where(x=>x.EmpID != null && filter.EmployeeIDs.Contains(x.EmpID));
            }
            if (filter.EmployeeStatuses?.Any() == true)
            {
                query = query.Where(x=>x.EmployeeStatusCode != null && filter.EmployeeStatuses.Contains(x.EmployeeStatusCode));
            }

            var result = new ManualEarnLeaveEntryEmployeeFilterListDto
            {
                Companies = await query.Where(x => x.CompanyCode != null && x.CompanyName != null)
                .Select(x => new ManualEarnLeaveEntryEmployeeFilterResultDto { Code = x.CompanyCode, Name = x.CompanyName })
                .Distinct().ToListAsyncSafe(),

                Branches = await query.Where(x => x.BranchCode != null && x.BranchName != null)
                .Select(x => new ManualEarnLeaveEntryEmployeeFilterResultDto { Code = x.BranchCode, Name = x.BranchName })
                .Distinct().ToListAsyncSafe(),

                Divisions = await query.Where(x => x.DivisionCode != null && x.DivisionName != null)
                .Select(x => new ManualEarnLeaveEntryEmployeeFilterResultDto { Code = x.DivisionCode, Name = x.DivisionName })
                .Distinct().ToListAsyncSafe(),
                Departments = await query.Where(x => x.DepartmentCode != null && x.DepartmentName != null)
                .Select(x => new ManualEarnLeaveEntryEmployeeFilterResultDto { Code = x.DepartmentCode, Name = x.DepartmentName })
                .Distinct().ToListAsyncSafe(),
                Designations = await query.Where(x => x.DesignationCode != null && x.DesignationName != null)
                .Select(x => new ManualEarnLeaveEntryEmployeeFilterResultDto { Code = x.DesignationCode, Name = x.DesignationName })
                .Distinct().ToListAsyncSafe(),
                Employees = await query.Where(x => x.EmpID != null && x.EmployeeName != null)
                .Select(x => new ManualEarnLeaveEntryEmployeeFilterResultDto
                {
                    Code = x.EmpID??"",
                    Name = x.EmployeeName??"",
                    Designation = x.DesignationName??"",
                    Department = x.DepartmentName??"",
                    Branch = x.BranchName??"",
                    Company = x.CompanyName??"",
                    EmployeeType = x.EmployeeTypeName??"",
                    EmploymentNature = x.EmploymentNature??"",
                    JoiningDate = x.JoiningDate != null ? x.JoiningDate.Value.ToString("dd/MM/yyyy"):""??"",
                    SeparationDate = x.SeparationDate,
                    ServiceDuration = x.ServiceDuration,
                    ServiceDuration2 = x.ServiceDuration2??""
                }).Distinct().ToListAsyncSafe(),

                ActivityStatuses = await query.Where(x => x.EmployeeTypeName != null)
                .Select(x => new ManualEarnLeaveEntryEmployeeFilterResultDto { Code = x.EmployeeStatusCode, Name = x.EmployeeTypeName })
                .Distinct().ToListAsyncSafe(),
            };
            return result;
        }
       public async Task<(bool isSuccess, string message, object data)> SaveUpdateEarnLeaveServices(ManualEarnLeaveEntryEmployeeCreateDto FromData)
        {
            if(FromData == null || FromData.Year == null || FromData.GrantedLeaveDays== null|| FromData.AvailedLeaveDays == null || FromData.BalancedLeaveDays == null )
            {
                return (false, "Missing dates or Granted Leave Day(.", null);
            }
            if (int.Parse(FromData.Year) < 1900 || int.Parse(FromData.Year) > 2100)
            {
                return (false, $"Invalid year value: '{FromData.Year}' is out of allowed range (1900-2100).", null);
            }
            try
            {
                if (FromData.EarnLeaveID == null)
                {
                    //if (FromData.EmployeeID == null || !FromData.EmployeeID.Any() || !FromData.Year.Any())
                    if (string.IsNullOrWhiteSpace(FromData.Year) || FromData.EmployeeID == null || !FromData.EmployeeID.Any())
                    {
                        return (false, "Invalid data received: Missing dates or employee IDs.", null);
                    }
                    var records = new List<HrmEarnLeaveEntry>();
                    var lastDeclaration = await earnLeaveEntryRepository.All()
                        .OrderByDescending(x => x.EarnLeaveId)
                        .FirstOrDefaultAsync();
                    int nextIdNumber = 1;
                    if (lastDeclaration != null && !string.IsNullOrEmpty(lastDeclaration.EarnLeaveId))
                    {
                        if (int.TryParse(lastDeclaration.EarnLeaveId, out int lastNumber))
                        {
                            nextIdNumber = lastNumber + 1;
                        }
                        else
                        {
                            nextIdNumber = 1;
                        }
                    }
                    foreach (var empId in FromData.EmployeeID)
                    {
                        //bool aleadyExists = earnLeaveEntryRepository.GetAll().Any(x => x.EmployeeID == empId);
                        //bool isEmpInclude = employeeRepo.GetAll().Any(x => FromData.EmployeeID.Contains(x.EmployeeId ));
                        bool isEmpInclude = employeeRepo.GetAll().Any(x => x.EmployeeId == empId);

                        //var employee = earnLeaveEntryRepository.GetAll().Any(x => x.EmployeeID.Contains((x.EmployeeID) && (x.Year == FromData.Year)));

                        bool existsEmployee = earnLeaveEntryRepository.GetAll().Any(x => x.EmployeeId == empId && x.Year == FromData.Year);
                        if (existsEmployee)
                        {
                            //var employees = earnLeaveEntryRepository.GetAll();
                            var employee = earnLeaveEntryRepository.GetAll().Where(x => x.EmployeeId == empId && x.Year == FromData.Year).ToList();
                            //var employee = earnLeaveEntryRepository.GetAll().FirstOrDefault(x => x.EmployeeID == FromData.EmpId);

                            await earnLeaveEntryRepository.DeleteRangeAsync(employee);
                        }

                        if (isEmpInclude)
                        {
                            string newEarnLeaveID = $"{nextIdNumber:D8}";
                            nextIdNumber++;
                            records.Add(new HrmEarnLeaveEntry
                            {
                                EarnLeaveId = newEarnLeaveID,
                                EmployeeId = empId,
                                Year = FromData.Year,
                                GrantedLeaveDays = FromData.GrantedLeaveDays,
                                AvailedLeaveDays = FromData.AvailedLeaveDays,
                                BalancedLeaveDays = FromData.BalancedLeaveDays,
                                Remarks = FromData.Remarks ?? "",
                                CompanyCode = FromData.CompanyCode ?? "",
                                Ldate = FromData.Ldate,
                                Luser = FromData.Luser,
                                Lip = FromData.Lip,
                                Lmac = FromData.Lmac,
                            });
                        }

                    }
                    if (!records.Any())
                    {
                        return (false, "Save Failed", null);
                    }
                    await earnLeaveEntryRepository.AddRangeAsync(records);

                    return (true, "Save SuccessFully", records);
                }
                else
                {
                    var emp = earnLeaveEntryRepository.GetAll().Where(x=>x.EarnLeaveId== FromData.EarnLeaveID).FirstOrDefault();
                    if (emp != null) { 
                    emp.Year=FromData.Year;
                        emp.GrantedLeaveDays=FromData.GrantedLeaveDays;
                        emp.AvailedLeaveDays=FromData.AvailedLeaveDays;
                        emp.BalancedLeaveDays=FromData.BalancedLeaveDays;
                        emp.Remarks=FromData.Remarks;
                        emp.ModifyDate=FromData.ModifyDate;
                        earnLeaveEntryRepository.Update(emp);
                    };
                    return (isSuccess:true, message : "Update Successfully", data:emp);
                }
            }
            catch (DbUpdateException dbEx)
            {
                return (false, "An error occurred while saving data to the database.", dbEx.InnerException?.Message ?? dbEx.Message);
            }
            catch (Exception ex) {
                return (false, "An unexpected error occurred while saving data.", ex.Message);
            }
        }
        public async Task<List<ManualEarnLeaveEntryEmployeeCreateDto>> GetEarnLeaveEmployeeService()
        {
            var data = earnLeaveEntryRepository.GetAll().
                Join(employeeRepo.GetAll(), x => x.EmployeeId,
                e => e.EmployeeId,
                (x, e) => new { x, e }).GroupJoin(empOffRepo.GetAll(),
                ee => ee.x.EmployeeId,
                ei => ei.EmployeeId,
                (ee, eiJoin) => new { ee, eiJoin })
                .SelectMany(x => x.eiJoin.DefaultIfEmpty(),
                (x, ei) => new { x.ee, ei })
                .GroupJoin(desiRepo.GetAll(),
                x => x.ei.DesignationCode,
                dd => dd.DesignationCode,
                (x, ddJoin) => new { x.ee, x.ei, ddJoin })
                .SelectMany(x => x.ddJoin.DefaultIfEmpty(),
                (x, dd) => new ManualEarnLeaveEntryEmployeeCreateDto
                {
                    AutoId = x.ee.x.AutoId,
                    EarnLeaveID = x.ee.x.EarnLeaveId??"",
                    EmpId = x.ee.e.EmployeeId??"",
                    EmployeeName = (x.ee.e.FirstName ?? "") + " " + (x.ee.e.LastName ?? ""),
                    Designation = dd != null ? (dd.DesignationName ?? "") : "",
                    Year = x.ee.x.Year??"",
                    AvailedLeaveDays = x.ee.x.AvailedLeaveDays,
                    GrantedLeaveDays = x.ee.x.GrantedLeaveDays,
                    BalancedLeaveDays = x.ee.x.BalancedLeaveDays,
                    Remarks = x.ee.x.Remarks??"",
                    EntryUser = x.ee.x.Luser??"",
                });
            var result = data.OrderByDescending(x => x.EarnLeaveID).ToList();
            return await Task.FromResult(result);
        }

        //download excel sheet
        public async Task<byte[]> GenerateEmpEarnLeaveExcelDownload()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var employees = earnLeaveEntryRepository.GetAll().ToList();
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Employees");

                worksheet.Cells[1, 1].Value = "EmployeeId";
                worksheet.Cells[1, 2].Value = "Year";
                worksheet.Cells[1, 3].Value = "Granted Leave Day (s)";
                worksheet.Cells[1, 4].Value = "Availed Leave Day (s)";
                worksheet.Cells[1, 5].Value = "Balanced Leave Day (s)";
                worksheet.Cells[1, 6].Value = "Remarks";
                
                worksheet.Column(1).Style.Numberformat.Format = "@";
                worksheet.Column(2).Style.Numberformat.Format = "@";
                int row = 2;
                foreach (var employee in employees)
                {
                    worksheet.Cells[row, 1].Value = employee.EmployeeId;
                    worksheet.Cells[row, 2].Value = employee.Year;
                    worksheet.Cells[row, 3].Value = employee.GrantedLeaveDays;
                    worksheet.Cells[row, 4].Value = employee.AvailedLeaveDays;
                    worksheet.Cells[row, 5].Value = employee.BalancedLeaveDays;
                    worksheet.Cells[row, 6].Value = employee.Remarks;
                    row++;
                }
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                return package.GetAsByteArray();
            }
        }

        //import excel file 
        public async Task<(bool isSuccess, string message, object data)> SaveEarnLeaveExcel(ManualEarnLeaveEntryEmployeeCreateDto model)
        {
            if (model == null || model.EmployeeID == null || model.AvailedLeaveDaysList == null || model.BalancedLeaveDaysList == null || model.GrantedLeaveDaysList == null
                || !model.AvailedLeaveDaysList.Any() || !model.BalancedLeaveDaysList.Any() || !model.GrantedLeaveDaysList.Any())
            {
                return (false, "Invalid data received: Missing data", null);
            }

            try
            {
                var records = new List<HrmEarnLeaveEntry>();
                var lastDeclaration = await earnLeaveEntryRepository.All().OrderByDescending(x => x.EarnLeaveId).FirstOrDefaultAsync();

                int nextIdNumber = 1;
                if (lastDeclaration != null && !string.IsNullOrEmpty(lastDeclaration.EarnLeaveId))
                {
                    if (int.TryParse(lastDeclaration.EarnLeaveId, out int lastNumber))
                    {
                        nextIdNumber = lastNumber + 1;
                    }
                }

                int totalRows = new[]
                {
            model.EmployeeID.Count,
            model.YearList.Count,
            model.GrantedLeaveDaysList.Count,
            model.AvailedLeaveDaysList.Count,
            model.BalancedLeaveDaysList.Count,
            model.RemarksList?.Count ?? 0 // null check
        }.Min();

                var existingEmployeeIds = employeeRepo.GetAll().Select(x => x.EmployeeId).ToHashSet();

                for (int i = 0; i < totalRows; i++)
                {
                    var empId = model.EmployeeID[i];
                    var year = model.YearList[i];
                    var grantedLeaveDay = model.GrantedLeaveDaysList[i];
                    var availedLeaveDay = model.AvailedLeaveDaysList[i];
                    var balancedLeaveDay = model.BalancedLeaveDaysList[i];
                    var remark = (model.RemarksList != null && i < model.RemarksList.Count) ? model.RemarksList[i] : "";

                   
                    if (existingEmployeeIds.Contains(empId))
                    {
                        // check if employee record already exists for same year
                        bool existsEmployee = earnLeaveEntryRepository.GetAll().Any(x => x.EmployeeId == empId && x.Year == year);
                        if (existsEmployee)
                        {
                            var employeesToDelete = earnLeaveEntryRepository.GetAll().Where(x => x.EmployeeId == empId && x.Year == year).ToList();
                            await earnLeaveEntryRepository.DeleteRangeAsync(employeesToDelete);
                        }

                        string newEarnLeaveID = $"{nextIdNumber:D8}";
                        nextIdNumber++;

                        records.Add(new HrmEarnLeaveEntry
                        {
                            EarnLeaveId = newEarnLeaveID,
                            EmployeeId = empId,
                            Year = year,
                            GrantedLeaveDays = grantedLeaveDay,
                            AvailedLeaveDays = availedLeaveDay,
                            BalancedLeaveDays = balancedLeaveDay,
                            Remarks = remark ?? "",
                            CompanyCode = model.CompanyCode ?? "",
                            Ldate = model.Ldate,
                            Luser = model.Luser,
                            Lip = model.Lip,
                            Lmac = model.Lmac
                        });
                    }

                }

                if (!records.Any())
                {
                    return (false, "Save Failed: No valid employees found", null);
                }

                await earnLeaveEntryRepository.AddRangeAsync(records);
                return (true, "Save Successfully", records);
            }
            catch (DbUpdateException dbEx)
            {
                return (false, "An error occurred while saving data to the database.", dbEx.InnerException?.Message ?? dbEx.Message);
            }
            catch (Exception ex)
            {
                return (false, "An unexpected error occurred while saving data.", ex.Message);
            }
        }
        public async Task<bool> BulkDeleteAsync(List<decimal> ids, DeleteHistoryViewModel dmodel)
        {
            await earnLeaveEntryRepository.BeginTransactionAsync();

            try
            {
                var employees = await earnLeaveEntryRepository.All().Where(c => ids.Contains(c.AutoId)).ToListAsync();

                if (employees == null || employees.Count == 0)
                {
                    await earnLeaveEntryRepository.RollbackTransactionAsync();
                    return false;
                }

                await earnLeaveEntryRepository.DeleteRangeAsync(employees);

                dmodel.tableName = earnLeaveEntryRepository.GetTableName();
                await deleteHistoryService.LogDeletedRecordsAsync(employees, dmodel);
                await earnLeaveEntryRepository.CommitTransactionAsync();

                return true;
            }
            catch (Exception ex)
            {
                await earnLeaveEntryRepository.RollbackTransactionAsync();
                Console.WriteLine($"Bulk delete error: {ex}");
                return (false);
            }
        }

        //edit
        public Task<ManualEarnLeaveEntryEmployeeCreateDto?> getEarnLeaveEmployeeById(string id)
        {
            var emp = earnLeaveEntryRepository.GetAll().FirstOrDefault(x => x.EarnLeaveId == id);
            if (emp == null)
            {
                return Task.FromResult<ManualEarnLeaveEntryEmployeeCreateDto?>(null);
            }

            return Task.FromResult(new ManualEarnLeaveEntryEmployeeCreateDto
            {
                AutoId = emp.AutoId,
                EarnLeaveID = emp.EarnLeaveId,
                EmpId = emp.EmployeeId,
                GrantedLeaveDays = emp.GrantedLeaveDays,
                AvailedLeaveDays = emp.AvailedLeaveDays,
                BalancedLeaveDays = emp.BalancedLeaveDays,
                Remarks = emp.Remarks,
                Year = emp.Year
            });
        }

    }
}

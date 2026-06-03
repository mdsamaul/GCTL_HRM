using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Dapper;
using DocumentFormat.OpenXml.InkML;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.Companies;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.Employees;
using GCTL.Core.ViewModels.HRM_EmployeeWeekendDeclaration;
using GCTL.Data.Models;
using GCTL.Service.DeleteHistories;
using iText.Commons.Actions.Contexts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;

namespace GCTL.Service.EmployeeWeekendDeclaration
{
    public class EmployeeWeekendDeclarationService : AppService<HrmEmployeeWeekendDeclaration>, IEmployeeWeekendDeclarationService
    {
        private readonly IRepository<HrmEmployeeWeekendDeclaration> employeeWeekendDeclarationRepository;
        private readonly IRepository<CoreCompany> companyRepository;
        private readonly IRepository<HrmDefDivision> divisionRepository;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly IRepository<HrmEmployeeOfficialInfo> employeeOfficialInfoRepository;
        private readonly IRepository<CoreBranch> branchRepository;
        private readonly IRepository<HrmDefDepartment> departmentRepository;
        private readonly IRepository<HrmDefDesignation> designationRepository;
        private readonly IRepository<HrmEmployee> employeeRepository;
        private readonly IRepository<HrmDefEmployeeStatus> employeeStatusRepository;
        private readonly IRepository<HrmDefEmpType> empTypeRepository;
        private readonly IDeleteHistoryService deleteHistoryService;

        public EmployeeWeekendDeclarationService(
            IRepository<HrmEmployeeWeekendDeclaration> employeeWeekendDeclarationRepository,
            IRepository<CoreCompany> companyRepository,
            IRepository<HrmDefDivision> divisionRepository,
            IRepository<CoreAccessCode> accessCodeRepository,
            IRepository<HrmEmployeeOfficialInfo> employeeOfficialInfoRepository,
            IRepository<CoreBranch> branchRepository,
            IRepository<HrmDefDepartment> departmentRepository,
            IRepository<HrmDefDesignation> designationRepository,
            IRepository<HrmEmployee> employeeRepository,
            IRepository<HrmDefEmployeeStatus> employeeStatusRepository,
            IRepository<HrmDefEmpType> empTypeRepository,
            IDeleteHistoryService deleteHistoryService
            )
            : base(employeeWeekendDeclarationRepository)
        {
            this.employeeWeekendDeclarationRepository = employeeWeekendDeclarationRepository;
            this.companyRepository = companyRepository;
            this.divisionRepository = divisionRepository;
            this.accessCodeRepository = accessCodeRepository;
            this.employeeOfficialInfoRepository = employeeOfficialInfoRepository;
            this.branchRepository = branchRepository;
            this.departmentRepository = departmentRepository;
            this.designationRepository = designationRepository;
            this.employeeRepository = employeeRepository;
            this.employeeStatusRepository = employeeStatusRepository;
            this.empTypeRepository = empTypeRepository;
            this.deleteHistoryService = deleteHistoryService;
        }
        #region Permission all type

        public async Task<bool> PagePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Employee Weekend Declaration" && x.TitleCheck);
        }

        public async Task<bool> SavePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Employee Weekend Declaration" && x.CheckAdd);
        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Employee Weekend Declaration" && x.CheckEdit);
        }

        public async Task<bool> DeletePermissionAsync(string accessCode)
        {
            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "Employee Weekend Declaration" && x.CheckDelete);
        }

        #endregion

        public List<CoreCompany> GetAllCompany()
        {

            return companyRepository.GetAll().ToList();       
        }

        public IQueryable<EmployeeFilterDataDto> BaseFilter(EmployeeFilterDto filter)
        {
            var query = from e in employeeOfficialInfoRepository.All().AsNoTracking()
                        join c in companyRepository.All().AsNoTracking() on e.CompanyCode equals c.CompanyCode into companyGroup
                        from c in companyGroup.DefaultIfEmpty()
                        join b in branchRepository.All().AsNoTracking() on e.BranchCode equals b.BranchCode into branchGroup
                        from b in branchGroup.DefaultIfEmpty()
                        join d in departmentRepository.All().AsNoTracking() on e.DepartmentCode equals d.DepartmentCode into deptGroup
                        from d in deptGroup.DefaultIfEmpty()
                        join ds in designationRepository.All().AsNoTracking() on e.DesignationCode equals ds.DesignationCode into desigGroup
                        from ds in desigGroup.DefaultIfEmpty()
                        join dv in divisionRepository.All().AsNoTracking() on e.DivisionCode equals dv.DivisionCode into divGroup
                        from dv in divGroup.DefaultIfEmpty()
                        join emp in employeeRepository.All().AsNoTracking() on e.EmployeeId equals emp.EmployeeId into empGroup
                        from emp in empGroup.DefaultIfEmpty()
                        join status in employeeStatusRepository.All().AsNoTracking() on e.EmployeeStatus equals status.EmployeeStatusId into statusGroup
                        from status in statusGroup.DefaultIfEmpty()
                        join emptype in empTypeRepository.All().AsNoTracking() on e.EmpTypeCode equals emptype.EmpTypeCode into empTypeGroup
                        from emptype in empTypeGroup.DefaultIfEmpty()
                        select new EmployeeFilterDataDto
                        {
                            EmpId = emp.EmployeeId,
                            EmpFName = emp.FirstName,
                            EmpLName = emp.LastName,
                            JoiningDate = e.JoiningDate,
                            CompanyCode = e.CompanyCode,
                            CompanyName = c.CompanyName,
                            BranchCode = e.BranchCode,
                            BranchName = b.BranchName,
                            DepartmentCode = e.DepartmentCode,
                            DepartmentName = d.DepartmentName,
                            DesignationCode = e.DesignationCode,
                            DesignationName = ds.DesignationName,
                            DivisionCode = e.DivisionCode,
                            DivisionName = dv.DivisionName,
                            EmployeeTypeName = emptype.EmpTypeName,
                            EmployeeStatusId = e.EmployeeStatus,
                            EmployeeStatus = status.EmployeeStatus
                        };

            // Apply filters
            if (filter.CompanyCodes?.Any() == true)
                query = query.Where(x => x.CompanyCode != null && filter.CompanyCodes.Contains(x.CompanyCode));

            if (filter.BranchCodes?.Any() == true)
                query = query.Where(x => x.BranchCode != null && filter.BranchCodes.Contains(x.BranchCode));

            if (filter.DivisionCodes?.Any() == true)
                query = query.Where(x => x.DivisionCode != null && filter.DivisionCodes.Contains(x.DivisionCode));

            if (filter.DepartmentCodes?.Any() == true)
                query = query.Where(x => x.DepartmentCode != null && filter.DepartmentCodes.Contains(x.DepartmentCode));

            if (filter.DesignationCodes?.Any() == true)
                query = query.Where(x => x.DesignationCode != null && filter.DesignationCodes.Contains(x.DesignationCode));

            if (filter.EmployeeIDs?.Any() == true)
                query = query.Where(x => x.EmpId != null && filter.EmployeeIDs.Contains(x.EmpId));

            if (filter.EmployeeStatuses?.Any() == true)
                query = query.Where(x => x.EmployeeStatusId != null && filter.EmployeeStatuses.Contains(x.EmployeeStatusId));

            return query;
        }
        public async Task<EmployeeFilterResultDto> GetFilterDataAsync(EmployeeFilterDto filter)
        {
            var data = await BaseFilter(filter).ToListAsyncSafe();

            var result = new EmployeeFilterResultDto
            {
                Companies = data.Where(x => x.CompanyCode != null && x.CompanyName != null)
                                .Select(x => new CodeNameDto { Code = x.CompanyCode, Name = x.CompanyName })
                                .DistinctBy(x => x.Code).ToList(),

                Branches = data.Where(x => x.BranchCode != null && x.BranchName != null)
                               .Select(x => new CodeNameDto { Code = x.BranchCode, Name = x.BranchName })
                               .DistinctBy(x => x.Code).ToList(),

                Divisions = data.Where(x => x.DivisionCode != null && x.DivisionName != null)
                                .Select(x => new CodeNameDto { Code = x.DivisionCode, Name = x.DivisionName })
                                .DistinctBy(x => x.Code).ToList(),

                Departments = data.Where(x => x.DepartmentCode != null && x.DepartmentName != null)
                                  .Select(x => new CodeNameDto { Code = x.DepartmentCode, Name = x.DepartmentName })
                                  .DistinctBy(x => x.Code).ToList(),

                Designations = data.Where(x => x.DesignationCode != null && x.DesignationName != null)
                                   .Select(x => new CodeNameDto { Code = x.DesignationCode, Name = x.DesignationName })
                                   .DistinctBy(x => x.Code).ToList(),

                Employees = data.Where(x => x.EmpId != null)
                                .Select(x => new CodeNameDto
                                {
                                    Code = x.EmpId,
                                    Name = (x.EmpFName ?? "") + " " + (x.EmpLName ?? ""),
                                    JoiningDate = x.JoiningDate?.ToString("dd/MM/yyyy") ?? "",
                                    DesignationName = x.DesignationName,
                                    DepartmentName = x.DepartmentName,
                                    BranchName = x.BranchName,
                                    CompanyName = x.CompanyName,
                                    EmpTypeName = x.EmployeeTypeName,
                                    EmployeeStatus = x.EmployeeStatus
                                })
                                .DistinctBy(x => x.Code).ToList(),

                ActivityStatuses = data.Where(x => x.EmployeeStatus != null)
                                       .Select(x => new CodeNameDto { Code = x.EmployeeStatus, Name = x.EmployeeStatus })
                                       .Distinct().ToList()
            };

            return result;
        }


        public async Task<(bool isSuccess, string message, object data)> SaveSelectedDatesAndEmployeesAsync(HRM_EmployeeWeekendDeclarationDto model)
        {
            if (model == null || model.WeekendDates == null || !model.WeekendDates.Any() || model.WeekendEmployeeIds == null || !model.WeekendEmployeeIds.Any())
            {
                return (false, "Invalid data received: Missing dates or employee IDs.", null);
            }

            try
            {
                var records = new List<HrmEmployeeWeekendDeclaration>();
                
                var lastDeclaration = await employeeWeekendDeclarationRepository.All() 
                                             .OrderByDescending(e => e.Tc) 
                                             .FirstOrDefaultAsync();

                int nextIdNumber = 1;
                if (lastDeclaration != null && !string.IsNullOrEmpty(lastDeclaration.Ewdid))
                {
                    if (int.TryParse(lastDeclaration.Ewdid.Substring(2), out int lastNumber))
                    {
                        nextIdNumber = lastNumber + 1;
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Could not parse numeric part of last EWDId '{lastDeclaration.Ewdid}'. Starting next ID from 1.");
                        nextIdNumber = 1;
                    }
                }

                foreach (var dateStr in model.WeekendDates)
                {
                    if (DateTime.TryParse(dateStr, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime date))
                    {
                        foreach (var empId in model.WeekendEmployeeIds)
                        {
                            bool alreadyExists = employeeWeekendDeclarationRepository.GetAll().Any(e => e.EmployeeId == empId && e.Date == date);
                            if (!alreadyExists)
                            {


                                string newEwdId = $"{nextIdNumber:D8}";
                                nextIdNumber++;

                                records.Add(new HrmEmployeeWeekendDeclaration
                                {
                                    Ewdid = newEwdId,
                                    EmployeeId = empId,
                                    Date = date,
                                    Remark = model.Remark ?? "",
                                    Ldate = model.Ldate,
                                    Luser = model.Luser,
                                    Lip = model.Lip,
                                    Lmac = model.Lmac,
                                    CompanyCode = model.CompanyCode ?? "",
                                });
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Could not parse date string '{dateStr}'. Skipping.");
                    }
                }

                if (!records.Any())
                {
                    return (false, "No valid records to save (check date formats?).", null);
                }


                await employeeWeekendDeclarationRepository.AddRangeAsync(records);

               

                return (true, "Data saved successfully", new
                {
                    employeeCount = model.WeekendEmployeeIds.Count,
                    dateCount = model.WeekendDates.Count,
                    totalRecordsSaved = records.Count,
                    dates = model.WeekendDates,
                    employeeIds = model.WeekendEmployeeIds
                });
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException != null) Console.WriteLine($"Inner Exception: {dbEx.InnerException.Message}");
                return (false, "An error occurred while saving data to the database.", dbEx.InnerException?.Message ?? dbEx.Message);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null) Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                return (false, "An unexpected error occurred while saving data.", ex.Message);
            }
        }


        public async Task<(bool isSuccess, string message, object data)> SaveSelectedDatesAndEmployeesFromExcelAsync(HRM_EmployeeWeekendDeclarationDto model)
        {
            if (model == null || model.WeekendDates == null || !model.WeekendDates.Any() ||
                model.WeekendEmployeeIds == null || !model.WeekendEmployeeIds.Any())
            {
                return (false, "Invalid data received: Missing dates or employee IDs.", null);
            }

            try
            {
                var records = new List<HrmEmployeeWeekendDeclaration>();
                var lastDeclaration = await employeeWeekendDeclarationRepository.All()
                                            .OrderByDescending(e => e.Tc)
                                            .FirstOrDefaultAsync();

                int nextIdNumber = 1;
                if (lastDeclaration != null && !string.IsNullOrEmpty(lastDeclaration.Ewdid))
                {
                    if (int.TryParse(lastDeclaration.Ewdid.Substring(2), out int lastNumber))
                    {
                        nextIdNumber = lastNumber + 1;
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Could not parse numeric part of last EWDId '{lastDeclaration.Ewdid}'. Starting from 1.");
                    }
                }

                int totalRows = Math.Min(model.WeekendDates.Count, model.WeekendEmployeeIds.Count); 

                for (int i = 0; i < totalRows; i++)
                {
                    var dateStr = model.WeekendDates[i];
                    var empId = model.WeekendEmployeeIds[i];
                    var remark = (model.ExcelRemark != null && i < model.ExcelRemark.Count) ? model.ExcelRemark[i] : "";

                    if (DateTime.TryParse(dateStr, out DateTime date))
                    {
                        bool alreadyExists = employeeWeekendDeclarationRepository.GetAll()
                            .Any(e => e.EmployeeId == empId && e.Date == date);

                        if (!alreadyExists)
                        {
                            string newEwdId = nextIdNumber.ToString("D8");
                            nextIdNumber++;

                            records.Add(new HrmEmployeeWeekendDeclaration
                            {
                                Ewdid = newEwdId,
                                EmployeeId = empId,
                                Date = date,
                                Remark = remark,
                                Ldate = model.Ldate,
                                Luser = model.Luser,
                                Lip = model.Lip,
                                Lmac = model.Lmac,
                                CompanyCode = model.CompanyCode ?? ""
                            });
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Could not parse date string '{dateStr}' at index {i}. Skipping.");
                    }
                }

                if (!records.Any())
                {
                    return (false, "No valid records to save (check date formats or duplicates).", null);
                }

                await employeeWeekendDeclarationRepository.AddRangeAsync(records);

                return (true, "Saved successfully", new
                {
                    SavedCount = records.Count,
                    Dates = model.WeekendDates,
                    Employees = model.WeekendEmployeeIds
                });
            }
            catch (DbUpdateException dbEx)
            {
                Console.WriteLine($"Database error: {dbEx.Message}");
                return (false, "Database error occurred.", dbEx.InnerException?.Message ?? dbEx.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General error: {ex.Message}");
                return (false, "Unexpected error occurred.", ex.Message);
            }
        }

        public async Task<List<EmployeeWeekendDeclarationViewModelShow>> GetWeekendEmpDecService()
        {
            var data = employeeWeekendDeclarationRepository.GetAll()
            .Join(employeeRepository.GetAll(),
                  ewc => ewc.EmployeeId,
                  e => e.EmployeeId,
                  (ewc, e) => new { ewc, e })
            .GroupJoin(employeeOfficialInfoRepository.GetAll(),
                       ee => ee.ewc.EmployeeId,
                       ei => ei.EmployeeId,
                       (ee, eiJoin) => new { ee, eiJoin })
            .SelectMany(x => x.eiJoin.DefaultIfEmpty(),
                        (x, ei) => new { x.ee, ei })
            .GroupJoin(designationRepository.GetAll(),
                       x => x.ei.DesignationCode,
                       dd => dd.DesignationCode,
                       (x, ddJoin) => new { x.ee, x.ei, ddJoin })
            .SelectMany(x => x.ddJoin.DefaultIfEmpty(),
                        (x, dd) => new EmployeeWeekendDeclarationViewModelShow
                        {
                            TC = x.ee.ewc.Tc,
                            ID = x.ee.ewc.Ewdid,
                            EmpID = x.ee.e.EmployeeId,
                            Name = x.ee.e.FirstName + " " + x.ee.e.LastName,
                            Designation = dd != null ? dd.DesignationName : null,
                            WeekendDate = x.ee.ewc.Date.ToString("dd/MM/yyyy"),
                            Remarks = x.ee.ewc.Remark,
                            EntryUser = x.ee.ewc.Luser,
                            Day = x.ee.ewc.Date.DayOfWeek.ToString()
                        });

            var result = data.OrderBy(c => c.ID).ToList();
            return await Task.FromResult(result);
        }

        public async Task<EmployeeWeekendDeclarationViewModelShow?> GetEmployeeWeekendDeclarationByIdAsync(string id)
        {
            var emp = employeeWeekendDeclarationRepository.GetAll()
                                    .Where(c => c.Ewdid == id)
                                    .FirstOrDefault();

            if (emp == null)
                return null;

            return new EmployeeWeekendDeclarationViewModelShow
            {
                ID = emp.Ewdid,
                EmpID = emp.EmployeeId,
                WeekendDate = emp.Date.ToString("yyyy-MM-dd"),
                Remarks = emp.Remark
            };
        }
        public async Task<(bool IsSuccess, string Message)> UpdateEmployeeWeekendDeclarationAsync(string id, string weekendDate, string remarks)
        {
            var emp = employeeWeekendDeclarationRepository.GetAll()
                                     .Where(c => c.Ewdid == id)
                                     .FirstOrDefault();

            if (emp == null)
                return (false, "Record not found");

            if (!DateTime.TryParse(weekendDate, out DateTime parsedDate))
                return (false, "Invalid date format");

            emp.Date = parsedDate;
            emp.Remark = remarks;

            employeeWeekendDeclarationRepository.Update(emp);

            return (true, "Updated successfully");
        }

        public async Task<byte[]> GenerateEmployeeWeekendDeclarationExcelAsync()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            var employees =  employeeWeekendDeclarationRepository.GetAll().ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Employees");

                worksheet.Cells[1, 1].Value = "EmployeeID";
                worksheet.Cells[1, 2].Value = "Date (YYYY-MM-DD)";
                worksheet.Cells[1, 3].Value = "Remarks";

                int row = 2;
                foreach (var employee in employees)
                {
                    worksheet.Cells[row, 1].Value = employee.EmployeeId;
                    worksheet.Cells[row, 2].Value = employee.Date.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 3].Value = employee.Remark;
                    row++;
                }

                return package.GetAsByteArray();
            }
        }
        public async Task<bool> BulkDeleteAsync(List<decimal> ids, DeleteHistoryViewModel dmodel)
        {
            await employeeWeekendDeclarationRepository.BeginTransactionAsync();

            try
            {
                var employees = await employeeWeekendDeclarationRepository.All().Where(c => ids.Contains(c.Tc)).ToListAsync();

                if (employees == null || employees.Count == 0)
                {
                    await employeeWeekendDeclarationRepository.RollbackTransactionAsync();
                    return false;
                }

                await employeeWeekendDeclarationRepository.DeleteRangeAsync(employees);

                dmodel.tableName = employeeOfficialInfoRepository.GetTableName();
                await deleteHistoryService.LogDeletedRecordsAsync(employees, dmodel);
                await employeeWeekendDeclarationRepository.CommitTransactionAsync();

                return true;
            }
            catch (Exception ex)
            {
                await employeeWeekendDeclarationRepository.RollbackTransactionAsync();
                Console.WriteLine($"Bulk delete error: {ex}");
                return (false);
            }
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

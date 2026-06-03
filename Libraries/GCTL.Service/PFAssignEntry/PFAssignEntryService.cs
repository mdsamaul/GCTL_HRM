using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.ManualEarnLeaveEntry;
using GCTL.Core.ViewModels.PFAssignEntry;
using GCTL.Data.Models;
using GCTL.Service.DeleteHistories;
using GCTL.Service.EmployeeWeekendDeclaration;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Org.BouncyCastle.Ocsp;

namespace GCTL.Service.PFAssignEntry
{
    public class PFAssignEntryService : AppService<HrmPayrollPfassignEntry>, IPFAssignEntryService
    {
        private readonly IRepository<HrmPayrollPfassignEntry> payrollPfAssignEntryRepo;
        private readonly IRepository<HrmEmployee> employeeRepo;
        private readonly IRepository<HrmEmployeeOfficialInfo> empOffRepo;
        private readonly IRepository<HrmDefDesignation> desiRepo;
        private readonly IRepository<HrmDefDepartment> depRepo;
        private readonly IRepository<HrmDefDivision> divRepo;
        private readonly IRepository<CoreBranch> branchRepo;
        private readonly IRepository<CoreCompany> companyRepo;
        private readonly IRepository<HrmDefEmpType> defEmpTypeRepo;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly IRepository<HrmEisDefEmploymentNature> empNatureRepo;
        private readonly IRepository<HrmDefEmployeeStatus> empStatusRepo;
        private readonly IRepository<HrmSeparation> SeparationRepo;
        private readonly IDeleteHistoryService deleteHistoryService;

        public PFAssignEntryService(
         IRepository<HrmPayrollPfassignEntry> payrollPfAssignEntryRepo,
          IRepository<HrmEmployee> employeeRepo,
          IRepository<HrmEmployeeOfficialInfo> empOffRepo,
          IRepository<HrmDefDesignation> desiRepo,
          IRepository<HrmDefDepartment> depRepo,
          IRepository<HrmDefDivision> divRepo,
          IRepository<CoreBranch> branchRepo,
          IRepository<CoreCompany> companyRepo,
          IRepository<HrmDefEmpType> defEmpTypeRepo,
          IRepository<CoreAccessCode> accessCodeRepository,
          IRepository<HrmEisDefEmploymentNature> empNatureRepo,
          IRepository<HrmDefEmployeeStatus> empStatusRepo,
          IRepository<HrmSeparation> SeparationRepo,
          IDeleteHistoryService deleteHistoryService
            ) :base(payrollPfAssignEntryRepo) 
        {
            this.payrollPfAssignEntryRepo = payrollPfAssignEntryRepo;
            this.employeeRepo = employeeRepo;
            this.empOffRepo = empOffRepo;
            this.desiRepo = desiRepo;
            this.depRepo = depRepo;
            this.divRepo = divRepo;
            this.branchRepo = branchRepo;
            this.companyRepo = companyRepo;
            this.defEmpTypeRepo = defEmpTypeRepo;
            this.accessCodeRepository = accessCodeRepository;
            this.empNatureRepo = empNatureRepo;
            this.empStatusRepo = empStatusRepo;
            this.SeparationRepo = SeparationRepo;
            this.deleteHistoryService = deleteHistoryService;
        }
        #region Permission all type

        public async Task<bool> PagePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "PF Assign" && x.TitleCheck);

        }

        public async Task<bool> SavePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "PF Assign" && x.CheckAdd);

        }

        public async Task<bool> UpdatePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "PF Assign" && x.CheckEdit);

        }

        public async Task<bool> DeletePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "PF Assign" && x.CheckDelete);

        }

        #endregion


        public async Task<PFAssignEntryFilterListDto> GetFilterDataAsync(PFAssignEntryFilterDto filter)
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
                            EmployeeName = (e.FirstName ?? "") + " " + (e.LastName ?? ""),
                            CompanyCode = eoi.CompanyCode,
                            BranchCode = cb.BranchCode,
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

            if (filter.CompanyCodes?.Any() == true)
            {
                query = query.Where(x => x.CompanyCode != null && filter.CompanyCodes.Contains(x.CompanyCode));
            }
            if (filter.BranchCodes?.Any() == true)
            {
                query = query.Where(x => x.BranchCode != null & filter.BranchCodes.Contains(x.BranchCode));
            }
            if (filter.DivisionCodes?.Any() == true)
            {
                query = query.Where(x => x.DivisionCode != null && filter.DivisionCodes.Contains(x.DivisionCode));
            }
            if (filter.DepartmentCodes?.Any() == true)
            {
                query = query.Where(x => x.DepartmentCode != null && filter.DepartmentCodes.Contains(x.DepartmentCode));
            }
            if (filter.DesignationCodes?.Any() == true)
            {
                query = query.Where(x => x.DesignationCode != null && filter.DesignationCodes.Contains(x.DesignationCode));
            }
            if (filter.EmployeeIDs?.Any() == true)
            {
                query = query.Where(x => x.EmpID != null && filter.EmployeeIDs.Contains(x.EmpID));
            }
            if (filter.EmployeeStatuses?.Any() == true)
            {
                query = query.Where(x => x.EmployeeStatusCode != null && filter.EmployeeStatuses.Contains(x.EmployeeStatusCode));
            }

            var result = new PFAssignEntryFilterListDto
            {
                Companies = await query.Where(x => x.CompanyCode != null && x.CompanyName != null)
                .Select(x => new PFAssignEntryFilterResultDto { Code = x.CompanyCode, Name = x.CompanyName })
                .Distinct().ToListAsyncSafe(),

                Branches = await query.Where(x => x.BranchCode != null && x.BranchName != null)
                .Select(x => new PFAssignEntryFilterResultDto { Code = x.BranchCode, Name = x.BranchName })
                .Distinct().ToListAsyncSafe(),

                Divisions = await query.Where(x => x.DivisionCode != null && x.DivisionName != null)
                .Select(x => new PFAssignEntryFilterResultDto { Code = x.DivisionCode, Name = x.DivisionName })
                .Distinct().ToListAsyncSafe(),
                Departments = await query.Where(x => x.DepartmentCode != null && x.DepartmentName != null)
                .Select(x => new PFAssignEntryFilterResultDto { Code = x.DepartmentCode, Name = x.DepartmentName })
                .Distinct().ToListAsyncSafe(),
                Designations = await query.Where(x => x.DesignationCode != null && x.DesignationName != null)
                .Select(x => new PFAssignEntryFilterResultDto { Code = x.DesignationCode, Name = x.DesignationName })
                .Distinct().ToListAsyncSafe(),
                Employees = await query.Where(x => x.EmpID != null && x.EmployeeName != null)
                .Select(x => new PFAssignEntryFilterResultDto
                {
                    Code = x.EmpID??"",
                    Name = x.EmployeeName??"",
                    Designation = x.DesignationName??"",
                    Department = x.DepartmentName??"",
                    Branch = x.BranchName??"",
                    Company = x.CompanyName??"",
                    EmployeeType = x.EmployeeTypeName??"",
                    EmploymentNature = x.EmploymentNature??"",
                    JoiningDate = x.JoiningDate != null ? x.JoiningDate.Value.ToString("dd/MM/yyyy") : "",
                    SeparationDate = x.SeparationDate,
                    ServiceDuration = x.ServiceDuration,
                    ServiceDuration2 = x.ServiceDuration2??""
                }).Distinct().ToListAsyncSafe(),

                ActivityStatuses = await query.Where(x => x.EmployeeTypeName != null)
                .Select(x => new PFAssignEntryFilterResultDto { Code = x.EmployeeStatusCode, Name = x.EmployeeTypeName })
                .Distinct().ToListAsyncSafe(),
            };
            return result;
        }
        public async Task<(bool isSuccess, string message, object data)> CreateUpdatePFAssignService(PFAssignEntrySetupViewModel fromData)
        {
            if (fromData == null 
                || fromData.EFDate == default(DateTime) 
                || fromData.PFApprovedStatus == null
                )

            {
                return (false, "Missing Data", null);
            }
            if(fromData.EFDate.Year < 1900 || fromData.EFDate.Year > 2100)
            {
                return (false, $"Invalid Year '{fromData.EFDate.Year}'is out of allowed range (1900-2100).", null);
            }
            try
            {
                if (fromData.PFAssignID == null)
                {
                    var records = new List<HrmPayrollPfassignEntry>();
                    var lastDeclaration = await payrollPfAssignEntryRepo.All()
                        .OrderByDescending(x => x.PfassignId)
                        .FirstOrDefaultAsync();
                    int nextIdNumber = 1;
                    if (lastDeclaration != null && !string.IsNullOrEmpty(lastDeclaration.PfassignId))
                    {
                        if (int.TryParse(lastDeclaration.PfassignId, out int lastNumber))
                        {
                            nextIdNumber = lastNumber + 1;
                        }
                        else
                        {
                            nextIdNumber = 1;
                        }
                    }
                    foreach (var empId in fromData.EmployeeIds)
                    {
                        bool isEmpInclude = employeeRepo.GetAll().Any(x => x.EmployeeId == empId);
                        bool existsEmployee = payrollPfAssignEntryRepo.GetAll().Any(x => x.EmployeeId == empId && x.PfapprovedStatus == fromData.PFApprovedStatus);
                      
                        if (isEmpInclude && !existsEmployee)
                        {
                            string newPFAssignID = $"{nextIdNumber:D8}";
                            nextIdNumber++;
                            records.Add(new HrmPayrollPfassignEntry
                            {
                                EmployeeId = empId,
                                PfassignId = newPFAssignID,
                                PfapprovedStatus = fromData.PFApprovedStatus,
                                ApprovalRemark = fromData.ApprovalRemark ?? "",
                                CompanyCode = fromData.CompanyCode ?? "",
                                Efdate=fromData.EFDate,
                                Ldate = fromData.Ldate,
                                Luser = fromData.Luser,
                                Lip = fromData.Lip,
                                Lmac = fromData.Lmac,
                            });
                        }                      
                    }
                    if (!records.Any())
                    {
                        return (false, "Save Failed", null);
                    }
                    await payrollPfAssignEntryRepo.AddRangeAsync(records);
                    return (true, "Save SuccessFully", records);
                }
                else
                {
                   
                    var emp = payrollPfAssignEntryRepo.GetAll()
    .Where(x => x.PfassignId == fromData.PFAssignID)
    .FirstOrDefault();

                    bool existsEmployee = payrollPfAssignEntryRepo.GetAll()
                        .Any(x => x.EmployeeId == fromData.EmployeeId
                               && x.PfapprovedStatus == fromData.PFApprovedStatus
                               && x.PfassignId != fromData.PFAssignID);

                    if (emp != null && !existsEmployee)
                    {
                        emp.ApprovalRemark = fromData.ApprovalRemark;
                        emp.Efdate = fromData.EFDate;
                        emp.PfapprovedStatus = fromData.PFApprovedStatus;
                        emp.ModifyDate = fromData.ModifyDate;
                        payrollPfAssignEntryRepo.Update(emp);

                        return (isSuccess: true, message: "Update Successfully.", data: emp);
                    }
                    else
                    {
                        return (isSuccess: false, message: "Already exists.", data: emp);
                    }

                }
            }
            catch (Exception ex) { 
                return (false, ex.Message, null);
            }
        }

        //download excel sheet
        public async Task<byte[]> GeneratePfAssignExcelDownload()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var employees = payrollPfAssignEntryRepo.GetAll().ToList();
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Employees");

                worksheet.Cells[1, 1].Value = "EmployeeId";
                worksheet.Cells[1, 2].Value = "Effective Date";
                worksheet.Cells[1, 3].Value = "PF Approved Status";
                worksheet.Cells[1, 4].Value = "Approval Remarks";

                worksheet.Column(1).Style.Numberformat.Format = "@";
                //worksheet.Column(2).Style.Numberformat.Format = "@";
                worksheet.Column(2).Style.Numberformat.Format = "yyyy-mm-dd";
                int row = 2;
                foreach (var employee in employees)
                {
                    worksheet.Cells[row, 1].Value = employee.EmployeeId;
                    worksheet.Cells[row, 2].Value = employee.Efdate;
                    worksheet.Cells[row, 3].Value = employee.PfapprovedStatus;
                    worksheet.Cells[row, 4].Value = employee.ApprovalRemark;
                    row++;
                }
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                return package.GetAsByteArray();
            }
        }
        public async Task<(bool isSuccess, string message, object data)> SavePFAssignExcel(PFAssignEntrySetupViewModel fromData)
        {
            if (fromData == null
                || fromData.EmployeeIds == null || !fromData.EmployeeIds.Any()
                || fromData.EFDateList == null || !fromData.EFDateList.Any()
                || fromData.PFApprovedStatusList == null || !fromData.PFApprovedStatusList.Any())
            {
                return (false, "Missing Data", null);
            }

            try
            {
                var records = new List<HrmPayrollPfassignEntry>();
                var lastDeclaration = await payrollPfAssignEntryRepo.All()
                    .OrderByDescending(x => x.PfassignId)
                    .FirstOrDefaultAsync();

                int nextIdNumber = 1;
                if (lastDeclaration != null && !string.IsNullOrEmpty(lastDeclaration.PfassignId))
                {
                    if (int.TryParse(lastDeclaration.PfassignId, out int lastNumber))
                    {
                        nextIdNumber = lastNumber + 1;
                    }
                }

                int totalRows = new[]
                {
            fromData.EmployeeIds.Count,
            fromData.EFDateList.Count,
            fromData.PFApprovedStatusList.Count,
            fromData.ApprovalRemarkList?.Count ?? 0
        }.Min();

                var existingEmployeeIds = employeeRepo.GetAll()
                    .Select(x => x.EmployeeId)
                    .ToHashSet();

                var existingPFAssigns = payrollPfAssignEntryRepo.GetAll()
                    .Where(x => fromData.EmployeeIds.Contains(x.EmployeeId))
                    .Select(x => new { x.EmployeeId, x.PfapprovedStatus })
                    .ToHashSet();

                for (int i = 0; i < totalRows; i++)
                {
                    var empId = fromData.EmployeeIds[i];
                    var efDate = fromData.EFDateList[i];
                    var pfApprovedStatus = fromData.PFApprovedStatusList[i] ?? "";
                    var remark = (fromData.ApprovalRemarkList != null && i < fromData.ApprovalRemarkList.Count)
                        ? fromData.ApprovalRemarkList[i] ?? ""
                        : "";

                    if (string.IsNullOrWhiteSpace(empId)) continue;

                    bool existsEmployee = existingPFAssigns
                        .Any(x => x.EmployeeId == empId && x.PfapprovedStatus == pfApprovedStatus);

                    if (!existingEmployeeIds.Contains(empId) || existsEmployee) continue;

                    if (!DateTime.TryParse(efDate, out var parsedDate)) continue;

                    records.Add(new HrmPayrollPfassignEntry
                    {
                        EmployeeId = empId,
                        PfassignId = $"{nextIdNumber:D8}",
                        PfapprovedStatus = pfApprovedStatus,
                        ApprovalRemark = remark,
                        CompanyCode = fromData.CompanyCode ?? "",
                        Efdate = parsedDate,
                        Ldate = fromData.Ldate,
                        Luser = fromData.Luser ?? "",
                        Lip = fromData.Lip ?? "",
                        Lmac = fromData.Lmac ?? "",
                    });

                    nextIdNumber++;
                }

                if (!records.Any())
                {
                    return (false, "Save Failed: No valid employees found", null);
                }

                await payrollPfAssignEntryRepo.AddRangeAsync(records);
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
        public async Task<List<PFAssignEntrySetupViewModel>> GetPfAssignDataService()
        {
            var data = payrollPfAssignEntryRepo.GetAll().
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
                (x, dd) => new PFAssignEntrySetupViewModel
                {
                    AutoId = x.ee.x.AutoId,
                    PFAssignID = x.ee.x.PfassignId??"",
                    EmployeeId = x.ee.e.EmployeeId??"",
                    EmployeeName = (x.ee.e.FirstName ?? "") + " " + (x.ee.e.LastName ?? ""),
                    Designation = dd != null ? (dd.DesignationName ?? "") : "",
                    //EFDate =  x.ee.x.EFDate??"",
                    EFDateShow = x.ee.x.Efdate.ToString("dd/MM/yyyy")??"",
                    ApprovalRemark = x.ee.x.ApprovalRemark??"",
                    PFApprovedStatus = x.ee.x.PfapprovedStatus??"",
                    EntryUser = x.ee.x.Luser??"",
                });
            var result = data.OrderByDescending(x => x.PFAssignID).ToList();
            return await Task.FromResult(result);
        }

        public async Task<bool> BulkDeleteAsync(List<decimal> ids, DeleteHistoryViewModel dmodel)
        {
            await payrollPfAssignEntryRepo.BeginTransactionAsync();

            try
            {
                var employees = await payrollPfAssignEntryRepo.All().Where(c => ids.Contains(c.AutoId)).ToListAsync();

                if (employees == null || employees.Count == 0)
                {
                    await payrollPfAssignEntryRepo.RollbackTransactionAsync();
                    return false;
                }

                await payrollPfAssignEntryRepo.DeleteRangeAsync(employees);

                dmodel.tableName = payrollPfAssignEntryRepo.GetTableName();
                await deleteHistoryService.LogDeletedRecordsAsync(employees, dmodel);

                await payrollPfAssignEntryRepo.CommitTransactionAsync();

                return true;
            }
            catch (Exception ex)
            {
                await payrollPfAssignEntryRepo.RollbackTransactionAsync();
                Console.WriteLine($"Bulk delete error: {ex}");
                return (false);
            }
        }
        //edit
        public Task<PFAssignEntrySetupViewModel> getAssignValueById(string id)
        {
            var emp = payrollPfAssignEntryRepo.GetAll().FirstOrDefault(x => x.PfassignId == id);
            if (emp == null)
            {
                return Task.FromResult<PFAssignEntrySetupViewModel>(null);
            }

            return Task.FromResult(new PFAssignEntrySetupViewModel
            {
                AutoId = emp.AutoId,
                PFAssignID = emp.PfassignId,
                EmployeeId = emp.EmployeeId,
                EFDateShow = emp.Efdate.ToString("yyyy-MM-dd"),
                PFApprovedStatus = emp.PfapprovedStatus,
                ApprovalRemark = emp.ApprovalRemark,
            });
        }
    }
}

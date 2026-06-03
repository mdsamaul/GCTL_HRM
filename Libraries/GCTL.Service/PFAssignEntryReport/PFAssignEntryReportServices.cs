using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.HRM_EmployeeWeekendDeclaration;
using GCTL.Core.ViewModels.PFAssignEntry;
using GCTL.Data.Models;
using GCTL.Service.EmployeeWeekendDeclaration;
using Microsoft.EntityFrameworkCore;

namespace GCTL.Service.PFAssignEntryReport
{
    public class PFAssignEntryReportServices : AppService<HrmPayrollPfassignEntry>, IPFAssignEntryReportServices
    {
        private readonly IRepository<HrmPayrollPfassignEntry> payrollPfAssignEntryRepo;
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
        private readonly IRepository<HrmRosterScheduleEntry> rosterEntryRepo;
        public PFAssignEntryReportServices(IRepository<HrmPayrollPfassignEntry> payrollPfAssignEntryRepo,
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
            IRepository<HrmRosterScheduleEntry> rosterEntryRepo
            ) : base(payrollPfAssignEntryRepo)
        {
            this.payrollPfAssignEntryRepo = payrollPfAssignEntryRepo;
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
            this.rosterEntryRepo = rosterEntryRepo;
        }

        #region Permission all type

        public async Task<bool> PagePermissionAsync(string accessCode)

        {

            return await accessCodeRepository.All().AnyAsync(x => x.AccessCodeId == accessCode && x.Title == "PF Assign" && x.TitleCheck);

        }


        #endregion


        public IQueryable<PFAssignEntryFilterDataDto> BaseFilter()
        {
            var query = from e in empOffRepo.All().AsNoTracking()
                        join c in companyRepo.All().AsNoTracking() on e.CompanyCode equals c.CompanyCode into companyGroup
                        from c in companyGroup.DefaultIfEmpty()
                        join b in branchRepo.All().AsNoTracking() on e.BranchCode equals b.BranchCode into branchGroup
                        from b in branchGroup.DefaultIfEmpty()
                        join d in depRepo.All().AsNoTracking() on e.DepartmentCode equals d.DepartmentCode into deptGroup
                        from d in deptGroup.DefaultIfEmpty()
                        join ds in desiRepo.All().AsNoTracking() on e.DesignationCode equals ds.DesignationCode into desigGroup
                        from ds in desigGroup.DefaultIfEmpty()
                        join dv in divRepo.All().AsNoTracking() on e.DivisionCode equals dv.DivisionCode into divGroup
                        from dv in divGroup.DefaultIfEmpty()
                        join emp in employeeRepo.All().AsNoTracking() on e.EmployeeId equals emp.EmployeeId into empGroup
                        from emp in empGroup.DefaultIfEmpty()
                        join status in empStRepo.All().AsNoTracking() on e.EmployeeStatus equals status.EmployeeStatusId into statusGroup
                        from status in statusGroup.DefaultIfEmpty()                        
                        select new PFAssignEntryFilterDataDto
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
                            EmployeeStatusId = e.EmployeeStatus,
                            EmployeeStatus = status.EmployeeStatus
                        };

            return query;
        }
        public async Task<PFAssignEntryFilterListDto> GetPFBaseAndFilteredDataAsync(PFAssignEntryFilterDto filter)
        {
            
            var allData = await BaseFilter().ToListAsyncSafe();

         
            var filteredData = allData.AsEnumerable();

            if (filter.CompanyCodes?.Any() == true)
                filteredData = filteredData.Where(x => filter.CompanyCodes.Contains(x.CompanyCode));

            if (filter.BranchCodes?.Any() == true)
                filteredData = filteredData.Where(x => filter.BranchCodes.Contains(x.BranchCode));

            if (filter.DepartmentCodes?.Any() == true)
                filteredData = filteredData.Where(x => filter.DepartmentCodes.Contains(x.DepartmentCode));

            if (filter.DesignationCodes?.Any() == true)
                filteredData = filteredData.Where(x => filter.DesignationCodes.Contains(x.DesignationCode));

            if (filter.EmployeeIDs?.Any() == true)
                filteredData = filteredData.Where(x => filter.EmployeeIDs.Contains(x.EmpId));

            if (filter.EmployeeStatuses?.Any() == true)
                filteredData = filteredData.Where(x => filter.EmployeeStatuses.Contains(x.EmployeeStatus));

            // STEP 3: Prepare dropdowns (from allData) & employees (from filteredData)
            var result = new PFAssignEntryFilterListDto
            {
                Companies =  allData.Where(x => !string.IsNullOrEmpty(x.CompanyCode) && !string.IsNullOrEmpty(x.CompanyName))
                                   .Select(x => new PFAssignEntryFilterResultDto { Code = x.CompanyCode, Name = x.CompanyName })
                                   .DistinctBy(x => x.Code).ToList(),

                Branches = allData.Where(x => !string.IsNullOrEmpty(x.BranchCode) && !string.IsNullOrEmpty(x.BranchName))
                                  .Select(x => new PFAssignEntryFilterResultDto { Code = x.BranchCode, Name = x.BranchName })
                                  .DistinctBy(x => x.Code).ToList(),

                Departments = allData.Where(x => !string.IsNullOrEmpty(x.DepartmentCode) && !string.IsNullOrEmpty(x.DepartmentName))
                                     .Select(x => new PFAssignEntryFilterResultDto { Code = x.DepartmentCode, Name = x.DepartmentName })
                                     .DistinctBy(x => x.Code).ToList(),

                Designations = allData.Where(x => !string.IsNullOrEmpty(x.DesignationCode) && !string.IsNullOrEmpty(x.DesignationName))
                                      .Select(x => new PFAssignEntryFilterResultDto { Code = x.DesignationCode, Name = x.DesignationName })
                                      .DistinctBy(x => x.Code).ToList(),

                Employees = filteredData.Where(x => x.EmpId != null)
                                        .Select(x => new PFAssignEntryFilterResultDto
                                        {
                                            Code = x.EmpId,
                                            Name = (x.EmpFName ?? "") + " " + (x.EmpLName ?? "")                                            
                                        })
                                        .DistinctBy(x => x.Code).ToList(),

                ActivityStatuses = allData.Where(x => x.EmployeeStatus != null)
                                          .Select(x => new PFAssignEntryFilterResultDto { Code = x.EmployeeStatus, Name = x.EmployeeStatus })
                                          .Distinct().ToList()
            };

            return result;
        }

        public async Task<PFAssignEntryFilterListDto> GetPFDataPdfAsync(PFAssignEntryFilterDto FilterData)
        {
            var queary = from Pfa in payrollPfAssignEntryRepo.All()
                         join eoi in empOffRepo.All() on Pfa.EmployeeId equals eoi.EmployeeId
                         join e in employeeRepo.All() on Pfa.EmployeeId equals e.EmployeeId into empJoin
                         from e in empJoin.DefaultIfEmpty()
                         join dg in desiRepo.All() on eoi.DesignationCode equals dg.DesignationCode into dgJoin
                         from dg in dgJoin.DefaultIfEmpty()
                         join cb in branchRepo.All() on eoi.BranchCode equals cb.BranchCode into cbJoin
                         from cb in cbJoin.DefaultIfEmpty()
                         join dp in depRepo.All() on eoi.DepartmentCode equals dp.DepartmentCode into dpJoin
                         from dp in dpJoin.DefaultIfEmpty()
                         join cp in companyRepo.All() on eoi.CompanyCode equals cp.CompanyCode into cpJoin
                         from cp in cpJoin.DefaultIfEmpty()                        
                         select new
                         {
                             empId = e.EmployeeId,
                             empName = (e.FirstName ?? " ") + " " + (e.LastName ?? " "),
                             companyName = cp.CompanyName ?? "",
                             companyCode = cp.CompanyCode ?? "",
                             branchName = cb.BranchName ?? "",
                             branchCode = cb.BranchCode ?? "",
                             desiganationName = dg.DesignationName ?? "",
                             desiganationCode = dg.DesignationCode ?? "",
                             departmentName = dp.DepartmentName ?? "",
                             departmentCode = dp.DepartmentCode ?? "",
                             date = Pfa.Efdate,
                             remark = Pfa.ApprovalRemark ?? "",
                             dayName = Pfa.Efdate.ToString("dddd") ?? "",
                             approveStatus = Pfa.PfapprovedStatus

                         };
            if (FilterData.CompanyCodes?.Any() == true)
            {
                queary = queary.Where(x => x.companyCode != null && FilterData.CompanyCodes.Contains(x.companyCode));
            }
            if (FilterData.BranchCodes?.Any() == true)
            {
                queary = queary.Where(x => x.branchCode != null && FilterData.BranchCodes.Contains(x.branchCode));
            }
            if (FilterData.DepartmentCodes?.Any() == true)
            {
                queary = queary.Where(x => x.departmentCode != null && FilterData.DepartmentCodes.Contains(x.departmentCode));
            }
            if (FilterData.DesignationCodes?.Any() == true)
            {
                queary = queary.Where(x => x.desiganationCode != null && FilterData.DesignationCodes.Contains(x.desiganationCode));
            }
            if (FilterData.EmployeeIDs?.Any() == true)
            {
                queary = queary.Where(x => x.empId != null && FilterData.EmployeeIDs.Contains(x.empId));
            }
            var earliestDate = await queary.OrderBy(x => x.date).Select(x => x.date).FirstOrDefaultAsync();
            var lastDate = await queary.OrderByDescending(x => x.date).Select(x => x.date).FirstOrDefaultAsync();
            int skip = (FilterData.PageNumber - 1) * FilterData.PageSize;
            var paginatedData = queary.OrderByDescending(x => x.date).Skip(skip).Take(FilterData.PageSize);
            int totalCount = await queary.CountAsync();

            var result = new PFAssignEntryFilterListDto
            {
                Companies = await queary.Where(x => x.companyCode != null && x.companyName != null).Select(x => new PFAssignEntryFilterResultDto { Code = x.companyCode, Name = x.companyName }).Distinct().ToListAsyncSafe(),
                Branches = await queary.Where(x => x.branchCode != null && x.branchName != null).Select(x => new PFAssignEntryFilterResultDto { Code = x.branchCode, Name = x.branchName }).Distinct().ToListAsyncSafe(),
                Departments = await queary.Where(x => x.departmentCode != null && x.departmentName != null).Select(x => new PFAssignEntryFilterResultDto { Code = x.departmentCode, Name = x.departmentName }).Distinct().ToListAsyncSafe(),
                Designations = await queary.Where(x => x.desiganationCode != null && x.desiganationName != null).Select(x => new PFAssignEntryFilterResultDto { Code = x.desiganationCode, Name = x.desiganationName }).Distinct().ToListAsyncSafe(),

                Employees = await queary.Where(x => x.empId != null && x.empName != null).Select(x => new PFAssignEntryFilterResultDto
                {
                    Code = x.empId,
                    Name = x.empName,
                    Department = x.departmentName,
                    Designation= x.desiganationName,
                    Branch = x.branchName,
                    Company = x.companyName,
                    ShowDate = x.date.ToString("dd/MM/yyyy"),
                    Remarks = x.remark,                   
                    Luser = FilterData.Luser ?? "",
                    PFApprove = x.approveStatus,
                    dayName=x.dayName
                }).Distinct().ToListAsyncSafe(),
            };
            return result;

        }
    }
}

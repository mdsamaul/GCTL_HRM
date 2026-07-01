using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Spreadsheet;
using GCTL.Core.Data;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.Users;
using GCTL.Data.Models;
using GCTL.Service.DeleteHistories;
using Microsoft.EntityFrameworkCore;

namespace GCTL.Service.Users
{
    public class UserService : AppService<CoreUserInfo>, IUserService
    {
        private readonly IRepository<CoreUserInfo> repository;
        private readonly IRepository<HrmEmployee> employeeRepository;
        private readonly IRepository<HrmEmployeeOfficialInfo> empOffRepository;
        private readonly IRepository<HrmDefDepartment> departmentRepository;
        private readonly IRepository<HrmDefDesignation> designationRepository;
        private readonly IRepository<CoreCompany> companyRepository;
        private readonly IRepository<CoreBranch> branchRepository;

        private readonly IRepository<HrmDefEmpType> empTypeRepository;
        private readonly IRepository<HrmEisDefEmploymentNature> empNatureRepository;
        private readonly IDeleteHistoryService deleteHistoryService;

        public UserService(IRepository<CoreUserInfo> repository,
                           IRepository<HrmEmployee> employeeRepository,
                           IRepository<HrmEmployeeOfficialInfo> empOffRepository,
                           IRepository<HrmDefDepartment> departmentRepository,
                           IRepository<HrmDefDesignation> designationRepository,
                           IRepository<CoreCompany> companyRepository,
                           IRepository<CoreBranch> branchRepository,
                           IRepository<HrmDefEmpType> empTypeRepository,
                           IRepository<HrmEisDefEmploymentNature> empNatureRepository,
                           IDeleteHistoryService deleteHistoryService) : base(repository)
        {
            this.repository = repository;
            this.employeeRepository = employeeRepository;
            this.empOffRepository = empOffRepository;
            this.departmentRepository = departmentRepository;
            this.designationRepository = designationRepository;
            this.companyRepository = companyRepository;
            this.branchRepository = branchRepository;
            this.empTypeRepository = empTypeRepository;
            this.empNatureRepository = empNatureRepository;
            this.deleteHistoryService = deleteHistoryService;
        }

        public async Task<int> GetIdByEmployee(string code)
        {
            int id = await repository
                        .FindBy(x => x.EmployeeId == code)
                        .Select(x => x.Id)
                        .FirstOrDefaultAsync();
            return id;
        }
        public async Task<int> GetIdByUser(string code)
        {
            int id = await repository
                        .FindBy(x => x.Username == code)
                        .Select(x => x.Id)
                        .FirstOrDefaultAsync();
            return id;
        }

        public async Task<IEnumerable<CoreUserInfo>> GetUsers()
        {
            return await repository.GetAllAsync();
        }

        public async Task<List<UserViewModel>> GetAllUsers()
        {
            return await (from u in repository.All()
                          join e in employeeRepository.All()
                          on u.EmployeeId equals e.EmployeeId into ue
                          from e in ue.DefaultIfEmpty()
                          select new UserViewModel
                          {
                              Id = u.Id,
                              AccessCode = u.AccessCode,
                              EmployeeId = !string.IsNullOrWhiteSpace(u.EmployeeId) ? u.EmployeeId : u.Id.ToString(),
                              EmployeeName = $"{e.FirstName} {e.LastName}",
                              Username = u.Username,
                              Role = u.Role
                          }).ToListAsync();
        }

        public async Task<List<CommonSelectModel>> GetEmployees()
        {
            var employees = await employeeRepository.All()
                .AsNoTracking()
                .Select(e => new { e.EmployeeId, e.FirstName, e.LastName })
                .ToListAsync();

            return employees.Select(e => new CommonSelectModel
            {
                Code = e.EmployeeId,
                Name = string.Join(" ", new[] { e.FirstName, e.LastName }
                                        .Where(s => !string.IsNullOrWhiteSpace(s)))
                       + $" ({e.EmployeeId})"
            }).ToList();
        }

        public async Task<List<CommonSelectModel>> GetCompanies()
        {
            var companies = await companyRepository.All()
                .AsNoTracking()
                .Select(e => new { e.CompanyCode, e.CompanyName })
                .ToListAsync();

            return companies.Select(e => new CommonSelectModel
            {
                Code = e.CompanyCode,
                Name = e.CompanyName
            }).ToList();
        }

        public async Task<List<CommonSelectModel>> GetBranch()
        {
            var branch = await branchRepository.All()
                .AsNoTracking()
                .Select(e => new { e.BranchCode, e.BranchName })
                .ToListAsync();

            return branch.Select(e => new CommonSelectModel
            {
                Code = e.BranchCode,
                Name = e.BranchName
            }).ToList();
        }

        public async Task<List<CommonSelectModel>> GetDepartments()
        {
            var department = await departmentRepository.All()
                .AsNoTracking()
                .Select(e => new { e.DepartmentCode, e.DepartmentName })
                .ToListAsync();

            return department.Select(e => new CommonSelectModel
            {
                Code = e.DepartmentCode,
                Name = e.DepartmentName
            }).ToList();
        }

        public async Task<CoreUserInfo> GetUser(int id)
        {
            return await repository.GetByIdAsync(id);
        }

        public async Task<CoreUserInfo> GetUser(string employeeId)
        {
            return await repository.FindBy(x => x.EmployeeId == employeeId).FirstOrDefaultAsync();
        }

        public async Task<UserViewModel> GetUserByEmployee(string employeeId)
        {
            return await (from u in repository.All()
                          join e in employeeRepository.All()
                          on u.EmployeeId equals e.EmployeeId into ue
                          from e in ue.DefaultIfEmpty()
                          where e.EmployeeId == employeeId
                          select new UserViewModel
                          {
                              Id = u.Id,
                              AccessCode = u.AccessCode,
                              EmployeeId = u.EmployeeId,
                              EmployeeName = e.FirstName,
                              Username = u.Username,
                              Role = u.Role
                          }).FirstOrDefaultAsync();
        }

        public async Task<UserViewModel> GetEmployeeDetails(string employeeId)
        {
            return await (
                        from e in employeeRepository.All().AsNoTracking()
                        join eoi in empOffRepository.All().AsNoTracking() on e.EmployeeId equals eoi.EmployeeId into eGroup
                        from eoi in eGroup.DefaultIfEmpty()
                        join c in companyRepository.All().AsNoTracking() on eoi.CompanyCode equals c.CompanyCode into cGroup
                        from c in cGroup.DefaultIfEmpty()
                        join b in branchRepository.All().AsNoTracking() on eoi.BranchCode equals b.BranchCode into bGroup
                        from b in bGroup.DefaultIfEmpty()
                        join type in empTypeRepository.All().AsNoTracking() on eoi.EmpTypeCode equals type.EmpTypeCode into typeGroup
                        from type in typeGroup.DefaultIfEmpty()
                        join nature in empNatureRepository.All().AsNoTracking() on eoi.EmploymentNatureId equals nature.EmploymentNatureId into natureGroup
                        from nature in natureGroup.DefaultIfEmpty()
                        join u in repository.All().AsNoTracking() on e.EmployeeId equals u.EmployeeId into uGroup
                        from u in uGroup.DefaultIfEmpty()
                        join des in designationRepository.All().AsNoTracking() on eoi.DepartmentCode equals des.DesignationCode into desGroup
                        from des in desGroup.DefaultIfEmpty()
                        join dep in departmentRepository.All().AsNoTracking() on eoi.DepartmentCode equals dep.DepartmentCode into depGroup
                        from dep in depGroup.DefaultIfEmpty()
                        where e.EmployeeId == employeeId
                        select new UserViewModel
                        {
                            EmployeeName = $"{e.FirstName} {e.LastName}",
                            NationalId = e.NationalIdno,
                            Company = c.CompanyName,
                            Branch = b.BranchName,
                            EmpNature = nature.EmploymentNature,
                            EmpType = type.EmpTypeName,
                            OffPhone = eoi.MobileNo,
                            OffEmail = eoi.Email,
                            JoiningDate = eoi.JoiningDate.HasValue
                                                    && eoi.JoiningDate.Value.Date != new DateTime(1900, 1, 1)
                                          ? eoi.JoiningDate.Value.ToString("dd/MM/yyyy")
                                          : "",
                            WorkStation = u.WorkStation,

                            DepartmentName = dep.DepartmentName,
                            DesignationName = des.DesignationName,
                            Username = u.Username,
                            Role = u.Role,
                            AccessCode = u.AccessCode
                        }
                    ).FirstOrDefaultAsync();
        }

        public async Task<UserViewModel> GetEmployeeDetailsByUser(string username)
        {
            return await (
                        from u in repository.All().AsNoTracking()
                        join e in employeeRepository.All().AsNoTracking() on u.EmployeeId equals e.EmployeeId into uGroup
                        from e in uGroup.DefaultIfEmpty()
                        join eoi in empOffRepository.All().AsNoTracking() on e.EmployeeId equals eoi.EmployeeId into eGroup
                        from eoi in eGroup.DefaultIfEmpty()
                        join c in companyRepository.All().AsNoTracking() on eoi.CompanyCode equals c.CompanyCode into cGroup
                        from c in cGroup.DefaultIfEmpty()
                        join b in branchRepository.All().AsNoTracking() on eoi.BranchCode equals b.BranchCode into bGroup
                        from b in bGroup.DefaultIfEmpty()
                        join type in empTypeRepository.All().AsNoTracking() on eoi.EmpTypeCode equals type.EmpTypeCode into typeGroup
                        from type in typeGroup.DefaultIfEmpty()
                        join nature in empNatureRepository.All().AsNoTracking() on eoi.EmploymentNatureId equals nature.EmploymentNatureId into natureGroup
                        from nature in natureGroup.DefaultIfEmpty()

                        join des in designationRepository.All().AsNoTracking() on eoi.DepartmentCode equals des.DesignationCode into desGroup
                        from des in desGroup.DefaultIfEmpty()
                        join dep in departmentRepository.All().AsNoTracking() on eoi.DepartmentCode equals dep.DepartmentCode into depGroup
                        from dep in depGroup.DefaultIfEmpty()
                        where u.Username == username
                        select new UserViewModel
                        {
                            EmployeeId = e.EmployeeId,
                            EmployeeName = $"{e.FirstName} {e.LastName}",
                            NationalId = e.NationalIdno,
                            Company = c.CompanyName,
                            Branch = b.BranchName,
                            EmpNature = nature.EmploymentNature,
                            EmpType = type.EmpTypeName,
                            OffPhone = eoi.MobileNo,
                            OffEmail = eoi.Email,

                            DepartmentName = dep.DepartmentName,
                            DesignationName = des.DesignationName,
                            Username = u.Username,
                            //Password = u.Password,
                            Role = u.Role,
                            AccessCode = u.AccessCode
                        }
                    ).FirstOrDefaultAsync();
        }

        public async Task<CoreUserInfo> GetBaseEmpData(string code)
        {
            if (string.IsNullOrWhiteSpace(code)) return new CoreUserInfo();

            return await (
                        from e in employeeRepository.All().AsNoTracking()
                        join eoi in empOffRepository.All().AsNoTracking() on e.EmployeeId equals eoi.EmployeeId into eGroup
                        from eoi in eGroup.DefaultIfEmpty()
                        where e.EmployeeId == code
                        select new CoreUserInfo
                        {
                            FirstName = e.FirstName != null ? e.FirstName : string.Empty,
                            LastName = e.LastName != null ? e.LastName : string.Empty,
                            Dob = e.DateOfBirthOrginal,
                            WorkStation = string.Empty,
                            Regulation = string.Empty,
                            //AccessPermissionDepartmentCode = string.Empty,
                            AccessPermissionJobTitle = string.Empty,
                            AccessPermissionDivisionCode = string.Empty,
                            OffPhone = eoi != null ? eoi.MobileNo : string.Empty,  // Fix 1
                            OffEmail = eoi != null ? eoi.Email : string.Empty,  // Fix 1
                            PerPhone = string.Empty,
                            PerEmail = string.Empty
                        }
                    ).FirstOrDefaultAsync() ?? new CoreUserInfo();
        }

        public async Task<CoreUserInfo> SaveUser(CoreUserInfo entity)
        {
            SanitizeStringProperties(entity);

            if (entity.Id > 0)
                await repository.UpdateAsync(entity);
            else
                await repository.AddAsync(entity);

            return entity;
        }

        private static void SanitizeStringProperties(object obj)
        {
            foreach (var prop in obj.GetType().GetProperties()
                         .Where(p => p.PropertyType == typeof(string) && p.CanWrite))
            {
                if (prop.GetValue(obj) is null)
                    prop.SetValue(obj, string.Empty);
            }
        }

        public async Task<bool> DeleteUser(int id, DeleteHistoryViewModel dm)
        {
            var tableName = repository.GetTableName();
            var deletedEntries = new List<CoreUserInfo>();

            await repository.BeginTransactionAsync();
            try
            {
                var company = await repository.GetByIdAsync(id);
                if (company == null)
                {
                    await repository.RollbackTransactionAsync();
                    return false;
                }

                await repository.DeleteAsync(company);

                deletedEntries.Add(company);
                if (!deletedEntries.Any())
                {
                    await repository.RollbackTransactionAsync();
                    return false;
                }

                dm.tableName = tableName;
                await deleteHistoryService.LogDeletedRecordsAsync(deletedEntries, dm);

                await repository.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await repository.RollbackTransactionAsync();
                return false;
            }

        }

        public async Task<bool> IsUserExistById(int id)
        {
            return await repository.FindBy(x => x.Id == id).AnyAsync();
        }

        public async Task<bool> IsUserExist(int id, string userName)
        {
            return await repository.FindBy(x => x.Id != id && x.Username == userName).AnyAsync();
        }

        public async Task<bool> IsUserExistByEmployee(string employeeId)
        {
            return await repository.FindBy(x => x.EmployeeId == employeeId).AnyAsync();
        }

        public async Task<bool> IsUserExistByName(string username)
        {
            return await repository.FindBy(x => x.Username == username).AnyAsync();
        }

        public async Task<bool> IsUserExistByName(string username, string employeeId)
        {
            return await repository.FindBy(x => x.Username == username && x.EmployeeId == employeeId).AnyAsync();
        }

        public async Task<IEnumerable<CommonSelectModel>> PreparerSelection(DefaultRoles role, string lUser)
        {
            var query = role == DefaultRoles.Admin
                ? repository.All()
                : repository.FindBy(x=>x.Username==lUser);

            return await query.Select(x => new CommonSelectModel
            {
                Code = string.IsNullOrWhiteSpace(x.EmployeeId) ? $"{x.Id}" : x.EmployeeId,
                Name = string.IsNullOrWhiteSpace(x.EmployeeId) ? x.Username : $"{x.Username} ({x.EmployeeId})"
            }).ToListAsync();
        }
    }
}

using GCTL.Core.Data;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.Users;
using GCTL.Data.Models;

namespace GCTL.Service.Users
{
    public class UserService : AppService<CoreUserInfo>, IUserService
    {
        private readonly IRepository<CoreUserInfo> repository;
        private readonly IRepository<HrmEmployee> employeeRepository;

        public UserService(IRepository<CoreUserInfo> repository,
                           IRepository<HrmEmployee> employeeRepository) : base(repository)
        {
            this.repository = repository;
            this.employeeRepository = employeeRepository;
        }

        public List<CoreUserInfo> GetUsers()
        {
            return GetAll();
        }

        public List<UserViewModel> GetAllUsers()
        {
            return (from u in repository.All()
                    join e in employeeRepository.All()
                    on u.EmployeeId equals e.EmployeeId into ue
                    from e in ue.DefaultIfEmpty()
                    select new UserViewModel
                    {
                        Id = u.Id,
                        AccessCode = u.AccessCode,
                        EmployeeId = u.EmployeeId ?? u.Id.ToString(),
                        EmployeeName = $"{e.FirstName} {e.LastName}",
                        Username = u.Username,
                        Role = u.Role
                    }).ToList();
        }

        public CoreUserInfo GetUser(int id)
        {
            return repository.GetById(id);
        }

        public CoreUserInfo GetUser(string employeeId)
        {
            return repository.All().FirstOrDefault(x => x.EmployeeId == employeeId);
        }

        public UserViewModel GetUserByEmployee(string employeeId)
        {
            return (from u in repository.All()
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
                    }).FirstOrDefault();
        }

        public CoreUserInfo SaveUser(CoreUserInfo entity)
        {
            if (entity.Id > 0)
                Update(entity);
            else
                Add(entity);

            return entity;
        }

        public bool DeleteUser(int id)
        {
            var company = GetUser(id);
            if (company != null)
            {
                repository.Delete(company);
                return true;
            }
            return false;
        }

        public bool IsUserExistById(int id)
        {
            return repository.All().Any(x => x.Id == id);
        }

        public bool IsUserExist(int id, string userName)
        {
            return repository.All().Any(x => x.Id != id && x.Username == userName);
            //if (id > 0)
            //    return repository.All().Where(x => x.Username == userName).Count() > 1;

            //return repository.All().Any(x => x.Username == userName);
        }

        public bool IsUserExistByEmployee(string employeeId)
        {
            return repository.All().Any(x => x.EmployeeId == employeeId);
        }

        public bool IsUserExistByName(string username)
        {
            return repository.All().Any(x => x.Username == username);
        }

        public bool IsUserExistByName(string username, string employeeId)
        {
            return repository.All().Any(x => x.Username == username && x.EmployeeId == employeeId);
        }

        public IEnumerable<CommonSelectModel> PreparerSelection(DefaultRoles role, string lUser)
        {
            if (role == DefaultRoles.Admin)
            {
                return repository.All().Select(x => new CommonSelectModel
                {
                    Code = string.IsNullOrWhiteSpace(x.EmployeeId) ? $"{x.Id}" : $"{x.EmployeeId}",
                    Name = string.IsNullOrWhiteSpace(x.EmployeeId) ? $"{x.Username}" : $"{x.Username} ({x.EmployeeId})"
                });
            }
            else
            {
                return repository.All().Where(x=> x.Username == lUser).Select(x => new CommonSelectModel
                {
                    Code = string.IsNullOrWhiteSpace(x.EmployeeId) ? $"{x.Id}" : $"{x.EmployeeId}",
                    Name = string.IsNullOrWhiteSpace(x.EmployeeId) ? $"{x.Username}" : $"{x.Username} ({x.EmployeeId})"
                });
            }
        }
    }
}

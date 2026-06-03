using DocumentFormat.OpenXml.Wordprocessing;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.Employees;
using GCTL.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections;

namespace GCTL.Service.Employees
{
    public class EmployeeService : AppService<HrmEmployee>, IEmployeeService
    {
        private readonly IRepository<HrmEmployee> employeeRepository;
        private readonly IRepository<HrmDefDepartment> departmentRepository;
        private readonly IRepository<HrmDefDesignation> designationRepository;

        public EmployeeService(IRepository<HrmEmployee> employeeRepository,
                               IRepository<HrmDefDepartment> departmentRepository,
                               IRepository<HrmDefDesignation> designationRepository)
            : base(employeeRepository)
        {
            this.employeeRepository = employeeRepository;
            this.departmentRepository = departmentRepository;
            this.designationRepository = designationRepository;
        }

        //public List<HrmEmployee> GetEmployees()
        //{
        //    return GetAll();
        //}

        public List<HrmEmployee> GetEmployees()
        {
            return (from e in employeeRepository.All()
                        //join dep in departmentRepository.All()
                        //on e.DepartmentCode equals dep.DepartmentCode into edep
                        //from dep in edep.DefaultIfEmpty()
                        //join des in designationRepository.All()
                        //on e.DesignationCode equals des.DesignationCode into edes
                        //from des in edes.DefaultIfEmpty()
                    select new HrmEmployee
                    {
                        EmployeeId = e.EmployeeId,
                        FirstName = e.FirstName,
                        LastName = e.LastName,
                        //PresentAddress = e.PresentAddress,
                        //MobileNo = e.MobileNo,
                        //Email = e.Email,
                        //DepartmentCode = dep.DepartmentName,
                        //DesignationCode = des.DesignationName,
                        //EmpPhoto= e.FirstName+" "+e.LastName
                    }).ToList();
        }

        public HrmEmployee GetEmployee(string id)
        {
            return employeeRepository.GetById(id)
;
        }


        public EmployeeViewModel GetEmployeeByCode(string employeeId)
        {
            return (from e in employeeRepository.All()
                        //join dep in departmentRepository.All()
                        //on e.DepartmentCode equals dep.DepartmentCode into edep
                        //from dep in edep.DefaultIfEmpty()
                        //join des in designationRepository.All()
                        //on e.DesignationCode equals des.DesignationCode into edes
                        //from des in edes.DefaultIfEmpty()
                    where e.EmployeeId == employeeId
                    select new EmployeeViewModel
                    {
                        EmployeeId = e.EmployeeId,
                        EmployeeName = e.FirstName,
                        //PresentAddress = e.PresentAddress,
                        //MobileNo = e.MobileNo,
                        //Email = e.Email,
                        //DepartmentName = dep.DepartmentName,
                        //DesignationName = des.DesignationName
                    }).FirstOrDefault();
        }

        public HrmEmployee SaveEmployee(HrmEmployee entity)
        {
            if (IsEmployeeExistByCode(entity.EmployeeId))
                Update(entity);
            else
                Add(entity);

            return entity;
        }

        public bool DeleteEmployee(string id)
        {
            var company = GetEmployee(id)
;
            if (company != null)
            {
                employeeRepository.Delete(company);
                return true;
            }
            return false;
        }

        public bool IsEmployeeExistByCode(string employeeId)
        {
            return employeeRepository.All().Any(x => x.EmployeeId == employeeId);
        }

        public bool IsEmployeeExist(string name)
        {
            return employeeRepository.All().Any(x => x.FirstName == name);
        }

        public bool IsEmployeeExist(string name, string employeeId)
        {
            return employeeRepository.All().Any(x => x.FirstName == name && x.EmployeeId != employeeId);
        }

        public IEnumerable GetEmployeeSelections()
        {
            return employeeRepository.All()
                  .Select(u => new
                  {
                      Code = u.EmployeeId,
                      Name = string.Format("{0} {1} ({2})", u.FirstName, u.LastName, u.EmployeeId)
                  }).ToList();
        }

        public IEnumerable GetEmployeeDropSelections()
        {
            return employeeRepository.All()
                  .Select(u => new
                  {
                      Code = u.EmployeeId,
                      Name = string.Format("{0} {1} ({2})", u.FirstName, u.LastName, u.EmployeeId)
                  }).ToList();

        }

        public async Task<EmployeeViewModel> GetEmployeeDetailsByCode(string code)
        {
            var Data = await (from e in employeeRepository.All()
                                  //join dep in departmentRepository.All()
                                  //on e.DepartmentCode equals dep.DepartmentCode into edep
                                  //from dep in edep.DefaultIfEmpty()
                                  //join des in designationRepository.All()
                                  //on e.DesignationCode equals des.DesignationCode into edes
                                  //from des in edes.DefaultIfEmpty()
                              where e.EmployeeId == code
                              select new EmployeeViewModel
                              {
                                  EmployeeId = e.EmployeeId,
                                  FullName = $"{e.FirstName} {e.LastName}",
                                  //DepartmentName = dep.DepartmentName,
                                  //DesignationName = des.DesignationName
                              }).FirstOrDefaultAsync();
            return Data;
        }
    }
}
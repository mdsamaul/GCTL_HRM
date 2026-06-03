using GCTL.Core.Data;
using GCTL.Core.ViewModels.Employees;
using GCTL.Data.Models;
using GCTL.Service.Common;

using GCTL.UI.Core.ViewModels.Employees;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;
using GCTL.Core.Helpers;

namespace GCTL.UI.Core.Controllers
{
//    public class EmployeesController : BaseController
//    {
//        private readonly IEmployeeService employeeService;
//        private readonly ICommonService commonService;
//        private readonly IRepository<HrmDefDepartment> departmentRepository;
//        private readonly IRepository<HrmDefBloodGroup> bloodRepository;
//        private readonly IRepository<HrmDefSex> sexRepository;
//        private readonly IRepository<HrmDefReligion> religionRepository;
//        private readonly IWebHostEnvironment webHostEnvironment;
//        private readonly IRepository<HrmDefDesignation> designationRepository;
//        private readonly IMapper mapper;
//        public EmployeesController(IEmployeeService employeeService,
//                                  ICommonService commonService,
//                                  IRepository<HrmDefDepartment> departmentRepository,
//                                  IRepository<HrmDefDesignation> designationRepository,
//                                  IRepository<HrmDefBloodGroup> bloodRepository,
//                                  IRepository<HrmDefSex> sexRepository,
//                                  IRepository<HrmDefReligion> religionRepository,
//                                  IWebHostEnvironment webHostEnvironment,
//                                  IMapper mapper)
//        {
//            this.employeeService = employeeService;
//            this.commonService = commonService;
//            this.departmentRepository = departmentRepository;
//            this.bloodRepository = bloodRepository;
//            this.sexRepository = sexRepository;
//            this.religionRepository = religionRepository;
//            this.webHostEnvironment = webHostEnvironment;
//            this.designationRepository = designationRepository;
//            this.mapper = mapper;
//        }

//        public ActionResult Index()
//        {
//            EmployeePageViewModel model = new EmployeePageViewModel()
//            {
//                PageUrl = Url.Action(nameof(Index)),
//                AddUrl = Url.Action(nameof(Setup))
//            };
//            return View(model);
//        }

//        public ActionResult Details(string id)
//        {
//            EmployeeSetupViewModel model = new EmployeeSetupViewModel();
//            var result = employeeService.GetEmployee(id)
//;
//            if (result == null)
//            {
//                return NotFound();
//            }

//            model = mapper.Map<EmployeeSetupViewModel>(result);
//            model.Code = id;

//            ViewBag.BloodGroup = new SelectList(bloodRepository.All(), "BloodGroupCode", "BloodGroup", model.BloodGroupCode);
//            ViewBag.Sex = new SelectList(sexRepository.All(), "SexCode", "Sex", model.SexCode);
//            ViewBag.Religion = new SelectList(religionRepository.All(), "ReligionCode", "Religion", model.ReligionCode);
//            ViewBag.Department = new SelectList(departmentRepository.All(), "DepartmentCode", "DepartmentName", model.DepartmentCode);
//            ViewBag.Designation = new SelectList(designationRepository.All(), "DesignationCode", "DesignationName", model.DesignationCode);

//            return View(model);
//        }

//        public ActionResult Setup(string id)
//        {
//            EmployeeSetupViewModel model = new EmployeeSetupViewModel();
//            var result = employeeService.GetEmployee(id)
//;
//            if (result != null)
//            {
//                model = mapper.Map<EmployeeSetupViewModel>(result);
//                model.Code = id;
//                model.DateOfBirth = result.DateOfBirthCertificate.HasValue ? result.DateOfBirthCertificate.Value.ToString("dd/MM/yyyy") : string.Empty;
//            }
//            else
//            {
//                model.EmployeeID = commonService.GenerateNextCode("EmployeeID", "HRM_Employee", 4);
//            }

//            model.PageUrl = Url.Action(nameof(Index));

//            ViewBag.BloodGroup = new SelectList(bloodRepository.All(), "BloodGroupCode", "BloodGroup", model.BloodGroupCode);
//            ViewBag.Sex = new SelectList(sexRepository.All(), "SexCode", "Sex", model.SexCode);
//            ViewBag.Religion = new SelectList(religionRepository.All(), "ReligionCode", "Religion", model.ReligionCode);
//            ViewBag.Department = new SelectList(departmentRepository.All(), "DepartmentCode", "DepartmentName", model.DepartmentCode);
//            ViewBag.Designation = new SelectList(designationRepository.All(), "DesignationCode", "DesignationName", model.DesignationCode);

//            return View(model);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Setup(EmployeeSetupViewModel model)
//        {
//            if (employeeService.IsEmployeeExist(model.FirstName, model.EmployeeID))
//            {
//                return Json(new { isSuccess = false, message = "Already Exists" });
//            }

//            if (ModelState.IsValid)
//            {
//                HrmEmployee employee = employeeService.GetEmployee(model.EmployeeID) ?? new HrmEmployee();
//                DateTime dateOfBirth = DateTime.Now.AddYears(-18);
//                if (!string.IsNullOrWhiteSpace(model.DateOfBirth))
//                {
//                    dateOfBirth = DateTime.ParseExact(model.DateOfBirth, "dd/MM/yyyy", CultureInfo.InvariantCulture);

//                }
//                model.ToAudit(LoginInfo);
//                mapper.Map(model, employee);
//                employee.DateOfBirthCertificate = dateOfBirth;

//                string directory = @"Uploads/Images/Employees";

//                //if (model.IsClearImage)
//                //{
//                //    employee.EmpPhoto = string.Empty;
//                //    model.Photo = null;
//                //}

//                string fileName = string.Empty;
//                if (model.Photo != null && model.Photo.Length > 0)
//                {
//                    string path = $"{webHostEnvironment.WebRootPath}/{directory}";
//                    if (!Directory.Exists(path))
//                        Directory.CreateDirectory(path);

//                    fileName = model.EmployeeID + Path.GetExtension(model.Photo.FileName);
//                    using (var stream = new FileStream($"{path}/{fileName}", FileMode.Create))
//                    {
//                        await model.Photo.CopyToAsync(stream)
//;
//                    }

//                    //employee.EmpPhoto = fileName;
//                }
//                else
//                {
//                    fileName = "~/images/images.png";
//                }

//                employeeService.SaveEmployee(employee);
//                return RedirectToAction("Index", "Employees");
//            }

//            ViewBag.BloodGroup = new SelectList(bloodRepository.All(), "BloodGroupCode", "BloodGroup", model.BloodGroupCode);
//            ViewBag.Sex = new SelectList(sexRepository.All(), "SexCode", "Sex", model.SexCode);
//            ViewBag.Religion = new SelectList(religionRepository.All(), "ReligionCode", "Religion", model.ReligionCode);
//            ViewBag.Department = new SelectList(departmentRepository.All(), "DepartmentCode", "DepartmentName", model.DepartmentCode);
//            ViewBag.Designation = new SelectList(designationRepository.All(), "DesignationCode", "DesignationName", model.DesignationCode);

//            return View(model);
//        }

//        public ActionResult Grid()
//        {
//            var resutl = employeeService.GetEmployees();
//            return Json(new { data = resutl });
//        }


//        [HttpPost]
//        public ActionResult Delete(string id)
//        {
//            bool success = false;
//            foreach (var item in id.Split(",", StringSplitOptions.RemoveEmptyEntries))
//            {
//                success = employeeService.DeleteEmployee(item);
//            }

//            return Json(new { success = success, message = "Deleted Successfully" });
//        }



//        [HttpPost]
//        public JsonResult CheckAvailability(string name, string code)
//        {
//            if (employeeService.IsEmployeeExist(name, code))
//            {
//                return Json(new { isSuccess = true, message = "Already Exists" });
//            }

//            return Json(new { isSuccess = false });
//        }
//    }
}
using AutoMapper;
using GCTL.Core.Data;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.UserAccesses;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.HrmEmployees2;
using GCTL.Service.Users;
using GCTL.UI.Core.ViewModels.UserAcesses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
//using GCTL.Service.EmployeeOfficialInfo;

namespace GCTL.UI.Core.Controllers
{
    public class UserAccessController : BaseController
    {
        private readonly IUserService userService;
        private readonly ICommonService commonService;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        private readonly IEncoderService encoderService;
        private readonly IMapper mapper;
        string strMaxNO = "";
        public UserAccessController(IUserService userService,
                                    ICommonService commonService,
                                    IRepository<CoreAccessCode> accessCodeRepository,
                                    IEncoderService encoderService,
                                    IMapper mapper)
        {
            this.userService = userService;
            this.commonService = commonService;
            this.accessCodeRepository = accessCodeRepository;
            this.encoderService = encoderService;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            UserAccessPageViewModel model = new UserAccessPageViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };
            commonService.FindMaxNo(ref strMaxNO, "CompanyCode", "Core_Company", 3);
            model.Setup = new UserAccessSetupViewModel();

            ViewBag.Roles = new SelectList(new List<string>() { "Admin", "User" });
            ViewBag.Sessions = new SelectList(new List<string>() { "Single", "Multiple" });
            ViewBag.AccessCodes = new SelectList(accessCodeRepository.All().GroupBy(p => p.AccessCodeId).Select(x => x.FirstOrDefault()).ToList(), "AccessCodeId", "AccessCodeName");
            ViewBag.Employees = new SelectList(await userService.GetEmployees(), "Code", "Name");
            ViewBag.Companies = new SelectList(await userService.GetCompanies(), "Code", "Name");
            ViewBag.Branches = new SelectList(await userService.GetBranch(), "Code", "Name");
            ViewBag.Departments = new SelectList(await userService.GetDepartments(), "Code", "Name");

            return View(model);
        }

        public async Task<IActionResult> Setup(int id, string employeeId = null)
        {
            UserAccessSetupViewModel model = new UserAccessSetupViewModel();
            var result = await userService.GetUser(id);
            if (result != null)
            {
                model = mapper.Map<UserAccessSetupViewModel>(result);
                model.Code = result.EmployeeId;
                model.UserId = result.Id;

                string password = "";
                try
                {
                    new PXLibrary.PXlibrary().PXDEcode(ref password, result.Password);
                }
                catch (Exception)
                {
                    password = result.Password;
                }

                model.UserPassword = password;

                var employee = await userService.GetEmployeeDetails(result.EmployeeId);
                if (employee != null)
                {
                    model.EmployeeName = employee.EmployeeName;
                    model.DepartmentName = employee.DepartmentName;
                    model.DesignationName = employee.DesignationName;
                    model.NationalId = employee.NationalId;
                    model.Company = employee.Company;
                    model.Branch = employee.Branch;
                    model.EmployeeType = employee.EmpType;
                    model.EmployeeNature = employee.EmpNature;
                    model.OfficePhone = employee.OffPhone;
                    model.OfficeEmail = employee.OffEmail;
                    model.JoiningDate = employee.JoiningDate;
                }
            }
            else
            {
                var employee = await userService.GetEmployeeDetails(employeeId);
                if (employee != null)
                {
                    model.EmployeeId = employeeId;
                    model.EmployeeName = employee.EmployeeName;
                    model.DepartmentName = employee.DepartmentName;
                    model.DesignationName = employee.DesignationName;
                    model.NationalId = employee.NationalId;
                    model.Company = employee.Company;
                    model.Branch = employee.Branch;
                    model.EmployeeType = employee.EmpType;
                    model.EmployeeNature = employee.EmpNature;
                    model.OfficePhone = employee.OffPhone;
                    model.OfficeEmail = employee.OffEmail;
                    model.JoiningDate = employee.JoiningDate;
                }
            }

            ViewBag.Roles = new SelectList(Enum.GetNames<DefaultRoles>(), model.Role);
            ViewBag.Sessions = new SelectList(new List<string>() { "Single", "Multiple" });
            ViewBag.AccessCodes = new SelectList(accessCodeRepository.All().GroupBy(p => p.AccessCodeId).Select(x => x.FirstOrDefault()).ToList(), "AccessCodeId", "AccessCodeName", model.AccessCode);
            ViewBag.Employees = new SelectList(await userService.GetEmployees(), "Code", "Name", model.EmployeeId);
            ViewBag.Companies = new SelectList(await userService.GetCompanies(), "Code", "Name");
            ViewBag.Branches = new SelectList(await userService.GetBranch(), "Code", "Name");
            ViewBag.Departments = new SelectList(await userService.GetDepartments(), "Code", "Name");
            return PartialView($"_{nameof(Setup)}", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Setup(UserAccessSetupViewModel model)
        {
            if (await userService.IsUserExist(model.UserId, model.Username))
                return Json(new { success = false, message = "Already Exists" });

            if (!ModelState.IsValid)
                return Json(new { success = false, message = ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage });

            model.CompanyCode = "001";
            var user = await userService.GetUser(model.UserId) ?? await userService.GetBaseEmpData(model.EmployeeId);
            model.ToAudit(LoginInfo, model.UserId > 0);

            if (model.UserId > 0)
                user.ModifyDate = DateTime.Now;
            else
            {
                user.Ldate = DateTime.Now;
                user.EntryDate = DateTime.Now;
            }

            mapper.Map(model, user);

            string Userpassword = "";
            encoderService.PXEncode(ref Userpassword, model.UserPassword);
            new PXLibrary.PXlibrary().PXEncode(ref Userpassword, model.UserPassword);
            user.Password = Userpassword;

            await userService.SaveUser(user);
            return Json(new { isSuccess = true, message = "Saved Successfully" });
        }

        public async Task<ActionResult> Grid()
        {
            var result = await userService.GetAllUsers();
            return Json(new { data = result });
        }


        [HttpPost]
        public async Task<ActionResult> Delete(string id)
        {
            bool success = false;

            DeleteHistoryViewModel dm = new DeleteHistoryViewModel();
            dm.ToAudit(LoginInfo);
            dm.CompanyCode = LoginInfo.CompanyCode;

            foreach (var item in id.Split(",", StringSplitOptions.RemoveEmptyEntries))
            {
                success = await userService.DeleteUser(int.Parse(item), dm);
            }
            return Json(new { success = success, message = "Deleted Successfully" });
        }

        public async Task<IActionResult> GetEmployee(string userId)
        {
            int id = await userService.GetIdByEmployee(userId);

            return await Setup(id, id == 0 ? userId : null);
        }

        public async Task<IActionResult> GetEmployeeByUser(string username)
        {
            int id = await userService.GetIdByUser(username);
            return await Setup(id);
        }
    }
}
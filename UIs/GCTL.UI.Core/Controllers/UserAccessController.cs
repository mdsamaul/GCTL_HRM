using GCTL.Core.Data;
using GCTL.Core.ViewModels.UserAccesses;
using GCTL.Data.Models;
using GCTL.Service.Common;

using GCTL.Service.Users;
using GCTL.UI.Core.ViewModels.UserAcesses;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using GCTL.Core.Helpers;
using GCTL.Service.HrmEmployees2;
//using GCTL.Service.EmployeeOfficialInfo;

namespace GCTL.UI.Core.Controllers
{
    public class UserAccessController : BaseController
    {
        private readonly IUserService userService;
        private readonly ICommonService commonService;
        private readonly IRepository<CoreAccessCode> accessCodeRepository;
        //private readonly IEmployeeOfficialInfoService employeeService;
        private readonly IEncoderService encoderService;
        private readonly IMapper mapper;
        string strMaxNO = "";
        public UserAccessController(IUserService userService,
                                    ICommonService commonService,
                                    IRepository<CoreAccessCode> accessCodeRepository,
                                    //IEmployeeOfficialInfoService employeeService,
                                    IEncoderService encoderService,
                                    IMapper mapper)
        {
            this.userService = userService;
            this.commonService = commonService;
            this.accessCodeRepository = accessCodeRepository;
            //this.employeeService = employeeService;
            this.encoderService = encoderService;
            this.mapper = mapper;
        }

        public async Task< IActionResult> Index()
        {
            UserAccessPageViewModel model = new UserAccessPageViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };
            commonService.FindMaxNo(ref strMaxNO, "CompanyCode", "Core_Company", 3);
            model.Setup = new UserAccessSetupViewModel
            {

            };

            ViewBag.Roles = new SelectList(new List<string>() { "Admin", "User" });
            ViewBag.AccessCodes = new SelectList(accessCodeRepository.All().GroupBy(p => p.AccessCodeId).Select(x => x.FirstOrDefault()).ToList(), "AccessCodeId", "AccessCodeName");
            //ViewBag.Employees = new SelectList( employeeService.EmployeeTypeSelection(), "Code", "Name");
            return View(model);
        }

        public   IActionResult Setup(int id)
        {
            UserAccessSetupViewModel model = new UserAccessSetupViewModel();
            var result = userService.GetUser(id);
            if (result != null)
            {
                model = mapper.Map<UserAccessSetupViewModel>(result);
                model.Code = result.EmployeeId;
                model.UserId = result.Id;

                //var employee = employeeService.GetEmployeeDetailsByCode(result.EmployeeId);
                //if (employee != null)
                //{
                //    //model.DepartmentName = employee.DepartmentName;
                //    //model.DesignationName = employee.DesignationName;
                //}
            }

            ViewBag.Roles = new SelectList(Enum.GetNames<DefaultRoles>(), model.Role);
            ViewBag.AccessCodes = new SelectList(accessCodeRepository.All().GroupBy(p => p.AccessCodeId).Select(x => x.FirstOrDefault()).ToList(), "AccessCodeId", "AccessCodeName", model.AccessCode);
           // ViewBag.Employees = new SelectList( employeeService.EmployeeTypeSelection(), "Code", "Name", model.EmployeeId);
            return PartialView($"_{nameof(Setup)}", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Setup(UserAccessSetupViewModel model)
        {
            if (userService.IsUserExist(model.UserId, model.Username))
            {
                return Json(new { success = false, message = "Already Exists" });
            }
            //CoreUserInfo user = userService.GetUser(model.EmployeeId);
            //if (user == null)
            //{
            //    if (userService.IsUserExistByName(model.Username, model.EmployeeId))
            //    {
            //        return Json(new { success = false, message = "Already Exists" });
            //    }
            //}
            if (ModelState.IsValid)
            {
                model.CompanyCode = "001";
                var user = userService.GetUser(model.UserId) ?? new CoreUserInfo();
                model.ToAudit(LoginInfo, model.UserId > 0);
                mapper.Map(model, user);
                //user.Id = model.UserId;
                string Userpassword = "";
                // encoderService.PXEncode(ref Userpassword, model.UserPassword);
                new PXLibrary.PXlibrary().PXEncode(ref Userpassword, model.UserPassword);
                user.Password = Userpassword;
                userService.SaveUser(user);
                return Json(new { isSuccess = true, message = "Saved Successfully" });
            }

            return Json(new { success = false, message = ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage });
        }

        public ActionResult Grid()
        {
            var result = userService.GetAllUsers();
            return Json(new { data = result });
        }


        [HttpPost]
        public ActionResult Delete(string id)
        {
            bool success = false;
            foreach (var item in id.Split(",", StringSplitOptions.RemoveEmptyEntries))
            {
                success = userService.DeleteUser(int.Parse(item));
            }
            return Json(new { success = success, message = "Deleted Successfully" });
        }

        //public IActionResult GetEmployee(string id)
        //{
        //    //var result = employeeService.GetEmployeeDetailsByCode(id);
        //    //return Json(result);
        //}
    }
}
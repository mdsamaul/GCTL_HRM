using GCTL.Core.ViewModels.DoctorTypes;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.DoctorTypes;
using GCTL.UI.Core.ViewModels.DoctorTypes;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using GCTL.Core.Helpers;
//using GCTL.Service.Prescriptions;

namespace GCTL.UI.Core.Controllers
{
    public class DoctorTypesController : BaseController
    {
        private readonly IDoctorTypeService doctorTypeService;
        private readonly ICommonService commonService;
        private readonly IMapper mapper;
        string strMaxNO = "";
        public DoctorTypesController(IDoctorTypeService doctorTypeService,
                                     ICommonService commonService,
                                     IMapper mapper)
        {
            this.doctorTypeService = doctorTypeService;
            this.commonService = commonService;
            this.mapper = mapper;
        }

        public IActionResult Index()
        {
            DoctorTypePageViewModel model = new()
            {
                PageUrl = Url.Action(nameof(Index))
            };
            commonService.FindMaxNo(ref strMaxNO, "DoctorTypeCode", "HMS_DoctorType", 3);
            model.Setup = new DoctorTypeSetupViewModel
            {
                DoctorTypeCode = strMaxNO
            };
            return View(model);
        }


        public IActionResult Setup(string id)
        {
            DoctorTypeSetupViewModel model = new();
            commonService.FindMaxNo(ref strMaxNO, "DoctorTypeCode", "HMS_DoctorType", 3);
            var result = doctorTypeService.GetDoctorType(id);
            if (result != null)
            {
                model = mapper.Map<DoctorTypeSetupViewModel>(result);
                model.Code = id;
                model.Id = result.DoctorTypeId;
            }
            else
            {
                model.DoctorTypeCode = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Setup(DoctorTypeSetupViewModel model)
        {
            if (doctorTypeService.IsDoctorTypeExist(model.DoctorTypeName, model.DoctorTypeCode))
            {
                return Json(new { isSuccess = false, message = "Already Exists" });
            }

            if (ModelState.IsValid)
            {
                if (doctorTypeService.IsDoctorTypeExistByCode(model.DoctorTypeCode))
                {
                    var hasPermission = doctorTypeService.UpdatePermission(LoginInfo.AccessCode);
                    if (hasPermission)
                    {
                        HmsDoctorType doctorType = doctorTypeService.GetDoctorType(model.DoctorTypeCode) ?? new HmsDoctorType();
                        model.ToAudit(LoginInfo, model.Id > 0);
                        mapper.Map(model, doctorType);
                        doctorType.CompanyCode = "";
                        doctorType.EmployeeId = "";
                        doctorTypeService.SaveDoctorType(doctorType);
                        return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = doctorType.DoctorTypeCode });
                    }
                    else
                    {

                        return Json(new { isSuccess = false, message = "You have no access" });
                    }

                }
                else
                {
                    var hasPermission = doctorTypeService.SavePermission(LoginInfo.AccessCode);
                    if (hasPermission)
                    {
                        HmsDoctorType doctorType = doctorTypeService.GetDoctorType(model.DoctorTypeCode) ?? new HmsDoctorType();
                        model.ToAudit(LoginInfo, model.Id > 0);
                        mapper.Map(model, doctorType);
                        doctorType.CompanyCode = "";
                        doctorType.EmployeeId = "";
                        doctorTypeService.SaveDoctorType(doctorType);
                        return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = doctorType.DoctorTypeCode });
                    }
                    else
                    {

                        return Json(new { isSuccess = false, message = "You have no access" });
                    }
                }
                
            }

            return Json(new { success = false, message = ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage });
        }

        public ActionResult Grid()
        {
            var resutl = doctorTypeService.GetDoctorTypes();
            return Json(new { data = resutl });
        }


        [HttpPost]
        public ActionResult Delete(string id)
        {
            var hasPermission = doctorTypeService.DeletePermission(LoginInfo.AccessCode);
            if (hasPermission)
            {
                bool success = false;
                foreach (var item in id.Split(",", StringSplitOptions.RemoveEmptyEntries))
                {
                    success = doctorTypeService.DeleteDoctorType(item);
                }

                return Json(new { success, message = "Deleted Successfully" });
            }
            else
            {
                return Json(new { isSuccess = false, message = "You have no access" });
            }
          
        }


        [HttpPost]
        public JsonResult CheckAvailability(string name, string code)
        {
            if (doctorTypeService.IsDoctorTypeExist(name, code))
            {
                return Json(new { isSuccess = true, message = "Already Exists" });
            }

            return Json(new { isSuccess = false });
        }
    }
}
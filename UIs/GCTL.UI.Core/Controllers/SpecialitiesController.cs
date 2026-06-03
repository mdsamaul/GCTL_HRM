using GCTL.Data.Models;
using GCTL.Service.Common;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using GCTL.Core.Helpers;
using GCTL.Service.Specialities;
using GCTL.UI.Core.ViewModels.Specialities;
using GCTL.Core.ViewModels.Specialities;

namespace GCTL.UI.Core.Controllers
{
    public class SpecialitiesController : BaseController
    {
        private readonly ISpecialityService specialityService;
        private readonly ICommonService commonService;
        private readonly IMapper mapper;
        string strMaxNO = "";
        public SpecialitiesController(ISpecialityService specialityService,
                                      ICommonService commonService,
                                      IMapper mapper)
        {
            this.specialityService = specialityService;
            this.commonService = commonService;
            this.mapper = mapper;
        }

        public IActionResult Index()
        {
            SpecialityPageViewModel model = new SpecialityPageViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };
            commonService.FindMaxNo(ref strMaxNO, "SpecialityCode", "HMS_Speciality", 3);
            model.Setup = new SpecialitySetupViewModel
            {
                SpecialityCode = strMaxNO
            };
            return View(model);
        }


        public IActionResult Setup(string id)
        {
            SpecialitySetupViewModel model = new SpecialitySetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "SpecialityCode", "HMS_Speciality", 3);
            var result = specialityService.GetSpeciality(id);
            if (result != null)
            {
                model = mapper.Map<SpecialitySetupViewModel>(result);
                model.Code = id;
                model.Id = (int)result.AutoId;
            }
            else
            {
                model.SpecialityCode = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Setup(SpecialitySetupViewModel model)
        {
            if (specialityService.IsSpecialityExist(model.SpecialityName, model.SpecialityCode))
            {
                return Json(new { isSuccess = false, message = "Already Exists" });
            }

            if (ModelState.IsValid)
            {
                if (specialityService.IsSpecialityExistByCode(model.SpecialityCode))
                {
                    var hasPermission = specialityService.UpdatePermission(LoginInfo.AccessCode);
                    if (hasPermission)
                    {
                        HmsSpeciality speciality = specialityService.GetSpeciality(model.SpecialityCode) ?? new HmsSpeciality();
                        model.ToAudit(LoginInfo, model.Id > 0);
                        mapper.Map(model, speciality);
                        speciality.CompanyCode = "";
                        speciality.EmployeeId = "";
                        specialityService.SaveSpeciality(speciality);
                        return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = speciality.SpecialityCode });
                    }
                    else
                    {

                        return Json(new { isSuccess = false, message = "You have no access" });
                    }

                }
                else
                {
                    var hasPermission = specialityService.SavePermission(LoginInfo.AccessCode);
                    if (hasPermission)
                    {
                        HmsSpeciality speciality = specialityService.GetSpeciality(model.SpecialityCode) ?? new HmsSpeciality();
                        model.ToAudit(LoginInfo, model.Id > 0);
                        mapper.Map(model, speciality);
                        speciality.CompanyCode = "";
                        speciality.EmployeeId = "";
                        specialityService.SaveSpeciality(speciality);
                        return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = speciality.SpecialityCode });
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
            var resutl = specialityService.GetSpecialities();
            return Json(new { data = resutl });
        }


        [HttpPost]
        public ActionResult Delete(string id)
        {
            var hasPermission = specialityService.DeletePermission(LoginInfo.AccessCode);
            if (hasPermission)
            {
                bool success = false;
                foreach (var item in id.Split(",", StringSplitOptions.RemoveEmptyEntries))
                {
                    success = specialityService.DeleteSpeciality(item);
                }

                return Json(new { success = success, message = "Deleted Successfully" });
            }
            else
            {
                return Json(new { isSuccess = false, message = "You have no access" });
            }
   
        }


        [HttpPost]
        public JsonResult CheckAvailability(string name, string code)
        {
            if (specialityService.IsSpecialityExist(name, code))
            {
                return Json(new { isSuccess = true, message = "Already Exists" });
            }

            return Json(new { isSuccess = false });
        }
    }
}
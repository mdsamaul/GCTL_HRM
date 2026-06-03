using GCTL.Core.ViewModels.DoctorWorkingPlace;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.DoctorWorkingPalace;
using GCTL.UI.Core.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using GCTL.Core.Helpers;
using GCTL.UI.Core.ViewModels.DoctorWorkingPlace;

namespace GCTL.UI.Core.Controllers
{
    public class DoctorWorkingPlaceController : BaseController
    {
        private readonly IDoctorWorkingPlaceService doctorWorkingPlaceServiceService;
        private readonly ICommonService commonService;
        private readonly IMapper mapper;
        string strMaxNO = "";
        public DoctorWorkingPlaceController(IDoctorWorkingPlaceService doctorWorkingPlaceServiceService,
                                     ICommonService commonService,
                                     IMapper mapper)
        {
            this.doctorWorkingPlaceServiceService = doctorWorkingPlaceServiceService;
            this.commonService = commonService;
            this.mapper = mapper;
        }

        public IActionResult Index(bool child = false)
        {
            DoctorWokingPlacePageViewModel model = new DoctorWokingPlacePageViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };
            commonService.FindMaxNo(ref strMaxNO, "WorkingPlaceCode", "HMS_DoctorWorkingPlace", 3);
            model.Setup = new DoctorWorkingPlaceSetupViewModel
            {
                WorkingPlaceCode = strMaxNO
            };

            if (child)
                return PartialView(model);

            return View(model);
        }


        public IActionResult Setup(string id)
        {
            DoctorWorkingPlaceSetupViewModel model = new DoctorWorkingPlaceSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "WorkingPlaceCode", "HMS_DoctorWorkingPlace", 3);
            var result = doctorWorkingPlaceServiceService.GetWorkingPlace(id);
            if (result != null)
            {
                model = mapper.Map<DoctorWorkingPlaceSetupViewModel>(result);
                model.Code = id;
                model.WorkingPlaceCode = (string)result.WorkingPlaceCode;
            }
            else
            {
                model.WorkingPlaceCode = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Setup(DoctorWorkingPlaceSetupViewModel model)
        {
            if (doctorWorkingPlaceServiceService.IsWorkingPlaceExist(model.WorkingPlaceName, model.WorkingPlaceCode))
            {
                return Json(new { isSuccess = false, message = "Already Exists" });
            }

            if (ModelState.IsValid)
            {
                if (doctorWorkingPlaceServiceService.IsWorkingPlaceExistByCode(model.WorkingPlaceCode))
                {
                    var hasPermission = doctorWorkingPlaceServiceService.UpdatePermission(LoginInfo.AccessCode);
                    if (hasPermission)
                    {
                        HmsDoctorWorkingPlace WorkingPalace = doctorWorkingPlaceServiceService.GetWorkingPlace(model.WorkingPlaceCode) ?? new HmsDoctorWorkingPlace();
                        model.ToAudit(LoginInfo, model.Id > 0);
                        mapper.Map(model, WorkingPalace);
                        doctorWorkingPlaceServiceService.SaveWorkingPlace(WorkingPalace);
                        return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = WorkingPalace.WorkingPlaceCode });
                    }
                    else
                    {

                        return Json(new { isSuccess = false, message = "You have no access" });
                    }

                }
                else
                {
                    var hasPermission = doctorWorkingPlaceServiceService.SavePermission(LoginInfo.AccessCode);
                    if (hasPermission)
                    {
                        HmsDoctorWorkingPlace WorkingPalace = doctorWorkingPlaceServiceService.GetWorkingPlace(model.WorkingPlaceCode) ?? new HmsDoctorWorkingPlace();
                        model.ToAudit(LoginInfo, model.Id > 0);
                        mapper.Map(model, WorkingPalace);
                        doctorWorkingPlaceServiceService.SaveWorkingPlace(WorkingPalace);
                        return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = WorkingPalace.WorkingPlaceCode });
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
            var result = doctorWorkingPlaceServiceService.GetWorkingPlaces();
            return Json(new { data = result });
        }


        [HttpPost]
        public ActionResult Delete(string id)
        {
            var hasPermission = doctorWorkingPlaceServiceService.DeletePermission(LoginInfo.AccessCode);
            if (hasPermission)
            {
                bool success = false;
                foreach (var item in id.Split(",", StringSplitOptions.RemoveEmptyEntries))
                {
                    success = doctorWorkingPlaceServiceService.DeleteWorkingPlace(item);
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
            if (doctorWorkingPlaceServiceService.IsWorkingPlaceExist(name, code))
            {
                return Json(new { isSuccess = true, message = "Already Exists" });
            }

            return Json(new { isSuccess = false });
        }
    }
}
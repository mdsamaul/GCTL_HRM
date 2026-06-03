using GCTL.Core.ViewModels.Shifts;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.Shifts;
using GCTL.UI.Core.ViewModels.Shifts;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using GCTL.Core.Helpers;

namespace GCTL.UI.Core.Controllers
{
    public class ShiftsController : BaseController
    {
        private readonly IShiftService shiftService;
        private readonly ICommonService commonService;
        private readonly IMapper mapper;
        string strMaxNO = "";
        public ShiftsController(IShiftService shiftService,
                                ICommonService commonService,
                                IMapper mapper)
        {
            this.shiftService = shiftService;
            this.commonService = commonService;
            this.mapper = mapper;
        }

        public IActionResult Index()
        {
            ShiftPageViewModel model = new ShiftPageViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };
            commonService.FindMaxNo(ref strMaxNO, "ShiftCode", "HMS_Shift", 3);
            model.Setup = new ShiftSetupViewModel
            {
                ShiftCode = strMaxNO
            };
            return View(model);
        }


        public IActionResult Setup(string id)
        {
            ShiftSetupViewModel model = new ShiftSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "ShiftCode", "HMS_Shift", 3);
            var result = shiftService.GetShift(id);
            if (result != null)
            {
                model = mapper.Map<ShiftSetupViewModel>(result);
                model.Code = id;
                model.Id = (int)result.AutoId;
            }
            else
            {
                model.ShiftCode = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Setup(ShiftSetupViewModel model)
        {
            if (shiftService.IsShiftExist(model.ShiftName, model.ShiftCode))
            {
                return Json(new { isSuccess = false, message = "Already Exists" });
            }

            if (ModelState.IsValid)
            {
                if (shiftService.IsShiftExistByCode(model.ShiftCode))
                {
                    var hasPermission = shiftService.UpdatePermission(LoginInfo.AccessCode);
                    if (hasPermission)
                    {
                        HmsShift shift = shiftService.GetShift(model.ShiftCode) ?? new HmsShift();
                        model.ToAudit(LoginInfo, model.Id > 0);
                        mapper.Map(model, shift);
                        shift.CompanyCode = "";
                        shift.EmployeeId = "";
                        shiftService.SaveShift(shift);
                        return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = shift.ShiftCode });
                    }
                    else
                    {

                        return Json(new { isSuccess = false, message = "You have no access" });
                    }

                }
                else
                {
                    var hasPermission = shiftService.SavePermission(LoginInfo.AccessCode);
                    if (hasPermission)
                    {
                        HmsShift shift = shiftService.GetShift(model.ShiftCode) ?? new HmsShift();
                        model.ToAudit(LoginInfo, model.Id > 0);
                        mapper.Map(model, shift);
                        shift.CompanyCode = "";
                        shift.EmployeeId = "";
                        shiftService.SaveShift(shift);
                        return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = shift.ShiftCode });
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
            var result = shiftService.GetShifts();
            return Json(new { data = mapper.Map<List<ShiftViewModel>>(result) });
        }


        [HttpPost]
        public ActionResult Delete(string id)
        {
            var hasPermission = shiftService.DeletePermission(LoginInfo.AccessCode);
            if (hasPermission)
            {
                bool success = false;
                foreach (var item in id.Split(",", StringSplitOptions.RemoveEmptyEntries))
                {
                    success = shiftService.DeleteShift(item);
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
            if (shiftService.IsShiftExist(name, code))
            {
                return Json(new { isSuccess = true, message = "Already Exists" });
            }

            return Json(new { isSuccess = false });
        }
    }
}
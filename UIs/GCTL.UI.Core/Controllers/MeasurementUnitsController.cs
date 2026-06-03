using GCTL.Core.ViewModels.Units;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.Units;
using GCTL.UI.Core.ViewModels.Units;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using GCTL.Core.Helpers;

namespace GCTL.UI.Core.Controllers
{
    public class MeasurementUnitsController : BaseController
    {
        private readonly IMeasurementUnitService unitService;
        private readonly ICommonService commonService;
        private readonly IMapper mapper;
        string strMaxNO = "";
        public MeasurementUnitsController(IMeasurementUnitService unitService,
                                     ICommonService commonService,
                                     IMapper mapper)
        {
            this.unitService = unitService;
            this.commonService = commonService;
            this.mapper = mapper;
        }

        public IActionResult Index()
        {
            MeasurementUnitPageViewModel model = new MeasurementUnitPageViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };
            commonService.FindMaxNo(ref strMaxNO, "UnitId", "Core_MeasurementUnit", 2);
            model.Setup = new MeasurementUnitSetupViewModel
            {
                UnitId = strMaxNO
            };

            return View(model);
        }


        public IActionResult Setup(string id)
        {
            MeasurementUnitSetupViewModel model = new MeasurementUnitSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "UnitId", "Core_MeasurementUnit", 2);
            var result = unitService.GetUnit(id);
            if (result != null)
            {
                model = mapper.Map<MeasurementUnitSetupViewModel>(result);
                model.Code = id;
                model.AutoId = result.AutoId;
            }
            else
            {
                model.UnitId = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Setup(MeasurementUnitSetupViewModel model)
        {
            if (unitService.IsUnitExist(model.UnitName, model.UnitId))
            {
                return Json(new { isSuccess = false, message = "Already Exists" });
            }

            if (ModelState.IsValid)
            {
                CoreMeasurementUnit unit = unitService.GetUnit(model.UnitId) ?? new CoreMeasurementUnit();
                model.ToAudit(LoginInfo, model.AutoId > 0);
                mapper.Map(model, unit);
                unitService.SaveUnit(unit);
                return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = unit.UnitId });
            }

            return Json(new { success = false, message = ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage });
        }

        public ActionResult Grid()
        {
            var result = unitService.GetUnits();
            return Json(new { data = result });
        }


        [HttpPost]
        public ActionResult Delete(string id)
        {
            bool success = false;
            foreach (var item in id.Split(",", StringSplitOptions.RemoveEmptyEntries))
            {
                success = unitService.DeleteUnit(item);
            }

            return Json(new { success = success, message = "Deleted Successfully" });
        }


        [HttpPost]
        public JsonResult CheckAvailability(string name, string code)
        {
            if (unitService.IsUnitExist(name, code))
            {
                return Json(new { isSuccess = true, message = "Already Exists" });
            }

            return Json(new { isSuccess = false });
        }
    }
}
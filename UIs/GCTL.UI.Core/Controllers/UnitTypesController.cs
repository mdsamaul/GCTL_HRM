using GCTL.Core.ViewModels.UnitTypes;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.UnitTypes;
using GCTL.UI.Core.ViewModels.UnitTypes;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using GCTL.Core.Helpers;
using Microsoft.Extensions.Options;
using GCTL.Service.Loggers;
using GCTL.Core.DataTables;

namespace GCTL.UI.Core.Controllers
{
    public class UnitTypesController : BaseController
    {
        private readonly IUnitTypeService unitTypeService;
        private readonly ICommonService commonService;
        private readonly IMapper mapper;
        string strMaxNO = "";
        public UnitTypesController(IUnitTypeService unitTypeService,
                                   ICommonService commonService,
                                   IMapper mapper)
        {
            this.unitTypeService = unitTypeService;
            this.commonService = commonService;
            this.mapper = mapper;
        }

        public IActionResult Index(bool child = false)
        {
            UnitTypePageViewModel model = new UnitTypePageViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };
            commonService.FindMaxNo(ref strMaxNO, "UnitTypId", "RMG_Prod_Def_UnitType", 3);
            model.Setup = new UnitTypeSetupViewModel
            {
                UnitTypId = strMaxNO
            };

            if (child)
                return PartialView(model);

            return View(model);
        }


        public IActionResult Setup(string id)
        {
            UnitTypeSetupViewModel model = new UnitTypeSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "UnitTypId", "RMG_Prod_Def_UnitType", 3);
            var result = unitTypeService.GetUnitType(id);
            if (result != null)
            {
                model = mapper.Map<UnitTypeSetupViewModel>(result);
                model.Code = id;
            }
            else
            {
                model.UnitTypId = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Setup(UnitTypeSetupViewModel model)
        {
            if (unitTypeService.IsUnitTypeExist(model.UnitTypeName, model.UnitTypId))
            {
                return Json(new { isSuccess = false, message = "Already Exists" });
            }

            if (ModelState.IsValid)
            {
                RmgProdDefUnitType unitType = unitTypeService.GetUnitType(model.UnitTypId) ?? new RmgProdDefUnitType();
                model.ToAudit(LoginInfo, model.Tc > 0);
                mapper.Map(model, unitType);
                unitTypeService.SaveUnitType(unitType);
                return Json(new { isSuccess = true, message = "Saved Successfully", data = unitType });
            }

            return Json(new { success = false, message = ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage });
        }

        [HttpPost]
        public ActionResult Grid(DataTablesOptions options)
        {
            var unitTypes = unitTypeService.GetPagedUnitTypes(options);
            return DataTablesResult(unitTypes);
        }


        [HttpPost]
        public ActionResult Delete(string id)
        {
            bool success = false;
            foreach (var item in id.Split(",", StringSplitOptions.RemoveEmptyEntries))
            {
                success = unitTypeService.DeleteUnitType(item);
            }

            return Json(new { success = success, message = "Deleted Successfully" });
        }


        [HttpPost]
        public JsonResult CheckAvailability(string name, string code)
        {
            if (unitTypeService.IsUnitTypeExist(name, code))
            {
                return Json(new { isSuccess = true, message = "Already Exists" });
            }

            return Json(new { isSuccess = false });
        }
    }
}
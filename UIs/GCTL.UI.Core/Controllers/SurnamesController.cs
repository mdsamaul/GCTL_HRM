using GCTL.Core.ViewModels.Surnames;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.Surnames;
using GCTL.UI.Core.ViewModels.Surnames;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using GCTL.Core.Helpers;

namespace GCTL.UI.Core.Controllers
{
    public class SurnamesController : BaseController
    {
        private readonly ISurnameService surnameService;
        private readonly ICommonService commonService;
        private readonly IMapper mapper;
        string strMaxNO = "";
        public SurnamesController(ISurnameService surnameService,
                                     ICommonService commonService,
                                     IMapper mapper)
        {
            this.surnameService = surnameService;
            this.commonService = commonService;
            this.mapper = mapper;
        }

        public IActionResult Index(bool child = false)
        {
            SurnamePageViewModel model = new SurnamePageViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };
            commonService.FindMaxNo(ref strMaxNO, "SurnameId", "HRM_Def_SurName", 2);
            model.Setup = new SurnameSetupViewModel
            {
                SurnameId = strMaxNO
            };

            if (child)
                return PartialView(model);

            return View(model);
        }


        public IActionResult Setup(string id)
        {
            SurnameSetupViewModel model = new SurnameSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "SurnameId", "HRM_Def_SurName", 2);
            var result = surnameService.GetSurname(id);
            if (result != null)
            {
                model = mapper.Map<SurnameSetupViewModel>(result);
                model.Code = id;
                model.AutoId = (int)result.AutoId;
            }
            else
            {
                model.SurnameId = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Setup(SurnameSetupViewModel model)
        {
            if (surnameService.IsSurnameExist(model.Surname, model.AutoId))
            {
                return Json(new { isSuccess = false, message = "Already Exists" });
            }

            if (ModelState.IsValid)
            {
                HrmDefSurName surname = surnameService.GetSurname(model.AutoId) ?? new HrmDefSurName();
                model.ToAudit(LoginInfo, model.AutoId > 0);
                mapper.Map(model, surname);
                commonService.FindMaxNo(ref strMaxNO, "SurnameId", "HRM_Def_SurName", 2);
                surname.SurnameId = strMaxNO;
                surnameService.SaveSurname(surname);
                return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = surname.SurnameId });
            }

            return Json(new { success = false, message = ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage });
        }

        public ActionResult Grid()
        {
            var result = surnameService.GetSurnames();
            return Json(new { data = result });
        }


        [HttpPost]
        public ActionResult Delete(string id)
        {
            bool success = false;
            foreach (var item in id.Split(",", StringSplitOptions.RemoveEmptyEntries))
            {
                success = surnameService.DeleteSurname(item);
            }

            return Json(new { success = success, message = "Deleted Successfully" });
        }


        [HttpPost]
        public JsonResult CheckAvailability(string name, string code)
        {
            if (surnameService.IsSurnameExist(name, code))
            {
                return Json(new { isSuccess = true, message = "Already Exists" });
            }

            return Json(new { isSuccess = false });
        }
    }
}
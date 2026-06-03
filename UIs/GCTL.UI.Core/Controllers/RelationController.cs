using GCTL.Core.ViewModels.Relation;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.Relation;
using GCTL.UI.Core.ViewModels.Relation;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using GCTL.Core.Helpers;

namespace GCTL.UI.Core.Controllers
{
    public class RelationController : BaseController
    {
        private readonly IRelationService relationService;
        private readonly ICommonService commonService;
        private readonly IMapper mapper;
        string strMaxNO = "";
        public RelationController(IRelationService relationService,
                                     ICommonService commonService,
                                     IMapper mapper)
        {
            this.relationService = relationService;
            this.commonService = commonService;
            this.mapper = mapper;
        }

        public IActionResult Index(bool child = false)
        {
            RelationPageViewModel model = new RelationPageViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };
            commonService.FindMaxNo(ref strMaxNO, "RelationCode", "HMS_Relation", 2);
            model.Setup = new RelationSetupViewModel
            {
                RelationCode = strMaxNO
            };

            if (child)
                return PartialView(model);

            return View(model);
        }


        public IActionResult Setup(string id)
        {
            RelationSetupViewModel model = new RelationSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "RelationCode", "HMS_Relation", 2);
            var result = relationService.GetRelation(id);
            if (result != null)
            {
                model = mapper.Map<RelationSetupViewModel>(result);
                model.Code = id;
                model.Id = (int)result.AutoId;
            }
            else
            {
                model.RelationCode = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Setup(RelationSetupViewModel model)
        {
            if (relationService.IsRelationExist(model.Relation, model.RelationCode))
            {
                return Json(new { isSuccess = false, message = "Already Exists" });
            }

            if (ModelState.IsValid)
            {
                HmsRelation relation = relationService.GetRelation(model.RelationCode) ?? new HmsRelation();
                model.ToAudit(LoginInfo, model.Id > 0);
                mapper.Map(model, relation);
                relationService.SaveRelation(relation);
                return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = relation.RelationCode });
            }

            return Json(new { success = false, message = ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage });
        }

        public ActionResult Grid()
        {
            var result = relationService.GetRelations();
            return Json(new { data = result });
        }


        [HttpPost]
        public ActionResult Delete(string id)
        {
            bool success = false;
            foreach (var item in id.Split(",", StringSplitOptions.RemoveEmptyEntries))
            {
                success = relationService.DeleteRelation(item);
            }

            return Json(new { success = success, message = "Deleted Successfully" });
        }


        [HttpPost]
        public JsonResult CheckAvailability(string name, string code)
        {
            if (relationService.IsRelationExist(name, code))
            {
                return Json(new { isSuccess = true, message = "Already Exists" });
            }

            return Json(new { isSuccess = false });
        }
    }
}
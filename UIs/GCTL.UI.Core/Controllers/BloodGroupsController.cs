using GCTL.Core.ViewModels.BloodGroups;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.BloodGroups;
using GCTL.UI.Core.ViewModels.BloodGroups;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using GCTL.Core.Helpers;

namespace GCTL.UI.Core.Controllers
{
    public class BloodGroupsController : BaseController
    {
        private readonly IBloodGroupService bloodGroupService;
        private readonly ICommonService commonService;
        private readonly IMapper mapper;
        string strMaxNO = "";
        public BloodGroupsController(IBloodGroupService bloodGroupService,
                                     ICommonService commonService,
                                     IMapper mapper)
        {
            this.bloodGroupService = bloodGroupService;
            this.commonService = commonService;
            this.mapper = mapper;
        }

        public IActionResult Index(bool child = false)
        {
            BloodGroupPageViewModel model = new BloodGroupPageViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };
            commonService.FindMaxNo(ref strMaxNO, "BloodGroupCode", "HRM_Def_BloodGroup", 2);
            model.Setup = new BloodGroupSetupViewModel
            {
                BloodGroupCode = strMaxNO
            };

            if (child)
                return PartialView(model);

            return View(model);
        }


        public IActionResult Setup(string id)
        {
            BloodGroupSetupViewModel model = new BloodGroupSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "BloodGroupCode", "HRM_Def_BloodGroup", 2);
            var result = bloodGroupService.GetBloodGroup(id);
            if (result != null)
            {
                model = mapper.Map<BloodGroupSetupViewModel>(result);
                model.Code = id;
                model.Tc = (int)result.AutoId;
            }
            else
            {
                model.BloodGroupCode = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Setup(BloodGroupSetupViewModel model)
        {
            if (bloodGroupService.IsBloodGroupExist(model.BloodGroup, model.Tc))
            {
                return Json(new { isSuccess = false, message = "Already Exists" });
            }

            if (ModelState.IsValid)
            {
                if (bloodGroupService.IsBloodGroupExistByCode(model.BloodGroupCode))
                {
                    var hasPermission = bloodGroupService.UpdatePermission(LoginInfo.AccessCode);
                    if (hasPermission)
                    {
                        HrmDefBloodGroup bloodGroup = bloodGroupService.GetBloodGroup(model.Tc) ?? new HrmDefBloodGroup();
                        model.ToAudit(LoginInfo, model.Tc > 0);
                        mapper.Map(model, bloodGroup);
                        commonService.FindMaxNo(ref strMaxNO, "BloodGroupCode", "HRM_Def_BloodGroup", 2);
                        bloodGroup.BloodGroupCode = strMaxNO;
                        bloodGroupService.SaveBloodGroup(bloodGroup);
                        return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = bloodGroup.BloodGroupCode });
                    }
                    else
                    {

                        return Json(new { isSuccess = false, message = "You have no access" });
                    }

                }
                else
                {
                    var hasPermission = bloodGroupService.SavePermission(LoginInfo.AccessCode);
                    if (hasPermission)
                    {
                        HrmDefBloodGroup bloodGroup = bloodGroupService.GetBloodGroup(model.Tc) ?? new HrmDefBloodGroup();
                        model.ToAudit(LoginInfo, model.Tc > 0);
                        mapper.Map(model, bloodGroup);
                        commonService.FindMaxNo(ref strMaxNO, "BloodGroupCode", "HRM_Def_BloodGroup", 2);
                        bloodGroup.BloodGroupCode = strMaxNO;
                        bloodGroupService.SaveBloodGroup(bloodGroup);
                        return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = bloodGroup.BloodGroupCode });
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
            var result = bloodGroupService.GetBloodGroups();
            return Json(new { data = result });
        }


        [HttpPost]
        public ActionResult Delete(string id)
        {
            var hasPermission = bloodGroupService.DeletePermission(LoginInfo.AccessCode);
            if (hasPermission)
            {
                bool success = false;
                foreach (var item in id.Split(",", StringSplitOptions.RemoveEmptyEntries))
                {
                    success = bloodGroupService.DeleteBloodGroup(item);
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
            if (bloodGroupService.IsBloodGroupExist(name, code))
            {
                return Json(new { isSuccess = true, message = "Already Exists" });
            }

            return Json(new { isSuccess = false });
        }
    }
}
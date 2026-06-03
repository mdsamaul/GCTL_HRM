using GCTL.Core.ViewModels.Qualifications;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.Qualifications;
using GCTL.UI.Core.ViewModels.Qualifications;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using GCTL.Core.Helpers;

namespace GCTL.UI.Core.Controllers
{
    public class QualificationsController : BaseController
    {
        private readonly IQualificationService qualificationService;
        private readonly ICommonService commonService;
        private readonly IMapper mapper;
        string strMaxNO = "";
        public QualificationsController(IQualificationService qualificationService,
                                        ICommonService commonService,
                                        IMapper mapper)
        {
            this.qualificationService = qualificationService;
            this.commonService = commonService;
            this.mapper = mapper;
        }

        public IActionResult Index()
        {
            QualificationPageViewModel model = new QualificationPageViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };
            commonService.FindMaxNo(ref strMaxNO, "QualificationCode", "HMS_Qualification", 3);
            model.Setup = new QualificationSetupViewModel
            {
                QualificationCode = strMaxNO
            };
            return View(model);
        }


        public IActionResult Setup(string id)
        {
            QualificationSetupViewModel model = new QualificationSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "QualificationCode", "HMS_Qualification", 3);
            var result = qualificationService.GetQualification(id);
            if (result != null)
            {
                model = mapper.Map<QualificationSetupViewModel>(result);
                model.Code = id;
                model.Id = (int)result.AutoId;
            }
            else
            {
                model.QualificationCode = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Setup(QualificationSetupViewModel model)
        {
            if (qualificationService.IsQualificationExist(model.QualificationName, model.QualificationCode))
            {
                return Json(new { isSuccess = false, message = "Already Exists" });
            }

            if (ModelState.IsValid)
            {
                if (qualificationService.IsQualificationExistByCode(model.QualificationCode))
                {
                    var hasPermission = qualificationService.UpdatePermission(LoginInfo.AccessCode);
                    if (hasPermission)
                    {
                        HmsQualification qualification = qualificationService.GetQualification(model.QualificationCode) ?? new HmsQualification();
                        model.ToAudit(LoginInfo, model.Id > 0);
                        mapper.Map(model, qualification);
                        qualification.CompanyCode = "";
                        qualification.EmployeeId = "";
                        qualificationService.SaveQualification(qualification);
                        return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = qualification.QualificationCode });
                    }
                    else
                    {

                        return Json(new { isSuccess = false, message = "You have no access" });
                    }

                }
                else
                {
                    var hasPermission = qualificationService.SavePermission(LoginInfo.AccessCode);
                    if (hasPermission)
                    {
                        HmsQualification qualification = qualificationService.GetQualification(model.QualificationCode) ?? new HmsQualification();
                        model.ToAudit(LoginInfo, model.Id > 0);
                        mapper.Map(model, qualification);
                        qualification.CompanyCode = "";
                        qualification.EmployeeId = "";
                        qualificationService.SaveQualification(qualification);
                        return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = qualification.QualificationCode });
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
            var resutl = qualificationService.GetQualifications();
            return Json(new { data = resutl });
        }


        [HttpPost]
        public ActionResult Delete(string id)
        {
            var hasPermission = qualificationService.DeletePermission(LoginInfo.AccessCode);
            if (hasPermission)
            {
                bool success = false;
                foreach (var item in id.Split(",", StringSplitOptions.RemoveEmptyEntries))
                {
                    success = qualificationService.DeleteQualification(item);
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
            if (qualificationService.IsQualificationExist(name, code))
            {
                return Json(new { isSuccess = true, message = "Already Exists" });
            }

            return Json(new { isSuccess = false });
        }
    }
}
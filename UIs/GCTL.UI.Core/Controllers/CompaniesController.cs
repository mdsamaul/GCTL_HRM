using GCTL.Core.ViewModels.Companies;
using GCTL.Data.Models;
using GCTL.Service.Common;
using GCTL.Service.Companies;
using GCTL.UI.Core.ViewModels.Companies;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using GCTL.Core.Helpers;

namespace GCTL.UI.Core.Controllers
{
    public class CompaniesController : BaseController
    {
        private readonly ICompanyService companyService;
        private readonly ICommonService commonService;
        private readonly IMapper mapper;
        string strMaxNO = "";
        public CompaniesController(ICompanyService companyService,
                                   ICommonService commonService,
                                   IMapper mapper)
        {
            this.companyService = companyService;
            this.commonService = commonService;
            this.mapper = mapper;
        }

        public IActionResult Index()
        {
            CompanyPageViewModel model = new CompanyPageViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };
            commonService.FindMaxNo(ref strMaxNO, "CompanyCode", "Core_Company", 3);
            model.Setup = new CompanySetupViewModel
            {
                CompanyCode = strMaxNO,
            };

            return View(model);
        }


        public IActionResult Setup(string id)
        {
            CompanySetupViewModel model = new CompanySetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "CompanyCode", "Core_Company", 3);
            var result = companyService.GetCompany(id);
            if (result != null)
            {
                model = mapper.Map<CompanySetupViewModel>(result);
                model.Code = id;
            }
            else
            {
                model.CompanyCode = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Setup(CompanySetupViewModel model)
        {
            if (companyService.IsCompanyExist(model.CompanyName, model.CompanyCode))
            {
                return Json(new { isSuccess = false, message = "Already Exists" });
            }

            if (ModelState.IsValid)
            {

                if (companyService.IsCompanyExistByCode(model.CompanyCode))
                {
                    var hasPermission = companyService.UpdatePermission(LoginInfo.AccessCode);
                    if (hasPermission)
                    {
                        CoreCompany company = companyService.GetCompany(model.CompanyCode) ?? new CoreCompany();
                        model.ToAudit(LoginInfo, model.Id > 0);
                        mapper.Map(model, company);
                        companyService.SaveCompany(company);
                        return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = model.CompanyCode });
                    }
                    else
                    {
                      
                       return Json(new { isSuccess = false, message = "You have no access" });
                    }
          
                }
                else
                {
                    var hasPermission = companyService.SavePermission(LoginInfo.AccessCode);
                    if (hasPermission)
                    {
                        CoreCompany company = companyService.GetCompany(model.CompanyCode) ?? new CoreCompany();
                        model.ToAudit(LoginInfo, model.Id > 0);
                        mapper.Map(model, company);
                        companyService.SaveCompany(company);
                        return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = model.CompanyCode });
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
            var resutl = companyService.GetCompanies();
            return Json(new { data = resutl });
        }


        [HttpPost]
        public ActionResult Delete(string id)
        {
            var hasPermission = companyService.DeletePermission(LoginInfo.AccessCode);
            if (hasPermission)
            {
                bool success = false;
                foreach (var item in id.Split(",", StringSplitOptions.RemoveEmptyEntries))
                {
                    success = companyService.DeleteCompany(item);
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
            if (companyService.IsCompanyExist(name, code))
            {
                return Json(new { isSuccess = true, message = "Already Exists" });
            }

            return Json(new { isSuccess = false });
        }
    }
}
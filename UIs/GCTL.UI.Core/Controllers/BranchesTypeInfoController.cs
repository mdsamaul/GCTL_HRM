using AutoMapper;
using GCTL.Core.Data;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.BranchesTypeInfo;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Data.Models;
using GCTL.Service.BranchesTypeInfo;
using GCTL.Service.Common;
using GCTL.UI.Core.ViewModels.BranchesTypeInfo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace GCTL.UI.Core.Controllers
{
    public class BranchesTypeInfoController : BaseController
    {
        private readonly IBranchTypeInfoService branchTypeInfoService;
        private readonly ICommonService commonService;
        private readonly IMapper mapper;
        private readonly IRepository<CoreCompany> _coreCompany;
        
        string strMaxNO = "";

        public BranchesTypeInfoController(
            IBranchTypeInfoService branchTypeInfoService,
            ICommonService commonService,
            IMapper mapper, IRepository<CoreCompany> coreCompany
        )
        {
            this.branchTypeInfoService = branchTypeInfoService;
            this.commonService = commonService;
            this.mapper = mapper;
            _coreCompany = coreCompany;
            
        }

        public IActionResult Index()
        {
            BranchTypePageViewModel model = new BranchTypePageViewModel()
            {
                PageUrl = Url.Action(nameof(Index)),
                //AddUrl = Url.Action(nameof(Setup))
            };
            commonService.FindMaxNo(ref strMaxNO, "BranchCode", "Core_Branch", 3);
            model.Setup = new BranchTypeSetupViewModel
            {
                BranchCode = strMaxNO,
            };
            var comapanies =  branchTypeInfoService.DropSelection();
            string selectedCompanyCode = comapanies.Count() == 1 ? comapanies.First().Code : null;
            ViewBag.CompanyCodeDD = new SelectList( branchTypeInfoService.DropSelection(), "Code", "Name", selectedCompanyCode);
           // ViewBag.CompanyCodeDD = new SelectList(branchTypeInfoService.DropSelection(), "Code", "Name");


            return View(model);
        }

        public ActionResult Details(string id)
        {
            BranchTypeSetupViewModel model = new BranchTypeSetupViewModel();
            var result = branchTypeInfoService.GetBranchTypeSetupView(id);
            if (result == null)
            {
                return NotFound();
            }

            model = mapper.Map<BranchTypeSetupViewModel>(result);
            model.Code = id;

            //  ViewBag.Company = new SelectList(_coreCompany.All(), "Code", "Name", model.CompanyCode);
            ViewBag.CompanyCodeDD = new SelectList(branchTypeInfoService.DropSelection(), "Code", "Name");

            return View(model);
        }


        public IActionResult Setup(string id)
        {
            BranchTypeSetupViewModel model = new BranchTypeSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "BranchCode", "Core_Branch", 3);
            var result = branchTypeInfoService.GetBranchTypeSetupView(id);
            if (result != null)
            {
                model = mapper.Map<BranchTypeSetupViewModel>(result);
                model.Code = id;
            }
            else
            {
                model.BranchCode = strMaxNO;
            }
            ViewBag.CompanyCodeDD = new SelectList(branchTypeInfoService.DropSelection(), "Code", "Name");
            return PartialView($"_{nameof(Setup)}", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Setup(BranchTypeSetupViewModel model)
        {
            if (branchTypeInfoService.IsBranchTypeInfoExist(model.BranchName, model.CompanyCode, model.BranchCode))
            {
                return Json(new { isSuccess = false, message = "Already Exists" });
            }

            if (ModelState.IsValid)
            {

                if (branchTypeInfoService.IsBranchTypeInfoExistByCode(model.BranchCode))
                {
                    var hasPermission = branchTypeInfoService.UpdatePermission(LoginInfo.AccessCode);
                    if (hasPermission)
                    {
                        CoreBranch branchTypeSetupViewModel = branchTypeInfoService.GetBranch(model.BranchCode) ?? new CoreBranch();
                        model.ToAudit(LoginInfo, model.Id > 0);
                        mapper.Map(model, branchTypeSetupViewModel);
                        branchTypeInfoService.SaveBranchTypeInfo(branchTypeSetupViewModel);
                        return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = model.BranchCode });
                    }
                    else
                    {

                        return Json(new { isSuccess = false, message = "You have no access" });
                    }

                }
                else
                {
                    var hasPermission = branchTypeInfoService.SavePermission(LoginInfo.AccessCode);
                    if (hasPermission)
                    {
                        CoreBranch branchTypeSetupViewModel = branchTypeInfoService.GetBranch(model.BranchCode) ?? new CoreBranch();
                        model.ToAudit(LoginInfo, model.Id > 0);
                        mapper.Map(model, branchTypeSetupViewModel);
                        branchTypeInfoService.SaveBranchTypeInfo( branchTypeSetupViewModel);
                        return Json(new { isSuccess = true, message = "Saved Successfully", lastCode = model.BranchCode });
                    }
                    else
                    {

                        return Json(new { isSuccess = false, message = "You have no access" });
                    }

                }

            }

            return Json(new { success = false, message = ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage });
        }

        public async Task<ActionResult> Grid(string CompanyCode)
        {
           
            var resutl = await branchTypeInfoService.GetCompaniess(CompanyCode);
            return Json(new { data = resutl });
        }

        [HttpPost]

        public async Task<ActionResult> Delete(string id)

        {

            var hasPermission = branchTypeInfoService.DeletePermission(LoginInfo.AccessCode);

            if (!hasPermission)

            {

                return Json(new { success = false, message = "You have no access" });

            }

            if (string.IsNullOrWhiteSpace(id))

            {

                return Json(new { success = false, message = "No IDs provided" });

            }

            var results = new List<(bool success, bool refSuccess, string message)>();

            var ids = id.Split(",", StringSplitOptions.RemoveEmptyEntries)

                 .Select(x => x.Trim())

                 .ToList();
            DeleteHistoryViewModel model = new DeleteHistoryViewModel();
            model.ToAudit(LoginInfo);
            model.CompanyCode = LoginInfo.CompanyCode;

            var result = await branchTypeInfoService.DeleteBranchTypeInfo(ids, model);

            results.Add(result);


            // Aggregate outcome

            bool overallSuccess = results.All(r => r.success);

            bool refSuccess = results.All(r => r.refSuccess);

            string combinedMessage = overallSuccess

                ? "Deleted Successfully"

                : string.Join("; ", results.Where(r => !r.success).Select(r => r.message));

            return Json(new { success = overallSuccess, message = combinedMessage, refSuccess = refSuccess });

        }



        //[HttpPost]
        //public ActionResult Delete(string id)
        //{
        //    var hasPermission = branchTypeInfoService.DeletePermission(LoginInfo.AccessCode);
        //    if (hasPermission)
        //    {
        //        bool success = false;
        //        foreach (var item in id.Split(",", StringSplitOptions.RemoveEmptyEntries))
        //        {
        //            success = branchTypeInfoService.DeleteBranchTypeInfo(item);
        //        }

        //        return Json(new { success = success, message = "Deleted Successfully" });
        //    }
        //    else
        //    {
        //        return Json(new { isSuccess = false, message = "You have no access" });
        //    }

        //}



        //[HttpPost]
        //public JsonResult CheckAvailability(string name, string code)
        //{
        //    if (branchTypeInfoService.IsBranchTypeInfoExist(name, code))
        //    {
        //        return Json(new { isSuccess = true, message = "Already Exists" });
        //    }

        //    return Json(new { isSuccess = false });
        //}
    }
}

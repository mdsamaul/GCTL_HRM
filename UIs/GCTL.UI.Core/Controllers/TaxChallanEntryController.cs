using GCTL.Core.Data;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.TaxChallanEntry;
using GCTL.Data.Models;
using GCTL.Service.TaxChallanEntryService;
using GCTL.UI.Core.ViewModels.TaxChallanEntry;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace GCTL.UI.Core.Controllers
{
    public class TaxChallanEntryController : BaseController
    {
        private readonly ITaxChallanEntryService taxChallanEntryService;
        private readonly IRepository<HrmPayMonth> monthRepo;
        private readonly IRepository<SalesDefBankInfo> bankRepo;

        public TaxChallanEntryController(
            ITaxChallanEntryService taxChallanEntryService,
            IRepository<HrmPayMonth> monthRepo,
            IRepository<SalesDefBankInfo> bankRepo
            )
        {
            this.taxChallanEntryService = taxChallanEntryService;
            this.monthRepo = monthRepo;
            this.bankRepo = bankRepo;
        }
        public async Task<IActionResult> Index()
        {
            var hasPermission = await taxChallanEntryService.PagePermissionAsync(LoginInfo.AccessCode);

            if (!hasPermission)

            {

                return RedirectToAction("Login", "Accounts");

            }

            ViewBag.MonthList = new SelectList(monthRepo.All().Select(x => new { x.MonthId, x.MonthName }), "MonthId", "MonthName", DateTime.Now.Month.ToString());
            ViewBag.BankList = new SelectList(bankRepo.All().Select(x => new { x.BankId, x.BankName }), "BankId", "BankName");
            EmployeeTaxChallanViewModel model = new EmployeeTaxChallanViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetFilterDropdownData([FromBody] EmployeeTaxChallanFilterViewModel filterData)
        {
            var result = await taxChallanEntryService.EmployeeTaxChallanDropdownList(filterData);
            return Json(result);
        }
        [HttpPost]
        public async Task<IActionResult> GetBankDetails([FromBody] string bankId)
        {
            var result = await taxChallanEntryService.GetBankDetails(bankId);
            return Json(result);
        }
        [HttpPost]
        public async Task<IActionResult> GetBankBranchAddress([FromBody] string branchId)
        {
            var result = await taxChallanEntryService.GetBankBranchAddressAsync(branchId);
            return Json(result);
        }
        [HttpGet]
        public async Task<IActionResult> TaxDipositAutoId()
        {
            var autoId = await taxChallanEntryService.TaxDipositAutoIdAsync();
            return Json(autoId);
        }

        public async Task<IActionResult> TaxChallanSaveEdit([FromBody] HrmPayMonthlyTaxDepositEntryDto fromData)
        {

            if (!ModelState.IsValid)
            {
                return Json(new { isSuccess = false });
            }
            try
            {
                fromData.ToAudit(LoginInfo);
                if (fromData.TaxDepositCode == 0)
                {
                    bool hasParmision = await taxChallanEntryService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasParmision)
                    {
                        var result = await taxChallanEntryService.TaxChallanSaveEditAsync(fromData, LoginInfo.CompanyCode);

                        return Json(new { isSuccess = result.isSuccess, message = result.message, data = result });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
                    }
                }
                else
                {
                    var hasUpdatePermission = await taxChallanEntryService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        var result = await taxChallanEntryService.TaxChallanSaveEditAsync(fromData, LoginInfo.CompanyCode);
                        return Json(new { isSuccess = result.isSuccess, message = result.message, data = result });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });
                    }
                }
            }
            catch (Exception)
            {

                return Json(new { isSuccess = false, message = "Faild" });
            }

        }

        [HttpPost]
        public async Task<IActionResult> GetchallanEntryGrid([FromBody] DataTableRequest request)
        {
            try
            {
                if (request == null)
                {
                    return BadRequest("Invalid request data");
                }

                var result = await taxChallanEntryService.GetchallanEntryGridServerSideAsync(request);

                return Json(new
                {
                    draw = request.Draw,
                    recordsTotal = result.TotalRecords,
                    recordsFiltered = result.FilteredRecords,
                    data = result.Data
                });
            }
            catch (Exception ex)
            {
                // Log the exception
                return Json(new
                {
                    draw = request?.Draw ?? 0,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<object>(),
                    error = "An error occurred while loading data"
                });
            }
        }




        [HttpPost]
        public async Task<IActionResult> DeleteTaxChallanEntryGrid([FromBody] List<string> selectedIds)
        {
            var result = await taxChallanEntryService.DeleteTaxChallanEntryGridAsync(selectedIds);
            return Json(new { isSuccess = result.isSuccess, message = result.message, data = result.data });
        }

        [HttpGet]
        public async Task<IActionResult> FinancialYearGet()
        {
            var result = await taxChallanEntryService.FinancialYearGetAsync();
            return Json(result);
        }
    }
}

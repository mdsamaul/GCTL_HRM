using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.AdvanceLoanAdjustment;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Service.AdvanceLoanAdjustment;
using GCTL.Service.Common;
using GCTL.Service.EmployeeOfficialInfoReport;
using GCTL.UI.Core.ViewModels.AdvanceLoanAdjustment;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GCTL.UI.Core.Controllers
{
    public class AdvanceLoanAdjustmentController : BaseController
    {
        private readonly IAdvanceLoanAdjustmentServices advanceLoanAdjustmentServices;

        public AdvanceLoanAdjustmentController(IAdvanceLoanAdjustmentServices advanceLoanAdjustmentServices)
        {
            this.advanceLoanAdjustmentServices = advanceLoanAdjustmentServices;
        }
        public async Task<IActionResult> IndexAsync()
        {
            var hasPermission = await advanceLoanAdjustmentServices.PagePermissionAsync(LoginInfo.AccessCode);

            if (!hasPermission)

            {

                return RedirectToAction("Login", "Accounts");

            }

            AdvanceLoanAdjustmentViewModel model = new AdvanceLoanAdjustmentViewModel()
            {
                PageUrl = Url.Action(nameof(IndexAsync))
            };

            return View(model);
        }

        //get all company
        [HttpGet]
        public async Task<IActionResult> GetAllAndFilterCompany(string companyName)
        {
            var companess = await advanceLoanAdjustmentServices.GetAllAndFilterCompanyAsync(companyName);
            return Json(companess);
        }

        //get employee by company
        [HttpGet]
        public async Task<IActionResult> GetEmployeesByFilter(string employeeStatusId, string companyCode, string EmployeeName, bool loanAdjustment) 
        {
            var employees = await advanceLoanAdjustmentServices.GetEmployeesByFilterAsync(employeeStatusId, companyCode, EmployeeName, loanAdjustment);
            return Json(employees);
        }
        [HttpPost]
        public async Task<IActionResult> GetLoadEmployeeById(string employeeId)
        {
            var employee = await advanceLoanAdjustmentServices.GetLoadEmployeeByIdAsync(employeeId);
            return Json(employee);
        }
        [HttpGet]
        public async Task<IActionResult> GetLoanByEmployeeId(string employeeId)
        {
            var Loans = await advanceLoanAdjustmentServices.GetLoanByEmployeeIdAsync(employeeId);
            return Json(Loans);
        }

        [HttpGet]
        public async Task<IActionResult> GetLoanById(string loanId)
        {
            var loan = await advanceLoanAdjustmentServices.GetLoanByIdAsync(loanId);
            return Json(loan);
        }

        [HttpPost]
        public async Task<IActionResult> SaveUpdateLoanAdjustment([FromBody] AdvanceLoanAdjustmentSetupViewModel modelData)
        {
            if (ModelState.IsValid)
            {

                if (modelData.AdvancePayCode == 0)

                {

                    var hasSavePermission = await advanceLoanAdjustmentServices.SavePermissionAsync(LoginInfo.AccessCode);

                    if (hasSavePermission)

                    {

                        modelData.ToAudit(LoginInfo);
                        var AdjustmentLoan = await advanceLoanAdjustmentServices.SaveUpdateLoanAdjustmentAsync(modelData);
                        return Json(new { success = AdjustmentLoan.isSuccess, message = AdjustmentLoan.message, data = AdjustmentLoan });

                    }

                    else

                    {

                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });

                    }

                }

                else

                {

                    var hasUpdatePermission = await advanceLoanAdjustmentServices.UpdatePermissionAsync(LoginInfo.AccessCode);

                    if (hasUpdatePermission)

                    {

                        modelData.ToAudit(LoginInfo);
                        var AdjustmentLoan = await advanceLoanAdjustmentServices.SaveUpdateLoanAdjustmentAsync(modelData);
                        return Json(new { success = AdjustmentLoan.isSuccess, message = AdjustmentLoan.message, data = AdjustmentLoan });

                    }

                    else

                    {

                        return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });

                    }

                }


            }

            return Json(new { success = false, message = "Invalid data." });
        }

        //auto ganerate id
        public async Task<IActionResult> AdjustmentAutoGanarateId()
        {
            var autoId = await advanceLoanAdjustmentServices.AdjustmentAutoGanarateIdAsync();
            return Json(autoId);
        }
        [HttpGet]
        public async Task<IActionResult> GetMonth()
        {
            var months = await advanceLoanAdjustmentServices.GetMonthAsync();
            return Json(months);
        }
        [HttpGet]
        public async Task<IActionResult> GetHeadDeduction()
        {
            var DeductionHeads = await advanceLoanAdjustmentServices.GetHeadDeductionAsync();
            return Json(DeductionHeads);
        }


        // Updated Controller Method
        [HttpPost]
        public async Task<JsonResult> GetAdvancePayData(DataTableRequest request)
        {
            try
            {
                //request.Draw = Request.Form["draw"].FirstOrDefault();

                //var start = Request.Form["start"].FirstOrDefault();

                //var length = Request.Form["length"].FirstOrDefault();

                //var searchValue = Request.Form["search[value]"].FirstOrDefault();

                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();

                 request.sortColumn = Request.Form[$"columns[{sortColumnIndex}][data]"].FirstOrDefault();

                request.sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();


                // Validate request parameters
                if (request.Page <= 0) request.Page = 1;
                if (request.PageSize <= 0) request.PageSize = 10;
                if (request.PageSize > 100) request.PageSize = 100; 

                var result = await advanceLoanAdjustmentServices.GetAdvancePayPaged(request);

                return Json(new
                {
                    draw = request.Draw,
                    recordsTotal = result.TotalRecords,
                    recordsFiltered = result.FilteredRecords,
                    data = result.Data ?? new List<AdvancePayViewModel>()
                });
            }
            catch (Exception ex)
            {
                // Log the full exception

                return Json(new
                {
                    draw = request.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<AdvancePayViewModel>(),
                    error = $"An error occurred: {ex.Message}"
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAdvancePay([FromBody] List<decimal> selectedIds)
        {

            var hasPermission = await advanceLoanAdjustmentServices.DeletePermissionAsync(LoginInfo.AccessCode);

            if (!hasPermission)

            {

                return Json(new { success = false, message = "You have no access." });

            }

            DeleteHistoryViewModel DModel = new DeleteHistoryViewModel();
            DModel.ToAudit(LoginInfo);
            DModel.CompanyCode = LoginInfo.CompanyCode;
            var result = await advanceLoanAdjustmentServices.DeleteAdvancePayAsync(selectedIds, DModel);
            return Json(new {isSuccess=result.isSuccess, message = result.message});
        }

    }
}

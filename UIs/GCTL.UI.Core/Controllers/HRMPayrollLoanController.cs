using DocumentFormat.OpenXml.Office2016.Drawing.Command;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HRMPayrollLoan;
using GCTL.Service.Common;
using GCTL.Service.HRMPayrollLoan;
using GCTL.UI.Core.ViewModels.HRMPayrollLoan;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GCTL.UI.Core.Controllers
{
    public class HRMPayrollLoanController : BaseController
    {
        private readonly IHRMPayrollLoanService hRMPayrollLoanService;

        public HRMPayrollLoanController(IHRMPayrollLoanService hRMPayrollLoanService)
        {
            this.hRMPayrollLoanService = hRMPayrollLoanService;
        }
        public async Task<IActionResult> Index()
        {
            var hasPermission = await hRMPayrollLoanService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }
            HRMPayrollLoanViewModel model = new HRMPayrollLoanViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };
            return View(model);
        }
        //get company
        [HttpPost]
        public async Task<IActionResult> GetFilterData([FromBody] PayrollLoanFilterEntryDto entryDto)
        {
            var result = await hRMPayrollLoanService.GetFilaterDataAsync(entryDto);
            return Json(new {isSuccess =true, messate="Successed data load", data=result});
        }
        //get payment receive 
        [HttpPost]
        public async Task<IActionResult> GetFilterPaymentReceive([FromBody] PayrollLoanFilterEntryDto entryDto)
        {
            var result = await hRMPayrollLoanService.GetFilterPaymentReceiveAsync(entryDto);
            return Json(new { isSuccess = true, messate = "Successed data load", data = result });
        }

        //get payment receive data all
        [HttpGet]       
        public async Task<IActionResult> GetPaymentReceive()
        {
            var result = await hRMPayrollLoanService.GetPaymentReceiveAsync();
            return Json(new { isSuccess = true, messate = "Successed data load", data = result });
        }
        //get employee id 
        [HttpGet]
        public async Task<IActionResult> GetEmpById(string empId)
        {
            if (string.IsNullOrWhiteSpace(empId))
            {
                return Json(new { isSuccess = false, message = "Employee not found" });
            }

            var result = await hRMPayrollLoanService.EmployeeGetById(empId);

            if (result.isSuccess)
            {
                return Json(new { result.isSuccess, result.message, data = result.Item3 });
            }
            else
            {
                return Json(new { result.isSuccess, result.message });
            }
        }
        //get payment receive employee id 
        [HttpGet]
        public async Task<IActionResult> PaymentReceiveGetEmpById(string empId)
        {
            if (string.IsNullOrWhiteSpace(empId))
            {
                return Json(new { isSuccess = false, message = "Employee not found" });
            }

            var result = await hRMPayrollLoanService.PaymentReciveEmployeeGetById(empId);

            if (result.isSuccess)
            {
                return Json(new { result.isSuccess, result.message, data = result.Item3 });
            }
            else
            {
                return Json(new { result.isSuccess, result.message });
            }
        }

        //get loan type 

        public async Task<IActionResult> getLoanType()
        {
            var result = await hRMPayrollLoanService.GetLoanTypeAsync();
            if(result.isSuccess)
            {
                return Json(new {isSuccess= result.isSuccess, message=result.message, data = result.data});
            }
            return Json(new { isSuccess = true, message = "Loan Type loaded" });
        }

        //get payment mode
        [HttpGet]
        public async Task<IActionResult> GetPaymentMode()
        {
            var result = await hRMPayrollLoanService.getPaymentModeAsync();
            return Json(new { data= result});
        }

        //get pay head
        [HttpGet]
        public async Task<IActionResult> getPayHeadDeduction()
        {
            var result = await hRMPayrollLoanService.GetPayHeadDeductionAsync();
            return Json(new { data = result });
        }

        //loan id
        [HttpGet]
        public async Task<IActionResult> CreateLoanId()
        {
            var LoanId = await hRMPayrollLoanService.createLoanIdAsync();
            return Json(new {LoanId});
        }
        //payment receive id
        [HttpGet]
        public async Task<IActionResult> GetPaymentAutoId()
        {
            var LoanId = await hRMPayrollLoanService.createPaymentReceiveIdAsync();
            return Json(new {LoanId});
        }
        //get bank
        [HttpGet]
        public async Task<IActionResult> GetBank()
        {
            var banks = await hRMPayrollLoanService.GetBankAsync();
            return Json(new { data=banks});
        }

        //create and edit loan 
        [HttpPost]
        public async Task<IActionResult> CreateEditLoan([FromBody] HRMPayrollLoanSetupViewModel modelData)
        {
            if (modelData.AutoId == 0)
            {
                var hasSavePermission = await hRMPayrollLoanService.SavePermissionAsync(LoginInfo.AccessCode);
                if (hasSavePermission)
                {
                    modelData.ToAudit(LoginInfo);
                    var result = await hRMPayrollLoanService.CreateEditLoanAsycn(modelData);
                    return Ok(new { isSuccess = result.isSuccess, message = result.message, data = result });
                }
                else
                {
                    return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
                }
            }
            else
            {
                var hasUpdatePermission = await hRMPayrollLoanService.UpdatePermissionAsync(LoginInfo.AccessCode);
                if (hasUpdatePermission)
                {
                    modelData.ToAudit(LoginInfo);
                    var result = await hRMPayrollLoanService.CreateEditLoanAsycn(modelData);
                    return Ok(new { isSuccess = result.isSuccess, message = result.message, data = result });
                }
                else
                {
                    return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });
                }
            }







            //if (!ModelState.IsValid)
            //{
            //    return BadRequest("Invalid data.");
            //}
            //modelData.ToAudit(LoginInfo);            
            //var result = await hRMPayrollLoanService.CreateEditLoanAsycn(modelData);          

            //return Ok(new {isSuccess = result.isSuccess, message = result.message, data= result});
        }

        //create and edit payment receive
        [HttpPost]
        public async Task<IActionResult> CreateEditPaymentReceive([FromBody] PaymentReceiveSetupViewModel modelData)
        {
            if (modelData.AutoId == 0)
            {
                var hasSavePermission = await hRMPayrollLoanService.SavePermissionAsync(LoginInfo.AccessCode);
                if (hasSavePermission)
                {
                    modelData.ToAudit(LoginInfo);
                    var result = await hRMPayrollLoanService.CreateEditPaymentReceiveAsync(modelData);
                    return Json(new { result.isSuccess, result.message, result.data });
                }
                else
                {
                    return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
                }
            }
            else
            {
                modelData.ToAudit(LoginInfo);
                var hasUpdatePermission = await hRMPayrollLoanService.UpdatePermissionAsync(LoginInfo.AccessCode);
                if (hasUpdatePermission)
                {
                    var result = await hRMPayrollLoanService.CreateEditPaymentReceiveAsync(modelData);
                    return Json(new { result.isSuccess, result.message, result.data });
                }
                else
                {
                    return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });
                }
            }


            //modelData.ToAudit(LoginInfo);
            //var result = await hRMPayrollLoanService.CreateEditPaymentReceiveAsync(modelData);
            //return Json(new { result.isSuccess, result.message, result.data});
        }
        //get payment receive by id
        [HttpGet]
        public async Task<IActionResult> GetPaymentReceiveById(string paymentId)
        {
            var result = await hRMPayrollLoanService.getPaymentReceiveByIdAsync(paymentId);
            return Json(new { data=result});
        }
        //loan get by id
        [HttpGet]
        public async Task<IActionResult> GetLoanId(string loanId)
        {
            var result = await hRMPayrollLoanService.getLoanIdAsync(loanId);
            return Json(new {data=result});
        }
        //get loan data
        [HttpGet]
        public async Task<IActionResult> GetLoanData()
        {
            var result =await hRMPayrollLoanService.GetLoanDataAsync();
            return Json(new { data= result});
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLoans([FromBody] List<decimal> autoIds)
        {
            var hasPermission = await hRMPayrollLoanService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            DeleteHistoryViewModel dModel = new DeleteHistoryViewModel();
            dModel.ToAudit(LoginInfo);
            dModel.CompanyCode = LoginInfo.CompanyCode;
            var result =await hRMPayrollLoanService.deleteLoanAsync(autoIds,dModel); 
            return Json(new { isSuccess= result.isSuccess, message= result.message});
        }

        [HttpPost]
        public async Task<IActionResult> DeletePaymentReceive([FromBody] List<decimal> autoIds)
        {
            var hasPermission = await hRMPayrollLoanService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }
            DeleteHistoryViewModel dModel = new DeleteHistoryViewModel();
            dModel.ToAudit(LoginInfo);
            dModel.CompanyCode = LoginInfo.CompanyCode;
            var result = await hRMPayrollLoanService.deletePaymentReceiveAsync(autoIds, dModel);
            return Json(new {result.isSuccess, result.message });
        }

       
    }
}

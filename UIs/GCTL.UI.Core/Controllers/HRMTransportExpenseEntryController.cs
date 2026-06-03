using GCTL.Core.Data;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.HRMTransportAssignEntry;
using GCTL.Core.ViewModels.HRMTransportExpenseEntry;
using GCTL.Data.Models;
using GCTL.Service.HRMTransportExpenseEntryService;
using GCTL.UI.Core.ViewModels.HRMTransportExpenseEntry;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace GCTL.UI.Core.Controllers
{
    public class HRMTransportExpenseEntryController : BaseController
    {
        private readonly IHRMTransportExpenseEntryServicec transportExpenseRepo;
        private readonly IRepository<SalesDefTransportExpenseHead> expenseHeadRepo;

        public HRMTransportExpenseEntryController(
            IHRMTransportExpenseEntryServicec transportExpenseRepo,
            IRepository<SalesDefTransportExpenseHead> expenseHeadRepo
            )
        {
            this.transportExpenseRepo = transportExpenseRepo;
            this.expenseHeadRepo = expenseHeadRepo;
        }
        public IActionResult Index()
        {
            ViewBag.ExpenseHeadList = new SelectList(expenseHeadRepo.All().Select(x=> new {x.ExpenseHeadId, x.ExpenseHead}), "ExpenseHeadId", "ExpenseHead").ToList();


            HRMTransportExpenseEntryViewModel model = new HRMTransportExpenseEntryViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> TransportExpenseDetails([FromBody] TransportExpenseDetailsTempDto vmData)
        {
            var result = await transportExpenseRepo.TransportExpenseDetailsAsync(vmData);
            return Json(new {isSuccess = result.isSuccess, message = result.message, data = result.data });
        }

        [HttpGet]
        public async Task<IActionResult> TransportExpenseTempDetailsList()
        {
            var result = await transportExpenseRepo.TransportExpenseTempDetailsListAsny();
            return Json(new {data = result});
        }

        public async Task<IActionResult> TransportExpenseTempDetailsEdit(decimal id)
        {
            var result = await transportExpenseRepo.TransportExpenseTempDetailsByIdAsny(id);
            return Json(new { data = result });
        }
        public async Task<IActionResult> TransportExpenseMasterDetailsEdit(decimal id)
        {
            var result = await transportExpenseRepo.TransportExpenseMasterDetailsByIdAsny(id);
            return Json(new { data = result });
        }
        public async Task<IActionResult> DeleteTransportExpense(decimal id)
        {
            var result = await transportExpenseRepo.DeleteTransportExpenseAsync(id);
            return Json(new { isSuccess = result.isSuccess, message = result.message, data = result.data });
        }

        [HttpPost]
        public async Task<IActionResult> GetAllTransportDetails(string trnsId)
        {
            var result = await transportExpenseRepo.GetAllTransportDetailsAsync(trnsId);
            return Json(new { Data = result});
        }


        [HttpPost]
        public async Task<IActionResult> GetEmpDetailsId([FromBody] string Emp)
        {
            var result = await transportExpenseRepo.GetEmpDetailsIdAsync(Emp);
            return Json(new { data = result });
        }

        [HttpGet]
        public async Task<IActionResult> LoadData()
        {
            try
            {
                var data = await transportExpenseRepo.GetAllAsync();
                return Json(new { data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> AutoId()
        {
            try
            {
                var newCategoryId = await transportExpenseRepo.AutoIdAsync();
                return Json(new { data = newCategoryId });
            }
            catch (Exception)
            {
                throw;
            }

        }


        [HttpPost]
        public async Task<IActionResult> CreateUpdate([FromBody] HRMTransportExpenseEntrySetupViewModel model)
          {
            if (!ModelState.IsValid)
            {
                return Json(new { isSuccess = false });
            }
            try
            {
                model.ToAudit(LoginInfo);
                if (model.AutoId == 0)
                {
                    bool hasParmision = await transportExpenseRepo.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasParmision)
                    {
                        var result = await transportExpenseRepo.CreateUpdateAsync(model, LoginInfo.CompanyCode, LoginInfo.EmployeeId);
                        return Json(new { isSuccess = result.isSuccess, message = result.message, data = result.data });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
                    }
                }
                else
                {
                    var hasUpdatePermission = await transportExpenseRepo.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        var result = await transportExpenseRepo.CreateUpdateAsync(model, LoginInfo.CompanyCode, LoginInfo.EmployeeId);
                        return Json(new { isSuccess = result.isSuccess, message = result.message, data = result.data });
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

        [HttpGet]
        public async Task<IActionResult> PopulatedDataForUpdate(string id)
        {
            var result = await transportExpenseRepo.GetByIdAsync(id);
            return Json(new { result });
        }

        [HttpPost]
        public async Task<IActionResult> deleteTransport([FromBody] List<decimal> selectedIds)
        {
            var hasUpdatePermission = await transportExpenseRepo.DeletePermissionAsync(LoginInfo.AccessCode);
            if (hasUpdatePermission)
            {
                DeleteHistoryViewModel dmodel = new DeleteHistoryViewModel();
                dmodel.ToAudit(LoginInfo);
                dmodel.CompanyCode = LoginInfo.CompanyCode;
                var result = await transportExpenseRepo.DeleteAsync(selectedIds, dmodel);
                return Json(new { isSuccess = result.isSuccess, message = result.message, data = result });
            }
            else
            {
                return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });
            }

        }
        [HttpPost]
        public async Task<IActionResult> alreadyExist([FromBody] string TransportValue)
        {
            var result = await transportExpenseRepo.AlreadyExistAsync(TransportValue);
            return Json(new { isSuccess = result.isSuccess, message = result.message, data = result });
        }
        //[HttpPost]
        //public async Task<IActionResult> transportTypeGetByTransportNo([FromBody] string transportNoId)
        //{
        //    var result = from tr in transportRepo.All()
        //                 join tt in transportTypeRepo.All() on tr.VehicleTypeId equals tt.VehicleTypeId
        //                 where tr.VehicleId == transportNoId
        //                 select new
        //                 {
        //                     tt.VehicleTypeId,
        //                     tt.VehicleType
        //                 };
        //    return Json(new { data = result });


        //}
        public async Task<IActionResult> ReloadDataBackTempToDetails()
        {
            var result = await transportExpenseRepo.ReloadDataBackTempToDetailsAsync();
            return Json(new { isSuccess = result.isSuccess, data = result });
        }
    }
}

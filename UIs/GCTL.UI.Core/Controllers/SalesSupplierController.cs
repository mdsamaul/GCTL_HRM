
using GCTL.Core.Data;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.SalesSupplier;
using GCTL.Data.Models;
using GCTL.Service.SalesSupplierService;
using GCTL.UI.Core.ViewModels.SalesSupplier;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GCTL.UI.Core.Controllers
{
    public class SalesSupplierController : BaseController
    {
        private readonly IRepository<InvDefSupplierType> supplierTypeRepo;
        private readonly IRepository<CoreCountry> countryRepo;
        private readonly ISalesSupplierService salesSupplierRepo;

        public SalesSupplierController(
            IRepository<InvDefSupplierType> supplierTypeRepo,
            IRepository<CoreCountry> countryRepo,
            ISalesSupplierService salesSupplierRepo
            )
        {
            this.supplierTypeRepo = supplierTypeRepo;
            this.countryRepo = countryRepo;
            this.salesSupplierRepo = salesSupplierRepo;
        }
        public IActionResult Index(bool isPartial)
        {
            try
            {
                //ViewBag.SupplirTypeList = new SelectList(supplierTypeRepo.All().Select(x => new { x.SupplierTypeId, x.SupplierTypeName }), "SupplierTypeId", "SupplierTypeName");
                ViewBag.SupplirTypeList = new SelectList(supplierTypeRepo.All().Select(x => new { x.SupplierTypeId, x.SupplierType }), "SupplierTypeId", "SupplierType");
                ViewBag.CountryAll = new SelectList(countryRepo.All().Select(d => new { d.CountryId, d.CountryName }), "CountryId", "CountryName");
                SalesSupplierViewModel model = new SalesSupplierViewModel()
                {
                    PageUrl = Url.Action(nameof(Index))
                };
                if (isPartial) return PartialView(model);
                return View(model);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> LoadData()
        {
            try
            {
                var data = await salesSupplierRepo.GetAllAsync();
                return Json(new { data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> AutoSupplierTypeId()
        {
            try
            {
                var newCategoryId = await salesSupplierRepo.AutoSalesSupplierIdAsync();
                return Json(new { data = newCategoryId });
            }
            catch (Exception)
            {
                throw;
            }

        }


        [HttpPost]
        public async Task<IActionResult> CreateUpdate([FromBody] SalesSupplierSetupViewModel model)
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
                    bool hasParmision = await salesSupplierRepo.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasParmision)
                    {
                        var result = await salesSupplierRepo.CreateUpdateAsync(model);
                        return Json(new { isSuccess = result.isSuccess, message = result.message, data = result.data });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
                    }
                }
                else
                {
                    var hasUpdatePermission = await salesSupplierRepo.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        var result = await salesSupplierRepo.CreateUpdateAsync(model);
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
            var result = await salesSupplierRepo.GetByIdAsync(id);
            return Json(new { result });
        }

        [HttpPost]
        public async Task<IActionResult> deleteSupplierType([FromBody] List<string> selectedIds)
        {
            var hasUpdatePermission = await salesSupplierRepo.DeletePermissionAsync(LoginInfo.AccessCode);
            if (hasUpdatePermission)
            {
                DeleteHistoryViewModel model = new DeleteHistoryViewModel();
                model.ToAudit(LoginInfo);
                model.CompanyCode = LoginInfo.CompanyCode;
                var result = await salesSupplierRepo.DeleteAsync(selectedIds, model);
                return Json(new { isSuccess = result.success, message = result.message, data = result });
            }
            else
            {
                return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });
            }

        }
        [HttpPost]
        public async Task<IActionResult> alreadyExist([FromBody] string CatagoryValue)
        {
            var result = await salesSupplierRepo.AlreadyExistAsync(CatagoryValue);
            return Json(new { isSuccess = result.isSuccess, message = result.message, data = result });
        }
        [HttpGet]
        public async Task<IActionResult> CloseSupplierType()
        {
            var supplierTypeList = supplierTypeRepo.All().Where(x => x.SupplierTypeId != null).OrderByDescending(x => x.SupplierTypeId).ToList();
            return Json(supplierTypeList);
        }
        [HttpGet]
        public async Task<IActionResult> CloseCountryModel()
        {
            var result = countryRepo.All().ToList();
            return Json(new { result });
        }
    }
}

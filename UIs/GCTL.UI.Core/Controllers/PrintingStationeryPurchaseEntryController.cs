using GCTL.Core.Data;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.DeleteHistories;
using GCTL.Core.ViewModels.PrintingStationeryPurchaseEntry;
using GCTL.Data.Models;
using GCTL.Service.EmployeeOfficialInfoReport;
using GCTL.Service.PrintingStationeryPurchaseEntry;
using GCTL.UI.Core.ViewModels.PrintingStationeryPurchaseEntry;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace GCTL.UI.Core.Controllers
{
    public class PrintingStationeryPurchaseEntryController : BaseController
    {
        private readonly IPrintingStationeryPurchaseEntryService printingStationeryPurchaseEntryService;
        private readonly IRepository<HrmItemMasterInformation> productRepo;
        private readonly IRepository<HrmBrand> brandRepo;
        private readonly IRepository<HrmSize> sizeRepo;
        private readonly IRepository<HmsLtrvPeriod> periodRepo;
        private readonly IRepository<RmgProdDefUnitType> unitRepo;
        private readonly IRepository<HrmDefDepartment> depRepo;
        private readonly IRepository<HrmModel> modelRepo;
        private readonly IRepository<SalesSupplier> supplier;
        private readonly IRepository<HrmEmployee> empRepo;
        private readonly IRepository<HrmEmployeeOfficialInfo> officialInfo;

        public PrintingStationeryPurchaseEntryController(
            IPrintingStationeryPurchaseEntryService printingStationeryPurchaseEntryService,
            IRepository<HrmItemMasterInformation> productRepo,
            IRepository<HrmBrand> brandRepo,
            IRepository<HrmSize> sizeRepo,
            IRepository<HmsLtrvPeriod> periodRepo,
            IRepository<RmgProdDefUnitType> unitRepo,
            IRepository<HrmDefDepartment> depRepo,
            IRepository<HrmModel> modelRepo,
            IRepository<SalesSupplier> supplier,
            IRepository<HrmEmployee> empRepo,
            IRepository<HrmEmployeeOfficialInfo> OfficialInfo
            )
        {
            this.printingStationeryPurchaseEntryService = printingStationeryPurchaseEntryService;
            this.productRepo = productRepo;
            this.brandRepo = brandRepo;
            this.sizeRepo = sizeRepo;
            this.periodRepo = periodRepo;
            this.unitRepo = unitRepo;
            this.depRepo = depRepo;
            this.modelRepo = modelRepo;
            this.supplier = supplier;
            this.empRepo = empRepo;
            this.officialInfo = OfficialInfo;
        }
        public async Task<IActionResult> Index()
        {
            var hasPermission = await printingStationeryPurchaseEntryService.PagePermissionAsync(LoginInfo.AccessCode);

            if (!hasPermission)

            {

                return RedirectToAction("Login", "Accounts");

            }

            ViewBag.ProductList = new SelectList(productRepo.All().Select(x=> new {x.ProductCode, x.ProductName}), "ProductCode", "ProductName");
           ViewBag.BrandList = new SelectList(brandRepo.All().Select(x=> new {x.BrandId, x.BrandName}), "BrandId", "BrandName");
            ViewBag.SizeList = new SelectList(sizeRepo.All().Select(x => new { x.SizeId, x.SizeName }), "SizeId", "SizeName");
            ViewBag.periodList = new SelectList(periodRepo.All().Select(x=> new {x.PeriodId, x.PeriodName}), "PeriodId", "PeriodName");
            ViewBag.unitList = new SelectList(unitRepo.All().Select(x=> new {x.UnitTypId, x.UnitTypeName}), "UnitTypId", "UnitTypeName");
            ViewBag.departmentList = new SelectList(depRepo.All().Select(x => new{ x.DepartmentCode, x.DepartmentName}), "DepartmentCode", "DepartmentName");
            ViewBag.modelList = new SelectList(modelRepo.All().Select(x => new { x.ModelId, x.ModelName }), "ModelId", "ModelName");
            ViewBag.SuppleirList = new SelectList(supplier.All().Select(x => new { x.SupplierId, x.SupplierName }), "SupplierId", "SupplierName");
            var employeeList = (from emp in empRepo.All()
                                join offi in officialInfo.All()
                                on emp.EmployeeId equals offi.EmployeeId
                                where offi.EmployeeStatus == "01"
                                select new
                                {
                                    emp.EmployeeId,
                                    FullName = emp.FirstName + " " + emp.LastName+"("+emp.EmployeeId+")"
                                }).ToList();

            ViewBag.EmployeeList = new SelectList(employeeList, "EmployeeId", "FullName");
            PrintingStationeryPurchaseEntryViewModel model = new PrintingStationeryPurchaseEntryViewModel() { 
                PageUrl = Url.Action(nameof(Index))
            };
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> SupplierCloseList()
        {
            var result = supplier.All().Where(x => x.SupplierId != null).OrderByDescending(x => x.SupplierId).ToList();
            return Json(new { data = result });
        }


        [HttpGet]
        public async Task<IActionResult> LoadData()
        {
            try
            {
                var data = await printingStationeryPurchaseEntryService.GetAllAsync();
                return Json(new { data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> AutoPrintingStationeryPurchaseId()
        {
            try
            {
                var newStationaryId = await printingStationeryPurchaseEntryService.AutoPrintingStationeryPurchaseIdAsync();
                return Json(new { data = newStationaryId });
            }
            catch (Exception)
            {
                throw;
            }

        }


        [HttpPost]
        public async Task<IActionResult> CreateUpdate([FromBody] PrintingStationeryPurchaseEntrySetupViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { isSuccess = false });
            }
            try
            {
                model.ToAudit(LoginInfo);
                if (model.TC == 0)
                {
                    bool hasParmision = await printingStationeryPurchaseEntryService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasParmision)
                    {
                        var result = await printingStationeryPurchaseEntryService.CreateUpdateAsync(model, LoginInfo.CompanyCode);
                        return Json(new { isSuccess = result.isSuccess, message = result.message, data = result.data });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
                    }
                }
                else
                {
                    var hasUpdatePermission = await printingStationeryPurchaseEntryService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        var result = await printingStationeryPurchaseEntryService.CreateUpdateAsync(model, LoginInfo.CompanyCode);
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
            ViewBag.ProductList = new SelectList(productRepo.All().Select(x => new { x.ProductCode, x.ProductName }), "ProductCode", "ProductName");
            ViewBag.BrandList = new SelectList(brandRepo.All().Select(x => new { x.BrandId, x.BrandName }), "BrandId", "BrandName");
            ViewBag.SizeList = new SelectList(sizeRepo.All().Select(x => new { x.SizeId, x.SizeName }), "SizeId", "SizeName");
            ViewBag.periodList = new SelectList(periodRepo.All().Select(x => new { x.PeriodId, x.PeriodName }), "PeriodId", "PeriodName");
            ViewBag.unitList = new SelectList(unitRepo.All().Select(x => new { x.UnitTypId, x.UnitTypeName }), "UnitTypId", "UnitTypeName");
            ViewBag.departmentList = new SelectList(depRepo.All().Select(x => new { x.DepartmentCode, x.DepartmentName }), "DepartmentCode", "DepartmentName");
            ViewBag.modelList = new SelectList(modelRepo.All().Select(x => new { x.ModelId, x.ModelName }), "ModelId", "ModelName");
            ViewBag.SuppleirList = new SelectList(supplier.All().Select(x => new { x.SupplierId, x.SupplierName }), "SupplierId", "SupplierName");
            var result = await printingStationeryPurchaseEntryService.GetByIdAsync(id);
            return Json(new { result });
        }

        [HttpPost]
        public async Task<IActionResult> deletePrintingStationeryPurchase([FromBody] List<string> selectedIds)
        {
            var hasUpdatePermission = await printingStationeryPurchaseEntryService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (hasUpdatePermission)
            {
                DeleteHistoryViewModel model = new DeleteHistoryViewModel();
                model.ToAudit(LoginInfo);
                model.CompanyCode = LoginInfo.CompanyCode;
                var result = await printingStationeryPurchaseEntryService.DeleteAsync(selectedIds, model);
                return Json(new { isSuccess = result.success, message = result.message, data = result });
            }
            else
            {
                return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });
            }

        }
        [HttpPost]
        public async Task<IActionResult> alreadyExist([FromBody] string PrintingStationeryPurchaseValue)
        {
            var result = await printingStationeryPurchaseEntryService.AlreadyExistAsync(PrintingStationeryPurchaseValue);
            return Json(new { isSuccess = result.isSuccess, message = result.message, data = result });
        }
        [HttpPost]
        public async Task<IActionResult> supplierIdDetails([FromBody] string supplierId)
        {
            var result = await printingStationeryPurchaseEntryService.getSupplierByIdAsync(supplierId);
            return Json(new { data = result });
        }

        [HttpPost]
        public async Task<IActionResult> productSelectIdDetails([FromBody] string productId)
        {
            var result = await printingStationeryPurchaseEntryService.productSelectIdDetailsAsync(productId);
            return Json(new { data = result });
        }
        [HttpPost]
        public async Task<IActionResult> brandIdDetailsonModel([FromBody] string brandId)
        {
            var result = await printingStationeryPurchaseEntryService.brandIdAsync(brandId);
            return Json(new { data = result });
        }
        [HttpGet]
        public async Task<IActionResult> addMoreLoadProduct()
        {
            var sizeList = sizeRepo.All().Select(x => new { value = x.SizeId, text = x.SizeName }).ToList();
            var periodList = periodRepo.All().Select(x => new { value = x.PeriodId, text = x.PeriodName }).ToList();
            var unitList = unitRepo.All().Select(x => new { value = x.UnitTypId, text = x.UnitTypeName }).ToList();

            var productList = productRepo.All()
                .Select(x => new
                {
                    value = x.ProductCode,
                    text = x.ProductName
                }).ToList();

            return Json(new
            {
                productList=productList,
                sizeList=sizeList,
                periodList=periodList,
                unitList = unitList
            });
        }
        [HttpGet]
        public  IActionResult productItemClose()
            {
            //var result = productRepo.All().Select(x => new { x.ProductCode, x.ProductName }).OrderByDescending(x=> x.ProductCode).ToList();
            var result = productRepo.All().Where(x => x.AutoId != null).OrderByDescending(x => x.AutoId).ToList();
                return Json(new {data=result});
            }

        [HttpGet]
        public IActionResult BrandListClose()
        {
            var result = brandRepo.All().Where(x => x.AutoId != null).OrderByDescending(x => x.AutoId).ToList();
            return Json(new { data = result });
        }
        [HttpGet]
        public IActionResult ModelListClose()
        {
            var result = modelRepo.All().Where(x => x.AutoId != null).OrderByDescending(x => x.AutoId).ToList();
            return Json(new { data = result });
        }
        [HttpGet]
        public IActionResult SizeListClose()
        {
            var result = sizeRepo.All().Where(x => x.AutoId != null).OrderByDescending(x => x.AutoId).ToList();
            return Json(new { data = result });
        }
        [HttpGet]
        public IActionResult UnitListClose()
        {
            var result = unitRepo.All().Where(x => x.Tc != null).OrderByDescending(x => x.Tc).ToList();
            return Json(new { data = result });
        }
    }
}

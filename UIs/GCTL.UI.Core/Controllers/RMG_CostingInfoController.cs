//using GCTL.Core.Data;
//using GCTL.Core.Helpers;
//using GCTL.Core.ViewModels.RMG_CostingInfo;
//using GCTL.Data.Models;
//using GCTL.Service.Common;
//using GCTL.Service.EmployeeOfficialInfoReport;
//using GCTL.Service.RMG_CostingInfo;
//using GCTL.UI.Core.ViewModels.RMG_CostingInfo;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using System.Threading.Tasks;


//namespace GCTL.UI.Core.Controllers
//{
//    public class RMG_CostingInfoController : BaseController
//    {
//        private readonly IRMG_CostingInfoService prodOrderService;
//        private readonly IRepository<RmgProdDefUnitType> unitRepo;
//        private readonly IRepository<RmgDefSupplier> supplier;
//        //private readonly IRepository<SalesSupplier> supplier;
//        private readonly IRepository<HrmEmployee> empRepo;
//        private readonly IRepository<InvDefItem> itemRepo;
//        private readonly IRepository<RmgProdDefColor> colorRepo;
//        private readonly IRepository<CaDefCurrency> currencyRepo;
//        private readonly ICommonService commonService;

//        public RMG_CostingInfoController(
//           IRMG_CostingInfoService _prodOrderService,
//            IRepository<RmgProdDefUnitType> unitRepo,
//            //IRepository<SalesSupplier> supplier,
//            IRepository<RmgDefSupplier> supplier,
//            IRepository<HrmEmployee> empRepo,
//            IRepository<InvDefItem> itemRepo,
//            IRepository<RmgProdDefColor> colorRepo,
//            IRepository<CaDefCurrency> currencyRepo,
//            ICommonService commonService
//            )
//        {
//            prodOrderService = _prodOrderService;
//            this.unitRepo = unitRepo;
//            //this.supplier = supplier;
//            this.supplier = supplier;
//            this.empRepo = empRepo;
//            this.itemRepo = itemRepo;
//            this.colorRepo = colorRepo;
//            this.currencyRepo = currencyRepo;
//            this.commonService = commonService;
//        }
//        public async Task<IActionResult> Index()
//        {
//            var hasPermission = await prodOrderService.PagePermissionAsync(LoginInfo.AccessCode);

//            if (!hasPermission)

//            {

//                return RedirectToAction("Login", "Accounts");

//            }

//            //ViewBag.buyerList = new SelectList(buyerRepo.All().Select(x => new { id = x.BuyerId, name = x.BuyerName }), "id", "name");
//            ViewBag.EmployeeList = new SelectList(empRepo.All().Select(x => new { id = x.EmployeeId, name = x.FirstName + " " + x.LastName }), "id", "name");
//            RMG_CostingInfoViewModel model = new RMG_CostingInfoViewModel()
//            {
//                PageUrl = Url.Action(nameof(Index))
//            };
//            return View(model);
//        }


//        [HttpGet]
//        public async Task<IActionResult> LoadedViewBackData()
//        {
//            try
//            {
//                var data = new
//                {
//                    itemList = itemRepo.All().Select(x => new { id = x.ItemId, name = x.ItemName }).ToList(),
//                    supplierList = supplier.All().Select(x => new { id = x.SupplierId, name = x.SupplierName }).ToList(),
//                    colorList = colorRepo.All().Select(x => new { id = x.ColorId, name = x.Color }).ToList(),
//                    currencyList = currencyRepo.All().Select(x => new { id = x.CurrencyId, name = x.ShortName }).ToList(),
//                    unitList = unitRepo.All().Select(x => new { id = x.UnitTypId, name = x.UnitTypeName }).ToList()
//                };

//                return Json(data);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = ex.Message });
//            }
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetFilterOptions([FromQuery] ProdOrderFilterDto filter)
//        {
//            try
//            {
//                var filters = await prodOrderService.GetFilterOptions(filter);
//                return Ok(filters);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = ex.Message });
//            }
//        }

//        [HttpPost]
//        public async Task<IActionResult> GetReport([FromBody] ProdOrderFilterDto filter)
//        {
//            try
//            {
//                var report = await prodOrderService.GetProdOrderReport(filter);
//                return Ok(report);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = ex.Message });
//            }
//        }
//        [HttpGet]
//        public async Task<IActionResult> AutoIdCosting()
//        {
//            var id = commonService.GenerateNextCode("CostingId", "RMG_CostingInfo", 8, "CO_");
//            return Json(id);
//        }



//        [HttpGet]
//        public async Task<JsonResult> GetCostingDetails(string costingId, bool clearTemp = true)
//        {
//            try
//            {

//                var result = await prodOrderService.GetAllByCostingIdAsync(costingId, clearTemp, LoginInfo.Username);
//                return Json(new { success = true, data = result });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = ex.Message });
//            }
//        }

//        [HttpPost]
//        public async Task<JsonResult> UploadExcel(IFormFile file, string costingId)
//        {
//            try
//            {
//                var result = await prodOrderService.ImportExcelAsync(file, costingId, LoginInfo.Username);

//                // Return updated data without clearing
//                var data = await prodOrderService.GetAllByCostingIdAsync(costingId, clearTemp: false, LoginInfo.Username);

//                return Json(new { success = result, data = data, message = "Excel imported successfully" });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = ex.Message });
//            }
//        }

//        [HttpPost]
//        public async Task<JsonResult> AddCostingDetail([FromBody] RmgCostingDetailsTempDto dto)
//        {
//            try
//            {
//                var costingDetailsId = commonService.GenerateNextCode("CostingDetailsId", "RMG_CostingDetails", 8, "CO_DL_");
//                //var costingId = commonService.GenerateNextCode("CostingId", "RMG_CostingInfo", 8, "CO_");
//                dto.CostingDetailsId = costingDetailsId;
//                //dto.CostingId = costingId;
//                var result = await prodOrderService.AddAsync(dto);
//                return Json(new { success = true, data = result });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = ex.Message });
//            }
//        }

//        [HttpPost]
//        public async Task<JsonResult> UpdateCostingDetail([FromBody] RmgCostingDetailsTempDto dto)
//        {
//            try
//            {
//                var result = await prodOrderService.UpdateAsync(dto);
//                return Json(new { success = true, data = result });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = ex.Message });
//            }
//        }


//        [HttpPost]
//        public async Task<JsonResult> DeleteCostingDetail(string id)
//        {
//            try
//            {
//                if (Convert.ToInt32(id) <= 0)
//                {
//                    return Json(new { success = true, message = "Row removed from UI" });
//                }

//                var result = await prodOrderService.DeleteAsync(id);
//                return Json(new { success = result, message = result ? "Deleted successfully" : "Record not found" });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = ex.Message });
//            }
//        }
//        [HttpPost]
//        public async Task<JsonResult> CalculateSummary([FromBody] CalculateSummaryRequest request)
//        {
//            try
//            {
//                var result = await prodOrderService.CalculateSummaryAsync(
//                    request.CostingId,
//                    request.DamagePercent,
//                    request.InterestPercent,
//                    request.CmAndProfit,
//                    request.HandlingCharge,
//                    request.ProductionUpchargePercent
//                );
//                return Json(new { success = true, data = result });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = ex.Message });
//            }
//        }


//        [HttpPost]
//        public async Task<JsonResult> PreviewExcel(IFormFile file)
//        {
//            try
//            {
//                var result = await prodOrderService.PreviewExcelAsync(file);
//                return Json(new { success = true, data = result });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = ex.Message });
//            }
//        }




//        [HttpPost]
//        public async Task<IActionResult> CreateUpdate([FromBody] RmgCostingInfoDto model)
//        {
//            if (!ModelState.IsValid)
//                return Json(new { isSuccess = false, message = "Invalid data." });

//            try
//            {
//                model.ToAudit(LoginInfo); // Set audit info

//                if (model.AutoId == 0)
//                {
//                    // Save Permission
//                    bool hasSavePermission = await prodOrderService.SavePermissionAsync(LoginInfo.AccessCode);
//                    if (!hasSavePermission)
//                        return Json(new { isSuccess = false, noSavePermission = true, message = "No save permission" });

//                    var result = await prodOrderService.CreateUpdateAsync(model, LoginInfo.CompanyCode);
//                    return Json(new { isSuccess = result.isSuccess, message = result.message, data = result.data });
//                }
//                else
//                {
//                    // Update Permission
//                    bool hasUpdatePermission = await prodOrderService.UpdatePermissionAsync(LoginInfo.AccessCode);
//                    if (!hasUpdatePermission)
//                        return Json(new { isSuccess = false, noUpdatePermission = true, message = "No update permission" });

//                    var result = await prodOrderService.CreateUpdateAsync(model, LoginInfo.CompanyCode);
//                    return Json(new { isSuccess = result.isSuccess, message = result.message, data = result.data });
//                }
//            }
//            catch (Exception ex)
//            {
//                return Json(new { isSuccess = false, message = ex.Message });
//            }
//        }
//        [HttpPost]
//        public async Task<IActionResult> GetAllForDataTable()
//        {
//            var draw = Request.Form["draw"].FirstOrDefault();
//            var start = Convert.ToInt32(Request.Form["start"].FirstOrDefault());
//            var length = Convert.ToInt32(Request.Form["length"].FirstOrDefault());
//            var search = Request.Form["search[value]"].FirstOrDefault();

//            var result = await prodOrderService.GetAllForDataTableAsync(start, length, search);

//            return Json(new
//            {
//                draw = draw,
//                recordsTotal = result.total,
//                recordsFiltered = result.total,
//                data = result.data
//            });
//        }

//        [HttpPost]
//        public async Task<IActionResult> Delete(int autoId)
//        {
//            var result = await prodOrderService.DeleteAsync(autoId);
//            return Json(new { success = result.isSuccess, message = result.message });
//        }

//        [HttpGet]
//        public async Task<IActionResult> Edit(int autoId)
//        {
//            var result = await prodOrderService.EditCostingAsync(autoId);

//            if (result.isSuccess)
//            {
//                return Json(new { success = true, data = result.data });
//            }

//            return Json(new { success = false, message = result.message });
//        }




//        [HttpGet]
//        public async Task<IActionResult> GetCostingReport(
//    string costingId,
//    string integraJobNo,
//    string purchaseOrder,
//    string productId)
//        {
//            if (string.IsNullOrWhiteSpace(costingId) ||
//                string.IsNullOrWhiteSpace(integraJobNo) ||
//                string.IsNullOrWhiteSpace(purchaseOrder) ||
//                string.IsNullOrWhiteSpace(productId))
//                return BadRequest("Missing required parameters");

//            var result = await prodOrderService.GetCostingReportByIdAsync(
//                costingId,
//                integraJobNo,
//                purchaseOrder,
//                productId);

//            if (result == null)
//                return NotFound("No data found");

//            return Ok(result);
//        }
//    }


//}

//using GCTL.Core.Data;
//using GCTL.Core.Helpers;
//using GCTL.Data.Models;
//using GCTL.Service.BuyerBrandEntry;
//using GCTL.Service.BuyerDLAddressEntry;
//using GCTL.Service.BuyerInfos;
//using GCTL.UI.Core.ViewModels.BuyerInfos;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;

//namespace GCTL.UI.Core.Controllers
//{
//    public class BuyerInfoController : BaseController
//    {
//        #region Private Fields

//        private readonly IRepository<InvDefCompanyInfo> bComRepo;
//        private readonly IRepository<InvDefBuyerDepartment> bDepRepo;
//        private readonly IBuyerBrandService brandEntryRepo;
//        private readonly IRepository<RmgProdDefBuyerPhoto> buyerPhotoRepo;
//        private readonly IRepository<RmgProdDefBuyer> buyerRepo;
//        private readonly IRepository<CaDefCountry> countryRepo;
//        private readonly IRepository<SalesContactPerson> cpRepo;
//        private readonly IRepository<InvDefSalesPerson> spRepo;
//        private readonly IBuyerDLAddressService dlEntryRepo;
//        private readonly IRepository<HrmDefDesignation> desigRepo;
//        private readonly IBuyerInfoService entryRepo;

//        #endregion Private Fields

//        #region Public Constructors

//        public BuyerInfoController(IBuyerInfoService entryRepo, IBuyerBrandService brandEntryRepo, IBuyerDLAddressService dlEntryRepo, IRepository<RmgProdDefBuyer> buyerRepo, IRepository<InvDefCompanyInfo> bComRepo, IRepository<InvDefBuyerDepartment> bDepRepo, IRepository<CaDefCountry> countryRepo, IRepository<SalesContactPerson> cpRepo, IRepository<InvDefSalesPerson> spRepo, IRepository<RmgProdDefBuyerPhoto> buyerPhotoRepo, IRepository<HrmDefDesignation> desigRepo)
//        {
//            this.entryRepo = entryRepo;
//            this.brandEntryRepo = brandEntryRepo;
//            this.dlEntryRepo = dlEntryRepo;

//            this.desigRepo = desigRepo;
//            this.buyerRepo = buyerRepo;
//            this.bComRepo = bComRepo;
//            this.bDepRepo = bDepRepo;
//            this.countryRepo = countryRepo;
//            this.cpRepo = cpRepo;
//            this.spRepo = spRepo;
//            this.buyerPhotoRepo = buyerPhotoRepo;
//        }

//        #endregion Public Constructors

//        #region Public Methods

//        public async Task<IActionResult> Index()
//        {
//            var hasPermission = await entryRepo.PagePermissionAsync(LoginInfo.AccessCode);
//            if (!hasPermission)
//            {
//                return RedirectToAction("Login", "Accounts");
//            }


//            ViewBag.Buyer = new SelectList(buyerRepo.All().Select(x => new
//            {
//                x.BuyerId,
//                BuyerName =
//                $"{x.BuyerName} ({x.BuyerId})"
//            }), "BuyerId", "BuyerName");

//            ViewBag.Company = new SelectList(bComRepo.All().Where(x => x.CompanyForId == "02").Select(x => new { x.CompanyId, x.CompanyName }), "CompanyId", "CompanyName");
//            ViewBag.Department = new SelectList(bDepRepo.All().Select(x => new { x.BuyerDepartmentId, x.DepartmentName }), "BuyerDepartmentId", "DepartmentName");
//            ViewBag.Country = new SelectList(countryRepo.All().Select(x => new { x.CountryId, x.CountryName }), "CountryId", "CountryName");
//            ViewBag.ContactPerson = new SelectList(cpRepo.All().Select(x => new { x.Cpid, x.ContactPersonName }), "Cpid", "ContactPersonName");
//            ViewBag.SalesPerson = new SelectList(spRepo.All().Select(x => new { x.SalesPersonId, x.SalesPerson }), "SalesPersonId", "SalesPerson");

//            BuyerInfoPageViewModel model = new BuyerInfoPageViewModel()
//            {
//                PageUrl = Url.Action(nameof(Index))
//            };

//            return View(model);
//        }

//        #endregion Public Methods

//        #region Buyer

//        [HttpPost]
//        public async Task<IActionResult> DeleteBuyerPhoto(decimal id)
//        {
//            var hasPermissions = await entryRepo.DeletePermissionAsync(LoginInfo.AccessCode);
//            if (!hasPermissions)
//            {
//                return Json(new { success = false, message = "You have no access." });
//            }

//            if (id < 0)
//            {
//                return Json(new { success = false, message = "Failed to delete image." });
//            }

//            var result = await entryRepo.DeleteImageAsync(id);

//            return Json(new { success = result.isSuccess, message = result.message });
//        }

//        [HttpDelete]
//        public async Task<IActionResult> BulkBuyerDelete([FromBody] List<decimal> tcs)
//        {
//            try
//            {
//                bool hasPermission = await entryRepo.DeletePermissionAsync(LoginInfo.AccessCode);

//                if (!hasPermission)
//                {
//                    return Json(new { success = false, message = "You have no access." });
//                }

//                if (tcs == null || !tcs.Any() || tcs.Count < 1)
//                    return Json(new { success = false, message = "No data is selected" });
//                var result = await entryRepo.BulkDeleteAsync(tcs);

//                return Json(new { success = result.isSuccess, message = result.message });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = ex.Message });
//            }
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetBuyerById(decimal id)
//        {
//            var result = await entryRepo.GetByIdAsync(id);
//            return Json(new { data = result });
//        }

//        [HttpPost]
//        public async Task<IActionResult> GetBuyerList(string id = null)
//        {
//            try
//            {
//                var draw = Request.Form["draw"].FirstOrDefault();
//                var start = Request.Form["start"].FirstOrDefault();
//                var length = Request.Form["length"].FirstOrDefault();
//                var searchValue = Request.Form["search[value]"].FirstOrDefault();
//                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
//                var sortColumn = Request.Form[$"columns[{sortColumnIndex}][data]"].FirstOrDefault();
//                var sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();

//                var pageSize = string.IsNullOrEmpty(length) ? 10 : Convert.ToInt32(length);
//                var page = string.IsNullOrEmpty(start) ? 1 : (Convert.ToInt32(start) / pageSize) + 1;

//                var result = await entryRepo.GetPaginatedDataAsync(searchValue, page, pageSize, sortColumn, sortDirection, id);

//                var response = new
//                {
//                    draw = draw,
//                    recordsTotal = result.totalRecord,
//                    recordsFiltered = result.curentRecord,
//                    data = result.Data
//                };

//                return Ok(response);
//            }
//            catch (Exception ex)
//            {
//                return Json(new { error = ex.Message });
//            }
//        }

//        [HttpGet]
//        public JsonResult GetContactPersons()
//        {
//            try
//            {
//                var contactPersons = (from cp in cpRepo.All()
//                                      join desig in desigRepo.All()
//                                      on cp.DesignationCode equals desig.DesignationCode
//                                      select new
//                                      {
//                                          cpid = cp.Cpid,
//                                          contactPersonName = cp.ContactPersonName,
//                                          designation = desig.DesignationName, // Assuming desigRepo has a Name property
//                                          phone = cp.ContactPersonMobile,
//                                          email = cp.ContactPersonEmail
//                                      }).ToList();

//                return Json(new { success = true, data = contactPersons });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = ex.Message });
//            }
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> SaveBuyer(BuyerInfoPageViewModel model)
//        {
//            if (model == null || model.Setup == null)
//                return Json(new
//                {
//                    success = false,
//                    message = "Saved Failed!"
//                });

//            if (model.Setup.Tc == 0)
//            {
//                if (!await entryRepo.SavePermissionAsync(LoginInfo.AccessCode))
//                {
//                    return Json(new
//                    {
//                        success = false,
//                        message = "You have no access to save."
//                    });
//                }
//            }
//            else
//            {
//                if (!await entryRepo.UpdatePermissionAsync(LoginInfo.AccessCode))
//                {
//                    return Json(new
//                    {
//                        success = false,
//                        message = "You have no access to update"
//                    });
//                }
//            }

//            model.Setup.ToAudit(LoginInfo, model.Setup.Tc > 0);

//            model.Setup.CompanyCode = LoginInfo.CompanyCode;
//            var result = await entryRepo.SaveAsync(model.Setup);
//            return Json(new
//            {
//                success = result.isSuccess,
//                message = result.message
//            });
//        }
//        #endregion

//        #region Brand

//        public async Task<IActionResult> GetBuyerForDD()
//        {
//            var buyers = await buyerRepo.All().Select(x => new
//            {
//                BuyerId = x.BuyerId,
//                BuyerName = $"{x.BuyerName} ({x.BuyerId})"
//            }).ToListAsync();
//            return Json(new { success = true, data = buyers });
//        }

//        public async Task<IActionResult> DeleteBrandImage(decimal id)
//        {
//            var hasPermissions = await brandEntryRepo.DeletePermissionAsync(LoginInfo.AccessCode);
//            if (!hasPermissions)
//            {
//                return Json(new { success = false, message = "You have no access." });
//            }

//            if (id < 0)
//            {
//                return Json(new { success = false, message = "Failed to delete image." });
//            }

//            var result = await brandEntryRepo.DeleteImageAsync(id);

//            return Json(new { success = result.isSuccess, message = result.message });
//        }

//        [HttpDelete]
//        public async Task<IActionResult> BulkBuyerBrandDelete([FromBody] List<decimal> tcs)
//        {
//            try
//            {
//                if (tcs == null || !tcs.Any() || tcs.Count < 1)
//                    return Json(new { success = false, message = "No data is selected" });

//                var hasPermission = await entryRepo.DeletePermissionAsync(LoginInfo.AccessCode);
//                if (!hasPermission)
//                {
//                    return Json(new { success = false, message = "You have No access." });
//                }

//                var result = await brandEntryRepo.BulkDeleteAsync(tcs);

//                return Json(new { success = result.isSuccess, message = result.message });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = ex.Message });
//            }
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetBuyerBrandById(decimal id)
//        {
//            var result = await brandEntryRepo.GetByIdAsync(id);
//            return Json(new { data = result });
//        }

//        [HttpPost]
//        public async Task<IActionResult> GetBuyerBrandList(string id = null, string buyerId = null)
//        {
//            try
//            {
//                var draw = Request.Form["draw"].FirstOrDefault();
//                var start = Request.Form["start"].FirstOrDefault();
//                var length = Request.Form["length"].FirstOrDefault();
//                var searchValue = Request.Form["search[value]"].FirstOrDefault();
//                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
//                var sortColumn = Request.Form[$"columns[{sortColumnIndex}][data]"].FirstOrDefault();
//                var sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();

//                var pageSize = string.IsNullOrEmpty(length) ? 10 : Convert.ToInt32(length);
//                var page = string.IsNullOrEmpty(start) ? 1 : (Convert.ToInt32(start) / pageSize) + 1;

//                var result = await brandEntryRepo.GetPaginatedDataAsync(searchValue, page, pageSize, sortColumn, sortDirection, id, buyerId);

//                var response = new
//                {
//                    draw = draw,
//                    recordsTotal = result.totalRecord,
//                    recordsFiltered = result.curentRecord,
//                    data = result.Data
//                };

//                return Ok(response);
//            }
//            catch (Exception ex)
//            {
//                return Json(new { error = ex.Message });
//            }
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> SaveBuyerBrand(BuyerInfoPageViewModel model)
//        {
//            if (model == null)
//                return Json(new
//                {
//                    success = false,
//                    message = "Saved Failed!"
//                });

//            if (model.Brand.Tc == 0)
//            {
//                if (!await brandEntryRepo.SavePermissionAsync(LoginInfo.AccessCode))
//                {
//                    return Json(new
//                    {
//                        success = false,
//                        message = "You have no access to save."
//                    });
//                }
//            }
//            else
//            {
//                if (!await brandEntryRepo.UpdatePermissionAsync(LoginInfo.AccessCode))
//                {
//                    return Json(new
//                    {
//                        success = false,
//                        message = "You have no access to update"
//                    });
//                }
//            }


//            model.Brand.ToAudit(LoginInfo, model.Brand.Tc > 0);

//            model.Brand.CompanyCode = LoginInfo.CompanyCode;
//            var result = await brandEntryRepo.SaveAsync(model.Brand);
//            return Json(new
//            {
//                success = result.isSuccess,
//                message = result.message
//            });
//        }
//        #endregion

//        #region DL

//        [HttpDelete]
//        public async Task<IActionResult> BulkBuyerDLAddressDelete([FromBody] List<decimal> tcs)
//        {
//            try
//            {
//                if (tcs == null || !tcs.Any() || tcs.Count < 1)
//                    return Json(new { success = false, message = "No data is selected" });

//                var hasPermission = await dlEntryRepo.DeletePermissionAsync(LoginInfo.AccessCode);
//                if (!hasPermission)
//                {
//                    return Json(new { success = false, message = "You have no access" });
//                }


//                var result = await dlEntryRepo.BulkDeleteAsync(tcs);

//                return Json(new { success = result.isSuccess, message = result.message });
//            }
//            catch (Exception ex)
//            {
//                return Json(new { success = false, message = ex.Message });
//            }
//        }

//        [HttpGet]
//        public async Task<IActionResult> GetBuyerDLAddressById(decimal id)
//        {
//            var result = await dlEntryRepo.GetByIdAsync(id);
//            return Json(new { data = result });
//        }

//        [HttpPost]
//        public async Task<IActionResult> GetBuyerDLAddressList(string id = null, string buyerId = null)
//        {
//            try
//            {
//                var draw = Request.Form["draw"].FirstOrDefault();
//                var start = Request.Form["start"].FirstOrDefault();
//                var length = Request.Form["length"].FirstOrDefault();
//                var searchValue = Request.Form["search[value]"].FirstOrDefault();
//                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
//                var sortColumn = Request.Form[$"columns[{sortColumnIndex}][data]"].FirstOrDefault();
//                var sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();

//                var pageSize = string.IsNullOrEmpty(length) ? 10 : Convert.ToInt32(length);
//                var page = string.IsNullOrEmpty(start) ? 1 : (Convert.ToInt32(start) / pageSize) + 1;

//                var result = await dlEntryRepo.GetPaginatedDataAsync(searchValue, page, pageSize, sortColumn, sortDirection, id, buyerId);

//                var response = new
//                {
//                    draw = draw,
//                    recordsTotal = result.totalRecord,
//                    recordsFiltered = result.curentRecord,
//                    data = result.Data
//                };

//                return Ok(response);
//            }
//            catch (Exception ex)
//            {
//                return Json(new { error = ex.Message });
//            }
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> SaveBuyerDLAddress(BuyerInfoPageViewModel model)
//        {
//            if (model == null)
//                return Json(new
//                {
//                    success = false,
//                    message = "Saved Failed!"
//                });

//            if (model.DLAddress.Tc == 0)
//            {
//                if (!await dlEntryRepo.SavePermissionAsync(LoginInfo.AccessCode))
//                {
//                    return Json(new
//                    {
//                        success = false,
//                        message = "You have no access to save."
//                    });
//                }
//            }
//            else
//            {
//                if (!await dlEntryRepo.UpdatePermissionAsync(LoginInfo.AccessCode))
//                {
//                    return Json(new
//                    {
//                        success = false,
//                        message = "You have no access to update"
//                    });
//                }
//            }

//            model.DLAddress.ToAudit(LoginInfo, model.DLAddress.Tc > 0);

//            model.Setup.CompanyCode = LoginInfo.CompanyCode;
//            var result = await dlEntryRepo.SaveAsync(model.DLAddress);

//            return Json(new
//            {
//                success = result.isSuccess,
//                message = result.message
//            });
//        }
//        #endregion



//        //public IActionResult QuickAdd()
//        //{
//        //    var model = new BuyerViewModel();
//        //    return PartialView("_BuyerInfoMain", model);
//        //}
//    }
//}

using GCTL.Core.Helpers;
using GCTL.Service.BuyerDepartmentEntry;
using GCTL.Service.Common;
using GCTL.UI.Core.ViewModels.InvBuyerDepartment;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class BuyerDepartmentController : BaseController
    {
        #region Private Fields

        private readonly ICommonService comService;
        private readonly IBuyerDepEntryService entryRepo;

        #endregion Private Fields

        #region Public Constructors

        public BuyerDepartmentController(ICommonService comService, IBuyerDepEntryService entryRepo)
        {
            this.comService = comService;
            this.entryRepo = entryRepo;
        }

        #endregion Public Constructors

        #region Public Methods

        public async Task<IActionResult> Index()
        {
            var hasPermission = await entryRepo.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            BuyerDepartmentPageViewModel model = new BuyerDepartmentPageViewModel()
            {
                PageUrl = Url.Action(nameof(Index)),
            };
            return View(model);
        }

        #endregion Public Methods

        #region DELETE
        [HttpDelete]
        public async Task<IActionResult> BulkBuyerDepartmentDelete([FromBody] List<int> tcs)
        {
            try
            {
                if (tcs == null || !tcs.Any() || tcs.Count < 1)
                    return Json(new { success = false, message = "No data is selected" });
                var result = await entryRepo.BulkDeleteAsync(tcs);

                return Json(new { success = result.isSuccess, message = result.message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        #endregion

        #region GenerateID
        [HttpGet]
        public async Task<string> GenerateDepID()
        {
            var newId = comService.GenerateNextCode("BuyerDepartmentId", "Inv_Def_BuyerDepartment", 3);
            return newId;
        }
        #endregion

        #region GetBY ID
        [HttpGet]
        public async Task<IActionResult> GetBuyerDepartmentById(int id)
        {
            var result = await entryRepo.GetByIdAsync(id);
            return Json(new { data = result });
        }
        #endregion

        #region GET ALL
        [HttpPost]
        public async Task<IActionResult> GetBuyerDepartmentList()
        {
            try
            {
                var draw = Request.Form["draw"].FirstOrDefault();
                var start = Request.Form["start"].FirstOrDefault();
                var length = Request.Form["length"].FirstOrDefault();
                var searchValue = Request.Form["search[value]"].FirstOrDefault();
                var sortColumnIndex = Request.Form["order[0][column]"].FirstOrDefault();
                var sortColumn = Request.Form[$"columns[{sortColumnIndex}][data]"].FirstOrDefault();
                var sortDirection = Request.Form["order[0][dir]"].FirstOrDefault();

                var pageSize = string.IsNullOrEmpty(length) ? 10 : Convert.ToInt32(length);
                var page = string.IsNullOrEmpty(start) ? 1 : (Convert.ToInt32(start) / pageSize) + 1;

                var result = await entryRepo.GetPaginatedDataAsync(searchValue, page, pageSize, sortColumn, sortDirection);

                var response = new
                {
                    draw = draw,
                    recordsTotal = result.totalRecord,
                    recordsFiltered = result.curentRecord,
                    data = result.Data
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }
        #endregion

        #region SAVE
        [HttpPost]
        public async Task<IActionResult> SaveBuyerDepartment(BuyerDepartmentPageViewModel model)
        {
            if (model == null)
                return Json(new
                {
                    success = false,
                    message = "Saved Failed!"
                });

            if (model.Department.Tc == 0)
            {
                if (!await entryRepo.SavePermissionAsync(LoginInfo.AccessCode))
                {
                    return Json(new
                    {
                        success = false,
                        message = "You have no access to save."
                    });
                }
            }
            else
            {
                if (!await entryRepo.UpdatePermissionAsync(LoginInfo.AccessCode))
                {
                    return Json(new
                    {
                        success = false,
                        message = "You have no access to update"
                    });
                }
            }


            model.Department.ToAudit(LoginInfo, model.Department.Tc > 0);

            var result = await entryRepo.SaveAsync(model.Department);
            return Json(new
            {
                success = result.isSuccess,
                message = result.message,
                lastCode = model.Department.BuyerDepartmentId
            });
        }
        #endregion
    }
}

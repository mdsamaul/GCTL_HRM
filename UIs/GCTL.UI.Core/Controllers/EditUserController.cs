using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.EditUserVM;
using GCTL.Service.Common;
using GCTL.Service.UserEditEntry;
using GCTL.UI.Core.ViewModels.EditUsers;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GCTL.UI.Core.Controllers
{
    public class EditUserController : BaseController
    {
        private readonly ICommonService commonService;
        private readonly IUserEditService entryService;

        public EditUserController(ICommonService commonService, IUserEditService entryService)
        {
            this.commonService = commonService;
            this.entryService = entryService;
        }

        public async Task<IActionResult> Index()
        {
            //var hasPermission = await entryService.PagePermissionAsync(LoginInfo.AccessCode);
            //if (!hasPermission)
            //    return RedirectToAction("Login", "Accounts");

            EditUserPageViewModel model = new EditUserPageViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };

            return View(model);
        }

        [HttpGet("EditUser/GetById")]
        public async Task<IActionResult> GetById()
        {
            var result = await entryService.GetByIdAsync(LoginInfo.Username);
            return Json(new { data = result });
        }

        [HttpGet("EditUser/GetById/{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await entryService.GetByIdAsync(id);
            return Json(new { data = result });
        }

        [HttpPost]
        public async Task<IActionResult> GetPaginatedList()
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

                var result = await entryService.GetPaginatedDataAsync(searchValue, page, pageSize, sortColumn, sortDirection, LoginInfo.Username);

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(EditUserSetupViewModel model)
        {
            if (model.UserId == 0)
                return Json(new
                {
                    success = false,
                    message = "Saved Failed!"
                });

            //var hasPermission = await entryService.SavePermissionAsync(LoginInfo.AccessCode);
                
            //if(!hasPermission){
            //    return Json(new
            //    {
            //        success = false,
            //        message = "You have no access to save"
            //    });
            //}

            model.ToAudit(LoginInfo, model.UserId > 0);

            // Encrypt password before saving
            if (!string.IsNullOrWhiteSpace(model.Password))
            {
                string encryptedPassword = model.Password;
                try
                {
                    new PXLibrary.PXlibrary().PXEncode(ref encryptedPassword, model.Password);
                    model.Password = encryptedPassword;
                }
                catch (Exception)
                {
                    model.Password = model.Password; 
                }
            }
            var result = await entryService.SaveAsync(model);

            return Json(new
            {
                success = result.isSuccesss,
                message = result.message,
                data = result.data
            });
        }
    }
}

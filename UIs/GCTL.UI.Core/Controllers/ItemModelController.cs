
using GCTL.Core.Data;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.INV_Catagory;
using GCTL.Core.ViewModels.ItemModel;
using GCTL.Data.Models;
using GCTL.Service.ItemModelService;
using GCTL.UI.Core.ViewModels.ItemModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GCTL.UI.Core.Controllers
{
    public class ItemModelController : BaseController
    {
        private readonly IItemModelService itemModelService;
        private readonly IRepository<HrmBrand> brandRepo;

        public ItemModelController(
            IItemModelService itemModelService,
            IRepository<HrmBrand> brandRepo
            )
        {
            this.itemModelService = itemModelService;
            this.brandRepo = brandRepo;
        }
        public IActionResult Index(bool isPartial)
        {
            ViewBag.BrandList = new SelectList(brandRepo.All().Select(x=> new {x.BrandId, x.BrandName}), "BrandId", "BrandName");
            ItemModelViewModel model = new ItemModelViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };
            if (isPartial)
            {
                return PartialView(model);
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> LoadData()
        {
            try
            {
                var data = await itemModelService.GetAllAsync();
                return Json(new { data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> AutoItemModelId()
        {
            try
            {
                var newCategoryId = await itemModelService.AutoItemIdAsync();
                return Json(new { data = newCategoryId });
            }
            catch (Exception)
            {
                throw;
            }

        }


        [HttpPost]
        public async Task<IActionResult> CreateUpdate([FromBody] ItemModelSetupViewModel model)
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
                    bool hasParmision = await itemModelService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasParmision)
                    {
                        var result = await itemModelService.CreateUpdateAsync(model);
                        return Json(new { isSuccess = result.isSuccess, message = result.message, data = result.data });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
                    }
                }
                else
                {
                    var hasUpdatePermission = await itemModelService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        var result = await itemModelService.CreateUpdateAsync(model);
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
            var result = await itemModelService.GetByIdAsync(id);
            return Json(new { result });
        }

        [HttpPost]
        public async Task<IActionResult> deleteItemModel([FromBody] List<string> selectedIds)
        {
            var hasUpdatePermission = await itemModelService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (hasUpdatePermission)
            {
                var result = await itemModelService.DeleteAsync(selectedIds);
                return Json(new { isSuccess = result.isSuccess, message = result.message, data = result });
            }
            else
            {
                return Json(new { isSuccess = false, message = "You have no access.", noUpdatePermission = true });
            }

        }
        [HttpPost]
        public async Task<IActionResult> alreadyExist([FromBody] string CatagoryValue)
        {
            var result = await itemModelService.AlreadyExistAsync(CatagoryValue);
            return Json(new { isSuccess = result.isSuccess, message = result.message, data = result });
        }
    }
}

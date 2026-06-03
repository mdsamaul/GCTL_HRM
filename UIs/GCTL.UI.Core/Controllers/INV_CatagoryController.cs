using GCTL.Core.Data;
using GCTL.Core.Helpers;
using GCTL.Core.ViewModels.INV_Catagory;
using GCTL.Service.INV_Catagory;
using GCTL.UI.Core.ViewModels.INV_Catagory;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class INV_CatagoryController : BaseController
    {
        private readonly IINV_CatagoryService InvCatagoryService;

        public INV_CatagoryController(
            IINV_CatagoryService InvCatagoryService
            )
        {
            this.InvCatagoryService = InvCatagoryService;
        }      

        public IActionResult Index(bool isPartial)
        {            
            INV_CatagoryViewModel model = new INV_CatagoryViewModel()
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
                var data = await InvCatagoryService.GetAllAsync();
                return Json(new { data });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public async Task<IActionResult> AutoCatagoryId()
        {
            try
            {
                var newCategoryId = await InvCatagoryService.AutoCatagoryIdAsync();
                return Json(new { data = newCategoryId });
            }
            catch (Exception)
            {
                throw;
            }
           
        }
       

        [HttpPost]
        public async Task<IActionResult> CreateUpdate([FromBody] INV_CatagorySetupViewModel model)
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
                    bool hasParmision = await InvCatagoryService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasParmision)
                    {
                        var result = await InvCatagoryService.CreateUpdateAsync(model);
                        return Json(new { isSuccess = result.isSuccess, message = result.message, data = result.data });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access.", noSavePermission = true });
                    }
                }
                else
                {
                    var hasUpdatePermission = await InvCatagoryService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        var result = await InvCatagoryService.CreateUpdateAsync(model);
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

                return Json(new { isSuccess = false, message = "Faild"});
            }
            
               
        }

        [HttpGet]
        public async Task<IActionResult> PopulatedDataForUpdate(string id)
        {
            var result = await InvCatagoryService.GetByIdAsync(id);
            return Json(new {result});
        }

        [HttpPost]
        public async Task<IActionResult> deleteCatagory([FromBody] List<string> selectedIds)
        {
            var hasUpdatePermission = await InvCatagoryService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (hasUpdatePermission)
            {
                var result = await InvCatagoryService.DeleteAsync(selectedIds);
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
            var result = await InvCatagoryService.AlreadyExistAsync(CatagoryValue);
            return Json(new {isSuccess = result.isSuccess,message = result.message, data = result});
        }
    }
}

using GCTL.Core.ViewModels.ItemType;
using GCTL.Core.ViewModels.StyleInformation;
using GCTL.Service.Common;
using GCTL.Core.Helpers;
using GCTL.Service.ItemType;
using GCTL.Service.StyleInformation;
using GCTL.UI.Core.ViewModels.ItemType;
using GCTL.UI.Core.ViewModels.StyleInformation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GCTL.UI.Core.Controllers
{
    public class ItemTypeController : BaseController
    {
        #region Service & Repository
        private readonly IItemTypeService itemTypeService;
        private readonly ICommonService commonService;
        string strMaxNO = string.Empty;

        public ItemTypeController(
            IItemTypeService itemTypeService, 
            ICommonService commonService
            
            )
        {
            this.itemTypeService = itemTypeService;
            this.commonService = commonService;
        }

        #endregion

        #region Index
        public async Task<IActionResult> Index(bool child = false)
        {
            var hasPermission = await itemTypeService.PagePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return RedirectToAction("Login", "Accounts");
            }

            var model = new ItemTypePageViewModel
            {
                PageUrl = Url.Action(nameof(Index))
            };

            try
            {

                var list = await itemTypeService.GetAllAsync();
                model.ItemList = list ?? new List<ItemTypeSetupViewModel>();

                commonService.FindMaxNo(ref strMaxNO, "ItemTypeID", "Inv_Def_ItemType", 3);

                model.Setup = new ItemTypeSetupViewModel
                {
                    ItemTypeId = strMaxNO
                };

            }
            catch (Exception ex)
            {

                model.ItemList = new List<ItemTypeSetupViewModel>();
                model.Setup = new ItemTypeSetupViewModel();
                Console.WriteLine("Error" + ex.Message);
            }


            if (child)
                return PartialView(model);

            return View(model);
        }
        #endregion

        #region Setup

        public async Task<IActionResult> Setup(string id)
        {
            ItemTypeSetupViewModel model = new ItemTypeSetupViewModel();
            commonService.FindMaxNo(ref strMaxNO, "ItemTypeID", "Inv_Def_ItemType", 3);

            if (!string.IsNullOrEmpty(id))
            {

                model = await itemTypeService.GetByIdAsync(id);
                if (model == null)
                {

                    return NotFound();
                }
            }
            else
            {

                model.ItemTypeId = strMaxNO;
            }

            return PartialView($"_{nameof(Setup)}", model);
        }

        #endregion

        #region Post Update 

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Setup(ItemTypeSetupViewModel modelVM)
        {
            try
            {

                if (await itemTypeService.IsExistAsync(modelVM.ItemName, modelVM.ItemTypeId))
                {
                    return Json(new { isSuccess = false, message = $"Already Exists!", isDuplicate = true });
                }


                if (!ModelState.IsValid)
                {

                    var errorMessage = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                    return Json(new { isSuccess = false, message = errorMessage });
                }

                modelVM.ToAudit(LoginInfo, modelVM.AutoId > 0);
                if (modelVM.AutoId == 0)
                {
                    var hasSavePermission = await itemTypeService.SavePermissionAsync(LoginInfo.AccessCode);
                    if (hasSavePermission)
                    {
                        await itemTypeService.SaveAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Saved Successfully.", lastCode = modelVM.ItemTypeId });

                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to save.", noSavePermission = true });
                    }
                }
                else
                {

                    var hasUpdatePermission = await itemTypeService.UpdatePermissionAsync(LoginInfo.AccessCode);
                    if (hasUpdatePermission)
                    {
                        await itemTypeService.UpdateAsync(modelVM);
                        return Json(new { isSuccess = true, message = "Updated Successfully.", lastCode = modelVM.ItemTypeId });
                    }
                    else
                    {
                        return Json(new { isSuccess = false, message = "You have no access to update.", noUpdatePermission = true });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:{ex.Message}");
                return RedirectToAction("Login", "Accounts");

            }
        }

        #endregion

        #region Delete

        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] List<string> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return BadRequest(new { success = false, message = "No IDs provided for delete." });
            }

            var hasPermission = await itemTypeService.DeletePermissionAsync(LoginInfo.AccessCode);
            if (!hasPermission)
            {
                return Json(new { success = false, message = "You have no access." });
            }

            bool success = await itemTypeService.DeleteTab(ids);
            if (success)
            {
                return Json(new { success = true, message = "Deleted Successfully." });
            }
            else
            {
                return Json(new { success = false, message = "Deletion failed." });
            }
        }

        #endregion

        #region TabeleLodaing

        [HttpGet]
        public async Task<IActionResult> GetTableData()
        {
            try
            {
                var list = await itemTypeService.GetAllAsync();
                return PartialView("_Grid", list);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        #endregion

        #region Chake Degian

        //public IActionResult Index()
        //{
        //    ItemTypePageViewModel model = new ItemTypePageViewModel
        //    {
        //        Setup = new ItemTypeSetupViewModel()
        //    };
        //    return View(model);
        //}

        #endregion
    }
}

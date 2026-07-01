using DocumentFormat.OpenXml.Office2010.Drawing;
using GCTL.Core.ViewModels.AccessCodes;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.MenuTab;
using GCTL.Data.Models;
using GCTL.Service.MenuTab;
using GCTL.Service.Users;
using GCTL.UI.Core.ViewModels.AccessCodes;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.SS.Formula.Functions;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace GCTL.UI.Core.Controllers
{
    public class MenuTabController : BaseController
    {
        private readonly GCTL_ERP_DB_DatapathContext _context;

        private readonly IMenuTabService _menuTabService;

        private readonly IAccessCodeService _accessCodeService;

        public MenuTabController(GCTL_ERP_DB_DatapathContext context, IAccessCodeService accessCodeService, IMenuTabService menuTabService)
        {
            _context = context;
            _accessCodeService = accessCodeService;
            _menuTabService = menuTabService;
        }

        #region AccessMenu

        public IActionResult AccessMenu()
        {
            var hasPermission = _accessCodeService.HasPermission(LoginInfo.AccessCode);
            if(!hasPermission)
                return RedirectToAction("Login", "Accounts");

            return View();
        }

        public IActionResult AccessCodeIndex(bool child = false)
        {
            var hasPermission = _accessCodeService.HasPermission(LoginInfo.AccessCode);
            if (!hasPermission && child)
                return Json(new { message = "You have no permission" });
            else if (!hasPermission)
                return RedirectToAction("Login", "Accounts");

            AccessCodePageViewModel model = new AccessCodePageViewModel()
            {
                PageUrl = Url.Action(nameof(Index))
            };

            if (child)
                return PartialView(model);

            return View(model);
        }

        [HttpPost("MenuTab/AccessCodeSetup")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AccessCodeSetup(AddAccessCodeDto model)
        {
            if (_accessCodeService.IsAccessCodeExist(model.AccessCodeName, model.AccessCodeId))
                return Json(new { isSuccess = false, message = "Already Exists" });
            
            if (ModelState.IsValid)
            {
                if (_accessCodeService.IsAccessCodeExistByCode(model.AccessCodeId))
                {
                    var result = await _accessCodeService.EditAccessCode(model);
                    return Json(new { isSuccess = result.success, message = result.message, lastCode = model.AccessCodeId });
                }
                else
                {
                    var result = await _accessCodeService.AddAccessCode(model);
                    return Json(new { isSuccess = result.success, message = result.message, lastCode = model.AccessCodeId });
                }
            }

            return Json(new { success = false, message = ModelState.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage });
        }


        [HttpGet]
        [Route("MenuTab/AccessMenus")]
        public async Task<IActionResult> GetAccessCodeList( string accessCodeId)
        {
            var accessCodes = await _menuTabService.GetAccessCodeMenu(accessCodeId);
            return Ok(accessCodes);
        }


        [HttpGet]
        [Route("MenuTab/GetAccessName")]
        public async Task<IActionResult> GetAccessName(string accessCodeId)
        {
            var accessCodes = await _menuTabService.GetAccessName(accessCodeId);
            return Ok(accessCodes);
        }

        [HttpGet]
        [Route("menuTab/GetAccessListTable")]
        public async Task<IActionResult> GetAccessListTable()
        {
            var accessCodes = await _menuTabService.GetAccessListTable();
            return Ok(accessCodes);
        }


        [HttpPost]
        [Route("menuTab/DeleteAccessCodes")]
        public async Task<IActionResult> GetAccessCodeList([FromBody]  List<string> selectedIds)
        {
            if (selectedIds == null || !selectedIds.Any())
                return BadRequest("No IDs provided.");

            try
            {
                var result = await _menuTabService.DeleteAccessCodes(selectedIds);
                return Ok(result);
            } 
            catch(Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        [Route("menuTab/SaveAccessCode")]
        public  IActionResult Save([FromBody] MenuAccessDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = _menuTabService.SaveAccessMenu(dto);

            return Ok(new { success = true });
        }

        [HttpGet]
        [Route("MenuTab/GetParentMenus")]
        public async Task<IActionResult> GetParentMenus()
        {
            var menus = await _accessCodeService.GetParentMenus();
            return Ok(menus);
        }

        [HttpPost]
        [Route("MenuTab/AddAccessCode")]
        public async Task<IActionResult> AddAccessCode([FromBody] AddAccessCodeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.AccessCodeId) || string.IsNullOrWhiteSpace(dto.AccessCodeName))
                return BadRequest("Both fields are required.");

            if (_accessCodeService.IsAccessCodeExistByCode(dto.AccessCodeId))
                return BadRequest("Duplicate Access Code.");

            await _accessCodeService.AddAccessCode(dto);
            return Ok(new { success = true });
        }

        #endregion





        #region MenuTab

        public IActionResult Index()
        {
            return View();
        }



        [HttpGet]
        [Route("CoreMenu/GetOrderIdAll")]
        public async Task< IActionResult> GetOrderIdAll(string parentId, string childId , string grandChildId)
        {
            if (parentId == null &&  childId == null  && grandChildId == null)
            {
                var nextParent =await _menuTabService.GetParentsListCount();
                nextParent++;

                return Ok(nextParent);

            }
            else if (parentId != null && childId == null && grandChildId == null)
            {
                int nextOrder =await _menuTabService.GetOrderCountByParent(parentId);
                nextOrder++;

                return Ok(nextOrder);
            }
            else if (parentId != null && childId != null && grandChildId == null)
            {
                int nextOrder =await _menuTabService.GetOrderCountByParentChild(parentId , childId);
                nextOrder++;

                return Ok(nextOrder);
            }
            else
            {
                int nextOrder =await _menuTabService.GetOrderCountByParentChildGchild(parentId, childId , grandChildId);
                nextOrder++;

                return Ok(nextOrder);
            }

            
        }

        [HttpGet]
        [Route("CoreMenu/getChildByParents")]
        public async Task<IActionResult> GetChildByParents(string parentId)
        {
            var childMenus = await _menuTabService.GetChildByParentId(parentId);
            return Ok( childMenus );
        }

        [HttpGet]
        [Route("CoreMenu/getGChildByChild")]
        public async Task<IActionResult> GetGChildByChild(string childId)
        {
            var gChildMenus = await _menuTabService.GetGChildByChildId(childId);
            return Ok( gChildMenus );
        }

        [HttpGet]
        [Route("CoreMenu/GetNextId")]
        public IActionResult GetNextId()
        {
            var lastEntry = _context.CoreMenuTab2.OrderByDescending(x => x.MenuId).FirstOrDefault();

            string nextId;

            if (lastEntry != null && int.TryParse(lastEntry.MenuId, out int lastId))
            {
               
                nextId = (lastId + 1).ToString("D3");
            }
            else
            {
                nextId = "001";
            }

            return Json(new { id = nextId });

        }

        [HttpGet]
        [Route("CoreMenu/GetParentMenuList")]
        public async Task< IActionResult> GetMenuTabs()
        {
            var menuTabs = _menuTabService.GetParentsList();
            return Ok(new { data = menuTabs.Result });
        }

        [HttpGet]
        [Route("CoreMenu/GetNextOrder")]
        public IActionResult GetNextOrder(string parentId)
        {
            var nextOrder = _context.CoreMenuTab2
              .Where(x => x.ParentId == parentId)
              .OrderByDescending(x => x.OrderBy) 
              .Select(x => x.OrderBy)            
              .FirstOrDefault();

            var nextOrderValue = nextOrder + 1;

            return Json(new { order = nextOrderValue });
        }

        [HttpPost]
        [Route("CoreMenuTab/Save")]
        public async Task< IActionResult> Save( [FromBody] MenuTabPostViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.AutoId > 0)
                {
                    var res   = await _menuTabService.UpdateAsync(model);
                    return Ok(res);
                }
                else
                {
                    var result = await _menuTabService.SaveMenu(model);
                    return Ok(result);
                }

                
            }
            else
            {
                return Json(new { success = false, message = "Error saving menu." });
            }
        }

        //[HttpGet]
        //[Route("api/CoreMenuTab/GetAll")]
        //public IActionResult GetAll()
        //{
        //    var allMenus = _context.CoreMenuTab2.ToList();

        //    var menuTabs = allMenus
        //        .GroupBy(m => m.ParentId)
        //        .Select(group => new
        //        {
        //            // ParentId = group.Key,
        //            ParentId = allMenus.FirstOrDefault(p => p.MenuId == group.Key)?.Title ?? "Main Menu",
        //            Menus = group.OrderBy(m => m.OrderBy).ToList()
        //        })
        //        .ToList();

        //    return Ok(new { data = menuTabs });
        //}


        [HttpGet]
        [Route("api/CoreMenuTab/GetAll")]
        public IActionResult GetAll()
        {
            // Get all menus from the database
            var allMenus = _context.CoreMenuTab2.ToList();

            // Build a complete hierarchical structure with level information
            var hierarchicalMenus = BuildCompleteHierarchy(allMenus);

            return Ok(new { data = hierarchicalMenus });
        }

        private List<dynamic> BuildCompleteHierarchy(List<CoreMenuTab2> allMenus)
        {
            // Create a lookup dictionary for faster access
            var menuLookup = allMenus.ToDictionary(m => m.MenuId, m => m);

            // Find all root menus (where ParentId is null or not found in the lookup)
            var rootMenus = allMenus.Where(m => string.IsNullOrEmpty(m.ParentId) ||
                                               !menuLookup.ContainsKey(m.ParentId)).ToList();

            // Create a lookup for all children by their parent ID
            var childrenLookup = allMenus
                .Where(m => !string.IsNullOrEmpty(m.ParentId))
                .GroupBy(m => m.ParentId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Result list to store all menus with hierarchical information
            var result = new List<dynamic>();

            // Process each root menu and its descendants
            foreach (var rootMenu in rootMenus.OrderBy(m => m.OrderBy))
            {
                // Add the menu group with its direct descendants
                result.Add(new
                {
                    parentId = rootMenu.Title, // Use title as display name
                    menus = GetFlattenedHierarchy(rootMenu, childrenLookup, 0)
                });
            }

            return result;
        }

        private List<dynamic> GetFlattenedHierarchy(CoreMenuTab2 parent,
                                                  Dictionary<string, List<CoreMenuTab2>> childrenLookup,
                                                  int level)
        {
            var result = new List<dynamic>();

            // Add the parent itself with appropriate prefix for its level
            string prefix = level == 0 ? "" : new string('>', level) + " ";

            result.Add(new
            {
                autoId = parent.AutoId,
                menuId = parent.MenuId,
                title = prefix + parent.Title,
                parentId = parent.ParentId,
                orderBy = parent.OrderBy,
                controllerName = parent.ControllerName,
                viewName = parent.ViewName,
                icon = parent.Icon,
                isActive = parent.IsActive,
                level = level // Add level information for UI rendering
            });

            // Process children if any
            if (childrenLookup.ContainsKey(parent.MenuId))
            {
                foreach (var child in childrenLookup[parent.MenuId].OrderBy(m => m.OrderBy))
                {
                    // Recursively get the flattened hierarchy for this child
                    result.AddRange(GetFlattenedHierarchy(child, childrenLookup, level + 1));
                }
            }

            return result;
        }



        [HttpGet]
        [Route("api/CoreMenuTab/GetAll222")]
        public IActionResult GetAll222()
        {
            var allMenus = _context.CoreMenuTab2.ToList();
            var flattenedMenus = new List<dynamic>();

            // Build dictionary for children lookup
            var childrenLookup = allMenus
                .Where(m => !string.IsNullOrEmpty(m.ParentId))
                .GroupBy(m => m.ParentId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Find root menus and build a flat list
            var rootMenus = allMenus
                .Where(m => string.IsNullOrEmpty(m.ParentId) || !allMenus.Any(x => x.MenuId == m.ParentId))
                .OrderBy(m => m.OrderBy);

            foreach (var root in rootMenus)
            {
                flattenedMenus.AddRange(GetFlattenedHierarchy(root, childrenLookup, 0));
            }

            return Ok(new { data = flattenedMenus });
        }







        [HttpPost]
        [Route("api/CoreMenuTab/UpdateMenuOrder1")]
        public async Task<IActionResult> UpdateMenuOrder()
        {
            try
            {
                // Read the request body directly
                using var reader = new StreamReader(Request.Body);
                var requestBody = await reader.ReadToEndAsync();

                // Deserialize the JSON manually
                var orderedMenus = System.Text.Json.JsonSerializer.Deserialize<List<MenuOrderUpdateDto>>(
                    requestBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (orderedMenus == null || !orderedMenus.Any())
                {
                    return BadRequest("No valid menu order data received");
                }

                
                 await _menuTabService.UpdateMenuOrder(orderedMenus);

                return Ok(new { success = true, message = "Menu order updated successfully", count = orderedMenus.Count });
            }
            catch (Exception ex)
            {
                // Log the error
                return StatusCode(500, new { success = false, message = "Error processing menu order", error = ex.Message });
            }
        }


        [HttpPost("CoreMenuTab/UpdateOrder")]
        public async Task<IActionResult> UpdateOrder()
        {
            using var reader = new StreamReader(Request.Body);
            var body = await reader.ReadToEndAsync();

            var orderData = JsonConvert.DeserializeObject<List<MenuOrderUpdateDto>>(body);

            if (orderData == null)
            {
                return BadRequest("Invalid data");
            }

            var result = await _menuTabService.SaveChangeOrder(orderData);

            return Ok(result);
        }


        //[HttpPost]
        //[Route("api/CoreMenuTab/UpdateOrder")]
        //public async Task<IActionResult> UpdateOrder1([FromBody] List<MenuOrderUpdateDto> orderedMenus)
        //{
        //    return Ok();
        //    //foreach (var item in updatedMenus)
        //    //{
        //    //    var menu = await _context.CoreMenuTab2.FindAsync(item.AutoId);
        //    //    if (menu != null)
        //    //    {
        //    //        menu.ParentId = item.ParentId;
        //    //        menu.OrderBy = item.OrderBy;
        //    //    }
        //    //}

        //    //await _context.SaveChangesAsync();
        //    //return Ok();
        //}


        [HttpPost]
        [Route("CoreMenuTab/DeleteMultiple")]
        public IActionResult DeleteMultipleMenu([FromBody] List<int> ids)
        {
            CommonReturn result = _menuTabService.DeleteMultipleMenu(ids);
            return Ok(result);
            
        }


        [HttpGet]
        [Route("coreMenu/getMenuById")]
        public  IActionResult GetMenuById(int id)
        {
            var menu = _context.CoreMenuTab2.FirstOrDefault(m => m.AutoId == id);
            if (menu == null)
            {
                return Ok( new CommonReturn
                {
                    Success = false,
                    Message = "Menu not found"
                });
            }
            return Ok(new CommonReturn
            {
                Success = true,
                Message = "Menu found",
                Data = new MenuTabPostViewModel
                {
                    AutoId = menu.AutoId,
                    MenuId = menu.MenuId,
                    Title = menu.Title,
                    ParentId = menu.ParentId,
                    OrderBy = menu.OrderBy,
                    ControllerName = menu.ControllerName,
                    TableName = menu.TableName,
                    ViewName = menu.ViewName,
                    Icon = menu.Icon,
                    IsActive = menu.IsActive
                }   
            });
        }



        [HttpPost]
        [Route("CoreMenuTab/UpdateMenu")]
        public async Task<IActionResult> UpdateMenu([FromBody] MenuUpdateModel menuData)
        {
            if (menuData == null || string.IsNullOrWhiteSpace(menuData.Title))
            {
                return Json(new { success = false, message = "Title is required." });
            }

            try
            {
                CommonReturn commonReturn = await _menuTabService.UpdateAsync(new MenuTabPostViewModel
                {
                    AutoId = menuData.AutoId,
                    Title = menuData.Title,
                    ControllerName = menuData.ControllerName,
                    ViewName = menuData.ViewName,
                    IsActive = menuData.IsActive,
                    Icon = menuData.Icon,
                    TableName = menuData.TableName
                });

                return Ok(commonReturn);


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating menu item: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while updating the menu item." });
            }
        }

        #endregion

    }


}

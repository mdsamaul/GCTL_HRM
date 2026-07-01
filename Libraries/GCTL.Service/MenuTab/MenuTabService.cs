using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.MenuTab;
using GCTL.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace GCTL.Service.MenuTab
{
    public class MenuTabService : IMenuTabService
    {
        private readonly GCTL_ERP_DB_DatapathContext _context;
        private readonly IRepository<CoreMenuTab2> _menuTabRepository;
        // private readonly IRepository<CoreAccessCodeTest> _AccessCodeRepository;
        private readonly IRepository<CoreAccessCode> _AccessCodeRepository;

        public MenuTabService(Core.Data.IRepository<CoreMenuTab2> menuTabRepository, IRepository<CoreAccessCode> accessCodeRepository, GCTL_ERP_DB_DatapathContext context)
        {
            _menuTabRepository = menuTabRepository;
            _AccessCodeRepository = accessCodeRepository;
            _context = context;
        }

        #region GetAccessList Table

        public async Task<CommonReturn> GetAccessListTable()
        {
            try
            {
                var result1 = await _AccessCodeRepository.All().ToListAsync();

                var result = _AccessCodeRepository.All().AsEnumerable().GroupBy(a => a.AccessCodeId).Select(g => g.First()).ToList();

                return new CommonReturn
                {
                    Success = true,
                    Message = "Access codes retrieved successfully",
                    Data = result
                };
            }
            catch (Exception)
            {

                throw;
            }

        }

        #endregion

        #region GetAccessCodeMenu
        public async Task<CommonReturn> GetAccessCodeMenu(string accessCodeId)
        {
            try
            {
                var test = _menuTabRepository.All().ToList();
                var test2 = _AccessCodeRepository.All().ToList();
                var allMenus = await (
                                      from m in _menuTabRepository.All()
                                      join a in _AccessCodeRepository.All().Where(x => x.AccessCodeId == accessCodeId)
                                      on m.MenuId equals a.MenuId into accessGroup
                                      from a in accessGroup.DefaultIfEmpty()


                                      select new
                                      {
                                          m.MenuId,
                                          m.Title,
                                          m.ParentId,
                                          m.OrderBy,
                                          m.ControllerName,
                                          m.ViewName,
                                          m.Icon,
                                          m.IsActive,
                                          AccessCodeId = a != null ? a.AccessCodeId : null,
                                          AccessCodeName = a != null ? a.AccessCodeName : null,
                                          PageUrl = a != null ? a.PageUrl : null,
                                          CheckAdd = a != null ? a.CheckAdd : false,
                                          CheckEdit = a != null ? a.CheckEdit : false,
                                          CheckDelete = a != null ? a.CheckDelete : false,
                                          CheckPrint = a != null ? a.CheckPrint : false,
                                      }).ToListAsync();

                // Build tree structure
                var menuDict = allMenus.ToDictionary(m => m.MenuId, m => new MenuItem
                {
                    MenuId = m.MenuId,
                    Title = m.Title,
                    ParentId = m.ParentId,
                    OrderBy = m.OrderBy,
                    ControllerName = m.ControllerName,
                    ViewName = m.ViewName,
                    Icon = m.Icon,
                    IsActive = m.IsActive,
                    AccessCodeId = m.AccessCodeId,
                    AccessCodeName = m.AccessCodeName,
                    PageUrl = m.PageUrl,
                    CheckAdd = m.CheckAdd,
                    CheckEdit = m.CheckEdit,
                    CheckDelete = m.CheckDelete,
                    CheckPrint = m.CheckPrint,
                    Children = new List<MenuItem>()
                });

                List<MenuItem> rootItems = new List<MenuItem>();

                foreach (var item in menuDict.Values)
                {
                    if (string.IsNullOrEmpty(item.ParentId) || item.ParentId == "0")
                    {
                        rootItems.Add(item); // it's a root
                    }
                    else if (menuDict.ContainsKey(item.ParentId))
                    {
                        menuDict[item.ParentId].Children.Add(item); // add as child
                    }
                }

                return new CommonReturn
                {
                    Success = true,
                    Message = "Menu structure retrieved successfully",
                    Data = rootItems
                };
            }
            catch (Exception ex)
            {
                return new CommonReturn
                {
                    Success = false,
                    Message = "Error retrieving menu structure",
                    Data = ex.Message
                };
            }
        }


        public async Task<string> GetAccessName(string accessCodeId)
        {
            var result = await _AccessCodeRepository.All().Where(e => e.AccessCodeId == accessCodeId).Select(m => m.AccessCodeName).FirstOrDefaultAsync();
            return result;
        }


        public CommonReturn SaveAccessMenu(MenuAccessDto dto)
        {
            if (dto == null || dto.MenuAccessList == null || dto.MenuAccessList.Count == 0)
            {
                return new CommonReturn
                {
                    Success = false,
                    Message = "Invalid access menu data"
                };
            }

            _AccessCodeRepository.BeginTransactionAsync();
            try
            {
                var existingAccessCodes = _AccessCodeRepository.All().AsNoTracking().Where(e => e.AccessCodeId == dto.AccessCodeId).ToList();
                if (existingAccessCodes.Any())
                {
                    foreach (var item in existingAccessCodes)
                    {
                        _AccessCodeRepository.Delete(item);
                    }
                }

                List<CoreAccessCode> accessCodesList = new List<CoreAccessCode>();
                List<CoreAccessCode> accessParentsList = new List<CoreAccessCode>();

                // Add new access codes
                foreach (var item in dto.MenuAccessList)
                {
                    var menuData = _menuTabRepository.All().FirstOrDefault(e => e.MenuId == item.MenuId);

                    accessCodesList.Add(new CoreAccessCode
                    {
                        AccessCodeId = dto.AccessCodeId,
                        MenuId = item.MenuId,
                        AccessCodeName = dto.AccessCodeName,
                        TitleCheck = (!item.CanAdd && !item.CanEdit && !item.CanDelete && !item.CanPrint) ? false : true,
                        Title = menuData.Title,
                        PageUrl = menuData.ViewName,
                        CheckAdd = item.CanAdd,
                        CheckEdit = item.CanEdit,
                        CheckDelete = item.CanDelete,
                        CheckPrint = item.CanPrint,
                        ParentId = menuData.ParentId,
                        OrderBy = menuData.OrderBy,
                        MenuText = menuData.Title,
                        ControllerName = menuData.ControllerName,
                        ViewName = menuData.ViewName,
                        Icon = menuData.Icon,
                        IsActive = menuData.IsActive
                    });
                }

                // Save the new access codes to the database
                _AccessCodeRepository.Add(accessCodesList);

                PropagateParentTitleCheck(dto.AccessCodeId, accessCodesList);

                //_AccessCodeRepository.Update(accessParentsList);
                _AccessCodeRepository.CommitTransactionAsync();
                return new CommonReturn
                {
                    Success = true,
                    Message = "Access menu saved successfully"
                };
            }
            catch (Exception ex)
            {
                _AccessCodeRepository.RollbackTransactionAsync();
                throw;
            }
        }

        private void PropagateParentTitleCheck(string id, List<CoreAccessCode> allRecord)
        {
            var recordDict = allRecord.ToDictionary(r => r.MenuId);

            var toUpdate = new HashSet<string>();

            foreach(var record in allRecord.Where(r=>r.TitleCheck == true))
            {
                var parentId = record.ParentId?.Trim();
                while (!string.IsNullOrWhiteSpace(parentId) && parentId != "0")
                {
                    if (recordDict.TryGetValue(parentId, out var parent))
                    {
                        if (parent.TitleCheck != true)
                        {
                            parent.TitleCheck = true;
                            toUpdate.Add(parent.MenuId);
                        }
                        parentId = parent.ParentId;
                    }
                    else break;
                }
            }

            if (toUpdate.Any())
            {
                var toUpdateList = allRecord.Where(x => toUpdate.Contains(x.MenuId)).ToList();
                _AccessCodeRepository.Update(toUpdateList);
            }
        }

        #endregion

        public async Task<CommonReturn> DeleteAccessCodes(List<string> ids)
        {
            try
            {
                var toDelete = _AccessCodeRepository.All().AsNoTracking()
                    .Where(e => ids.Contains(e.AccessCodeId))
                    .ToList();

                if (!toDelete.Any())
                    return new CommonReturn { Success = false, Message = "No matching records found." };

                _AccessCodeRepository.Delete(toDelete);

                return new CommonReturn { Success = true, Message = "Deleted Successsfully." };
            }
            catch (Exception ex) 
            {
                return new CommonReturn { Success = false, Message = ex.Message };
            }

        }

        #region Delete Multiple Menu

        public CommonReturn DeleteMultipleMenu(List<int> ids)
        {
            try
            {
                if (ids == null || ids.Count == 0)
                {
                    return new CommonReturn
                    {
                        Success = false,
                        Message = "Invalid menu IDs"
                    };
                }

                List<CoreMenuTab2> accessChildList = new List<CoreMenuTab2>();

                foreach (var id in ids)
                {
                    var menuData = _menuTabRepository.GetById(id);

                    var childChk = _menuTabRepository.All().FirstOrDefault(e => e.ParentId == menuData.MenuId);

                    if (childChk != null)
                    {
                        accessChildList.Add(childChk);

                        var gChildChk = _menuTabRepository.All().FirstOrDefault(m => m.ParentId == childChk.MenuId);

                        if (gChildChk != null)
                        {
                            accessChildList.Add(gChildChk);

                            var gGChildChk = _menuTabRepository.All().FirstOrDefault(m => m.ParentId == gChildChk.MenuId);

                            if (gGChildChk != null)
                            {
                                accessChildList.Add(gGChildChk);
                            }
                        }
                    }



                    if (menuData != null)
                    {
                        _menuTabRepository.Delete(menuData);
                    }
                }

                foreach (var item in accessChildList)
                {
                    var menuData = _menuTabRepository.All().FirstOrDefault(e => e.AutoId == item.AutoId);

                    if (menuData != null)
                    {
                        _menuTabRepository.Delete(item);
                    }
                }
                return new CommonReturn
                {
                    Success = true,
                    Message = "Menus deleted successfully"
                };
            }
            catch (Exception)
            {
                throw;
            }

        }

        #endregion

        public async Task<CommonReturn> GetAccessCodeMenuOld()
        {
            try
            {
                var allMenus = await (from m in _menuTabRepository.All()
                                      join a in _AccessCodeRepository.All()
                                        on m.MenuId equals a.MenuId into accessGroup
                                      from a in accessGroup.DefaultIfEmpty()
                                      select new
                                      {
                                          m.MenuId,
                                          m.Title,
                                          m.ParentId,
                                          m.OrderBy,
                                          m.ControllerName,
                                          m.ViewName,
                                          m.Icon,
                                          m.IsActive,


                                          AccessCodeId = a != null ? a.AccessCodeId : null,
                                          AccessCodeName = a != null ? a.AccessCodeName : null,
                                          //  TitleCheck = a != null ? a.TitleCheck : null,
                                          PageUrl = a != null ? a.PageUrl : null,
                                          CheckAdd = a != null ? a.CheckAdd : false,
                                          CheckEdit = a != null ? a.CheckEdit : false,
                                          CheckDelete = a != null ? a.CheckDelete : false,
                                          CheckPrint = a != null ? a.CheckPrint : false,
                                      }).ToListAsync();

                // Step 1: Separate parent and child menus
                var flatList = new List<object>();

                var parents = allMenus.Where(x => x.ParentId == null || x.ParentId == "0")
                                      .OrderBy(x => x.OrderBy)
                                      .ToList();

                foreach (var parent in parents)
                {
                    flatList.Add(new
                    {
                        IsParent = true,
                        DisplayTitle = parent.Title,
                        parent.MenuId,
                        parent.ControllerName,
                        parent.ViewName,
                        parent.Icon,
                        parent.IsActive,
                        parent.AccessCodeId,
                        parent.AccessCodeName,
                        //  parent.TitleCheck,
                        parent.PageUrl,
                        parent.CheckAdd,
                        parent.CheckEdit,
                        parent.CheckDelete,
                        parent.CheckPrint
                    });

                    var children = allMenus.Where(x => x.ParentId == parent.MenuId)
                                           .OrderBy(x => x.OrderBy)
                                           .ToList();

                    foreach (var child in children)
                    {
                        flatList.Add(new
                        {
                            IsParent = false,
                            DisplayTitle = $">> {child.Title}",
                            child.MenuId,
                            child.ControllerName,
                            child.ViewName,
                            child.Icon,
                            child.IsActive,
                            child.AccessCodeId,
                            child.AccessCodeName,
                            //   child.TitleCheck,
                            child.PageUrl,
                            child.CheckAdd,
                            child.CheckEdit,
                            child.CheckDelete,
                            child.CheckPrint
                        });
                    }
                }

                return new CommonReturn
                {
                    Success = true,
                    Message = "Menu structure retrieved successfully",
                    Data = flatList
                };
            }
            catch (Exception ex)
            {
                return new CommonReturn
                {
                    Success = false,
                    Message = "Error retrieving menu structure",
                    Data = ex.Message
                };
            }
        }



        public async Task<CommonReturn> GetAccessCodeMenu2()
        {
            try
            {
                var allMenus = await (from m in _menuTabRepository.All()
                                      join a in _AccessCodeRepository.All()
                                        on m.MenuId equals a.MenuId into accessGroup
                                      from a in accessGroup.DefaultIfEmpty()
                                      select new
                                      {
                                          m.MenuId,
                                          m.Title,
                                          m.ParentId,
                                          m.OrderBy,
                                          m.ControllerName,
                                          m.ViewName,
                                          m.Icon,
                                          m.IsActive,

                                          AccessCodeId = a != null ? a.AccessCodeId : null,
                                          AccessCodeName = a != null ? a.AccessCodeName : null,
                                          //  TitleCheck = a != null ? a.TitleCheck : null,
                                          PageUrl = a != null ? a.PageUrl : null,
                                          CheckAdd = a != null ? a.CheckAdd : false,
                                          CheckEdit = a != null ? a.CheckEdit : false,
                                          CheckDelete = a != null ? a.CheckDelete : false,
                                          CheckPrint = a != null ? a.CheckPrint : false,
                                      }).ToListAsync();

                var grouped = allMenus
                    .GroupBy(x => x.ParentId ?? "0")
                    .Select(group => new
                    {
                        ParentId = group.Key,
                        ParentName = allMenus
                            .FirstOrDefault(p => p.MenuId == group.Key)?.Title ?? "Root",
                        Children = group
                            .OrderBy(x => x.OrderBy)
                            .ToList()
                    })
                    .ToList();

                return new CommonReturn
                {
                    Success = true,
                    Message = "Access codes retrieved successfully",
                    Data = grouped
                };
            }
            catch (Exception ex)
            {
                // Better error handling
                return new CommonReturn
                {
                    Success = false,
                    Message = "Error occurred while retrieving access codes.",
                    Data = ex.Message
                };
            }
        }



        public async Task<CommonReturn> GetParentsList()
        {
            return await Task.Run(() =>
            {
                var menuTabs = _menuTabRepository.GetAll().Where(x => x.ParentId == "0").ToList();
                return new CommonReturn
                {
                    Success = true,
                    Message = "Menu tabs retrieved successfully",
                    Data = menuTabs
                };
            });
        }

        public async Task<CommonReturn> SaveMenu(MenuTabPostViewModel model)
        {
            try
            {
                await _menuTabRepository.BeginTransactionAsync();
                if (model == null)
                {
                    return new CommonReturn
                    {
                        Success = false,
                        Message = "Invalid menu tab data"
                    };
                }

                CoreMenuTab2 coreMenuTab2 = new CoreMenuTab2();

                if (model.ParentId == "")
                {
                    model.ParentId = null;
                }
                if (model.GrandChildId == "")
                {
                    model.GrandChildId = null;
                }
                if (model.ChildId == "")
                {
                    model.ChildId = null;
                }


                if (model.ParentId == null && model.ChildId == null && model.GrandChildId == null)
                {
                    coreMenuTab2.ParentId = "0";
                }
                else if (model.ParentId != null && model.ChildId == null && model.GrandChildId == null)
                {
                    coreMenuTab2.ParentId = model.ParentId;
                }
                else if (model.ParentId != null && model.ChildId != null && model.GrandChildId == null)
                {
                    coreMenuTab2.ParentId = model.ChildId;
                }
                else if (model.ParentId != null && model.ChildId != null && model.GrandChildId != null)
                {
                    coreMenuTab2.ParentId = model.GrandChildId;
                }

                coreMenuTab2.MenuId = model.MenuId;
                coreMenuTab2.Title = model.Title;
                coreMenuTab2.ControllerName = model.ControllerName;
                coreMenuTab2.TableName = model.TableName;
                coreMenuTab2.OrderBy = model.OrderBy;
                coreMenuTab2.ViewName = model.ViewName;
                coreMenuTab2.IsActive = model.IsActive;
                coreMenuTab2.Icon = model.Icon;




                await _menuTabRepository.AddAsync(coreMenuTab2);
                await _menuTabRepository.CommitTransactionAsync();
                return new CommonReturn
                {
                    Success = true,
                    Message = "Menu tab saved successfully",
                    Data = model
                };
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<CommonReturn> UpdateAsync(MenuTabPostViewModel model)
        {
            try
            {
                await _menuTabRepository.BeginTransactionAsync();
                if (model == null)
                {
                    return new CommonReturn
                    {
                        Success = false,
                        Message = "Invalid menu tab data"
                    };
                }

                var existingMenuTab = await _menuTabRepository.GetByIdAsync(model.AutoId);

                if (existingMenuTab == null)
                {
                    return new CommonReturn
                    {
                        Success = false,
                        Message = "Menu tab not found"
                    };
                }


                existingMenuTab.Title = model.Title;
                existingMenuTab.ControllerName = model.ControllerName;
                existingMenuTab.TableName = model.TableName;
                existingMenuTab.ViewName = model.ViewName;
                existingMenuTab.IsActive = model.IsActive;
                existingMenuTab.Icon = model.Icon;




                await _menuTabRepository.UpdateAsync(existingMenuTab);

                await _menuTabRepository.CommitTransactionAsync();
                return new CommonReturn
                {
                    Success = true,
                    Message = "Menu tab saved successfully",
                    Data = model
                };
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> UpdateMenuOrder(List<MenuOrderUpdateDto> orderedMenus)
        {

            try
            {
                await _menuTabRepository.BeginTransactionAsync();
                foreach (var menu in orderedMenus)
                {
                    var menuToUpdate = await _menuTabRepository.GetByIdAsync(menu.AutoId);
                    if (menuToUpdate != null)
                    {
                        menuToUpdate.OrderBy = (int)menu.OrderBy;
                        await _menuTabRepository.UpdateAsync(menuToUpdate);
                    }
                }
                await _menuTabRepository.CommitTransactionAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public async Task<CommonReturn> SaveChangeOrder(List<MenuOrderUpdateDto> orderData)
        {
            try
            {
                List<CoreMenuTab2> ListMenu = new List<CoreMenuTab2>();

                foreach (var item in orderData)
                {
                    var menu = await _menuTabRepository.All().FirstOrDefaultAsync(e => e.AutoId == item.AutoId);
                    var menu14 = await _menuTabRepository.All().Where(e => e.AutoId == item.AutoId).FirstOrDefaultAsync();

                    menu.OrderBy = (int)item.OrderBy;
                    menu.ParentId = item.ParentId;

                    ListMenu.Add(menu);
                }

                await _menuTabRepository.UpdateRangeAsync(ListMenu);

                return new CommonReturn
                {
                    Success = true,
                    Message = "successfull"
                };
            }
            catch (Exception)
            {

                throw;
            }

        }

        public async Task<int> GetParentsListCount()
        {

            var menuTabs = await _menuTabRepository.All().Where(x => x.ParentId == "0").CountAsync();

            return menuTabs++;


        }

        public async Task<int> GetOrderCountByParent(string parentId)
        {
            var menuTabs = await _menuTabRepository.All().Where(x => x.ParentId == parentId).CountAsync();

            return menuTabs++;
        }

        public async Task<int> GetOrderCountByParentChild(string parentId, string childId)
        {
            var menuTabs = await _menuTabRepository.All().Where(x => x.ParentId == childId).CountAsync(); // x.ParentId == parentId &&

            return menuTabs++;
        }

        public async Task<int> GetOrderCountByParentChildGchild(string parentId, string childId, string grandChildId)
        {
            var menuTabs = await _menuTabRepository.All().Where(x => x.ParentId == grandChildId).CountAsync(); //x.ParentId == parentId && x.ParentId == childId &&

            return menuTabs++;
        }

        public async Task<CommonReturn> GetChildByParentId(string parentId)
        {
            var menuTabs = await _menuTabRepository.All().Where(x => x.ParentId == parentId).ToListAsync();

            return new CommonReturn
            {
                Success = true,
                Message = "Menu tabs retrieved successfully",
                Data = menuTabs
            };
        }

        public async Task<CommonReturn> GetGChildByChildId(string childId)
        {
            var menuTabs = await _menuTabRepository.All().Where(x => x.ParentId == childId).ToListAsync();

            return new CommonReturn
            {
                Success = true,
                Message = "Menu tabs retrieved successfully",
                Data = menuTabs
            };
        }


    }
}

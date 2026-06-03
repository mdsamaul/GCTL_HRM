using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.ViewModels.Common;
using GCTL.Core.ViewModels.MenuTab;
using GCTL.Data.Models;

namespace GCTL.Service.MenuTab
{
    public interface IMenuTabService
    {
        CommonReturn DeleteMultipleMenu(List<int> ids);
        Task<CommonReturn> GetAccessCodeMenu(string accessCodeId);
        Task<CommonReturn> GetAccessListTable();
        Task<string> GetAccessName(string accessCodeId);
        Task<CommonReturn> GetChildByParentId(string parentId);
        Task<CommonReturn> GetGChildByChildId(string childId);
        Task<int> GetOrderCountByParent(string parentId);
        Task<int> GetOrderCountByParentChild(string parentId, string childId);
        Task<int> GetOrderCountByParentChildGchild(string parentId, string childId, string grandChildId);
        Task<CommonReturn> GetParentsList();
        Task<int> GetParentsListCount();
        CommonReturn SaveAccessMenu(MenuAccessDto dto);
        Task<CommonReturn> SaveChangeOrder(List<MenuOrderUpdateDto> orderData);
        Task<CommonReturn> SaveMenu(MenuTabPostViewModel model);
        Task<CommonReturn> UpdateAsync(MenuTabPostViewModel model);
        Task<bool> UpdateMenuOrder(List<MenuOrderUpdateDto> orderedMenus);
    }
}

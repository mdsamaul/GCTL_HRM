using GCTL.Core.ViewModels.AccessCodes;
using GCTL.Core.ViewModels.Accounts;
using GCTL.Core;
using GCTL.Service.Users;
using Microsoft.AspNetCore.Mvc;
using GCTL.UI.Core.Extensions;

namespace GCTL.UI.Core.Controllers
{
    public class MenuController : BaseController
    {
        private readonly IAccessCodeService accessCodeService;

        public MenuController(IAccessCodeService accessCodeService)
        {
            this.accessCodeService = accessCodeService;
        }

        [HttpGet]
        public async Task<IActionResult> GreatGrandChildrenView(string grandchildId)
        {
            // Get the current user's access code
            var loginInfo = HttpContext.Session.Get<UserInfoViewModel>(nameof(ApplicationConstants.LoginSessionKey));
            if (loginInfo == null)
                return RedirectToAction("Login", "Account");

            // Get the full menu hierarchy
            var menuItems = await accessCodeService.GetAccessCodesAsync(loginInfo.AccessCode);

            // Find the grandchild and its great-grandchildren
            AccessCodeModel grandchild = FindGrandchildById(menuItems, grandchildId);

            if (grandchild == null)
                return NotFound();

            // Return a view with the grandchild (which contains great-grandchildren)
            return View(grandchild);
        }

        private AccessCodeModel FindGrandchildById(List<AccessCodeModel> menuItems, string grandchildId)
        {
            foreach (var parent in menuItems)
            {
                foreach (var child in parent.Children)
                {
                    var grandchild = child.Children.FirstOrDefault(gc => gc.MenuId == grandchildId);
                    if (grandchild != null)
                    {
                        return grandchild;
                    }
                }
            }
            return null;
        }
    }
}

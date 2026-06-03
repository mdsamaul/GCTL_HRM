using GCTL.Data.Models;
using GCTL.UI.Core.Extensions;
using GCTL.UI.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GCTL.Core;
using GCTL.Core.ViewModels.Accounts;
using GCTL.Service.Users;
using GCTL.Core.ViewModels.AccessCodes;

namespace GCTL.UI.Core.Components
{
    public class MenuViewComponent : ViewComponent
    {
        private readonly GCTL_ERP_DB_DatapathContext context;
        private readonly IAccessCodeService accessCodeService;

        public MenuViewComponent(GCTL_ERP_DB_DatapathContext context,
                                 IAccessCodeService accessCodeService)
        {
            this.context = context;
            this.accessCodeService = accessCodeService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<AccessCodeModel> result = new List<AccessCodeModel>();
            var loginInfo = HttpContext.Session.Get<UserInfoViewModel>(nameof(ApplicationConstants.LoginSessionKey));
            if (loginInfo != null)
            {
                result = await accessCodeService.GetAccessCodesAsync(loginInfo.AccessCode);
                return View(result);
            }
            else
            {
                return View(result);
            }
        }
    }
}

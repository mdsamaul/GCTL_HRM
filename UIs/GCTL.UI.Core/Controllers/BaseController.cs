using Dapper;
using GCTL.Core;
using GCTL.Core.DataTables;
using GCTL.Core.ViewModels.Accounts;
using GCTL.Core.ViewModels.HRLettersReportViewModel;
using GCTL.Data.Models;
using GCTL.UI.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace GCTL.UI.Core.Controllers
{
    public abstract class BaseController : Controller
    {
        protected GCTL_ERP_DB_DatapathContext context =>
     HttpContext.RequestServices.GetService(typeof(GCTL_ERP_DB_DatapathContext)) as GCTL_ERP_DB_DatapathContext;

        public UserInfoViewModel LoginInfo => GetCurrentSession();

        private UserInfoViewModel GetCurrentSession()
        {
            return HttpContext.Session.Get<UserInfoViewModel>(nameof(ApplicationConstants.LoginSessionKey));
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (LoginInfo == null)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Accounts",
                    action = "Login"
                }));
            }
            else
            {
                ViewBag.LoginUser = LoginInfo.Username;
            }

            base.OnActionExecuting(context);
        }

        protected OkObjectResult DataTablesResult<T>(PagedList<T> paginatedItems)
        {
            return Ok(new
            {
                recordsTotal = paginatedItems.TotalCount,
                recordsFiltered = paginatedItems.TotalCount,
                data = paginatedItems
            });
        }

        protected async Task<FullEmployeeDetailsGetByIdViewModel?> AllInfoEmployeeById(string employeeCode)
        {
            try
            {
                var connectionString = context.Database.GetConnectionString();

                await using var conn = new SqlConnection(connectionString);

                return await conn.QueryFirstOrDefaultAsync<FullEmployeeDetailsGetByIdViewModel>(
                    "FullEmployeeDetailsGetByid",
                    new { EmployeeCode = employeeCode },
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}

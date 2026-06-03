using GCTL.Data.Models;
using GCTL.UI.Core.Extensions;
using GCTL.UI.Core.ViewModels;
using Microsoft.AspNetCore.Mvc;
using GCTL.Core;
using System.Net.NetworkInformation;
using GCTL.Core.ViewModels.Accounts;
using GCTL.Core.Helpers;
using System.Web;

namespace GCTL.UI.Core.Controllers
{
    public class AccountsController : Controller
    {
        private readonly GCTL_ERP_DB_DatapathContext context;
        public AccountsController(GCTL_ERP_DB_DatapathContext context)
        {
            this.context = context;
        }

        [Route("")]
        public ActionResult Login(string returnUrl)
        {
            var userInfo = HttpContext.Session.Get<UserInfoViewModel>(nameof(ApplicationConstants.LoginSessionKey));
            if (userInfo != null && returnUrl != null)
            {
                Uri uri = new Uri("https://localhost" + returnUrl);
                var queryParams = HttpUtility.ParseQueryString(uri.Query);


                string leaveId = queryParams["leaveId"];
                string empId = queryParams["empId"];

                if (empId == userInfo.EmployeeId)
                {
                    return Redirect(returnUrl);
                }

                //return Redirect(returnUrl);
            }


            ViewBag.ReturnUrl = returnUrl;
            return View();

        }

        [Route("")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginPageViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                HttpContext.Session.Set<UserInfoViewModel>(nameof(ApplicationConstants.LoginSessionKey), default);

                var user = context.CoreUserInfo
                    .Where(a => a.Username.Equals(model.Username)).FirstOrDefault();
                if (user != null)
                {
                    string password = "";
                    try
                    {
                        new PXLibrary.PXlibrary().PXDEcode(ref password, user.Password);
                    }
                    catch (Exception)
                    {
                        password = user.Password;
                    }

                    if (model.Password.Equals(password))
                    {
                        HttpContext.Session.Set(nameof(ApplicationConstants.LoginSessionKey), new UserInfoViewModel
                        {
                            EmployeeId = user.EmployeeId,
                            CompanyCode = user.AccessPermissionCompanyCode,
                            Role = (DefaultRoles)Enum.Parse(typeof(DefaultRoles), user.Role, true),

                            Username = user.Username,
                            AccessCode = user.AccessCode,

                            IPAddress = GetLocalIP(),
                            MacAddress = GetMacAddress()
                        });


                        if (!string.IsNullOrEmpty(returnUrl))
                        {

                            return Redirect(returnUrl);
                        }




                        return RedirectToAction("Index", "Dashboard");
                    }
                    else
                    {
                        TempData["errorMessage"] = "Invalid User Name or Password";
                        //ViewBag.errorMessage = "Error: " + ex.Message + " - " + ex.InnerException;
                        return View(model);
                    }
                }
                else
                {
                    TempData["errorMessage"] = "Invalid User Name or Password";
                    //ViewBag.errorMessage = "Error: " + ex.Message + " - " + ex.InnerException;
                    return View(model);
                }
            }
            return View(model);

        }
        public ActionResult LogOut()
        {
            HttpContext.Session.Set<UserInfoViewModel>(nameof(ApplicationConstants.LoginSessionKey), default);
            HttpContext.Session.Clear(); // it will clear the session at the end of request
            return RedirectToAction("Login", "Accounts");
        }


        public string GetLocalIP()
        {
            string ipAddress = string.Empty;
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up && n.NetworkInterfaceType != NetworkInterfaceType.Loopback);

            foreach (var networkInterface in networkInterfaces)
            {
                var properties = networkInterface.GetIPProperties();
                var ipv4Address = properties.UnicastAddresses.FirstOrDefault(ip => ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

                if (ipv4Address != null)
                {
                    ipAddress = ipv4Address.Address.ToString();
                    break;
                }
            }

            return ipAddress;
        }


        public string GetMacAddress()
        {
            string macAddress = string.Empty;
            var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
           .Where(n => n.OperationalStatus == OperationalStatus.Up && n.NetworkInterfaceType != NetworkInterfaceType.Loopback);
            foreach (var networkInterface in networkInterfaces)
            {
                macAddress = networkInterface.GetPhysicalAddress().ToString();
                if (!string.IsNullOrEmpty(macAddress))
                {
                    break;
                }
            }

            return macAddress;
        }

        
    }
}
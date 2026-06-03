
using Microsoft.AspNetCore.Mvc;

using System.Diagnostics;
using System.Web.Http;

namespace GCTL.UI.Core.Controllers
{
    public class ConsoleAppController : BaseController
    {

        public ConsoleAppController()
        {
        }
        public IActionResult Index()
        {
            // Specify the path to your console application
            string consoleAppPath = @"C:\Farhad\RestartIIS\CrmCustomerMail\bin\Debug\CrmCustomerMail.exe";

            // Create a process start info

       

            System.Diagnostics.ProcessStartInfo info =
              new System.Diagnostics.ProcessStartInfo(consoleAppPath, "");

            //System.Diagnostics.Process p = System.Diagnostics.Process.Start(info);
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo = info;

            //while (p.Responding)
            while (!IsProcessOpen("abc"))
            //Here we can check for the application is running priorly or not
            {
                p.Start();
                //This line gets the start time of the process
                string startTime = p.StartTime.ToString();
               // Response.Write(startTime);
            }

            string endTime = p.ExitTime.ToString();
            //This line gets the end time of the process

           // Response.Write(endTime);
            return RedirectToAction("Dashboard", "Home");
        }

        public bool IsProcessOpen(string name)
        {
            //here we're going to get a list of all running processes on

            foreach (Process clsProcess in Process.GetProcesses())
            {
                if (clsProcess.ProcessName.Contains(name))
                {
                    return true;
                }
            }

            return false;
        }

    }
}
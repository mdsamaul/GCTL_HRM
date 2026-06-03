using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.Email
{
    //TODO: Sf
    public class EmailSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string MailFrom { get; set; }
        public bool EnableSsl { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}

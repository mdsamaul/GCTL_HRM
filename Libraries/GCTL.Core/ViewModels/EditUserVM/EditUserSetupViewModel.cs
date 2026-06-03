using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.EditUserVM
{
    public class EditUserSetupViewModel : BaseViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SecureCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmployeeId { get; set; }
        public DateTime? Dob { get; set; }
        public string OffPhone { get; set; }
        public string PerPhone { get; set; }
        public string OffEmail { get; set; }
        public string PerEmail { get; set; }
        public string WorkStation { get; set; }
        public string Regulation { get; set; }
    }

    public class EditUserGridViewModel : BaseViewModel
    {
        public int UserId { get; set; } //PK 
        public string Username { get; set; }
        public string FullName { get; set; }
        public string UserType { get; set; }
        public string EntryDate { get; set; }
    }
}

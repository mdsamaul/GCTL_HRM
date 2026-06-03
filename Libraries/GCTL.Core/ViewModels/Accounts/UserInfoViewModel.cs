using GCTL.Core.Helpers;

namespace GCTL.Core.ViewModels.Accounts
{
    public class UserInfoViewModel
    {
        public string Username { get; set; }
        public string AccessCode { get; set; }
        public string EmployeeId { get; set; }
        public string CompanyCode { get; set; }
        public DefaultRoles Role { get; set; }
        public string IPAddress { get; set; }
        public string MacAddress { get; set; }
    }
}

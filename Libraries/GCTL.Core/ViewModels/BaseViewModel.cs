using GCTL.Core.Helpers;

namespace GCTL.Core.ViewModels
{
    public class BaseViewModel
    {
        public decimal Id { get; set; }
        public string Code { get; set; }
        public int? SerialNo { get; set; }
        public string PageTitle { get; set; }
        public string FormTitle { get; set; }
        public string GridTitle { get; set; }
        public string Breadcrumb { get; set; }
        public string PageUrl { get; set; }
        public string AddUrl { get; set; }

        // Audit
        public string Luser { get; set; }
       // public string EmployeeId { get; set; }
        public string UserInfoEmployeeId { get; set; }
        public DateTime? Ldate { get; set; }
        public string Lip { get; set; }
        public string Lmac { get; set; }
        public DateTime? ModifyDate { get; set; }

        public DefaultRoles Role { get; set; }
        public bool IsAdmin { get; set; } 
    }
}

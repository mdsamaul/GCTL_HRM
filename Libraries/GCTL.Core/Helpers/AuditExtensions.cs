using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.Accounts;

namespace GCTL.Core.Helpers
{
    public static class AuditExtensions
    {
        public static BaseViewModel ToAudit(this BaseViewModel model, UserInfoViewModel info, bool isUpdate = false)
        {
            model.Lip = info.IPAddress;
            model.Lmac = info.MacAddress;
            model.Luser = info.Username;
            model.UserInfoEmployeeId = info.EmployeeId;
            if (isUpdate)
                model.ModifyDate = DateTime.Now;
            else
                model.Ldate = DateTime.Now;

            return model;
        }
    }
}

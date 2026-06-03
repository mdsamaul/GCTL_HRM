using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.CompanyInfo;
using GCTL.Core.ViewModels.ContactPersonInfo;

namespace GCTL.UI.Core.ViewModels.ContactPersonInfo
{
    public class ContactPersonInfoPageViewModel : BaseViewModel
    {
        public ContactPersonInfoSetupViewModel Setup { get; set; } = new ContactPersonInfoSetupViewModel();
        public List<ContactPersonInfoSetupViewModel> ContactPersonList { get; set; } = new List<ContactPersonInfoSetupViewModel>();
    }
}

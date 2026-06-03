using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.EmailSettingsViewModel;

namespace GCTL.UI.Core.ViewModels.EmailSendingViewModel
{
    public class EmailSettingsDTO : BaseViewModel
    {
        public EmailRequestDTO model { get; set; } = new EmailRequestDTO();
    }
}

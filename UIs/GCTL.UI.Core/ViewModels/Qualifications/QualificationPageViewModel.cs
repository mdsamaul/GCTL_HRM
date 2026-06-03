using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.Qualifications;

namespace GCTL.UI.Core.ViewModels.Qualifications
{
    public class QualificationPageViewModel : BaseViewModel
    {
        public QualificationSetupViewModel Setup { get; set; } = new QualificationSetupViewModel();
    }
}

using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.Grades;

namespace GCTL.UI.Core.ViewModels.Grades
{
    public class GradesPageViewModel: BaseViewModel
    {
        public GradesSetupViewModel Setup { get; set; } = new GradesSetupViewModel();
        public List<GradesSetupViewModel> TableList { get; set; } = new List<GradesSetupViewModel>();
    }
}

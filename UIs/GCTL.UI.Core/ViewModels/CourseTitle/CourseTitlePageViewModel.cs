using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.CourseTitle;

namespace GCTL.UI.Core.ViewModels.CourseTitle
{
    public class CourseTitlePageViewModel: BaseViewModel
    {
        public CourseTitleSetupViewModel Setup { get; set; } = new CourseTitleSetupViewModel();
        public List<CourseTitleSetupViewModel> CourseTitleList { get; set; } = new List<CourseTitleSetupViewModel>();
    }
}

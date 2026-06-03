using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.InstructionInformation;
using GCTL.Core.ViewModels.SupplierType;

namespace GCTL.UI.Core.ViewModels.InstructionInformation
{
    public class InstructionInformationPageViewModel : BaseViewModel
    {
        public InstructionInformationSetupViewModel Setup { get; set; } = new InstructionInformationSetupViewModel();
        public List<InstructionInformationSetupViewModel> InstructionList { get; set; } = new List<InstructionInformationSetupViewModel>();
    }
}


using GCTL.Core.ViewModels;
using GCTL.Core.ViewModels.HrmEmployeeDocumentInfos;

namespace GCTL.UI.Core.ViewModels.HrmEmployeeDocumentInfos
{
    public class HrmEmployeeDocumentInfosPageViewModel:BaseViewModel
    {
        public HrmEmployeeDocumentInfosSetup Setup { get; set; }=new HrmEmployeeDocumentInfosSetup();
        public List<HrmEmployeeDocumentInfosSetup> TableListData { get; set; }=new List<HrmEmployeeDocumentInfosSetup>();
    }
}

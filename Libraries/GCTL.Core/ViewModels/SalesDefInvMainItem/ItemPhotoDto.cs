using Microsoft.AspNetCore.Http;

namespace GCTL.Core.ViewModels.SalesDefInvMainItem
{
    public class ItemPhotoDto : BaseViewModel
    {
        public int AutoId { get; set; }
        public string ItemID { get; set; }
        public IFormFile Photo { get; set; }
        public byte[]? PhotoBytes { get; set; }
        public string ImgType { get; set; }
        public long ImgSize { get; set; }
        public string CompanyCode { get; set; }
        public string EmployeeID { get; set; }
    }

}

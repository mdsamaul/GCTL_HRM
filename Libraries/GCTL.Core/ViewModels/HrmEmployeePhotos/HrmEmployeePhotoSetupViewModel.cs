using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.ViewModels.HrmEmployeePhotos
{
    public  class HrmEmployeePhotoSetupViewModel
    {
        public int AutoId { get; set; }
        public string EmployeeId { get; set; }
        public byte[] Photo { get; set; }
        public string ImgType { get; set; }
        public long ImgSize { get; set; }
    }
}

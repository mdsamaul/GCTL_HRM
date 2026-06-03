using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.ViewModels.HolidayTypes;

namespace GCTL.Service.HodayPdfServices
{
    public interface IHoidayPdfService
    {
        public byte[] CreatePdf(HRMDefHolidayTypeSetupViewModel model);
    }
}

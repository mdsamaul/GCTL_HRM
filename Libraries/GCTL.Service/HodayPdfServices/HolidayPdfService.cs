using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCTL.Core.Data;
using GCTL.Core.ViewModels.HolidayTypes;
using GCTL.Data.Models;

namespace GCTL.Service.HodayPdfServices
{
    public class HolidayPdfService : IHoidayPdfService
    {
        private readonly IRepository<HrmDefHolidayType> repository;

        public byte[] CreatePdf(HRMDefHolidayTypeSetupViewModel model)
        {
            throw new NotImplementedException();
        }

    }
}

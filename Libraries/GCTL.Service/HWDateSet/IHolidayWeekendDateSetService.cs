using GCTL.Core.ViewModels.HolidayWeekendDSet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HWDateSet
{
    public interface IHolidayWeekendDateSetService
    {
        Task<List<HolidayWeekendDateVM>> GetHolidayAndWeekendAsync(int year);
        void ClearCalendarCache(int year);
        void ClearAllCalendarCache();
     }
}

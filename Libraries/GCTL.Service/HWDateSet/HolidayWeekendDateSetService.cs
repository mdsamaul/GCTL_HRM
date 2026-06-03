using GCTL.Core.Data;
using GCTL.Core.ViewModels.HolidayWeekendDSet;
using GCTL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Service.HWDateSet
{
    public class HolidayWeekendDateSetService : IHolidayWeekendDateSetService
    {
        private readonly IRepository<HrmAtdHoliday> atdHolidayRepo;
        private readonly IRepository<HrmAtdCompanyWeekEnd> companyWeekEndRepo;
        private readonly IMemoryCache _cache;

        public HolidayWeekendDateSetService(IRepository<HrmAtdHoliday> atdHolidayRepo, IRepository<HrmAtdCompanyWeekEnd> companyWeekEndRepo, IMemoryCache cache)
        {
            this.atdHolidayRepo = atdHolidayRepo;
            this.companyWeekEndRepo = companyWeekEndRepo;
            _cache = cache;
        }

        public async Task<List<HolidayWeekendDateVM>> GetHolidayAndWeekendAsync(int year)
        {
            string cacheKey = $"CalendarData_{year}";

            return await _cache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1);
                entry.SlidingExpiration = TimeSpan.FromHours(3);
                entry.Priority = CacheItemPriority.Normal;
                return await GenerateCalanderData(year);
            });
        }

        private async Task<List<HolidayWeekendDateVM>> GenerateCalanderData(int year)
        {
            var result = new List<HolidayWeekendDateVM>();

            var holidays = await atdHolidayRepo.All()
                .Where(x => x.FromDate.Year == year)
                .ToListAsync();

            foreach (var h in holidays)
            {
                result.Add(new HolidayWeekendDateVM
                {
                    Date = h.FromDate.ToString("yyyy-MM-dd"),
                    Title = h.HolidayName,
                    Type = "holiday"
                });
            }

            var weekendCofigs = await companyWeekEndRepo.All()
                .Where(x => x.EffectiveDate.Year <= year)
                .OrderBy(x => x.EffectiveDate).ToListAsync();

            if (weekendCofigs.Any())
            {
                var start = new DateTime(year, 1, 1);
                var end = new DateTime(year, 12, 31);

                for (var date = start; date <= end; date = date.AddDays(1)) 
                {
                    var applicableConfig = weekendCofigs
                    .Where(x => x.EffectiveDate <= date)
                    .OrderByDescending(x => x.EffectiveDate)
                    .FirstOrDefault();

                    if (applicableConfig != null)
                    {
                        var weekendDays = applicableConfig.Weekend
                            .Split(',')
                            .Select(d => d.Trim())
                            .ToList();

                        if (weekendDays.Contains(date.DayOfWeek.ToString()))
                        {
                            result.Add(new HolidayWeekendDateVM
                            {
                                Date = date.ToString("yyyy-MM-dd"),
                                Title = "Weekend",
                                Type = "weekend"
                            });
                        }
                    }
                }
            }

            return result;
        }

        public void ClearCalendarCache(int year)
        {
            _cache.Remove($"CalendarData_{year}");
        }

        public void ClearAllCalendarCache()
        {
            int currentYear = DateTime.Now.Year;
            for(int year = currentYear -2; year <= currentYear+2; year++)
            {
                _cache.Remove($"CalendarData_{year}");
            }
        }
    }
}

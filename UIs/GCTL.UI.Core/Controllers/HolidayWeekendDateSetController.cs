using GCTL.Service.HWDateSet;
using Microsoft.AspNetCore.Mvc;

namespace GCTL.UI.Core.Controllers
{
    public class HolidayWeekendDateSetController : BaseController
    {
        private IHolidayWeekendDateSetService _service;

        public HolidayWeekendDateSetController(IHolidayWeekendDateSetService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetCalendarData(int year)
        {
            try
            {
                if (year < 2000 || year > 2100)
                {
                    return BadRequest(new { error = "Invalid year range (2000-2100)" });
                }

                var data = await _service.GetHolidayAndWeekendAsync(year);

                return Json(data);
            }
            catch (Exception)
            {
                return StatusCode(500, new { error = "Failed to load calendar data" });
            }
        }
    }
}

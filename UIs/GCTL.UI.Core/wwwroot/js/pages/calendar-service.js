const CalendarService = (function () {
    let calendarDataByYear = {};
    let activeRequests = {};

    function loadYear(year) {
        if (calendarDataByYear[year]) {
            return Promise.resolve(calendarDataByYear[year]);
        }

        if (activeRequests[year]) {
            return activeRequests[year];
        }

        const request = $.ajax({
            url: '/HolidayWeekendDateSet/GetCalendarData',
            type: 'GET',
            data: { year: year },
            cache: true
        }).then(function (data) {

            const yearData = {
                holidays: {},
                weekends: {}
            };

            $.each(data, function (_, item) {
                if (item.type === "holiday") {
                    yearData.holidays[item.date] = item.title;
                } else if (item.type === "weekend") {
                    yearData.weekends[item.date] = item.title;
                }
            });

            calendarDataByYear[year] = yearData;
            delete activeRequests[year];

            return yearData;

        }).catch(function (err) {
            console.error(`CalendarService: Failed to load data for year ${year}`, err);
            delete activeRequests[year];
            throw err;
        });

        activeRequests[year] = request;
        return request;
    }

    function getDateInfo(dateStr, year) {
        const yearData = calendarDataByYear[year];
        if (!yearData) return null;

        if (yearData.holidays[dateStr]) {
            return {
                type: 'holiday',
                title: yearData.holidays[dateStr]
            };
        }

        if (yearData.weekends[dateStr]) {
            return {
                type: 'weekend',
                title: yearData.weekends[dateStr]
            };
        }

        return null;
    }

    function createConfig(userConfig = {}) {
        return {
            dateFormat: "Y-m-d",
            altInput: true,
            altFormat: "d/m/Y",
            allowInput: true,
            disableMobile: true,
            onDayCreate: function (dObj, dStr, fp, dayElem) {
                const date = [
                    dayElem.dateObj.getFullYear(),
                    String(dayElem.dateObj.getMonth() + 1).padStart(2, '0'),
                    String(dayElem.dateObj.getDate()).padStart(2, '0')
                ].join('-');

                const year = dayElem.dateObj.getFullYear();
                const info = getDateInfo(date, year);

                if (info) {
                    dayElem.classList.add(info.type + "-day");
                    dayElem.title = info.title;
                }
            },
            onOpen: function (_, __, instance) {
                instance.redraw();
                setTimeout(() => hideEmptyRows(instance), 100);
            },

            onMonthChange: function (_, __, instance) {
                instance.redraw();
                setTimeout(() => hideEmptyRows(instance), 100);
            },

            onYearChange: function (_, __, instance) {
                const newYear = instance.currentYear;
                loadYear(newYear).then(() => {
                    instance.redraw();
                    setTimeout(() => hideEmptyRows(instance), 100);
                });
            },

            onReady: function (_, __, instance) {
                instance.altInput.placeholder = "dd/mm/yyyy";

                const currentYear = instance.currentYear || new Date().getFullYear();
                loadYear(currentYear).then(() => {
                    instance.redraw();
                    setTimeout(() => hideEmptyRows(instance), 0);
                });
            },

            ...userConfig
        };
    }

    function init() {
        const currentYear = new Date().getFullYear();
        loadYear(currentYear);
        console.log('CalendarService initialized');
    }

    function clearYearCache(year) {
        delete calendarDataByYear[year];
        // console.log(`CalendarService: Cache cleared for year ${year}`);
    }
    function hideEmptyRows(instance) {
        if (!instance || !instance.days) return;

        const days = instance.days.querySelectorAll('.flatpickr-day');
        const rows = [];
        let currentRow = [];

        days.forEach((day, i) => {
            currentRow.push(day);
            if ((i + 1) % 7 === 0) {
                rows.push(currentRow);
                currentRow = [];
            }
        });
        if (currentRow.length) rows.push(currentRow);

        let hiddenRowCount = 0;

        // Hide rows with only prev/next month days
        rows.forEach(row => {
            const allHidden = row.every(day =>
                day.classList.contains('prevMonthDay') ||
                day.classList.contains('nextMonthDay')
            );
            row.forEach(day => {
                if (allHidden) {
                    day.style.display = 'none';
                }
            });
            if (allHidden) hiddenRowCount++;
        });

        if (hiddenRowCount > 0) {
            const dayContainer = instance.days;
            const visibleDay = Array.from(days).find(d => d.style.display !== 'none');
            if (dayContainer && visibleDay) {
                instance._positionCalendar();
            }
        }
    }

    return {
        init: init,
        createConfig: createConfig,
        loadYear: loadYear,
        clearYearCache: clearYearCache
    };

})();

$(document).ready(function () {
    CalendarService.init();
});
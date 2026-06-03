using System.Globalization;
using System.Reflection;

namespace GCTL.Core.Helpers
{
    public static class DateTimeHelpers
    {
        public static string ToDateFormat(this DateTime date)
        {
            return date.Year > 1900 ? date.ToString(ApplicationConstants.DateFormat) : string.Empty;
        }

        public static string ToDateTimeFormat(this DateTime date)
        {
            return date.Year > 1900 ? date.ToString(ApplicationConstants.DateTimeFormat) : string.Empty;
        }

        public static DateTime ToDate(this string date)
        {
            if (!string.IsNullOrEmpty(date))
                return DateTime.ParseExact(date, ApplicationConstants.DateFormat, CultureInfo.InvariantCulture);

            return DateTime.MinValue.AddYears(1900);
        }

        public static DateTime? ToDateNullable(this string date)
        {
            if (!string.IsNullOrEmpty(date))
                return DateTime.ParseExact(date, ApplicationConstants.DateFormat, CultureInfo.InvariantCulture);

            return null;
        }

        public static DateTime ToFullDate(this string date)
        {
            DateTime dateTime = DateTime.MinValue;
            if (!string.IsNullOrEmpty(date))
            {
                dateTime = DateTime.ParseExact(date, ApplicationConstants.DateFormat, CultureInfo.InvariantCulture);
                dateTime = dateTime.AddDays(1).AddSeconds(-1);
                return dateTime;
            }

            return DateTime.MinValue.AddYears(1900);
        }

        public static DateTime ToDateTime(this string date)
        {
            if (!string.IsNullOrEmpty(date))
                return DateTime.ParseExact(date, ApplicationConstants.DateTimeFormat, CultureInfo.InvariantCulture);

            return DateTime.MinValue.AddYears(1900);
        }

        public static string CalculateReportDeliveryDate(int reportTime = 1, string reportUnit = "days", int deliveryHour = 19)
        {
            var now = DateTime.Now;
            var deliveryTime = now;
            if (reportUnit.ToLower().StartsWith("min"))
                deliveryTime = now.AddMinutes(reportTime);
            if (reportUnit.ToLower().StartsWith("hour"))
                deliveryTime = now.AddHours(reportTime);
            if (reportUnit.ToLower().StartsWith("days"))
                deliveryTime = now.AddDays(reportTime);

            if (deliveryTime.Hour > deliveryHour)
            {
                deliveryTime = deliveryTime.AddDays(1);
            }

            deliveryTime = new DateTime(deliveryTime.Year, deliveryTime.Month, deliveryTime.Day, deliveryHour, 0, 0);

            if (now.Hour > 12)
                deliveryTime = new DateTime(now.Year, now.Month, now.Day + 1, deliveryHour, 0, 0);

            return deliveryTime.ToString(ApplicationConstants.DateTimeFormat);
        }
    }
}

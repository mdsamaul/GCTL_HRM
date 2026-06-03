using System.ComponentModel;
using System.Globalization;

namespace GCTL.Core.Helpers
{
    public static class StringExtensions
    {
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static int AsInt(this string value)
        {
            return value.AsInt(0);
        }

        public static int AsInt(this string value, int defaultValue)
        {
            int result;
            return !int.TryParse(value, out result) ? defaultValue : result;
        }

        public static decimal AsDecimal(this string value)
        {
            return value.As<decimal>();
        }

        public static decimal AsDecimal(this string value, Decimal defaultValue)
        {
            return value.As(defaultValue);
        }

        public static float AsFloat(this string value)
        {
            return value.AsFloat(0.0f);
        }

        public static float AsFloat(this string value, float defaultValue)
        {
            float result;
            return !float.TryParse(value, out result) ? defaultValue : result;
        }

        public static DateTime AsDateTime(this string value)
        {
            return value.AsDateTime(new DateTime());
        }

        public static DateTime AsDateTime(this string value, DateTime defaultValue)
        {
            DateTime result;
            return !DateTime.TryParse(value, out result) ? defaultValue : result;
        }

        public static TValue As<TValue>(this string value)
        {
            return value.As(default(TValue));
        }

        public static bool AsBool(this string value)
        {
            return value.AsBool(false);
        }

        public static bool AsBool(this string value, bool defaultValue)
        {
            bool result;
            return !bool.TryParse(value, out result) ? defaultValue : result;
        }

        public static TValue As<TValue>(this string value, TValue defaultValue)
        {
            try
            {
                TypeConverter converter1 = TypeDescriptor.GetConverter(typeof(TValue));
                if (converter1.CanConvertFrom(typeof(string)))
                    return (TValue)converter1.ConvertFrom((object)value);
                TypeConverter converter2 = TypeDescriptor.GetConverter(typeof(string));
                if (converter2.CanConvertTo(typeof(TValue)))
                    return (TValue)converter2.ConvertTo((object)value, typeof(TValue));
            }
            catch
            {
            }
            return defaultValue;
        }

        public static bool IsBool(this string value)
        {
            return bool.TryParse(value, out bool _);
        }

        public static bool IsNumber(this string value)
        {
            return IsInt(value) || IsDecimal(value) || IsFloat(value);
        }

        public static bool IsInt(this string value)
        {
            return int.TryParse(value, out int _);
        }

        public static bool IsDecimal(this string value)
        {
            return value.Is<decimal>();
        }

        public static bool IsFloat(this string value)
        {
            return float.TryParse(value, out float _);
        }

        public static bool IsDateTime(this string value)
        {
            return DateTime.TryParse(value, out DateTime _);
        }

        public static bool Is<TValue>(this string value)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(typeof(TValue));
            if (converter != null)
            {
                try
                {
                    if (value != null)
                    {
                        if (!converter.CanConvertFrom(null, value.GetType()))
                            goto label_5;
                    }
                    converter.ConvertFrom(null, CultureInfo.CurrentCulture, value);
                    return true;
                }
                catch
                {
                }
            }
        label_5:
            return false;
        }
    }
}

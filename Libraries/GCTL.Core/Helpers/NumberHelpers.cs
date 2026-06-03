namespace GCTL.Core.Helpers
{
    public static class NumberHelpers
    {
        public static decimal? ToNumber(this decimal? amount)
        {
            if (amount.HasValue && amount.Value > 0)
            {
                if ((amount.Value % 1) > 0)
                    return amount.Value;
                else
                    return (int)amount.Value;
            }
            return null;
        }

        public static string ToNumberFormat(this decimal? amount)
        {
            if (amount.HasValue && amount.Value > 0)
            {
                if ((amount.Value % 1) > 0)
                    return amount.Value.ToString();
                else
                    return ((int)amount.Value).ToString();
            }
            return "-";
        }

        public static decimal? ToDecimal(this string amount)
        {
            decimal result = decimal.Zero;
            if (decimal.TryParse(amount, out result) && result > 0)
                return result;

            return null;
        }
    }
}

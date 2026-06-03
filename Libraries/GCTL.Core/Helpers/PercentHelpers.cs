namespace GCTL.Core.Helpers
{
    public static class PercentHelpers
    {
        public static string ToPercent(this decimal amount)
        {
            if (amount > 0)
            {
                if ((amount % 1) > 0)
                    return $"{amount} %";
                else
                    return $"{(int)amount} %";
            }
            else
                return "-";
        }

        public static string ToPercent(this decimal? amount)
        {
            if (amount.HasValue)
            {
                if ((amount % 1) > 0)
                    return $"{amount} %";
                else
                    return $"{(int)amount} %";
            }
            else
                return "-";
        }
    }
}

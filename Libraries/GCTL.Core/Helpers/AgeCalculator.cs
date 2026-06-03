using System.Text;

namespace GCTL.Core.Helpers
{
    public static class AgeCalculator
    {
        public static string CalculateAge(int? ageYear, int? ageMonth, int? ageDay)
        {
            StringBuilder age = new StringBuilder();
            if (ageYear.HasValue)
            {
                age.Append($"{ageYear}Y ");
            }

            if (ageMonth.HasValue)
            {
                age.Append($"{ageMonth}M ");
            }

            if (ageDay.HasValue)
            {
                age.Append($"{ageDay}D");
            }

            return age.ToString();
        }
        public static string CalculateAge2(int? ageYear, int? ageMonth, int? ageDay)
        {
            StringBuilder age = new StringBuilder();
            if (ageYear.HasValue)
            {
                if (ageYear.Value > 1)
                {
                    age.Append($"{ageYear} years ");
                }
                else
                {
                    age.Append($"{ageYear} years ");
                }
            }

            if (ageMonth.HasValue)
            {
                if (ageMonth.Value > 1)
                {
                    age.Append($"{ageMonth} months ");
                }
                else
                {
                    age.Append($"{ageMonth} month ");
                }
            }

            if (ageDay.HasValue)
            {
                if (ageDay.Value > 1)
                {
                    age.Append($"{ageDay} days");
                }
                else
                {
                    age.Append($"{ageDay} day");
                }
            }

            return age.ToString();
        }
    }
}

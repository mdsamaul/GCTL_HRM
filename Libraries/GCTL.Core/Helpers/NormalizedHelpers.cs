using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GCTL.Core.Helpers
{
    
    public static class NormalizedHelpers
    {
        public static string ToNormalizedTableKey(this string s) =>
            string.IsNullOrWhiteSpace(s)
                ? string.Empty
                : s.Replace("_", "").Trim().ToLowerInvariant();

        public static string TrimSafe(this string? s) => s?.Trim() ?? string.Empty;
    }
}

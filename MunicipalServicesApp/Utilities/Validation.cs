using System;
using System.Collections.Generic;
using System.Linq;

namespace MunicipalServicesApp.Utilities
{
    /// <summary>
    /// Validation helpers.
    /// </summary>
    public static class Validation
    {
        /// <summary>
        /// Checks basic "required" constraints:
        /// </summary>
        public static bool Required(string value, int minLen = 2, int maxLen = 200)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;

            var trimmed = value.Trim();
            return trimmed.Length >= minLen && trimmed.Length <= maxLen;
        }

        /// <summary>
        /// Ensures the provided category exists in the supplied allowed set.
        /// Ignores case to keep UX forgiving.
        /// </summary>
        public static bool ValidCategory(string category, IEnumerable<string> allowed)
        {
            if (string.IsNullOrWhiteSpace(category) || allowed == null) return false;

            return allowed.Any(a => string.Equals(a, category, StringComparison.OrdinalIgnoreCase));
        }
    }
}

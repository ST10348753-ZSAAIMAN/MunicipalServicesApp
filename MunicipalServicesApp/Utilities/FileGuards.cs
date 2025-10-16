using System.IO;

namespace MunicipalServicesApp.Utilities
{
    /// <summary>
    /// Demonstrates safe recursion.
    /// </summary>
    public static class FileGuards
    {
        /// <summary>
        /// Recursively checks subdirectory depth up to maxDepth.
        /// </summary>
        public static bool SafeDirectoryDepth(string path, int maxDepth)
        {
            if (string.IsNullOrWhiteSpace(path) || maxDepth < 0) return false;

            if (!Directory.Exists(path)) return true;

            try
            {
                return CheckDepth(path, currentDepth: 0, maxDepth: maxDepth);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Recursive worker. If current depth exceeds maxDepth, fail.
        /// Otherwise recurse into each subdirectory.
        /// </summary>
        private static bool CheckDepth(string current, int currentDepth, int maxDepth)
        {
            if (currentDepth > maxDepth) return false;

            var subDirs = Directory.GetDirectories(current);
            foreach (var d in subDirs)
            {
                if (!CheckDepth(d, currentDepth + 1, maxDepth))
                    return false;
            }

            return true;
        }
    }
}

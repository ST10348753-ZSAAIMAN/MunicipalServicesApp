using System.Globalization;
using System.Threading;

namespace MunicipalServicesApp.Utilities
{
    public static class CultureInit
    {
        /// <summary>
        /// Sets the process/UI culture for consistent SA formatting.
        /// </summary>
        public static void SetCulture(string cultureName)
        {
            var culture = new CultureInfo(cultureName);

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }
    }
}

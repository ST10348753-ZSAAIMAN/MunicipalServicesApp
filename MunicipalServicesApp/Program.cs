using System;
using System.Windows.Forms;
using MunicipalServicesApp.Utilities;
using MunicipalServicesApp.Services;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp
{
    /// <summary>
    /// Application entry point.
    /// </summary>
    internal static class Program
    {
        private const bool SEED_EXAMPLES = true;

        [STAThread]
        private static void Main()
        {
            // 1) Set UI culture to South Africa ("en-ZA") for consistent UX and formatting.
            CultureInit.SetCulture("en-ZA");

            // 2) Standard WinForms startup plumbing.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // 3) Optional: add a couple of issues to demonstrate List<T> usage.
            if (SEED_EXAMPLES)
            {
                TrySeed();
            }

            // 4) Run the Main Menu. The Startup object in Project Properties must be set to this Program.
            Application.Run(new Views.MainMenuForm());
        }

        /// <summary>
        /// Adds sample issues safely.
        /// </summary>
        private static void TrySeed()
        {
            try
            {
                var repo = IssueRepository.Instance;
                if (repo.Count == 0)
                {
                    // Overload using field parameters:
                    repo.Add(
                        location: "Cape Town CBD",
                        category: "Sanitation",
                        description: "Overflowing public bin near the taxi rank.",
                        attachmentPath: null
                    );

                    // Overload using full IssueItem:
                    repo.Add(new IssueItem
                    {
                        Location = "Mitchells Plain",
                        Category = "Community Safety",
                        Description = "Flickering street light near school entrance.",
                        AttachmentPath = null
                    });
                }
            }
            catch
            {
                // Swallow any unexpected issues silently; seeding is non-critical.
            }
        }
    }
}

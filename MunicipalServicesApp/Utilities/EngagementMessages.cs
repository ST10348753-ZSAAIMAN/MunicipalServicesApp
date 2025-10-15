using System;

namespace MunicipalServicesApp.Utilities
{
    /// <summary>
    /// Encapsulates the engagement logic
    /// </summary>
    public static class EngagementMessages
    {
        /// <summary>
        /// Fields considered: Location, Category, Description, Attachment.
        /// </summary>
        public static int ComputeCompletionPercent(bool hasLocation, bool hasCategory, bool hasDescription, bool hasAttachment)
        {
            int filled = 0;
            if (hasLocation) filled++;
            if (hasCategory) filled++;
            if (hasDescription) filled++;
            if (hasAttachment) filled++;

            // 4 "slots" total → convert to percent.
            return (int)Math.Round((filled / 4.0) * 100.0);
        }

        /// <summary>
        /// Returns a short hint based on progress percentage.
        /// </summary>
        public static string PickMessage(int percent)
        {
            if (percent <= 0) return "Welcome! Start by entering the location.";
            if (percent < 50) return "Great — choose a category next.";
            if (percent < 75) return "Good progress. Add a short description.";
            if (percent < 100) return "Nearly there — attach a file if you can (optional).";
            return "Excellent! Click Submit when you're ready.";
        }
    }
}

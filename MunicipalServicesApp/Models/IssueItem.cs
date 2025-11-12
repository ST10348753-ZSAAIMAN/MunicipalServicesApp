using System;

namespace MunicipalServicesApp.Models
{
    public class IssueItem
    {
        /// <summary>
        /// Unique identifier for the issue.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// UTC timestamp.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Location text as entered by the user.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Category (Sanitation, Roads, Utilities, Community Safety, Other).
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Short free-text description of the problem.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Optional absolute file path to an attachment (image or document).
        /// </summary>
        public string AttachmentPath { get; set; }
    }
}

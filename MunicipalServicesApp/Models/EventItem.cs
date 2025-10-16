using System;
using System.Collections.Generic;

namespace MunicipalServicesApp.Models
{
    /// <summary>
    /// Represents a local municipal event or announcement.
    /// Priority is simulated as an integer:
    ///   0 = High (most urgent), 1 = Medium, 2 = Low (least urgent).
    /// Tags help the recommender understand event topics (e.g., "water", "outage").
    /// </summary>
    public class EventItem
    {
        /// <summary>Unique id for the event.</summary>
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>Short title to display in the list.</summary>
        public string Title { get; set; }

        /// <summary>Main category (e.g., Water, Electricity, Roads, Clinics, Libraries, Safety, Community, Solid Waste).</summary>
        public string Category { get; set; }

        /// <summary>Brief description shown in details dialog.</summary>
        public string Description { get; set; }

        /// <summary>Date of the event/announcement (date component is used for sorting and filtering).</summary>
        public DateTime Date { get; set; }

        /// <summary>Where the event takes place (suburb/venue/area).</summary>
        public string Location { get; set; }

        /// <summary>Topics/keywords used for search/recommendations (case-insensitive).</summary>
        public HashSet<string> Tags { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>0=High, 1=Medium, 2=Low (lower number means higher urgency).</summary>
        public int Priority { get; set; }

        /// <summary>Human-friendly label for the Priority integer.</summary>
        public string PriorityLabel => Priority == 0 ? "High" : (Priority == 1 ? "Medium" : "Low");
    }
}

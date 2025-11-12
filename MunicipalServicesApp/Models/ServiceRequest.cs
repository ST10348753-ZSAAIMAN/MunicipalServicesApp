using System;
using System.Collections.Generic;

namespace MunicipalServicesApp.Models
{
    /// <summary>
    /// Represents a municipal service request/ticket.
    /// Priority: 3=Critical, 2=High, 1=Normal, 0=Low
    /// Status: New, InProgress, OnHold, Resolved, Closed
    /// </summary>
    public class ServiceRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string TicketNumber { get; set; }      // e.g., SR-2025-0001
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Category { get; set; }          // Water/Electricity/Roads/etc.
        public string SubCategory { get; set; }       // e.g., Leak/Outage
        public string Location { get; set; }          // Area/Region
        public string Description { get; set; }
        public int Priority { get; set; }             // 3..0
        public string Status { get; set; } = "New";
        public List<string> History { get; set; } = new List<string>(); // textual progress notes
    }
}

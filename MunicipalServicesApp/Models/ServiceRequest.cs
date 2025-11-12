using System;
using System.Collections.Generic;

namespace MunicipalServicesApp.Models
{
    public class ServiceRequest
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string TicketNumber { get; set; }     
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Category { get; set; }          
        public string SubCategory { get; set; }       
        public string Location { get; set; }          
        public string Description { get; set; }
        public int Priority { get; set; }            
        public string Status { get; set; } = "New";
        public List<string> History { get; set; } = new List<string>();
    }
}

using System;
using System.Text.Json.Serialization;

namespace RewashPlus.Models
{
    public class Booking
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ServiceType { get; set; } = string.Empty; // e.g. Wash, Interior, Full Valet
        public DateTime AppointmentAt { get; set; }
        public bool IsSynced { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
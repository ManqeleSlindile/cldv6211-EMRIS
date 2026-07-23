using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Event
    {
        public int EventID { get; set; }

        [Required]
        public string? EventName { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string? Description { get; set; }

        public int? VenueID { get; set; }
        public Venue? Venue { get; set; }

        public int? EventTypeId { get; set; }
        public EventType? EventType { get; set; }
    }
}
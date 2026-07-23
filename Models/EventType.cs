using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class EventType
    {
        public int EventTypeId { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<Event>? Events { get; set; }
    }
}

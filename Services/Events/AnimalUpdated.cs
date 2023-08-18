using Events.Models;

namespace Events;

public class AnimalUpdated
{
    public string Id { get; set; }
    public string PublicId { get; set; }
    public int Age { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Breed { get; set; }
    public string Sex { get; set; }
    public int Weight { get; set; }
    public string Color { get; set; }
    public string Description { get; set; }
    public string CoverImageUrl { get; set; }
    public string Status { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public EventAddress Address { get; set; }
    public List<EventImage> Images { get; set; }
}

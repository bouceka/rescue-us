using Events.Models;

namespace Events;
public class AnimalCreated
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
    public Address Address { get; set; }
    public List<Image> Images { get; set; }

}

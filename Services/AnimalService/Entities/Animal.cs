using System.ComponentModel.DataAnnotations;
using shortid;
using shortid.Configuration;

namespace AnimalService.Entities

{
    public class Animal
    {
        [Key]
        public Guid Id { get; set; }
        public string PublicId { get; set; } = ShortId.Generate(new GenerationOptions(useNumbers: true, useSpecialCharacters: false));
        public int Age { get; set; }
        public string Slug { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Breed { get; set; }
        public string Sex { get; set; }
        public int Weight { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public Status Status { get; set; }
        public Address Address { get; set; }
        public List<Image> Images { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    }


}
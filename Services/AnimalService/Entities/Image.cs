using System.ComponentModel.DataAnnotations.Schema;

namespace AnimalService.Entities
{
    [Table("Images")]
    public class Image
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public bool IsMain { get; set; }
        public string PublicId { get; set; }
        public Guid AnimalId { get; set; }
        public Animal Animal { get; set; }
    }
}
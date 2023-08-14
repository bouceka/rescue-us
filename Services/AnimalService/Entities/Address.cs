
namespace AnimalService.Entities
{
    public class Address
    {
        public Guid Id { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }

        // public Animal Animal { get; set; }
        public Guid AnimalId { get; set; }
    }
}
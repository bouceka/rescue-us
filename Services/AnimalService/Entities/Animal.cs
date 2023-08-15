using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AnimalService.Entities
{
    public class Animal
    {
        [Key]
        public Guid Id { get; set; }
        public int PublicId { get; set; } =  GenerateRandom7DigitNumber();
        public int Age { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Breed { get; set; }
        public string Sex { get; set; }
        public int Weight { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public string CoverImageUrl { get; set; }
        public Status Status { get; set; }
        public Address Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        private static readonly Random _random = new();

        private static int GenerateRandom7DigitNumber()
        {
            int min = 1000000; // Smallest 7-digit number
            int max = 9999999; // Largest 7-digit number

            return _random.Next(min, max + 1);
        }
    }
}
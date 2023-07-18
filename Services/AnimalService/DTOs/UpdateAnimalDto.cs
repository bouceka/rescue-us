using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnimalService.Entities;

namespace AnimalService.DTOs
{
    public class UpdateAnimalDto
    {

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

    }
}
using AnimalService.Data;
using AnimalService.DTOs;
using AnimalService.Entities;
using AnimalService.Helpers;
using AutoMapper;
using Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnimalService.Controllers
{
    [Route("api/[controller]")]
    public class AnimalsController : Controller
    {
        private readonly AnimalDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public AnimalsController(AnimalDbContext context, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
            _mapper = mapper;
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<List<AnimalDto>>> GetAllAnimals()
        {
            var animals = await _context.Animals
            .OrderBy(x => x.UpdatedAt)
            .ToListAsync();

            return _mapper.Map<List<AnimalDto>>(animals);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<AnimalDto>> GetAnimalById(Guid id)
        {
            var foundAnimal = await _context.Animals.FirstOrDefaultAsync(x => x.Id == id);

            if (foundAnimal == null) return NotFound();

            return _mapper.Map<AnimalDto>(foundAnimal);
        }
        [HttpPost]
        public async Task<ActionResult<AnimalDto>> CreateAnimal(CreateAnimalDto createAnimalDto)
        {
            var animal = _mapper.Map<Animal>(createAnimalDto);

            _context.Animals.Add(animal);

            var newAnimal = _mapper.Map<AnimalDto>(animal);

            await _publishEndpoint.Publish(_mapper.Map<AnimalCreated>(newAnimal));

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return BadRequest("Could not save changes to the DB");

            return CreatedAtAction(nameof(GetAnimalById),
                new { animal.Id }, newAnimal);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAnimal(Guid id, UpdateAnimalDto updateAnimalDto)
        {

            var animal = await _context.Animals
    .FirstOrDefaultAsync(x => x.Id == id);
            if (animal == null) return NotFound();

            animal.Description = updateAnimalDto.Description ?? animal.Description;
            animal.Name = updateAnimalDto.Name ?? animal.Name;
            animal.Status = updateAnimalDto.Status != null ? EnumHelper.EnumParse(updateAnimalDto.Status, animal.Status) : animal.Status;
            animal.Breed = updateAnimalDto.Breed ?? animal.Breed;
            animal.CoverImageUrl = updateAnimalDto.CoverImageUrl ?? animal.CoverImageUrl;
            animal.Color = updateAnimalDto.Color ?? animal.Color;
            animal.Type = updateAnimalDto.Type ?? animal.Type;
            animal.CoverImageUrl = updateAnimalDto.CoverImageUrl ?? animal.CoverImageUrl;
            animal.Weight = updateAnimalDto.Weight == 0 ? animal.Weight : updateAnimalDto.Weight;
            animal.Age = updateAnimalDto.Age == 0 ? animal.Age : updateAnimalDto.Age;
            animal.UpdatedAt = DateTime.UtcNow;

            await _publishEndpoint.Publish(_mapper.Map<AnimalUpdated>(animal));

            var result = await _context.SaveChangesAsync() > 0;

            if (result) return Ok();

            return BadRequest("Problem saving changes");
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAnimal(Guid id)
        {
            var animal = await _context.Animals.FindAsync(id);

            if (animal == null) return NotFound();

            _context.Animals.Remove(animal);

            await _publishEndpoint.Publish<AnimalDeleted>(new { Id = animal.Id.ToString() });

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return BadRequest("Could not update DB");

            return Ok();
        }
    }
}
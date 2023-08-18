using AnimalService.Data;
using AnimalService.DTOs;
using AnimalService.Entities;
using AnimalService.Helpers;
using AnimalService.Interfaces;
using AutoMapper;
using Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnimalService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnimalsController : ControllerBase
    {
        private readonly AnimalDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IImageService _imageService;

        public AnimalsController(AnimalDbContext context, IMapper mapper, IPublishEndpoint publishEndpoint, IImageService imageService)
        {
            _publishEndpoint = publishEndpoint;
            _mapper = mapper;
            _context = context;
            _imageService = imageService;
        }


        [HttpGet]
        public async Task<ActionResult<List<AnimalDto>>> GetAllAnimals()
        {
            var animals = await _context.Animals.Include(x => x.Address).Include(x => x.Images)
            .OrderBy(x => x.UpdatedAt)
            .ToListAsync();

            return _mapper.Map<List<AnimalDto>>(animals);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<AnimalDto>> GetAnimalById(Guid id)
        {
            var foundAnimal = await _context.Animals.Include(x => x.Address).Include(x => x.Images).FirstOrDefaultAsync(x => x.Id == id);

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

            var animal = await _context.Animals.Include(x => x.Address)
    .FirstOrDefaultAsync(x => x.Id == id);
            if (animal == null) return NotFound();

            animal.Description = updateAnimalDto.Description ?? animal.Description;
            animal.Name = updateAnimalDto.Name ?? animal.Name;
            animal.Status = updateAnimalDto.Status != null ? EnumHelper.EnumParse(updateAnimalDto.Status, animal.Status) : animal.Status;
            animal.Breed = updateAnimalDto.Breed ?? animal.Breed;
            animal.Color = updateAnimalDto.Color ?? animal.Color;
            animal.Type = updateAnimalDto.Type ?? animal.Type;
            animal.Weight = updateAnimalDto.Weight == 0 ? animal.Weight : updateAnimalDto.Weight;
            animal.Age = updateAnimalDto.Age == 0 ? animal.Age : updateAnimalDto.Age;
            animal.Address.City = updateAnimalDto.City ?? animal.Address.City;
            animal.Address.Address1 = updateAnimalDto.AddressOne ?? animal.Address.Address1;
            animal.Address.Address2 = updateAnimalDto.AddressTwo ?? animal.Address.Address2;
            animal.Address.Country = updateAnimalDto.Country ?? animal.Address.Country;
            animal.Address.PostalCode = updateAnimalDto.PostalCode ?? animal.Address.PostalCode;
            animal.Address.State = updateAnimalDto.State ?? animal.Address.State;
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

        [HttpPost("add-image")]
        public async Task<ActionResult<ImageDto>> AddImage([FromForm] Guid animalId, [FromForm] IFormFile file)
        {
            var animal = await _context.Animals.Include(x => x.Address).Include(x => x.Images).FirstOrDefaultAsync(x => x.Id == animalId);

            if (animal == null) return NotFound();

            var result = await _imageService.AddImageAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var image = new Image
            {
                Id = Guid.NewGuid(),
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                AnimalId = animalId,
            };

            if (animal.Images.Count == 0) image.IsMain = true;

            // Add new image to the existing animal
            animal.Images.Add(image);
            // Update time
            animal.UpdatedAt = DateTime.UtcNow;

            // Publish our events to RabbitMQ
            await _publishEndpoint.Publish(_mapper.Map<AnimalUpdated>(animal));

            // Save changes to the database
            var complete =  _context.SaveChanges() > 0;

            if (complete)
            {
                return CreatedAtAction(nameof(GetAnimalById),
                new { animal.Id }, _mapper.Map<AnimalDto>(animal));
            }

            return BadRequest("Problem adding image");
        }

        [HttpPut("set-main-image")]
        public async Task<ActionResult> SetMainImage([FromForm] string publicId, [FromForm] Guid animalId)
        {
            var animal = await _context.Animals.Include(x => x.Images)
    .FirstOrDefaultAsync(x => x.Id == animalId);

            if (animal == null) return NotFound("Animal not found");

            var image = animal.Images.FirstOrDefault(x => x.PublicId == publicId);

            if (image == null) return NotFound("Image not found");

            var currentMain = animal.Images.FirstOrDefault(x => x.IsMain);

            if (currentMain != null) currentMain.IsMain = false;

            image.IsMain = true;

            animal.UpdatedAt = DateTime.UtcNow;

            await _publishEndpoint.Publish(_mapper.Map<AnimalUpdated>(animal));

            var complete = await _context.SaveChangesAsync() > 0;

            if (complete) return Ok();

            return BadRequest("Problem setting the main photo");
        }

        [HttpDelete("delete-image/{imageId}")]
        public async Task<ActionResult> DeleteImage(Guid imageId)
        {
            var image = await _context.Images
                                         .Include(img => img.Animal) // Include the associated animal
                                         .FirstOrDefaultAsync(x => x.Id == imageId);

            if (image == null) return NotFound("Image not found");

            //* Another call Start
            var animalId = image.AnimalId; // Assuming you have an AnimalId property on the Image entity

            var animal = _context.Animals
                                 .Include(a => a.Images)
                                 .FirstOrDefault(a => a.Id == animalId); // Access the associated animal directly

            if (image.IsMain)
            {
                var newMainImage = animal.Images.FirstOrDefault(x => !x.IsMain && x.Id != imageId);
                if (newMainImage != null) newMainImage.IsMain = true;
            }
            // end

            // Alternative solution
            // if (image.IsMain) return BadRequest("Could not delete main image");

            if (image.PublicId != null)
            {
                var result = await _imageService.DeleteImageAsync(image.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            _context.Images.Remove(image); // Mark the image for deletion

            animal.UpdatedAt = DateTime.UtcNow;

            await _publishEndpoint.Publish(_mapper.Map<AnimalUpdated>(animal));

            var complete = await _context.SaveChangesAsync() > 0;

            if (complete) return Ok();

            return BadRequest("Could not delete image");
        }

    }
}
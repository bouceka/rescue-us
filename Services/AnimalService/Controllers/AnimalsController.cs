using AnimalService.Data;
using AnimalService.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnimalService.Controllers
{
    [Route("api/[controller]")]
    public class AnimalsController : Controller
    {
        private readonly AnimalDbContext _context;

        public AnimalsController(AnimalDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<List<Animal>>> GetAllAnimals()
        {
            return  await _context.Animals.OrderBy(x => x.PublicId).ToListAsync();
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZooLink.Domain.Enums;
using ZooLink.Domain.Models;
using ZooLink.DTO;
using ZooLink.Services;

namespace ZooLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalsController : ControllerBase
    {
        private readonly AnimalService _animalService;

        public AnimalsController(AnimalService animalService)
        {
            _animalService = animalService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnimalModelDTO>>> GetAnimals()
        {
            var animals = await _animalService.GetAnimals();
            return Ok(animals);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<AnimalModelDTO>> GetAnimal(Guid id)
        {
            var animal = await _animalService.GetAnimal(id);

            if (animal is null)
            {
                return NotFound();
            }

            return Ok(animal);
        }

        [HttpPost]
        public async Task<ActionResult> PostAnimals(AnimalsImportDTO animalsImportDto)
        {
            var importedAnimals = await _animalService.AddAnimals(animalsImportDto);

            return Ok(importedAnimals);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using ZooLink.DTO;
using ZooLink.Services;

namespace ZooLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalsController : ControllerBase
    {
        private readonly IAnimalService _animalService;

        public AnimalsController(IAnimalService animalService)
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

        [HttpDelete]
        public async Task<ActionResult> DeleteAnimal(Guid id)
        {
            var removedAnimalId = await _animalService.RemoveAnimal(id);

            if (removedAnimalId == Guid.Empty)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}

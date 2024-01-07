using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZooLink.Domain.Enums;
using ZooLink.Domain.Models;
using ZooLink.DTO;

namespace ZooLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalsController : ControllerBase
    {
        private static readonly int SpeciesPreferenceWeight = -3;
        private readonly AppDbContext _context;

        public AnimalsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> PostAnimals(AnimalsImportDTO animalsImportDto)
        {
            foreach (var animalGroupDto in animalsImportDto.Animals)
            {
                var animalType = await _context.AnimalTypes.FirstOrDefaultAsync(x => x.Species == animalGroupDto.Species);
                if (animalType is null) { return NotFound($"Animal type {animalGroupDto.Species} does not exist"); }

                var enclosureId = FindBestEnclosure(animalType);

                var animals = Enumerable.Range(1, animalGroupDto.Amount).Select(_ => new Animal
                {
                    AnimalTypeId = animalType.Id,
                    EnclosureId = enclosureId,
                });

                await _context.AddRangeAsync(animals);
                await _context.SaveChangesAsync();

            }

            return Ok();
        }

        private int GetEnclosureSpace(Enclosure enclosure)
        {
            var animalCount = _context.Animals.Count(a => a.EnclosureId == enclosure.Id);

            var capacity = (int)enclosure.Size;

            var spaceLeft = capacity - animalCount;

            Console.WriteLine($"Enclosure {enclosure.Name} has {spaceLeft} spaces left");

            return spaceLeft;
        }

        private IEnumerable<AnimalType> GetEnclosedAnimals(Enclosure enclosure)
        {
            var animalTypeIds = _context.Animals.Where(a => a.EnclosureId == enclosure.Id).Select(x => x.AnimalTypeId).Distinct().ToHashSet();

            var animalTypes = _context.AnimalTypes.Where(x => animalTypeIds.Contains(x.Id)).ToList();

            return animalTypes;
        }

        private Guid FindBestEnclosure(AnimalType animal)
        {
            var enclosures = _context.Enclosures.ToList().Where(e => GetEnclosureSpace(e) > 0).ToList();

            var queue = new PriorityQueue<Enclosure, int>();

            foreach (var enclosure in enclosures)
            {
                var enclosurePriority = CalculateEnclosurePriority(enclosure, animal);

                if (enclosurePriority.HasValue)
                {
                    queue.Enqueue(enclosure, enclosurePriority.Value);
                }
            }

            return queue.Peek().Id;
        }

        private int? CalculateEnclosurePriority(Enclosure enclosure, AnimalType animal)
        {
            var neighboringAnimals = GetEnclosedAnimals(enclosure);

            var sameSpecies = neighboringAnimals
                .Where(a => a.Species == animal.Species)
                .Count();

            var favObjectIds = new List<Guid>();

            var animalPreferenceIds = _context.AnimalPreferredAssets.Where(x => x.AnimalTypeId == animal.Id).ToList();

            foreach (var preference in animalPreferenceIds)
            {
                var enrichmentObject = _context.ZooAssets.FirstOrDefault(x => x.Id == preference.AssetId);

                favObjectIds.Add(enrichmentObject.Id);
            }

            var favObjectCount = _context.EnclosureAssets.Where(x => x.EnclosureId == enclosure.Id && favObjectIds.Contains(x.AssetId)).Count();

            if (animal.Food is FoodType.Herbivore)
            {
                if (neighboringAnimals.All(a => a.Food == FoodType.Herbivore))
                {
                    return -(sameSpecies + favObjectCount);
                }
                return null;
            }
            else if (animal.Food is FoodType.Carnivore)
            {
                if (neighboringAnimals.All(a => a.Food == FoodType.Carnivore))
                {
                    var diffSpecies = neighboringAnimals
                        .Where(a => a.Species != animal.Species)
                        .Count(a => a.Food == FoodType.Carnivore);

                    if (diffSpecies > 1) { return null; }

                    return -(sameSpecies + favObjectCount + (diffSpecies * SpeciesPreferenceWeight));
                }
            }
            return null;
        }



    }
}

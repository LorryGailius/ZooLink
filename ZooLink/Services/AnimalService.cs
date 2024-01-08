using Microsoft.EntityFrameworkCore;
using ZooLink.Domain.Enums;
using ZooLink.Domain.Models;
using ZooLink.DTO;
using ZooLink.Extensions;

namespace ZooLink.Services
{
    public class AnimalService
    {
        private static readonly int SpeciesPreferenceWeight = -3;

        private readonly AppDbContext _context;
        private readonly EnclosureService _enclosureService;

        public AnimalService(AppDbContext context, EnclosureService enclosureService)
        {
            _context = context;
            _enclosureService = enclosureService;
        }

        public async Task<IEnumerable<AnimalModelDTO>> GetAnimals()
        {
            var animals = await _context.Animals.ToListAsync();

            return GetModelDTOList(animals);
        }

        public async Task<AnimalModelDTO> GetAnimal(Guid id)
        {
            var animal = await _context.Animals.FirstOrDefaultAsync(x => x.Id == id);

            return animal is not null ? GetModelDto(animal) : null;
        }

        public async Task<IEnumerable<AnimalModelDTO>> AddAnimals(AnimalsImportDTO animalsImportDto)
        {
            var importedAnimals = new List<AnimalModelDTO>();
            foreach (var animalGroupDto in animalsImportDto.Animals)
            {
                var importedGroup = await AssignAnimalGroup(animalGroupDto);
                importedAnimals.AddRange(importedGroup);
            }

            return importedAnimals;
        }

        public async Task<Guid> RemoveAnimal(Guid id)
        {
            var animal = await _context.Animals.FirstOrDefaultAsync(x => x.Id == id);

            if (animal is null) { return Guid.Empty; }

            _context.Animals.Remove(animal);
            await _context.SaveChangesAsync();

            return animal.Id;
        }

        private async Task<IEnumerable<AnimalModelDTO>> AssignAnimalGroup(AnimalGroupDTO animalGroupDto)
        {
            var importedAnimals = new List<AnimalModelDTO>();

            var animalType = await _context.AnimalTypes.FirstOrDefaultAsync(x => x.Species == animalGroupDto.Species);
            if (animalType is not null)
            {
                for (var i = 0; i < animalGroupDto.Amount; i++)
                {
                    var importedAnimal = await AssignAnimal(animalType, FindBestEnclosure(animalType));

                    if (importedAnimal is not null)
                    {
                        importedAnimals.Add(importedAnimal);

                        await _context.SaveChangesAsync();
                    }
                }
            }

            return importedAnimals;
        }

        private async Task<AnimalModelDTO> AssignAnimal(AnimalType animalType, Guid enclosureId)
        {
            if (enclosureId == Guid.Empty)
            {
                return null;
            }

            var animal = new Animal
            {
                Id = Guid.NewGuid(),
                AnimalTypeId = animalType.Id,
                EnclosureId = enclosureId
            };

            await _context.Animals.AddAsync(animal);

            return animal.ToModelDTO(animalType);
        }

        private IEnumerable<AnimalType> GetEnclosedAnimals(Enclosure enclosure)
        {
            var animalTypeIds = _context.Animals.Where(a => a.EnclosureId == enclosure.Id).Select(x => x.AnimalTypeId).Distinct().ToHashSet();

            var animalTypes = _context.AnimalTypes.Where(x => animalTypeIds.Contains(x.Id)).ToList();

            return animalTypes;
        }

        private Guid FindBestEnclosure(AnimalType animal)
        {
            var enclosures = _context.Enclosures.ToList().Where(e => _enclosureService.GetEnclosureSpace(e) > 0).ToList();

            if (enclosures.Count == 0)
            {
                return Guid.Empty;
            }

            var queue = new PriorityQueue<Enclosure, int>();

            foreach (var enclosure in enclosures)
            {
                var enclosurePriority = CalculateEnclosurePriority(enclosure, animal);

                if (enclosurePriority.HasValue)
                {
                    queue.Enqueue(enclosure, enclosurePriority.Value);
                }
                else
                {
                    var enclosureName = _context.Enclosures.FirstOrDefault(x => x.Id == enclosure.Id).Name;
                }
            }
            return queue.Count == 0 ? Guid.Empty : queue.Peek().Id;
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

        private IEnumerable<AnimalModelDTO> GetModelDTOList(IEnumerable<Animal> animals)
        {
            return animals.Select(GetModelDto).ToList();
        }

        private AnimalModelDTO GetModelDto(Animal animal)
        {
            var animalType = _context.AnimalTypes.FirstOrDefault(x => x.Id == animal.AnimalTypeId);

            return animal.ToModelDTO(animalType);
        }
    }
}

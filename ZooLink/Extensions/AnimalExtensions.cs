using ZooLink.Domain.Models;
using ZooLink.DTO;

namespace ZooLink.Extensions
{
    public static class AnimalExtensions
    {
        public static AnimalModelDTO ToModelDTO(this Animal animal, AnimalType animalType) => new()
        {
            Id = animal.Id,
            Species = animalType.Species,
            Food = animalType.Food,
        };
    }
}

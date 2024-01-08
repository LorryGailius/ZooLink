using ZooLink.DTO;

namespace ZooLink.Services;

public interface IAnimalService
{
    Task<IEnumerable<AnimalModelDTO>> GetAnimals();
    Task<AnimalModelDTO> GetAnimal(Guid id);
    Task<IEnumerable<AnimalModelDTO>> AddAnimals(AnimalsImportDTO animalsImportDto);
    Task<Guid> RemoveAnimal(Guid id);
}
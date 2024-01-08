using ZooLink.Domain.Models;
using ZooLink.DTO;

namespace ZooLink.Services;

public interface IEnclosureService
{
    Task<IEnumerable<EnclosureModelDTO>> GetEnclosures();
    Task<EnclosureModelDTO> GetEnclosure(Guid id);
    Task<IEnumerable<EnclosureModelDTO>> AddEnclosures(EnclosureImportDTO enclosureImportDto);
    Task<EnclosureModelDTO> AddEnclosure(EnclosureDTO enclosureDto);
    Task Populate();
    int GetEnclosureSpace(Enclosure enclosure);
}
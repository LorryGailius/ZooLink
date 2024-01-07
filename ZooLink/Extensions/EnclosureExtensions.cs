using ZooLink.Domain.Models;
using ZooLink.DTO;

namespace ZooLink.Extensions 
{
    public static class EnclosureExtensions
    {
        public static EnclosureModelDTO ToModelDTO(this Enclosure enclosure, IEnumerable<ZooAsset> zooAssets, IEnumerable<AnimalModelDTO> animals) => new()
        {
            Id = enclosure.Id,
            Name = enclosure.Name,
            Size = enclosure.Size,
            Location = enclosure.Location,
            Objects = zooAssets,
            Animals = animals,
        };
    }
}

using ZooLink.DTO;

namespace ZooLink.DTO
{
    public class EnclosureImportDTO
    {
        public required IEnumerable<EnclosureDTO> Enclosures { get; set; }
    }
}

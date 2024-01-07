using ZooLink.Domain;

namespace ZooLink.DTO
{
    public class EnclosureDTO
    {
        public required string Name { get; set; }

        public EnclosureSize Size { get; set; }

        public EnclosureLocation Location { get; set; }

        public required IEnumerable<string> Objects { get; set; }
    }
}

using ZooLink.Enums;

namespace ZooLinkDTOs;

public class EnclosureDTO
{
    public string Name { get; set; }

    public EnclosureSize Size { get; set; }

    public EnclosureLocation Location { get; set; }

    public required IEnumerable<string> Objects { get; set; }
}

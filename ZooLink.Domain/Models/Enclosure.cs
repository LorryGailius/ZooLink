using ZooLink.Domain.Enums;

namespace ZooLink.Domain.Models;

public class Enclosure
{
    public Guid Id { get; set; }

    public required string Name { get; set; }

    public EnclosureSize Size { get; set; }

    public EnclosureLocation Location { get; set; }
}

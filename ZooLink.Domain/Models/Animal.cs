namespace ZooLink.Domain.Models;

public class Animal
{
    public Guid Id { get; set; }

    public Guid AnimalTypeId { get; set; }

    public Guid EnclosureId { get; set; }
}

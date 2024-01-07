using ZooLink.Domain.Enums;

namespace ZooLink.Domain.Models;

public class AnimalType
{
    public Guid Id { get; set; }

    public required string Species { get; set; }

    public FoodType Food { get; set; }
}

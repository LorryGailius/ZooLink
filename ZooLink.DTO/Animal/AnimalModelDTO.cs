using ZooLink.Domain.Enums;

namespace ZooLink.DTO
{
    public class AnimalModelDTO
    {
        public Guid Id { get; set; }
        public required string Species { get; set; }
        public FoodType Food { get; set; }
    }
}

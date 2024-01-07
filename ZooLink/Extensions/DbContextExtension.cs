using ZooLink.Domain.Enums;
using ZooLink.Domain.Models;

namespace ZooLink.Extensions;

public static class DbContextExtension
{
    private static readonly Dictionary<string, ZooAsset> ZooAssets = new List<ZooAsset>
    {
        new() { Id = Guid.NewGuid(), Name = "Rocks" },
        new() { Id = Guid.NewGuid(), Name = "Logs" },
        new() { Id = Guid.NewGuid(), Name = "Water Pond" },
        new() { Id = Guid.NewGuid(), Name = "Climbing Structures" },
        new() { Id = Guid.NewGuid(), Name = "Shelter" },
        new() { Id = Guid.NewGuid(), Name = "Pool" },
        new() { Id = Guid.NewGuid(), Name = "Trees" },
        new() { Id = Guid.NewGuid(), Name = "Mud Bath Area" },
        new() { Id = Guid.NewGuid(), Name = "Water Trough" },
        new() { Id = Guid.NewGuid(), Name = "Tall Trees" },
        new() { Id = Guid.NewGuid(), Name = "Feeding Platform" },
        new() { Id = Guid.NewGuid(), Name = "Shade Structure" },
        new() { Id = Guid.NewGuid(), Name = "Tunnels" },
        new() { Id = Guid.NewGuid(), Name = "Enrichment Toys" },
        new() { Id = Guid.NewGuid(), Name = "Nesting Boxes" },
        new() { Id = Guid.NewGuid(), Name = "Perches" },
        new() { Id = Guid.NewGuid(), Name = "Swing" }
    }.ToDictionary(x => x.Name, x => x);

    private static readonly Dictionary<string, AnimalType> AnimalTypes = new List<AnimalType>
    {
        new() { Id = Guid.NewGuid(), Species = "Lion", Food = FoodType.Carnivore },
        new() { Id = Guid.NewGuid(), Species = "Tiger", Food = FoodType.Carnivore },
        new() { Id = Guid.NewGuid(), Species = "Elephant", Food = FoodType.Herbivore },
        new() { Id = Guid.NewGuid(), Species = "Giraffe", Food = FoodType.Herbivore },
        new() { Id = Guid.NewGuid(), Species = "Polar Bear", Food = FoodType.Carnivore },
        new() { Id = Guid.NewGuid(), Species = "Zebra", Food = FoodType.Herbivore },
        new() { Id = Guid.NewGuid(), Species = "Cheetah", Food = FoodType.Carnivore },
        new() { Id = Guid.NewGuid(), Species = "Jaguar", Food = FoodType.Carnivore },
        new() { Id = Guid.NewGuid(), Species = "Gorilla", Food = FoodType.Herbivore },
        new() { Id = Guid.NewGuid(), Species = "Wolf", Food = FoodType.Carnivore },
        new() { Id = Guid.NewGuid(), Species = "Hyena", Food = FoodType.Carnivore }
    }.ToDictionary(x => x.Species, x => x);

    private static readonly IEnumerable<AnimalPreferedAssets> AnimalPreferences =
        new Dictionary<string, IEnumerable<string>>
            {
                { "Gorilla", new[] { "Climbing Structures", "Tall Trees", "Swing", "Perches" } },
                { "Lion", new[] { "Rocks", "Logs", "Shelter" } },
                { "Tiger", new[] { "Climbing Structures", "Shelter" } },
                { "Elephant", new[] { "Water Pond", "Mud Bath Area", "Tall Trees" } },
                { "Giraffe", new[] { "Tall Trees", "Feeding Platform", "Shade Structure" } },
                { "Polar Bear", new[] { "Rocks", "Water Pond", "Pool" } },
                { "Zebra", new[] { "Tall Trees", "Shade Structure" } },
                { "Cheetah", new[] { "Rocks", "Shelter", "Logs" } },
                { "Jaguar", new[] { "Climbing Structures", "Shelter", "Tall Trees" } },
                { "Wolf", new[] { "Rocks", "Logs", "Shelter" } },
                { "Hyena", new[] { "Rocks", "Shelter", "Logs" } }
            }.SelectMany(x => x.Value.Select(y => new { AnimalTypeId = x.Key, AssetName = y }))
            .Select(x => new AnimalPreferedAssets
            {
                AnimalTypeId = AnimalTypes[x.AnimalTypeId].Id,
                AssetId = ZooAssets[x.AssetName].Id
            });

    public static async Task PopulateAsync(this AppDbContext context)
    {
        await context.ZooAssets.AddRangeAsync(ZooAssets.Values);
         
        await context.AnimalTypes.AddRangeAsync(AnimalTypes.Values);
         
        await context.AnimalPreferedAssets.AddRangeAsync(AnimalPreferences);
         
        await context.SaveChangesAsync();
    }
}
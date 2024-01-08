using System.Buffers;
using Microsoft.EntityFrameworkCore;
using Moq;
using ZooLink.Domain.Enums;
using ZooLink.Domain.Models;
using ZooLink.DTO;
using ZooLink.Extensions;
using ZooLink.Services;

namespace ZooLink.Tests;
public class AnimalServiceTests
{
    public AppDbContext GetMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "EnclosureTests")
            .Options;
        return new AppDbContext(options);
    }

    public void Dispose(AppDbContext context)
    {
        // Based on https://stackoverflow.com/questions/33490696/how-can-i-reset-an-ef7-inmemory-provider-between-unit-tests
        // DO NOT USE IN PRODUCTION
        context.Database.EnsureDeleted();
    }

    [Fact]

    public async Task AddAnimals_SingleEnclosureSpeciesConflict_AddsOnlyLion()
    {
        var enclosure1 = new Enclosure
        {
            Id = Guid.NewGuid(),
            Name = "Enclosure 1",
            Size = EnclosureSize.Small,
            Location = EnclosureLocation.Inside,
        };

        var animals = new AnimalsImportDTO()
        {
            Animals = new List<AnimalGroupDTO>()
            {
                new()
                {
                    Species = "Lion",
                    Food = FoodType.Carnivore.ToString(),
                    Amount = 1,
                },
                new()
                {
                    Species = "Zebra",
                    Food = FoodType.Herbivore.ToString(),
                    Amount = 2,
                },
            }
        };

        using var context = GetMemoryContext();

        var enclosureService = new EnclosureService(context);
        var animalService = new AnimalService(context, enclosureService);

        await context.Enclosures.AddAsync(enclosure1);
        await context.PopulateAsync();
        var addedAnimals = await animalService.AddAnimals(animals);

        Dispose(context);

        Assert.Equal(1, addedAnimals.Count());
        Assert.All(addedAnimals, x => Assert.Equal("Lion", x.Species));
        Assert.DoesNotContain(addedAnimals, x => x.Species is "Zebra");
    }

    [Fact]
    public async Task AddAnimals_CarnivoreLimit_DoesNotExceed()
    {
        var enclosure1 = new Enclosure
        {
            Id = Guid.NewGuid(),
            Name = "Enclosure 1",
            Size = EnclosureSize.Small,
            Location = EnclosureLocation.Inside,
        };

        var animals = new AnimalsImportDTO()
        {
            Animals = new List<AnimalGroupDTO>()
            {
                new()
                {
                    Species = "Lion",
                    Food = FoodType.Carnivore.ToString(),
                    Amount = 1,
                },
                new()
                {
                    Species = "Tiger",
                    Food = FoodType.Carnivore.ToString(),
                    Amount = 2,
                },
                new()
                {
                    Species = "Jaguar",
                    Food = FoodType.Carnivore.ToString(),
                    Amount = 1,
                }
            }
        };

        using var context = GetMemoryContext();

        var enclosureService = new EnclosureService(context);
        var animalService = new AnimalService(context, enclosureService);

        await context.Enclosures.AddAsync(enclosure1);
        await context.PopulateAsync();
        var addedAnimals = await animalService.AddAnimals(animals);
        
        Dispose(context);

        Assert.Equal(3, addedAnimals.Count());
        Assert.All(addedAnimals, x => Assert.Equal(FoodType.Carnivore, x.Food));
        Assert.Equal(2, addedAnimals.Select(x => x.Species).Distinct().Count());
        Assert.DoesNotContain(addedAnimals, x => x.Species is "Jaguar");
    }

    [Fact]
    public async Task AddAnimals_ObjectPreference_ChooseSecondEnclosure()
    {
        var enclosureImportDto = new EnclosureImportDTO()
        {
            Enclosures = new List<EnclosureDTO>()
            {
                new()
                {
                    Name = "Enclosure 1",
                    Size = EnclosureSize.Small,
                    Location = EnclosureLocation.Inside,
                    Objects = new List<string>()
                    {
                        "Rocks",
                    },
                },
                new()
                {
                    Name = "Enclosure 2",
                    Size = EnclosureSize.Small,
                    Location = EnclosureLocation.Inside,
                    Objects = new List<string>()
                    {
                        "Rocks",
                        "Logs",
                    },
                }
            }
        };

        var animals = new AnimalsImportDTO()
        {
            Animals = new List<AnimalGroupDTO>()
            {
                new()
                {
                    Species = "Lion",
                    Food = FoodType.Carnivore.ToString(),
                    Amount = 1,
                },
            }
        };

        using var context = GetMemoryContext();

        var enclosureService = new EnclosureService(context);
        var animalService = new AnimalService(context, enclosureService);

        await context.PopulateAsync();

        await enclosureService.AddEnclosures(enclosureImportDto);

        await animalService.AddAnimals(animals);

        var enclosures = await enclosureService.GetEnclosures();

        Dispose(context);

        Assert.Equal("Enclosure 2", enclosures.FirstOrDefault(x => x.Animals.Any(y => y.Species is "Lion")).Name);
        Assert.Equal(0, enclosures.ElementAt(1).Animals.Count());
    }

    [Fact]
    public async Task AddAnimals_NoHerbivoreLimit_CanAdd3HerbivoreSpeciesToEnclosure()
    {
        var enclosure1 = new Enclosure
        {
            Id = Guid.NewGuid(),
            Name = "Enclosure 1",
            Size = EnclosureSize.Small,
            Location = EnclosureLocation.Inside,
        };

        var animals = new AnimalsImportDTO()
        {
            Animals = new List<AnimalGroupDTO>()
            {
                new()
                {
                    Species = "Zebra",
                    Food = FoodType.Herbivore.ToString(),
                    Amount = 2,
                },
                new()
                {
                    Species = "Giraffe",
                    Food = FoodType.Herbivore.ToString(),
                    Amount = 1,
                },
                new()
                {
                    Species = "Elephant",
                    Food = FoodType.Herbivore.ToString(),
                    Amount = 1,
                }
            }
        };

        using var context = GetMemoryContext();
        var enclosureService = new EnclosureService(context);
        var animalService = new AnimalService(context, enclosureService);

        await context.Enclosures.AddAsync(enclosure1);
        await context.PopulateAsync();
        var addedAnimals = await animalService.AddAnimals(animals);

        Dispose(context);

        Assert.Equal(4, addedAnimals.Count());
        Assert.All(addedAnimals, x => Assert.Equal(FoodType.Herbivore, x.Food));
        Assert.Equal(3, addedAnimals.Select(x => x.Species).Distinct().Count());
        Assert.Contains(addedAnimals, x => x.Species is "Zebra");
        Assert.Contains(addedAnimals, x => x.Species is "Giraffe");
        Assert.Contains(addedAnimals, x => x.Species is "Elephant");
    }

    [Fact]
    public async Task AddAnimal_SameSpeciesHigherPriority_AddsToSecondEnclosure()
    {
        var enclosure1 = new Enclosure
        {
            Id = Guid.NewGuid(),
            Name = "Enclosure 1",
            Size = EnclosureSize.Small,
            Location = EnclosureLocation.Inside,
        };

        var enclosure2 = new Enclosure
        {
            Id = Guid.NewGuid(),
            Name = "Enclosure 2",
            Size = EnclosureSize.Small,
            Location = EnclosureLocation.Inside,
        };

        var animalImportFirst = new AnimalsImportDTO()
        {
            Animals = new List<AnimalGroupDTO>()
            {
                new()
                {
                    Species = "Elephant",
                    Food = FoodType.Herbivore.ToString(),
                    Amount = 1,
                },
                new()
                {
                    Species = "Lion",
                    Food = FoodType.Carnivore.ToString(),
                    Amount = 1,
                },
            }
        };

        var animalImportSecond = new AnimalsImportDTO()
        {
            Animals = new List<AnimalGroupDTO>()
            {
                new()
                {
                    Species = "Lion",
                    Food = FoodType.Carnivore.ToString(),
                    Amount = 1,
                },
            }
        };

        using var context = GetMemoryContext();

        var enclosureService = new EnclosureService(context);
        var animalService = new AnimalService(context, enclosureService);

        await context.Enclosures.AddAsync(enclosure1);
        await context.Enclosures.AddAsync(enclosure2);
        await context.PopulateAsync();
        await animalService.AddAnimals(animalImportFirst);
        await animalService.AddAnimals(animalImportSecond);

        var enclosures = await enclosureService.GetEnclosures();

        Dispose(context);

        Assert.Single(enclosures.ElementAt(0).Animals);
        Assert.Equal(2, enclosures.ElementAt(1).Animals.Count());
        Assert.Contains(enclosures.ElementAt(0).Animals, x => x.Species is "Elephant");
        Assert.Equal(2, enclosures.ElementAt(1).Animals.Count(x => x.Species is "Lion"));
    }
}
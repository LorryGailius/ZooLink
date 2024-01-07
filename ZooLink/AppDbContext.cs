using Microsoft.EntityFrameworkCore;
using ZooLink.Domain.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Animal> Animals { get; set; }

    public DbSet<AnimalType> AnimalTypes { get; set; }

    public DbSet<Enclosure> Enclosure { get; set; }

    public DbSet<ZooAsset> ZooAssets { get; set; }

    public DbSet<EnclosureAssets> EnclosureAssets { get; set; }

    public DbSet<AnimalPreferedAssets> AnimalPreferredAssets { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var animal = modelBuilder.Entity<Animal>();
        animal.HasKey(x => x.Id);
        animal.HasOne<AnimalType>().WithMany().HasForeignKey(x => x.AnimalTypeId);
        animal.HasOne<Enclosure>().WithMany().HasForeignKey(x => x.EnclosureId);

        var animalType = modelBuilder.Entity<AnimalType>();
        animalType.HasKey(x => x.Id);

        var enclosure = modelBuilder.Entity<Enclosure>();
        enclosure.HasKey(x => x.Id);
        enclosure.HasMany<Animal>().WithOne().HasForeignKey(x => x.EnclosureId);
        enclosure.HasMany<ZooAsset>().WithOne().HasForeignKey(x => x.Id);

        var zooAsset = modelBuilder.Entity<ZooAsset>();
        zooAsset.HasKey(x => x.Id);

        var enclosureAssets = modelBuilder.Entity<EnclosureAssets>();
        enclosureAssets.HasKey(x => new { x.EnclosureId, x.AssetId});
        enclosureAssets.HasOne<Enclosure>().WithMany().HasForeignKey(x => x.EnclosureId);

        var animalPreferedAssets = modelBuilder.Entity<AnimalPreferedAssets>();
        animalPreferedAssets.HasKey(x => new { x.AnimalTypeId, x.AssetId });
        animalPreferedAssets.HasOne<Animal>().WithMany().HasForeignKey(x => x.AnimalTypeId);
        animalPreferedAssets.HasOne<ZooAsset>().WithMany().HasForeignKey(x => x.AssetId);
    }
}

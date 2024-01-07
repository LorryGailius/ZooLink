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

    public DbSet<Enclosure> Enclosures { get; set; }

    public DbSet<ZooAsset> ZooAssets { get; set; }

    public DbSet<EnclosureAssets> EnclosureAssets { get; set; }

    public DbSet<AnimalPreferredAssets> AnimalPreferredAssets { get; set; }


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

        var animalPereferredAssets = modelBuilder.Entity<AnimalPreferredAssets>();
        animalPereferredAssets.HasKey(x => new { x.AnimalTypeId, x.AssetId });
        animalPereferredAssets.HasOne<Animal>().WithMany().HasForeignKey(x => x.AnimalTypeId);
        animalPereferredAssets.HasOne<ZooAsset>().WithMany().HasForeignKey(x => x.AssetId);
    }
}

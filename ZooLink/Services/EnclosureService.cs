using Microsoft.EntityFrameworkCore;
using ZooLink.Domain.Models;
using ZooLink.DTO;
using ZooLink.Extensions;

namespace ZooLink.Services
{
    public class EnclosureService
    {
        private readonly AppDbContext _context;
        public EnclosureService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EnclosureModelDTO>> GetEnclosures()
        {
            var enclosures = await _context.Enclosures.ToListAsync();

            return GetModelDTOList(enclosures);
        }

        public async Task<EnclosureModelDTO> GetEnclosure(Guid id)
        {
            var enclosure = await _context.Enclosures.FirstOrDefaultAsync(x => x.Id == id);

            return enclosure is not null ? GetModelDto(enclosure) : null;
        }

        public async Task<IEnumerable<EnclosureModelDTO>> AddEnclosures(EnclosureImportDTO enclosureImportDto)
        {
            var enclosureImport = enclosureImportDto.Enclosures.Select(x =>
            {
                var enclosureId = Guid.NewGuid();

                var enclosureAssets = _context.ZooAssets
                    .Where(y => x.Objects.Contains(y.Name))
                    .ToList();

                // Each asset in inclosure added to Relation table
                foreach (var asset in enclosureAssets)
                {
                    var enclosureAsset = new EnclosureAssets
                    {
                        EnclosureId = enclosureId,
                        AssetId = asset.Id,
                    };

                    _context.EnclosureAssets.Add(enclosureAsset);
                }

                return new Enclosure
                {
                    Id = enclosureId,
                    Name = x.Name,
                    Size = x.Size,
                    Location = x.Location,
                };
            });

            // Add Enclosures domain models to database
            await _context.AddRangeAsync(enclosureImport);

            await _context.SaveChangesAsync();

            var importedEnclosures = _context.Enclosures
                .Where(x => enclosureImport.Select(y => y.Name).Contains(x.Name)).ToList();

            return GetModelDTOList(importedEnclosures);
        }
        public async Task Populate()
        {
            await _context.PopulateAsync();
        }

        private IEnumerable<EnclosureModelDTO> GetModelDTOList(IEnumerable<Enclosure> enclosures)
        {
            return enclosures.Select(GetModelDto).ToList();
        }

        private EnclosureModelDTO GetModelDto(Enclosure enclosure)
        {
            var zooAssets = new List<ZooAsset>();

            var enclosureAssets = _context.EnclosureAssets.Where(x => x.EnclosureId == enclosure.Id);

            // Get all objects in enclosure
            foreach (var enclosureAsset in enclosureAssets)
            {
                var asset = _context.ZooAssets.FirstOrDefault(x => x.Id == enclosureAsset.AssetId);

                zooAssets.Add(asset);
            }

            return enclosure.ToModelDTO(zooAssets);
        }

    }
}

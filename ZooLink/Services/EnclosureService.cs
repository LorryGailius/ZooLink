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
            var enclosureImport = enclosureImportDto.Enclosures.ToList();

            foreach (var enclosureDto in enclosureImport)
            {
                await AddEnclosure(enclosureDto);
            }

            await _context.SaveChangesAsync();

            var importedEnclosures = _context.Enclosures
                .Where(x => enclosureImport
                    .Select(y => y.Name)
                    .Contains(x.Name)).ToList();

            return GetModelDTOList(importedEnclosures);
        }

        public async Task<EnclosureModelDTO> AddEnclosure(EnclosureDTO enclosureDto)
        {
            var enclosureId = Guid.NewGuid();

            var enclosure = new Enclosure
            {
                Id = enclosureId,
                Name = enclosureDto.Name,
                Size = enclosureDto.Size,
                Location = enclosureDto.Location,
            };

            await AddAssetList(enclosureId, enclosureDto.Objects);

            await _context.Enclosures.AddAsync(enclosure);

            return GetModelDto(enclosure);
        }

        public async Task AddAssetList(Guid enclosureId, IEnumerable<string> assets)
        {
            var enclosureAssets = _context.ZooAssets
                .Where(x => assets
                .Contains(x.Name)).ToList();

            foreach (var asset in enclosureAssets)
            {
                await AddAsset(enclosureId, asset.Id);
            }
        }
        public async Task AddAsset(Guid enclosureId, Guid assetId)
        {
            var enclosureAsset = new EnclosureAssets
            {
                EnclosureId = enclosureId,
                AssetId = assetId,
            };

            await _context.EnclosureAssets.AddAsync(enclosureAsset);
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

            var animalList = GetAnimalModelList(enclosure);

            // Get all objects in enclosure
            foreach (var enclosureAsset in enclosureAssets)
            {
                var asset = _context.ZooAssets
                    .FirstOrDefault(x => x.Id == enclosureAsset.AssetId);

                zooAssets.Add(asset);
            }

            return enclosure.ToModelDTO(zooAssets, animalList);
        }

        private IEnumerable<AnimalModelDTO> GetAnimalModelList(Enclosure enclosure)
        {
            var animals = _context.Animals
                .Where(x => x.EnclosureId == enclosure.Id).ToList();

            var animalIds = animals.Select(x => x.AnimalTypeId).ToHashSet();

            var animalTypes = _context.AnimalTypes.Where(x => animalIds.Contains(x.Id)).ToDictionary(x => x.Id, x => x);

            var animalModels = animals.Select(x => x.ToModelDTO(animalTypes[x.AnimalTypeId])).ToList();

            return animalModels;
        }

        private int GetEnclosureSpace(Enclosure enclosure)
        {
            var animalCount = _context.Animals
                .Count(a => a.EnclosureId == enclosure.Id);

            var capacity = (int)enclosure.Size;

            var spaceLeft = capacity - animalCount;

            Console.WriteLine($"Enclosure {enclosure.Name} has {spaceLeft} spaces left");

            return spaceLeft;
        }
    }
}

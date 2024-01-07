using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZooLink.Domain.Models;
using ZooLink.DTO;
using ZooLink.Extensions;

namespace ZooLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnclosuresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EnclosuresController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnclosureModelDTO>>> GetEnclosures()
        {
            var enclosures = await _context.Enclosure.ToListAsync();

            return Ok(GetModelDTOList(enclosures));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EnclosureModelDTO>> GetEnclosure(Guid id)
        {
            var enclosure = await _context.Enclosure.FirstOrDefaultAsync(x => x.Id == id);

            if (enclosure is not null)
            {
                return Ok(GetModelDto(enclosure));
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<EnclosureModelDTO>>> PostEnclosures(EnclosureImportDTO enclosureImportDto)
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

            // Add Enclosure domain models to database
            await _context.AddRangeAsync(enclosureImport);

            await _context.SaveChangesAsync();

            var importedEnclosures = _context.Enclosure
                .Where(x => enclosureImport.Select(y => y.Name).Contains(x.Name)).ToList();

            return Ok(GetModelDTOList(importedEnclosures));
        }


        [HttpPost("/api/Populate")]
        public async Task<ActionResult> Populate()
        {
            // Populates AnimalTypes, ZooAssets and AnimalPreferredAssets tables
            await _context.PopulateAsync();

            return Ok("Populated successfully");
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

using Microsoft.AspNetCore.Mvc;
using ZooLink.DTO;
using ZooLink.Services;

namespace ZooLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnclosuresController : ControllerBase
    {
        private readonly IEnclosureService _enclosureService;

        public EnclosuresController(IEnclosureService enclosureService)
        {
            _enclosureService = enclosureService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EnclosureModelDTO>>> GetEnclosures()
        {
            var enclosures = await _enclosureService.GetEnclosures();
            return Ok(enclosures);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EnclosureModelDTO>> GetEnclosure(Guid id)
        {
            var enclosure = await _enclosureService.GetEnclosure(id);

            if (enclosure is null)
            {
                return NotFound();
            }

            return Ok(enclosure);
        }

        [HttpPost]
        public async Task<ActionResult<IEnumerable<EnclosureModelDTO>>> PostEnclosures(EnclosureImportDTO enclosureImportDto)
        {
            var importedEnclosures = await _enclosureService.AddEnclosures(enclosureImportDto);

            return Ok(importedEnclosures);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteEnclosure(Guid id)
        {
            var removedEnclosureId = await _enclosureService.RemoveEnclosure(id);

            if (removedEnclosureId == Guid.Empty)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpPost("/api/Populate")]
        public async Task<ActionResult> Populate()
        {
            // Populates AnimalTypes, ZooAssets and AnimalPreferredAssets tables
            await _enclosureService.Populate();
            return Ok("Populated successfully");
        }
    }


}

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
using ZooLink.Services;

namespace ZooLink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnclosuresController : ControllerBase
    {
        private readonly EnclosureService _enclosureService;

        public EnclosuresController(EnclosureService enclosureService)
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


        [HttpPost("/api/Populate")]
        public async Task<ActionResult> Populate()
        {
            // Populates AnimalTypes, ZooAssets and AnimalPreferredAssets tables
            await _enclosureService.Populate();
            return Ok("Populated successfully");
        }
    }


}

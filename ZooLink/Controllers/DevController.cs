using Microsoft.AspNetCore.Mvc;
using ZooLink.Extensions;

namespace ZooLink.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DevController : ControllerBase
{
    private readonly AppDbContext _context;

    public DevController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("Populate")]
    public async Task<ActionResult> Populate()
    {
        // Populates AnimalTypes, ZooAssets and AnimalPreferredAssets tables
        await _context.PopulateAsync();
        return Ok("Populated successfully");
    }

}
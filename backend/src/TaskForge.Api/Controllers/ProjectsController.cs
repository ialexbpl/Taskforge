using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskForge.Domain;
using TaskForge.Infrastructure;

namespace TaskForge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _db;
    public ProjectsController(AppDbContext db) => _db = db;

    public record CreateProjectDto(string Name, string Key);

    [HttpGet]
    public Task<List<Project>> GetAll() =>
        _db.Projects.OrderBy(p => p.CreatedAt).ToListAsync();

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectDto dto)
    {
        var name = (dto.Name ?? "").Trim();
        var key = (dto.Key ?? "").Trim().ToUpperInvariant();

        if (name.Length is < 2 or > 200) return BadRequest("Name must be 2–200 chars.");
        if (key.Length is < 2 or > 20 || !key.All(ch => char.IsLetterOrDigit(ch) || ch == '-'))
            return BadRequest("Key must be 2–20 chars (A–Z, 0–9, '-').");

        if (await _db.Projects.AnyAsync(p => p.Key == key))
            return Conflict("Key already exists");

        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? User.FindFirst("nameidentifier")?.Value;
        if (string.IsNullOrWhiteSpace(userIdStr)) return Unauthorized();

        var ownerId = Guid.Parse(userIdStr);

        var p = new Project { Name = name, Key = key, OwnerId = ownerId };
        _db.Projects.Add(p);
        await _db.SaveChangesAsync();

        return Ok(p);
    }

    [HttpGet("{id:guid}")]
    public Task<Project?> Get(Guid id) =>
        _db.Projects.FirstOrDefaultAsync(p => p.Id == id);
}

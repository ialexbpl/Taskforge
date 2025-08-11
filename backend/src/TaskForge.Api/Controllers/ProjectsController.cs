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
    public async Task<IActionResult> Create(CreateProjectDto dto)
    {
        var key = dto.Key.Trim().ToUpper();
        if (await _db.Projects.AnyAsync(p => p.Key == key))
            return Conflict("Key already exists");

        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? User.FindFirst("nameidentifier")?.Value;
        if (string.IsNullOrWhiteSpace(userIdStr)) return Unauthorized();

        var ownerId = Guid.Parse(userIdStr);
        var p = new Project { Name = dto.Name.Trim(), Key = key, OwnerId = ownerId };

        _db.Projects.Add(p);
        await _db.SaveChangesAsync();
        return Ok(p);
    }

    [HttpGet("{id:guid}")]
    public Task<Project?> Get(Guid id) =>
        _db.Projects.FirstOrDefaultAsync(p => p.Id == id);
}

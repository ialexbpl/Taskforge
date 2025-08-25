using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskForge.Domain;
using TaskForge.Infrastructure;


namespace TaskForge.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class IssuesController : ControllerBase
{
    private readonly AppDbContext _db;
    public IssuesController(AppDbContext db) => _db = db;

    public record CreateIssueDto(
        Guid ProjectId,
        string Title,
        string? Description,
        IssuePriority Priority,
        IssueType Type
    );

    // GET /api/issues/by-project/{projectId}
    [HttpGet("by-project/{projectId:guid}")]
    public async Task<ActionResult<List<Issue>>> GetByProject(Guid projectId)
    {
        var exists = await _db.Projects.AnyAsync(p => p.Id == projectId);
        if (!exists) return NotFound("Project not found");

        var items = await _db.Issues
            .Where(i => i.ProjectId == projectId)
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync();

        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateIssueDto dto)
    {
        if (dto.ProjectId == Guid.Empty) return BadRequest("ProjectId is required");
        if (string.IsNullOrWhiteSpace(dto.Title)) return BadRequest("Title is required");

        var projectExists = await _db.Projects.AnyAsync(p => p.Id == dto.ProjectId);
        if (!projectExists) return NotFound("Project not found");

        var issue = new Issue
        {
            ProjectId = dto.ProjectId,
            Title = dto.Title.Trim(),
            Description = dto.Description?.Trim() ?? "",
            Priority = dto.Priority,
            Type = dto.Type
        };

        _db.Issues.Add(issue);
        await _db.SaveChangesAsync();
        return Ok(issue);
    }

    [HttpPatch("{id:guid}/status/{status}")]
    public async Task<IActionResult> UpdateStatus(Guid id, IssueStatus status)
    {
        var issue = await _db.Issues.FindAsync(id);
        if (issue is null) return NotFound();
        issue.Status = status;
        issue.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(issue);
    }
}

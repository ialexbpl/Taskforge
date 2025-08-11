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

    public record CreateIssueDto(Guid ProjectId, string Title, string? Description, IssuePriority Priority, IssueType Type);

    [HttpGet("by-project/{projectId:guid}")]
    public Task<List<Issue>> GetByProject(Guid projectId) =>
        _db.Issues.Where(i => i.ProjectId == projectId)
                  .OrderByDescending(i => i.CreatedAt)
                  .ToListAsync();

    [HttpPost]
    public async Task<IActionResult> Create(CreateIssueDto dto)
    {
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

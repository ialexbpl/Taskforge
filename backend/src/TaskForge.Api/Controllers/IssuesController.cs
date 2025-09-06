using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskForge.Domain;
using TaskForge.Infrastructure;
using System.Security.Claims;

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
    public record UpdateIssueDto(
        string Title,
        string? Description,
        IssueStatus Status,
        IssuePriority Priority,
        IssueType Type
        );
    public record AddCommentDto(string Body);

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Issue>> Get(Guid id)
      => await _db.Issues.FindAsync(id) is { } i ? Ok(i) : NotFound();

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateIssueDto dto)
    {
        var i = await _db.Issues.FindAsync(id);
        if (i is null) return NotFound();

        i.Title = dto.Title.Trim();
        i.Description = dto.Description?.Trim() ?? "";
        i.Status = dto.Status;
        i.Priority = dto.Priority;
        i.Type = dto.Type;
        i.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Ok(i);
    }

    [HttpPost("{id:guid}/comments")]
    public async Task<IActionResult> AddComment(Guid id, [FromBody] AddCommentDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Body)) return BadRequest("Comment body is required.");
        var exists = await _db.Issues.AnyAsync(x => x.Id == id);
        if (!exists) return NotFound();

        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? User.FindFirst("nameidentifier")?.Value;
        if (string.IsNullOrWhiteSpace(userIdStr)) return Unauthorized();
        var authorId = Guid.Parse(userIdStr);

        var c = new Comment { IssueId = id, AuthorId = authorId, Body = dto.Body.Trim() };
        _db.Comments.Add(c);
        await _db.SaveChangesAsync();
        return Ok(c);
    }

    [HttpGet("{id:guid}/comments")]
    public Task<List<Comment>> GetComments(Guid id) =>
       _db.Comments.Where(c => c.IssueId == id)
           .OrderBy(c => c.CreatedAt).ToListAsync();

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

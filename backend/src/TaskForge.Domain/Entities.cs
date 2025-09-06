using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskForge.Domain;

public enum IssueStatus { Backlog, Selected, InProgress, Review, Done }
public enum IssuePriority { Low, Medium, High, Critical }
public enum IssueType { Task, Bug, Story, Epic }

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Role { get; set; } = "Member"; // Admin, Manager, Member
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Project> OwnedProjects { get; set; } = new List<Project>();
    public ICollection<Issue> AssignedIssues { get; set; } = new List<Issue>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}

public class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(200)]
    public string Name { get; set; } = "";

    // short code like DEMO; make it unique + length‑limited
    [Required, MaxLength(20)]
    public string Key { get; set; } = "";

    [Required]
    public Guid OwnerId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public User? Owner { get; set; }
    public ICollection<Issue> Issues { get; set; } = new List<Issue>();
}
public class Issue
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public Guid ProjectId { get; set; }

    public Guid? AssigneeId { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = "";

    [MaxLength(4000)]
    public string Description { get; set; } = "";

    public IssueStatus Status { get; set; } = IssueStatus.Backlog;
    public IssuePriority Priority { get; set; } = IssuePriority.Medium;
    public IssueType Type { get; set; } = IssueType.Task;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // nav
    public Project? Project { get; set; }
    public User? Assignee { get; set; }
}

public class RefreshToken
{

public Guid Id { get; set; } = Guid.NewGuid();

[Required]
public Guid UserId { get; set; }

[Required, MaxLength(300)]
public string Token { get; set; } = "";

public DateTime ExpiresAt { get; set; }
public DateTime? RevokedAt { get; set; }

// nav
public User? User { get; set; }
}
public class Comment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    [Required] public Guid IssueId { get; set; }
    [Required] public Guid AuthorId { get; set; }
    [Required, MaxLength(4000)] public string Body { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Issue? Issue { get; set; }
    public User? Author { get; set; }
}

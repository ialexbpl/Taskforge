using System;
using System.Collections.Generic;
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
}

public class Project
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "";
    public string Key { get; set; } = ""; // unique short code, e.g., DEMO
    public Guid OwnerId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<Issue> Issues { get; set; } = new();
}
public class Issue
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ProjectId { get; set; }
    public Guid? AssigneeId { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public IssueStatus Status { get; set; } = IssueStatus.Backlog;
    public IssuePriority Priority { get; set; } = IssuePriority.Medium;
    public IssueType Type { get; set; } = IssueType.Task;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Token { get; set; } = "";
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
}
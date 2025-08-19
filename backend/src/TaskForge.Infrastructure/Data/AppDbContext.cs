using Microsoft.EntityFrameworkCore;
using TaskForge.Domain;


namespace TaskForge.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Issue> Issues => Set<Issue>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        // USERS
        b.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
        });

        // PROJECTS
        b.Entity<Project>(e =>
        {
            e.HasIndex(p => p.Key).IsUnique();
            e.HasOne(p => p.Owner)
             .WithMany(u => u.OwnedProjects)
             .HasForeignKey(p => p.OwnerId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        // ISSUES
        b.Entity<Issue>(e =>
        {
            e.HasOne(i => i.Project)
             .WithMany(p => p.Issues)
             .HasForeignKey(i => i.ProjectId)
             .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(i => i.Assignee)
             .WithMany(u => u.AssignedIssues)
             .HasForeignKey(i => i.AssigneeId)
             .OnDelete(DeleteBehavior.SetNull);
        });

        // REFRESH TOKENS
        b.Entity<RefreshToken>(e =>
        {
            e.HasOne(t => t.User)
             .WithMany(u => u.RefreshTokens)
             .HasForeignKey(t => t.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }

}

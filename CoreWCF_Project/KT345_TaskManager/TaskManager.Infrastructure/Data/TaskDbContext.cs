using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Models;

namespace TaskManager.Infrastructure.Data;

public class TaskDbContext : DbContext
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<UserInfo> Users => Set<UserInfo>();

    public TaskDbContext(DbContextOptions<TaskDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaskItem>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Title).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<Project>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<UserInfo>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.Username).HasMaxLength(50).IsRequired();
        });

        // Seed data
        modelBuilder.Entity<UserInfo>().HasData(
            new UserInfo { Id = 1, Username = "admin", FullName = "Admin User", Role = "Admin" },
            new UserInfo { Id = 2, Username = "dev1", FullName = "Developer One", Role = "Developer" },
            new UserInfo { Id = 3, Username = "dev2", FullName = "Developer Two", Role = "Developer" }
        );

        modelBuilder.Entity<Project>().HasData(
            new Project { Id = 1, Name = "Main Project", Description = "Core product development", CreatedAt = DateTime.UtcNow, OwnerId = 1 },
            new Project { Id = 2, Name = "Mobile App", Description = "Mobile application", CreatedAt = DateTime.UtcNow, OwnerId = 1 }
        );

        modelBuilder.Entity<TaskItem>().HasData(
            new TaskItem { Id = 1, Title = "Setup CI/CD", Description = "Configure pipeline", Priority = Core.Enums.TaskPriority.High, Status = Core.Enums.TaskItemStatus.InProgress, ProjectId = 1, AssigneeId = 2, CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(7) },
            new TaskItem { Id = 2, Title = "Write unit tests", Description = "Cover core logic", Priority = Core.Enums.TaskPriority.Medium, Status = Core.Enums.TaskItemStatus.New, ProjectId = 1, AssigneeId = 3, CreatedAt = DateTime.UtcNow, DueDate = DateTime.UtcNow.AddDays(14) },
            new TaskItem { Id = 3, Title = "Design UI mockups", Description = "Mobile screens", Priority = Core.Enums.TaskPriority.Low, Status = Core.Enums.TaskItemStatus.Done, ProjectId = 2, AssigneeId = 2, CreatedAt = DateTime.UtcNow.AddDays(-10) }
        );
    }
}

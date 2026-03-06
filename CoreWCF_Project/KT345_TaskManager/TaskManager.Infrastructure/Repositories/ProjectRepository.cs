using TaskManager.Core.Interfaces;
using TaskManager.Core.Models;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly TaskDbContext _db;

    public ProjectRepository(TaskDbContext db)
    {
        _db = db;
    }

    public Project? GetById(int id) => _db.Projects.Find(id);

    public List<Project> GetAll() => _db.Projects.ToList();

    public Project Add(Project project)
    {
        _db.Projects.Add(project);
        _db.SaveChanges();
        return project;
    }
}

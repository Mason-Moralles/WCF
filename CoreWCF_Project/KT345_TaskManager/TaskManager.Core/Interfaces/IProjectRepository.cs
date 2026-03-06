using TaskManager.Core.Models;

namespace TaskManager.Core.Interfaces;

public interface IProjectRepository
{
    Project? GetById(int id);
    List<Project> GetAll();
    Project Add(Project project);
}

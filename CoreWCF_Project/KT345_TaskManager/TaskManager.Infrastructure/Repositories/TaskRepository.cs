using TaskManager.Core.Interfaces;
using TaskManager.Core.Models;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly TaskDbContext _db;

    public TaskRepository(TaskDbContext db)
    {
        _db = db;
    }

    public TaskItem? GetById(int id) => _db.Tasks.Find(id);

    public List<TaskItem> GetByProjectId(int projectId) =>
        _db.Tasks.Where(t => t.ProjectId == projectId).ToList();

    public List<TaskItem> GetByAssigneeId(int assigneeId) =>
        _db.Tasks.Where(t => t.AssigneeId == assigneeId).ToList();

    public TaskItem Add(TaskItem task)
    {
        _db.Tasks.Add(task);
        _db.SaveChanges();
        return task;
    }

    public bool Update(TaskItem task)
    {
        var existing = _db.Tasks.Find(task.Id);
        if (existing == null) return false;

        existing.Title = task.Title;
        existing.Description = task.Description;
        existing.Priority = task.Priority;
        existing.Status = task.Status;
        existing.AssigneeId = task.AssigneeId;
        existing.DueDate = task.DueDate;

        _db.SaveChanges();
        return true;
    }

    public bool Delete(int id)
    {
        var task = _db.Tasks.Find(id);
        if (task == null) return false;

        _db.Tasks.Remove(task);
        _db.SaveChanges();
        return true;
    }
}

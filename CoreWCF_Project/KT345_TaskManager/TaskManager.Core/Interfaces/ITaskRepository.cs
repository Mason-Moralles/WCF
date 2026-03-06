using TaskManager.Core.Models;

namespace TaskManager.Core.Interfaces;

public interface ITaskRepository
{
    TaskItem? GetById(int id);
    List<TaskItem> GetByProjectId(int projectId);
    List<TaskItem> GetByAssigneeId(int assigneeId);
    TaskItem Add(TaskItem task);
    bool Update(TaskItem task);
    bool Delete(int id);
}

using System.ServiceModel;
using TaskManager.Core.Enums;
using TaskManager.Core.Models;

namespace TaskManager.Core.Contracts;

[ServiceContract]
public interface ITaskManagerService
{
    // Аутентификация
    [OperationContract]
    AuthToken Authenticate(string username, string password);

    // Управление задачами
    [OperationContract]
    TaskItem CreateTask(string token, string title, string description, TaskPriority priority, int projectId, int assigneeId, DateTime? dueDate);

    [OperationContract]
    TaskItem GetTask(string token, int taskId);

    [OperationContract]
    List<TaskItem> GetTasksByProject(string token, int projectId);

    [OperationContract]
    List<TaskItem> GetTasksByAssignee(string token, int userId);

    [OperationContract]
    bool UpdateTaskStatus(string token, int taskId, TaskItemStatus status);

    [OperationContract]
    bool DeleteTask(string token, int taskId);

    // Управление проектами
    [OperationContract]
    Project CreateProject(string token, string name, string description);

    [OperationContract]
    List<Project> GetAllProjects(string token);

    // Отчёты
    [OperationContract]
    WorkloadReport GetUserWorkload(string token, int userId);
}

using TaskManager.Application.Validators;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Models;

namespace TaskManager.Application.Services;

public class TaskAppService
{
    private readonly ITaskRepository _taskRepo;
    private readonly IProjectRepository _projectRepo;
    private readonly IUserRepository _userRepo;

    public TaskAppService(ITaskRepository taskRepo, IProjectRepository projectRepo, IUserRepository userRepo)
    {
        _taskRepo = taskRepo;
        _projectRepo = projectRepo;
        _userRepo = userRepo;
    }

    public TaskItem CreateTask(string title, string description, TaskPriority priority,
        int projectId, int assigneeId, DateTime? dueDate)
    {
        var validator = new CreateTaskValidator();
        var result = validator.Validate(new CreateTaskRequest(title, description, (int)priority, projectId, assigneeId, dueDate));
        if (!result.IsValid)
            throw new ArgumentException(string.Join("; ", result.Errors.Select(e => e.ErrorMessage)));

        if (_projectRepo.GetById(projectId) == null)
            throw new ArgumentException($"Project {projectId} not found.");

        if (_userRepo.GetById(assigneeId) == null)
            throw new ArgumentException($"User {assigneeId} not found.");

        var task = new TaskItem
        {
            Title = title,
            Description = description,
            Priority = priority,
            Status = TaskItemStatus.New,
            ProjectId = projectId,
            AssigneeId = assigneeId,
            CreatedAt = DateTime.UtcNow,
            DueDate = dueDate
        };

        return _taskRepo.Add(task);
    }

    public TaskItem? GetTask(int taskId) => _taskRepo.GetById(taskId);

    public List<TaskItem> GetTasksByProject(int projectId) => _taskRepo.GetByProjectId(projectId);

    public List<TaskItem> GetTasksByAssignee(int userId) => _taskRepo.GetByAssigneeId(userId);

    public bool UpdateTaskStatus(int taskId, TaskItemStatus status)
    {
        var task = _taskRepo.GetById(taskId)
            ?? throw new ArgumentException($"Task {taskId} not found.");

        task.Status = status;
        return _taskRepo.Update(task);
    }

    public bool DeleteTask(int taskId) => _taskRepo.Delete(taskId);

    public Project CreateProject(string name, string description, int ownerId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Project name is required.");

        var project = new Project
        {
            Name = name,
            Description = description,
            OwnerId = ownerId,
            CreatedAt = DateTime.UtcNow
        };

        return _projectRepo.Add(project);
    }

    public List<Project> GetAllProjects() => _projectRepo.GetAll();

    public WorkloadReport GetUserWorkload(int userId)
    {
        var user = _userRepo.GetById(userId)
            ?? throw new ArgumentException($"User {userId} not found.");

        var tasks = _taskRepo.GetByAssigneeId(userId);

        return new WorkloadReport
        {
            UserId = userId,
            UserName = user.FullName,
            TotalTasks = tasks.Count,
            CompletedTasks = tasks.Count(t => t.Status == TaskItemStatus.Done),
            InProgressTasks = tasks.Count(t => t.Status == TaskItemStatus.InProgress),
            OverdueTasks = tasks.Count(t => t.DueDate.HasValue && t.DueDate < DateTime.UtcNow && t.Status != TaskItemStatus.Done)
        };
    }
}

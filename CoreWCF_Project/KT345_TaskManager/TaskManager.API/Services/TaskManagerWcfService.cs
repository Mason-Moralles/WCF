using System.Security.Claims;
using CoreWCF;
using TaskManager.Application.Services;
using TaskManager.Core.Contracts;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Models;

namespace TaskManager.API.Services;

public class TaskManagerWcfService : ITaskManagerService
{
    private readonly TaskAppService _appService;
    private readonly IUserRepository _userRepo;
    private readonly JwtService _jwt;
    private readonly ILogger<TaskManagerWcfService> _logger;

    public TaskManagerWcfService(
        TaskAppService appService,
        IUserRepository userRepo,
        JwtService jwt,
        ILogger<TaskManagerWcfService> logger)
    {
        _appService = appService;
        _userRepo = userRepo;
        _jwt = jwt;
        _logger = logger;
    }

    public AuthToken Authenticate(string username, string password)
    {
        _logger.LogInformation("Auth attempt: {User}", username);

        // Mock аутентификация
        var user = _userRepo.GetByUsername(username)
            ?? throw new FaultException($"User '{username}' not found.");

        // Простая проверка пароля для демо
        if (password != "pass123")
            throw new FaultException("Invalid password.");

        var token = _jwt.GenerateToken(user.Username, user.Id, user.Role);

        _logger.LogInformation("User {User} authenticated, role={Role}", username, user.Role);

        return new AuthToken
        {
            Token = token,
            Expires = DateTime.UtcNow.AddHours(2),
            Role = user.Role
        };
    }

    public TaskItem CreateTask(string token, string title, string description,
        TaskPriority priority, int projectId, int assigneeId, DateTime? dueDate)
    {
        var user = Authorize(token);
        _logger.LogInformation("User {User} creating task '{Title}' in project {Project}",
            user.Identity?.Name, title, projectId);

        try
        {
            return _appService.CreateTask(title, description, priority, projectId, assigneeId, dueDate);
        }
        catch (ArgumentException ex)
        {
            throw new FaultException(ex.Message);
        }
    }

    public TaskItem GetTask(string token, int taskId)
    {
        Authorize(token);
        return _appService.GetTask(taskId)
            ?? throw new FaultException($"Task {taskId} not found.");
    }

    public List<TaskItem> GetTasksByProject(string token, int projectId)
    {
        Authorize(token);
        return _appService.GetTasksByProject(projectId);
    }

    public List<TaskItem> GetTasksByAssignee(string token, int userId)
    {
        Authorize(token);
        return _appService.GetTasksByAssignee(userId);
    }

    public bool UpdateTaskStatus(string token, int taskId, TaskItemStatus status)
    {
        var user = Authorize(token);
        _logger.LogInformation("User {User} updating task {TaskId} status to {Status}",
            user.Identity?.Name, taskId, status);

        try
        {
            return _appService.UpdateTaskStatus(taskId, status);
        }
        catch (ArgumentException ex)
        {
            throw new FaultException(ex.Message);
        }
    }

    public bool DeleteTask(string token, int taskId)
    {
        var user = Authorize(token, "Admin");
        _logger.LogWarning("Admin {User} deleting task {TaskId}", user.Identity?.Name, taskId);
        return _appService.DeleteTask(taskId);
    }

    public Project CreateProject(string token, string name, string description)
    {
        var user = Authorize(token, "Admin");
        var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        _logger.LogInformation("Admin {User} creating project '{Name}'", user.Identity?.Name, name);

        try
        {
            return _appService.CreateProject(name, description, userId);
        }
        catch (ArgumentException ex)
        {
            throw new FaultException(ex.Message);
        }
    }

    public List<Project> GetAllProjects(string token)
    {
        Authorize(token);
        return _appService.GetAllProjects();
    }

    public WorkloadReport GetUserWorkload(string token, int userId)
    {
        Authorize(token);

        try
        {
            return _appService.GetUserWorkload(userId);
        }
        catch (ArgumentException ex)
        {
            throw new FaultException(ex.Message);
        }
    }

    private ClaimsPrincipal Authorize(string token, params string[] requiredRoles)
    {
        var principal = _jwt.ValidateToken(token)
            ?? throw new FaultException("Authentication failed. Invalid or expired token.");

        if (requiredRoles.Length > 0)
        {
            var userRoles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value);
            if (!requiredRoles.Any(r => userRoles.Contains(r)))
                throw new FaultException($"Access denied. Required roles: {string.Join(", ", requiredRoles)}");
        }

        return principal;
    }
}

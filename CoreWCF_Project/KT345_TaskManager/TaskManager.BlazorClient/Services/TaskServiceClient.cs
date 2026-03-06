using System.ServiceModel;
using TaskManager.Core.Contracts;
using TaskManager.Core.Enums;
using TaskManager.Core.Models;

namespace TaskManager.BlazorClient.Services;

public class TaskServiceClient : IDisposable
{
    private readonly ITaskManagerService _client;
    private readonly IClientChannel _channel;

    public string? Token { get; private set; }
    public string? Role { get; private set; }
    public string? Username { get; private set; }
    public bool IsAuthenticated => Token != null;

    public TaskServiceClient()
    {
        var binding = new BasicHttpBinding { MaxReceivedMessageSize = 65536 * 10 };
        var endpoint = new EndpointAddress("http://localhost:5020/TaskService/basic");
        var factory = new ChannelFactory<ITaskManagerService>(binding, endpoint);
        _client = factory.CreateChannel();
        _channel = (IClientChannel)_client;
    }

    public async Task<(bool Success, string Message)> LoginAsync(string username, string password)
    {
        try
        {
            var result = await Task.Run(() => _client.Authenticate(username, password));
            Token = result.Token;
            Role = result.Role;
            Username = username;
            return (true, $"Welcome, {username}! Role: {result.Role}");
        }
        catch (FaultException ex)
        {
            return (false, ex.Message);
        }
        catch (Exception ex)
        {
            return (false, $"Connection error: {ex.Message}");
        }
    }

    public void Logout()
    {
        Token = null;
        Role = null;
        Username = null;
    }

    public async Task<List<Project>> GetProjectsAsync()
    {
        EnsureAuth();
        return await Task.Run(() => _client.GetAllProjects(Token!));
    }

    public async Task<List<TaskItem>> GetTasksByProjectAsync(int projectId)
    {
        EnsureAuth();
        return await Task.Run(() => _client.GetTasksByProject(Token!, projectId));
    }

    public async Task<List<TaskItem>> GetTasksByAssigneeAsync(int userId)
    {
        EnsureAuth();
        return await Task.Run(() => _client.GetTasksByAssignee(Token!, userId));
    }

    public async Task<TaskItem> CreateTaskAsync(string title, string description,
        TaskPriority priority, int projectId, int assigneeId, DateTime? dueDate)
    {
        EnsureAuth();
        return await Task.Run(() =>
            _client.CreateTask(Token!, title, description, priority, projectId, assigneeId, dueDate));
    }

    public async Task<bool> UpdateTaskStatusAsync(int taskId, TaskItemStatus status)
    {
        EnsureAuth();
        return await Task.Run(() => _client.UpdateTaskStatus(Token!, taskId, status));
    }

    public async Task<bool> DeleteTaskAsync(int taskId)
    {
        EnsureAuth();
        return await Task.Run(() => _client.DeleteTask(Token!, taskId));
    }

    public async Task<WorkloadReport> GetWorkloadAsync(int userId)
    {
        EnsureAuth();
        return await Task.Run(() => _client.GetUserWorkload(Token!, userId));
    }

    public async Task<Project> CreateProjectAsync(string name, string description)
    {
        EnsureAuth();
        return await Task.Run(() => _client.CreateProject(Token!, name, description));
    }

    private void EnsureAuth()
    {
        if (!IsAuthenticated)
            throw new InvalidOperationException("Not authenticated.");
    }

    public void Dispose()
    {
        try { _channel.Close(); } catch { _channel.Abort(); }
    }
}

using Moq;
using TaskManager.Application.Services;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces;
using TaskManager.Core.Models;

namespace TaskManager.Tests;

public class TaskAppServiceTests
{
    private readonly Mock<ITaskRepository> _taskRepoMock = new();
    private readonly Mock<IProjectRepository> _projectRepoMock = new();
    private readonly Mock<IUserRepository> _userRepoMock = new();
    private readonly TaskAppService _sut;

    public TaskAppServiceTests()
    {
        _sut = new TaskAppService(_taskRepoMock.Object, _projectRepoMock.Object, _userRepoMock.Object);
    }

    [Fact]
    public void CreateTask_ValidData_ReturnsTask()
    {
        _projectRepoMock.Setup(r => r.GetById(1)).Returns(new Project { Id = 1, Name = "Test" });
        _userRepoMock.Setup(r => r.GetById(2)).Returns(new UserInfo { Id = 2, Username = "dev1" });
        _taskRepoMock.Setup(r => r.Add(It.IsAny<TaskItem>())).Returns<TaskItem>(t => { t.Id = 1; return t; });

        var result = _sut.CreateTask("Test Task", "Desc", TaskPriority.High, 1, 2, null);

        Assert.Equal("Test Task", result.Title);
        Assert.Equal(TaskItemStatus.New, result.Status);
        Assert.Equal(TaskPriority.High, result.Priority);
    }

    [Fact]
    public void CreateTask_EmptyTitle_ThrowsArgException()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            _sut.CreateTask("", "Desc", TaskPriority.Low, 1, 1, null));

        Assert.Contains("Title is required", ex.Message);
    }

    [Fact]
    public void CreateTask_InvalidProjectId_ThrowsArgException()
    {
        _projectRepoMock.Setup(r => r.GetById(It.IsAny<int>())).Returns((Project?)null);
        _userRepoMock.Setup(r => r.GetById(It.IsAny<int>())).Returns(new UserInfo { Id = 1 });

        var ex = Assert.Throws<ArgumentException>(() =>
            _sut.CreateTask("Title", "Desc", TaskPriority.Low, 999, 1, null));

        Assert.Contains("not found", ex.Message);
    }

    [Fact]
    public void UpdateTaskStatus_ExistingTask_ReturnsTrue()
    {
        var task = new TaskItem { Id = 1, Title = "Task", Status = TaskItemStatus.New };
        _taskRepoMock.Setup(r => r.GetById(1)).Returns(task);
        _taskRepoMock.Setup(r => r.Update(It.IsAny<TaskItem>())).Returns(true);

        var result = _sut.UpdateTaskStatus(1, TaskItemStatus.InProgress);

        Assert.True(result);
        _taskRepoMock.Verify(r => r.Update(It.Is<TaskItem>(t => t.Status == TaskItemStatus.InProgress)), Times.Once);
    }

    [Fact]
    public void UpdateTaskStatus_NonExistent_ThrowsArgException()
    {
        _taskRepoMock.Setup(r => r.GetById(999)).Returns((TaskItem?)null);

        Assert.Throws<ArgumentException>(() => _sut.UpdateTaskStatus(999, TaskItemStatus.Done));
    }

    [Fact]
    public void GetUserWorkload_ReturnsCorrectReport()
    {
        _userRepoMock.Setup(r => r.GetById(1)).Returns(new UserInfo { Id = 1, FullName = "Test User" });
        _taskRepoMock.Setup(r => r.GetByAssigneeId(1)).Returns(new List<TaskItem>
        {
            new() { Status = TaskItemStatus.Done },
            new() { Status = TaskItemStatus.InProgress },
            new() { Status = TaskItemStatus.New, DueDate = DateTime.UtcNow.AddDays(-1) }
        });

        var report = _sut.GetUserWorkload(1);

        Assert.Equal(3, report.TotalTasks);
        Assert.Equal(1, report.CompletedTasks);
        Assert.Equal(1, report.InProgressTasks);
        Assert.Equal(1, report.OverdueTasks);
    }

    [Fact]
    public void CreateProject_EmptyName_ThrowsArgException()
    {
        Assert.Throws<ArgumentException>(() => _sut.CreateProject("", "Desc", 1));
    }

    [Fact]
    public void DeleteTask_CallsRepository()
    {
        _taskRepoMock.Setup(r => r.Delete(1)).Returns(true);

        var result = _sut.DeleteTask(1);

        Assert.True(result);
        _taskRepoMock.Verify(r => r.Delete(1), Times.Once);
    }
}
